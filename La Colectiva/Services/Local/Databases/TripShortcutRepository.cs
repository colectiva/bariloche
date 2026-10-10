using La_Colectiva.Models.Local;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Services.Local.Databases
{
    public interface TripShortcutRepository : LocalRepository<TripShortcutDto, string>
    {
        Task Delete(int fromId, int toId);
    }
}
