using La_Colectiva.Models.Api;
using La_Colectiva.Models.Classic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Services.Classic
{
    public interface DockService
    {
        ApiRequest<SpringPageRes<EmbeddedDocks>, Page<Dock>> Search(string q, double lat, double lon, int page, int size);
        ApiRequest<SpringPageRes<EmbeddedDocks>, Page<Dock>> FindNearestDocks(double lat, double lon, int count);
    }
}
