using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Models.Api
{
    public class EmbbeddedRes<T>
    {
        [JsonProperty("_embedded")]
        public T Data { get; set; }
    }

    public class ErrorWindowData
    {
        public string Icon { get; set; } = "\uE7BA";
        public string IconColor { get; set; } = "#D98A00";
        public string Title { get; set; } = "Error";
        public string Subtitle { get; set; } = string.Empty;

        public ErrorWindowData() { }
    }


    public interface IEmbedded<T>
    {
        List<T> Items { get; set; }
    }

    public class Page<T>
    {
        public List<T> Content { get; set; } = new List<T>();
        public PageData Pageable { get; set; }

        public Page() { }

        public static Page<T> Mapper<E>(SpringPageRes<IEmbedded<E>> data, Func<E, T> mapper)
        {
            var page = new Page<T>();
            page.Pageable = data.Page;
            if (data.Embedded != null && data.Embedded.Items != null)
            {
                foreach (var item in data.Embedded.Items)
                {
                    page.Content.Add(mapper(item));
                }
            }
            return page;
        }

        public static Page<T> Mapper<E, TEmbedded>(SpringPageRes<TEmbedded> data, Func<E, T> mapper)
        where TEmbedded : IEmbedded<E>
        {
            var page = new Page<T> { Pageable = data?.Page };
            if (data?.Embedded?.Items != null)
            {
                foreach (var item in data.Embedded.Items)
                {
                    page.Content.Add(mapper(item));
                }
            }
            return page;
        }

        

        // 2. Retornando la función para simplificar el .Map(...)
        public static Func<SpringPageRes<TEmbedded>, Page<T>> FromEmbedded<E, TEmbedded>(Func<E, T> mapper)
        where TEmbedded : IEmbedded<E>
        {
            return data => Mapper(data, mapper);
        }

        public static Func<SpringControlledPageRes<E>, Page<T>> MapControlled<E>(Func<E, T> mapper)
        {
            return data => {
                var page = new Page<T>();
                page.Pageable = data?.Page;
                if (data?.Content != null)
                {
                    foreach (var item in data.Content)
                    {
                        page.Content.Add(mapper(item));
                    }
                }
                return page;
            };
        }

    }

    public class PageData
    {
        public int Number { get; set; }
        public int Size { get; set; }
        public int TotalElements { get; set; }
        public int TotalPages { get; set; }
        public PageData() { }

        public override string ToString()
        {
            return $"PageData {{ Number = {Number}, Size = {Size}, TotalElements = {TotalElements}, TotalPages = {TotalPages} }}";
        }
    }

    public class PageBuilder<T>
    {
        private readonly Page<T> _page = new Page<T>();
        private readonly PageData _pageData = new PageData();

        public PageBuilder<T> Content(IEnumerable<T> items)
        {
            if (items != null)
            {
                _page.Content = items.ToList();
            }
            return this;
        }

        public PageBuilder<T> Page(int number)
        {
            _pageData.Number = number;
            return this;
        }

        public PageBuilder<T> Size(int size)
        {
            _pageData.Size = size;
            return this;
        }

        public PageBuilder<T> TotalElements(int totalElements)
        {
            _pageData.TotalElements = totalElements;
            return this;
        }

        public PageBuilder<T> TotalPages(int totalPages)
        {
            _pageData.TotalPages = totalPages;
            return this;
        }

        public PageBuilder<T> Pageable(PageData pageData)
        {
            if (pageData != null)
            {
                _pageData.Number = pageData.Number;
                _pageData.Size = pageData.Size;
                _pageData.TotalElements = pageData.TotalElements;
                _pageData.TotalPages = pageData.TotalPages;
            }
            return this;
        }

        public Page<T> Build()
        {
            // Auto-calcula páginas totales si no se asignó explícitamente
            if (_pageData.TotalPages == 0 && _pageData.Size > 0 && _pageData.TotalElements > 0)
            {
                _pageData.TotalPages = (int)Math.Ceiling((double)_pageData.TotalElements / _pageData.Size);
            }

            _page.Pageable = _pageData;
            return _page;
        }

        public SpringPageRes<E> BuildSpringPageRes<E>()
            where E : IEmbedded<T>, new()
        {
            var page = Build();
            var s = new SpringPageRes<E>();
            s.Page = page.Pageable;
            s.Embedded = new E();
            s.Embedded.Items = page.Content;
            return s;
        }

        public SpringControlledPageRes<T> BuildSpringControlledPageRes()
        {
            var page = Build();
            var s = new SpringControlledPageRes<T>() {
                Content = page.Content,
                Empty = page.Content.Count == 0,
                First = page.Pageable.Number == 1,
                Last = page.Pageable.Number == page.Pageable.TotalPages,
                Number = page.Pageable.Number,
                NumberOfElements = page.Content.Count,
                Size = page.Pageable.Size,
                TotalElements = page.Pageable.TotalElements,
                TotalPages = page.Pageable.TotalPages
            };
            return s;
        }

    }

    public class SpringPageRes<E>
    {
        [JsonProperty("_embedded")]
        public E Embedded { get; set; }
        [JsonProperty("page")]
        public PageData Page { get; set; }
        public SpringPageRes() { }

    }

    public class SpringControlledPageRes<E>
    {
        public List<E> Content { get; set; } = new List<E>();
        public bool Empty { get; set; }
        public bool First { get; set; }
        public bool Last { get; set; }
        public int Number { get; set; }
        public int NumberOfElements { get; set; }
        public int Size { get; set; }
        public int TotalElements { get; set; }
        public int TotalPages { get; set; }

        [JsonIgnore]
        public PageData Page {
            get {
                var p = new PageData()
                {
                    Number = this.Number,
                    Size = this.Size,
                    TotalElements = this.TotalElements,
                    TotalPages = this.TotalPages
                };
                Debug.WriteLine($"129A: {p}");
                return p;
            }
        }

    }

    public class SpringRes<E>
    {
        [JsonProperty("_embedded")]
        public E Embedded { get; set; }

        public SpringRes() { }
    }





    public class ApiUnprocessableEntityErrorResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("errors")]
        public Dictionary<string, string[]> Errors { get; set; }
    }

    public class ApiFatalErrorResponse
    {

        [JsonIgnore]
        public HttpStatusCode? Status { get; set; }

        public DateTime Timestamp { get; set; }

        public string Error { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        public string Path { get; set; }

        [JsonProperty("trace")]
        public string Trace { get; set; }



        public ApiFatalErrorResponse() { }

        public string PrintFullError()
        {
            return new StringBuilder()
                .AppendLine($"Código de error: {Status}")
                .AppendLine($"Error: {Error ?? "No informado."}")
                .AppendLine($"Mensaje: {Message ?? "No informado."}")
                .AppendLine(Timestamp.ToString())
                .AppendLine($"Origen: {Path ?? "No informado."}")
                .AppendLine($"Traza: ")
                .Append(Trace ?? "")
                .ToString();
        }

    }

    public class ApiErrorTrace
    {
        [JsonProperty("file")]
        public string File { get; set; }

        [JsonProperty("line")]
        public int Line { get; set; }

        [JsonProperty("function")]
        public string Function { get; set; }

        [JsonProperty("class")]
        public string Class { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        public override string ToString()
        {
            var location = File != null ? $"{File}:{Line}" : "Ubicación desconocida";
            var method = Class != null ? $"{Class}.{Function}()" : Function;

            return $"[{Type ?? "Error"}] {method} en {location}";
        }

    }
    public class ApiResponse<T>
    {

        [JsonProperty("data")]
        public T Data { get; set; }

        public ApiResponse() { }

    }
}
