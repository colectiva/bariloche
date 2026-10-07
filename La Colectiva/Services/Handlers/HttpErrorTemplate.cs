using La_Colectiva.Models.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Services.Handlers
{
    public class HttpErrorTemplate
    {
        private readonly Dictionary<HttpStatusCode, ErrorWindowData> _templates = new Dictionary<HttpStatusCode, ErrorWindowData>();
        private ErrorWindowData _defaultData = new ErrorWindowData() { Title = "Error inesperado" };

        public static HttpErrorTemplate Default()
        {
            var template = new HttpErrorTemplate();

            template.On(
                HttpStatusCode.NotFound,
                new ErrorWindowData
                {
                    Title = "Recurso no encontrado",
                    Subtitle = "El recurso solicitado no fue encontrado en la base de datos. ",
                    Icon = "\uE9CE"
                }
            );
            template.On(
                HttpStatusCode.Forbidden,
                new ErrorWindowData
                {
                    Title = "Acceso denegado",
                    Subtitle = "No tiene los permisos suficientes para realizar esta acción. ",
                    Icon = "\uE8D7"
                }
            );
            template.On(
                HttpStatusCode.Unauthorized,
                new ErrorWindowData
                {
                    Title = "No autorizado",
                    Subtitle = "Necesita autenticarse para realizar esta acción. ",
                    Icon = "\uEA18"
                }
            );
            template.On(
                HttpStatusCode.InternalServerError,
                new ErrorWindowData
                {
                    Title = "Error interno",
                    Subtitle = "Ocurrió un error en el servidor. Intente de nuevo más tarde",
                    IconColor = "#D90000",
                    Icon = "\uE783"
                }
            );

            return template;
        }

        public HttpErrorTemplate NotFound(ErrorWindowData data) => On(HttpStatusCode.NotFound, data);
        public HttpErrorTemplate Forbidden(ErrorWindowData data) => On(HttpStatusCode.Forbidden, data);
        public HttpErrorTemplate Unauthorized(ErrorWindowData data) => On(HttpStatusCode.Unauthorized, data);

        public HttpErrorTemplate Fallback(ErrorWindowData data)
        {
            _defaultData = data;
            return this;
        }

        public HttpErrorTemplate On(HttpStatusCode? code, ErrorWindowData data)
        {
            if (code == null)
            {
                _defaultData = data;
                return this;
            }
            _templates[code ?? HttpStatusCode.Continue] = data;
            return this;
        }

        public HttpErrorTemplate With(ApiFatalErrorResponse err)
        {
            if (err.Message == null) return this;
            if (err.Message.Contains("Json"))
            {
                On(null, new ErrorWindowData
                {
                    Title = "Error al procesar la respuesta",
                    Subtitle = "El servidor respondió mal, o en un idioma que no conocemos. "
                });
            }
            if (err.Message.Contains("HttpRequestException"))
            {
                On(null, new ErrorWindowData
                {
                    Title = "Error de conexión",
                    Subtitle = "Hubo un error al intentar conectarse al servidor. "
                });
            }
            return this;
        }

        public ErrorWindowData GetLocalError()
        {
            return _defaultData;
        }

        public ErrorWindowData Get(HttpStatusCode code)
        {
            return _templates.TryGetValue(code, out var data) ? data : _defaultData;
        }
    }
}
