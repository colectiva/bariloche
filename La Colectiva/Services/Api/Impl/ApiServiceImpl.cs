using La_Colectiva.Models.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Services.Api.Impl
{
    public class ApiServiceImpl : ApiService
    {
        private HttpClient _client;
        private AuthService _authService;
        public ApiServiceImpl(HttpClient client, AuthService authService)
        {
            this._client = client;
            this._authService = authService;
        }


        public ApiRequest<R, T> AbsRequest<R, T>(string url, HttpMethod method)
        {
            return new ApiRequest<R, T>(this._client, url, method);
        }

        public ApiRequest<R, R> AbsRequest<R>(string url, HttpMethod method)
        {
            return new ApiRequest<R, R>(this._client, url, method);
        }

        public ApiRequest<R, T> AbsGet<R, T>(string url)
        {
            return new ApiRequest<R, T>(_client, url, HttpMethod.Get);
        }
        public ApiRequest<R, R> AbsGet<R>(string url) { return AbsGet<R, R>(url); }


        public ApiRequest<R, T> AbsHead<R, T>(string url)
        {
            return new ApiRequest<R, T>(_client, url, HttpMethod.Head);
        }
        public ApiRequest<R, R> AbsHead<R>(string url) { return AbsHead<R, R>(url); }


        public ApiRequest<R, T> AbsPost<R, T>(string url)
        {
            return new ApiRequest<R, T>(_client, url, HttpMethod.Post);
        }
        public ApiRequest<R, R> AbsPost<R>(string url) { return AbsPost<R, R>(url); }


        public ApiRequest<R, T> AbsPut<R, T>(string url)
        {
            return new ApiRequest<R, T>(_client, url, HttpMethod.Put);
        }
        public ApiRequest<R, R> AbsPut<R>(string url) { return AbsPut<R, R>(url); }

        


        public ApiRequest<R, T> AbsDelete<R, T>(string url)
        {
            return new ApiRequest<R, T>(_client, url, HttpMethod.Delete);
        }
        public ApiRequest<R, R> AbsDelete<R>(string url) { return AbsDelete<R, R>(url); }


        public ApiRequest<R, T> AbsOptions<R, T>(string url)
        {
            return new ApiRequest<R, T>(_client, url, HttpMethod.Options);
        }
        public ApiRequest<R, R> AbsOptions<R>(string url) { return AbsOptions<R, R>(url); }



        public ApiRequest<R, T> Request<R, T>(string relativeUrl, HttpMethod method)
        {
            return new ApiRequest<R, T>(this._client, $"{_authService.BaseUrl}{relativeUrl}", method);
        }

        public ApiRequest<R, R> Request<R>(string relativeUrl, HttpMethod method)
        {
            return new ApiRequest<R, R>(this._client, $"{_authService.BaseUrl}{relativeUrl}", method);
        }

        public ApiRequest<R, T> Get<R, T>(string relativeUrl)
        {
            return new ApiRequest<R, T>(_client, $"{_authService.BaseUrl}{relativeUrl}", HttpMethod.Get);
        }
        public ApiRequest<R, R> Get<R>(string relativeUrl) { return Get<R, R>(relativeUrl); }


        public ApiRequest<R, T> Head<R, T>(string relativeUrl)
        {
            return new ApiRequest<R, T>(_client, $"{_authService.BaseUrl}{relativeUrl}", HttpMethod.Head);
        }
        public ApiRequest<R, R> Head<R>(string relativeUrl) { return Head<R, R>(relativeUrl); }


        public ApiRequest<R, T> Post<R, T>(string relativeUrl)
        {
            return new ApiRequest<R, T>(_client, $"{_authService.BaseUrl}{relativeUrl}", HttpMethod.Post);
        }
        public ApiRequest<R, R> Post<R>(string relativeUrl) { return Post<R, R>(relativeUrl); }


        public ApiRequest<R, T> Put<R, T>(string relativeUrl)
        {
            return new ApiRequest<R, T>(_client, $"{_authService.BaseUrl}{relativeUrl}", HttpMethod.Put);
        }
        public ApiRequest<R, R> Put<R>(string relativeUrl) { return Put<R, R>(relativeUrl); }

        

        public ApiRequest<R, T> Delete<R, T>(string relativeUrl)
        {
            return new ApiRequest<R, T>(_client, $"{_authService.BaseUrl}{relativeUrl}", HttpMethod.Delete);
        }
        public ApiRequest<R, R> Delete<R>(string relativeUrl) { return Delete<R, R>(relativeUrl); }


        public ApiRequest<R, T> Options<R, T>(string relativeUrl)
        {
            return new ApiRequest<R, T>(_client, $"{_authService.BaseUrl}{relativeUrl}", HttpMethod.Options);
        }
        public ApiRequest<R, R> Options<R>(string relativeUrl) { return Options<R, R>(relativeUrl); }

    }
}
