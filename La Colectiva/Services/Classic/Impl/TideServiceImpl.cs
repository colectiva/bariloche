using La_Colectiva.Models.Api;
using La_Colectiva.Models.Classic;
using La_Colectiva.Services.Api;
using La_Colectiva.Services.Api.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Services.Classic.Impl
{
    public class TideServiceImpl : TideService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;
        private readonly ApiService _apiService;

        public TideServiceImpl(HttpClient httpClient, AuthService authService)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _authService = authService;
            _apiService = new ApiServiceImpl(_httpClient, _authService);
        }

        public ApiRequest<EmbbeddedRes<EmbeddedTides>, List<Tide>> TopData(long stationId) {
            return _apiService
                .Get<EmbbeddedRes<EmbeddedTides>>("/api/tides/search/topData")
                .Query(new { projection = "full", stationId })
                .Map(res => res.Data.Tides)
                .Json().Debug();
        }
        public ApiRequest<EmbbeddedRes<EmbeddedTides>, List<Tide>> TodayForecast(long stationId)
        {
            string start = DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss");
            string end = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
            return _apiService
                .Get<EmbbeddedRes<EmbeddedTides>>("/api/tides/search/todayForecast")
                .Query(new {
                    stationId,
                    start,
                    end,
                    projection = "full"
                })
                .Map(res => res.Data.Tides)
                .Json().Debug();
        }

        public ApiRequest<EmbbeddedRes<EmbeddedTides>, List<Tide>> Forecast(long stationId, DateTime startDateTime, DateTime endDateTime)
        {
            string start = startDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            string end = endDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            return _apiService
                .Get<EmbbeddedRes<EmbeddedTides>>("/api/tides/search/todayForecast")
                .Query(new
                {
                    stationId,
                    start,
                    end,
                    projection = "full"
                })
                .Map(res => res.Data.Tides)
                .Json().Debug();
        }

    }
}
