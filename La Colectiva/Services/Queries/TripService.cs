using La_Colectiva.Models.Api;
using La_Colectiva.Models.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Services.Queries
{
    public interface TripService
    {
        ApiRequest<List<NextTripDto>, List<NextTrip>> FindNextTrip(NextTripRequest request);
    }
}
