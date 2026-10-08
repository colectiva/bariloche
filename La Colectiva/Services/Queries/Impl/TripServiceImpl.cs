using La_Colectiva.Models.Api;
using La_Colectiva.Models.Queries;
using La_Colectiva.Services.Api;
using La_Colectiva.Services.Api.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Services.Queries.Impl
{
    public class TripServiceImpl: TripService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;
        private readonly ApiService _apiService;

        public TripServiceImpl(HttpClient httpClient, AuthService authService)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _authService = authService;
            _apiService = new ApiServiceImpl(_httpClient, _authService);
        }

        public ApiRequest<List<NextTripDto>, List<NextTrip>> FindNextTrip(NextTripRequest request)
        {
            return _apiService
                .Get<List<NextTripDto>>($"/api/next-trips")
                .Query(new
                {
                    origin = request.OriginId,
                    destination = request.DestinationId,
                    time = request.Time.ToString(@"hh\:mm"),
                    weekday = request.Weekday,
                    date = request.Date.ToString("yyyy-MM-dd")
                })
                .Json()
                .Debug()
                .Map(arr => {
                    var list = new List<NextTrip>();
                    foreach (var item in arr)
                    {
                        list.Add(new NextTrip(item));
                    }
                    return list;
                });
        }

    }
}
