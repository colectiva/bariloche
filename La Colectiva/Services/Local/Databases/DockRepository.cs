using La_Colectiva.Models.Api;
using La_Colectiva.Models.Classic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Services.Local.Databases
{
    public interface DockRepository : LocalRepository<DockDto, int> {
        Task<PageBuilder<DockDto>> Search(string q, double lat, double lon, int page, int size);
        Task AddBookmark(int dockId);
        Task DeleteBookmark(int dockId);
        Task<List<DockDto>> GetBookmarkedDocks();
        Task PopulateVias(List<DockDto> docks);
    }
}
