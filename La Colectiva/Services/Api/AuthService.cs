using La_Colectiva.Models.Api;
using La_Colectiva.Services.Api.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Services.Api
{
    public interface AuthService
    {
        string Token { get; }
        bool IsAuthenticated { get; }
        string BaseUrl { get; }
        Uri BaseUri { get; }
        void SetBaseUrl(string url);
        ApiRequest<LoginResponse, LoginResponse> Login(string email, string password, string baseUrl);
        void Logout();
    }
}
