using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using La_Colectiva.Services.Handlers;
using Newtonsoft.Json; // Requiere paquete NuGet Newtonsoft.Json

namespace La_Colectiva.Models.Api
{
    public class ApiRequest<R, T>
    {
        private readonly HttpClient _client;

        public string Url { get; }
        public HttpMethod Method { get; }
        public object Payload { get; private set; }
        private readonly Dictionary<string, object> _query = new Dictionary<string, object>();
        private string _accept { get; set; }

        private string _token { get; set; }

        private CancellationToken _cancellationToken = CancellationToken.None;
        private IProgress<int> _progress = new Progress<int>();

        private bool _log;

        private readonly Func<R, T> _mapper;

        private Action<T> _onSuccess;
        private Action<HttpStatusCode> _onEmptyResponse;
        private Action<ApiUnprocessableEntityErrorResponse> _onValidationError;
        private Action<ApiFatalErrorResponse> _onFatalError = ErrorHandler.Detailed(HttpErrorTemplate.Default());
        private bool _expectNoContent;

        public ApiRequest(
            HttpClient client,
            string url,
            HttpMethod method,
            Func<R, T> mapper = null)
        {
            _client = client;
            Url = url;
            Method = method;
            _mapper = mapper ?? (value => (T)(object)value);
        }

        public ApiRequest<R, T> ExpectNoContent()
        {
            _expectNoContent = true;
            return this;
        }

        public ApiRequest<R, T> Authenticate(string token)
        {
            this._token = token;
            return this;
        }

        public ApiRequest<R, T> WithCancellation(CancellationToken token)
        {
            _cancellationToken = token;
            return this;
        }

        public ApiRequest<R, T> ReportProgress(IProgress<int> progress)
        {
            this._progress = progress;
            return this;
        }

        public ApiRequest<R, T> Query(string name, object value)
        {
            _query[name] = value;
            return this;
        }

        public ApiRequest<R, T> Query(object parameters)
        {
            foreach (var property in parameters.GetType().GetProperties())
            {
                var value = property.GetValue(parameters);
                Query(property.Name, value);
            }

            return this;
        }

        private string BuildUrl()
        {
            if (_query.Count == 0)
                return Url;

            var queryString = string.Join("&",
                _query
                    .Where(x => x.Value != null)
                    .Select(x =>
                        $"{Uri.EscapeDataString(x.Key)}=" +
                        $"{Uri.EscapeDataString(x.Value.ToString())}"
                    )
            );

            return $"{Url}?{queryString}";
        }

        public ApiRequest<R, T> Accept(string accept)
        {
            this._accept = accept;
            return this;
        }

        public ApiRequest<R, T> Json()
        {
            return this.Accept("application/json");
        }

        public ApiRequest<R, T> SetPayload(object payload)
        {
            Payload = payload;
            return this;
        }

        public ApiRequest<R, T> Ok(Action<T> action)
        {
            _onSuccess = action;
            return this;
        }

        public ApiRequest<R, T> NoContent(Action<HttpStatusCode> action)
        {
            _onEmptyResponse = action;
            return this;
        }

        public ApiRequest<R, T> Error(
            Action<ApiUnprocessableEntityErrorResponse> action)
        {
            _onValidationError = action;
            return this;
        }

        public ApiRequest<R, T> FatalError(
            Action<ApiFatalErrorResponse> action)
        {
            _onFatalError = action;
            return this;
        }

        public ApiRequest<R, T> Debug()
        {
            _log = true;
            return this;
        }

        public ApiRequest<R, T2> Map<T2>(Func<T, T2> mapper)
        {
            var req = new ApiRequest<R, T2>(
                _client,
                Url,
                Method,
                value => mapper(_mapper(value))
            )
            {
                Payload = Payload,
                _log = _log
            };
            foreach (var q in _query)
            {
                req.Query(q.Key, q.Value);
            }
            if (this._token != null)
                req._token = this._token;
            req._log = this._log;
            return req
                .WithCancellation(this._cancellationToken)
                .ReportProgress(this._progress);
        }

        public async Task<bool> DownloadAsync(string destinationFilePath)
        {
            try
            {
                using (var requestMessage = new HttpRequestMessage(Method, BuildUrl()))
                {
                    if (this._token != null) requestMessage.Headers.Add("Authorization", $"Bearer {this._token}");

                    if (_log) System.Diagnostics.Debug.WriteLine($"\n---> DOWNLOAD {Method.Method} {BuildUrl()}");

                    using (var response = await _client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, _cancellationToken))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            _onFatalError?.Invoke(new ApiFatalErrorResponse { Message = $"Error HTTP: {response.StatusCode}" });
                            return false;
                        }

                        using (var stream = await response.Content.ReadAsStreamAsync()) // ReadAsStreamAsync no toma CancellationToken en versiones viejas de UWP
                        using (var fileStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                        {
                            var totalBytes = response.Content.Headers.ContentLength;
                            var buffer = new byte[8192];
                            int bytesRead;
                            long totalRead = 0;

                            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, _cancellationToken)) > 0)
                            {
                                await fileStream.WriteAsync(buffer, 0, bytesRead, _cancellationToken);
                                totalRead += bytesRead;

                                if (totalBytes.HasValue && this._progress != null)
                                {
                                    int percentage = (int)Math.Round((double)totalRead / totalBytes.Value * 100);
                                    this._progress.Report(percentage);
                                }
                            }

                            if (_log) System.Diagnostics.Debug.WriteLine($"<--- DESCARGA COMPLETADA: {destinationFilePath}");
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (_log) System.Diagnostics.Debug.WriteLine($"<--- EXCEPCIÓN: {ex.Message}");
                _onFatalError?.Invoke(new ApiFatalErrorResponse
                {
                    Message = ex.GetType().Name,
                    Error = $"{ex.GetType().Name}: {ex.Message}",
                    Trace = ex.StackTrace
                });
                return false;
            }
        }

        public async Task RunAsync()
        {
            try
            {
                if (_log) System.Diagnostics.Debug.WriteLine($"\n---> {Method.Method} {BuildUrl()}");

                using (var requestMessage = new HttpRequestMessage(Method, BuildUrl()))
                {
                    if (this._accept != null) requestMessage.Headers.Add("Accept", this._accept);
                    if (this._token != null)
                    {
                        requestMessage.Headers.Add("Authorization", $"Bearer {this._token}");
                        if (_log) System.Diagnostics.Debug.WriteLine($"---> Token: {this._token}");
                    }

                    if (Payload != null)
                    {
                        var jsonPayload = JsonConvert.SerializeObject(Payload);
                        requestMessage.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                        if (_log) System.Diagnostics.Debug.WriteLine($"---> Body: {jsonPayload}");
                    }

                    using (var response = await _client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, _cancellationToken))
                    {
                        string jsonResponse = string.Empty;
                        var totalBytes = response.Content.Headers.ContentLength;

                        using (var stream = await response.Content.ReadAsStreamAsync())
                        using (var ms = new MemoryStream())
                        {
                            var buffer = new byte[8192];
                            int bytesRead;
                            long totalRead = 0;

                            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, _cancellationToken)) > 0)
                            {
                                await ms.WriteAsync(buffer, 0, bytesRead, _cancellationToken);
                                totalRead += bytesRead;

                                if (totalBytes.HasValue && this._progress != null)
                                {
                                    int percentage = (int)Math.Round((double)totalRead / totalBytes.Value * 100);
                                    this._progress.Report(percentage);
                                }
                            }

                            jsonResponse = Encoding.UTF8.GetString(ms.ToArray());
                        }

                        if (_log)
                        {
                            System.Diagnostics.Debug.WriteLine($"<--- HTTP {(int)response.StatusCode} {response.ReasonPhrase}");
                            System.Diagnostics.Debug.WriteLine($"<--- Body: {jsonResponse}\n");
                        }

                        // SUCCESS
                        if (response.IsSuccessStatusCode)
                        {
                            if (_expectNoContent)
                            {
                                _onEmptyResponse?.Invoke(response.StatusCode);
                                return;
                            }
                            if (string.IsNullOrWhiteSpace(jsonResponse))
                            {
                                _onEmptyResponse?.Invoke(response.StatusCode);
                                return;
                            }

                            var raw = JsonConvert.DeserializeObject<R>(jsonResponse);
                            if (raw == null) return;
                            var result = _mapper(raw);
                            _onSuccess?.Invoke(result);
                            return;
                        }

                        // VALIDATION ERROR - 422
                        if (response.StatusCode == (HttpStatusCode)422) // Usamos casteo para asegurar compatibilidad si falta la constante en el enum viejo
                        {
                            var error = JsonConvert.DeserializeObject<ApiUnprocessableEntityErrorResponse>(jsonResponse);
                            if (error != null) _onValidationError?.Invoke(error);
                            return;
                        }

                        // FATAL ERROR
                        ApiFatalErrorResponse fatalError = null;
                        if (string.IsNullOrWhiteSpace(jsonResponse))
                        {
                            fatalError = new ApiFatalErrorResponse
                            {
                                Message = $"Error HTTP: {response.StatusCode}"
                            };
                        }
                        else
                        {
                            try
                            {
                                fatalError = JsonConvert.DeserializeObject<ApiFatalErrorResponse>(jsonResponse);
                            }
                            catch
                            {
                                // Falla al parsear el error custom
                            }
                        }

                        if (fatalError == null)
                        {
                            fatalError = new ApiFatalErrorResponse();
                        }

                        fatalError.Status = response.StatusCode;
                        _onFatalError?.Invoke(fatalError);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_log)
                {
                    System.Diagnostics.Debug.WriteLine($"<--- EXCEPCIÓN: {ex.Message}\n{ex.StackTrace}");
                }
                _onFatalError?.Invoke(new ApiFatalErrorResponse
                {
                    Status = null,
                    Message = ex.GetType().Name,
                    Error = $"{ex.GetType().Name}: {ex.Message}",
                    Trace = ex.StackTrace
                });
            }
        }
    }

    public static class ApiRequest
    {
        public static ApiRequest<R, R> Create<R>(
            HttpClient client,
            string url,
            HttpMethod method)
        {
            return new ApiRequest<R, R>(
                client,
                url,
                method
            );
        }

        public static ApiRequest<R, T> Create<R, T>(
            HttpClient client,
            string url,
            HttpMethod method)
        {
            return new ApiRequest<R, T>(
                client,
                url,
                method
            );
        }
    }
}