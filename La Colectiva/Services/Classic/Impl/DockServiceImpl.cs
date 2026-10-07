using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using La_Colectiva.Core;
using La_Colectiva.Models.Api;
using La_Colectiva.Models.Classic;
using La_Colectiva.Services.Api;
using La_Colectiva.Services.Api.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace La_Colectiva.Services.Classic.Impl
{
    public class DockServiceImpl : DockService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;
        private readonly ApiService _apiService;

        public DockServiceImpl(HttpClient httpClient, AuthService authService) {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _authService = authService;
            _apiService = new ApiServiceImpl(_httpClient, _authService);
        }

        public ApiRequest<SpringPageRes<EmbeddedDocks>, Page<Dock>> FindNearestDocks(double lat, double lon, int count)
        {
            string latitude = lat.ToString(CultureInfo.InvariantCulture);
            string longitude = lon.ToString(CultureInfo.InvariantCulture);
            return _apiService
                .Get<SpringPageRes<EmbeddedDocks>>("/api/docks/search/findNearby")
                .Query(new { lat = latitude, lng = longitude, size = 5, projection = "populated" })
                .Debug()
                .Map(raw => {
                    var page = new Page<Dock>();
                    foreach (var dock in raw.Embedded.Docks)
                    {
                        if (dock != null)
                        {
                            var ds = Dock.Create(dock);
                            ds.Distance = new Distance(GeoCalculator.GetDistance(lat, lon, ds.Latitude, ds.Longitude));
                            page.Content.Add(ds);
                        }
                    }
                    page.Pageable = raw.Page;
                    return page;
                });
        }

        

        public ApiRequest<SpringPageRes<EmbeddedDocks>, Page<Dock>> Search(string q, double lat, double lon, int p, int size)
        {
            string latitude = lat.ToString(CultureInfo.InvariantCulture);
            string longitude = lon.ToString(CultureInfo.InvariantCulture);
            return _apiService
                .Get<SpringPageRes<EmbeddedDocks>>("/api/docks/search/findNearby")
                .Query(new { lat = latitude, lng = longitude, page = p, size, projection = "populated", q })
                .Debug()
                .Map(raw => {
                    var page = new Page<Dock>();
                    foreach (var dock in raw.Embedded.Docks)
                    {
                        if (dock != null)
                        {
                            var ds = Dock.Create(dock);
                            ds.Distance = new Distance(GeoCalculator.GetDistance(lat, lon, ds.Latitude, ds.Longitude));
                            page.Content.Add(ds);
                        }
                    }
                    page.Pageable = raw.Page;
                    return page;
                });
        }

    }
}
