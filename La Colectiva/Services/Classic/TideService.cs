using La_Colectiva.Models.Api;
using La_Colectiva.Models.Classic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Services.Classic
{
    public interface TideService
    {
        ApiRequest<EmbbeddedRes<EmbeddedTides>, List<Tide>> TopData(long stationId);
        ApiRequest<EmbbeddedRes<EmbeddedTides>, List<Tide>> TodayForecast(long stationId);
        ApiRequest<EmbbeddedRes<EmbeddedTides>, List<Tide>> Forecast(long stationId, DateTime startDateTime, DateTime endDateTime);
    }
}
