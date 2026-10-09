using La_Colectiva.Models.Api;
using La_Colectiva.Models.Classic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Services.Classic
{
    public interface NetworkStationService {
        ApiRequest<SpringPageRes<EmbeddedNetworkStations>, Page<NetworkStation>> SearchSync(int page, int size);
        ApiRequest<NetworkStation, NetworkStation> FindById(long id);
    }
}
