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
    public class NetworkStationServiceImpl : NetworkStationService {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;
        private readonly ApiService _apiService;

        public NetworkStationServiceImpl(HttpClient httpClient, AuthService authService) {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _authService = authService;
            _apiService = new ApiServiceImpl(_httpClient, _authService);
        }


        public ApiRequest<SpringPageRes<EmbeddedNetworkStations>, Page<NetworkStation>> SearchSync(int page, int size) {
            return _apiService
                .Get<SpringPageRes<EmbeddedNetworkStations>>("/api/networkStations/search/findByStationsToSync_SyncTrue")
                .Query(new { projection = "full", page, size })
                .Map(Page<NetworkStation>.FromEmbedded<NetworkStation, EmbeddedNetworkStations>(x => x))
                .Json().Debug();
        }

        public ApiRequest<NetworkStation, NetworkStation> FindById(long id) {
            return _apiService
                .Get<NetworkStation>($"/api/networkStations/{id}")
                .Query(new { projection = "full" })
                .Json().Debug();
        }


    }
}
