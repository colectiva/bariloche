using La_Colectiva.Models.Api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Services.Api.Impl
{
    public class LoginRequest
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }

    public class AuthServiceImpl : AuthService
    {
        private readonly HttpClient _httpClient;
        public bool IsAuthenticated { get; private set; }
        public string Token { get; private set; }

        public string BaseUrl { get; private set; } = "http://192.168.0.236:8081";
        public Uri BaseUri => new Uri(BaseUrl);

        public void SetBaseUrl(string url)
        {
            BaseUrl = url.TrimEnd('/');
        }

        public AuthServiceImpl(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public ApiRequest<LoginResponse, LoginResponse> Login(string email, string password, string baseUrl)
        {
            return ApiRequest
                .Create<LoginResponse, LoginResponse>(_httpClient, $"{baseUrl}/api/login", HttpMethod.Post)
                .SetPayload(new LoginRequest { Email = email, Password = password })
                .Json()
                .Debug()
                .Map<LoginResponse>(res => {
                    if (res?.Token != null)
                    {
                        Token = res.Token;
                        SetBaseUrl(baseUrl);
                        IsAuthenticated = true;
                    }
                    return res;
                });

        }


        public void Logout()
        {
            Token = null;
        }
    }
}
