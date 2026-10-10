using La_Colectiva.Models.Api;
using La_Colectiva.Models.Classic;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace La_Colectiva.Services.Local.Databases.Impl
{
    public class DockRepositoryImpl : DockRepository {

        private readonly DatabaseService dbService = App.Current.Services.GetRequiredService<DatabaseService>();
        private readonly ViaRepository viaRepository = App.Current.Services.GetRequiredService<ViaRepository>();
        private SQLiteAsyncConnection db => dbService.db;
        public DockRepositoryImpl() {

        }
        private async Task Init() {
            if (db != null) return;
            await dbService.Init();
        }

        public async Task AddBookmark(int dockId)
        {
            await Init();
            var bookmark = new BookmarkDockDto { DockId = dockId, BookmarkedAt = DateTime.UtcNow };
            await db.InsertOrReplaceAsync(bookmark);
        }

        public async Task DeleteBookmark(int dockId)
        {
            await Init();
            await db.DeleteAsync<BookmarkDockDto>(dockId);
        }
        
        private async Task PopulateBookmarks(List<DockDto> docks)
        {
            if (docks == null || !docks.Any()) return;
            
            var bookmarks = await db.Table<BookmarkDockDto>().ToListAsync();
            var bookmarkedIds = new HashSet<int>(bookmarks.Select(b => b.DockId));

            foreach (var dock in docks)
            {
                dock.IsBookmarked = bookmarkedIds.Contains(dock.Id);
            }
        }

        public bool NeedsUpdate(DockDto local, DockDto retrieved) {
            return !local.UpdatedAt.Equals(retrieved.UpdatedAt);
        }

        public async Task<DockDto> Save(DockDto data) {
            await Init();
            var local = await GetById(data.Id);
            if (local != null && !NeedsUpdate(local, data)) return local;

            if (data.Via != null) {
                data.Via = await viaRepository.Save(data.Via);
                data.ViaId = data.Via.Id;
            } else if (data.ViaId > 0) {
                data.Via = await viaRepository.Save(new ViaDto() {
                    Id = data.ViaId,
                    UpdatedAt = DateTime.MinValue
                });
                data.ViaId = data.Via.Id;
            }
            await db.InsertOrReplaceAsync(data);
            return data;
        }

        public async Task<IEnumerable<DockDto>> Save(IEnumerable<DockDto> data) {
            foreach (var item in data) {
                await Save(item);
            }
            return data;
        }

        public async Task<PageBuilder<DockDto>> Search(string q, int page, int size)
        {
            await Init();

            var query = db.Table<DockDto>().Where(x => x.Name.ToUpper().Contains(q.ToUpper()));
            int totalElements = await query.CountAsync();
            var allItems = await query.ToListAsync();

            await PopulateBookmarks(allItems);

            var items = allItems
                .OrderByDescending(x => x.IsBookmarked)
                .Skip(page * size)
                .Take(size)
                .ToList();

            await PopulateVias(items);

            return new PageBuilder<DockDto>()
                .Content(items)
                .Page(page)
                .Size(size)
                .TotalElements(totalElements);
        }

        public async Task PopulateVias(List<DockDto> docks)
        {
            if (docks == null || !docks.Any()) return;
            
            var viaIds = docks.Select(d => d.ViaId).Where(id => id > 0).Distinct().ToList();
            if (!viaIds.Any()) return;
            
            var vias = (await viaRepository.List())
                .Where(v => viaIds.Contains(v.Id))
                .ToDictionary(v => v.Id);
            
            foreach (var dock in docks)
            {
                if (dock.ViaId > 0 && vias.TryGetValue(dock.ViaId, out var via))
                {
                    dock.Via = via;
                }
            }
        }

        public async Task<PageBuilder<DockDto>> Search(string q, double lat, double lon, int page, int size)
        {
            await Init();
            
            var query = db.Table<DockDto>().Where(x => x.Name.ToUpper().Contains(q.ToUpper()));
            int totalElements = await query.CountAsync();
            var allItems = await query.ToListAsync();
            
            await PopulateBookmarks(allItems);
            
            var items = allItems
                .OrderByDescending(x => x.IsBookmarked)
                .ThenBy(x =>
                    ((x.Latitude - lat) * (x.Latitude - lat)) +
                    ((x.Longitude - lon) * (x.Longitude - lon))
                )
                .Skip(page * size)
                .Take(size)
                .ToList();
            
            await PopulateVias(items);

            return new PageBuilder<DockDto>()
                .Content(items)
                .Page(page)
                .Size(size)
                .TotalElements(totalElements);
        }

        public async Task<List<DockDto>> List()
        {
            await Init();
            var query = db.Table<DockDto>();
            return await query.ToListAsync();
        }

        public async Task<DockDto> GetById(int id)
        {
            await Init();
            return await db.Table<DockDto>().Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
        }


        public async Task<List<DockDto>> GetBookmarkedDocks()
        {
            await Init();
            
            var bookmarks = await db.Table<BookmarkDockDto>().OrderByDescending(b => b.BookmarkedAt).ToListAsync();

            if (bookmarks == null || !bookmarks.Any())
                return new List<DockDto>();

            var bookmarkedIds = bookmarks.Select(b => b.DockId).ToList();
            
            var allDocks = await db.Table<DockDto>().ToListAsync();
            var favoriteDocks = allDocks
                .Where(d => bookmarkedIds.Contains(d.Id))
                .ToList();
            
            foreach (var dock in favoriteDocks)
            {
                dock.IsBookmarked = true;
            }

            await PopulateVias(favoriteDocks);

            return favoriteDocks;
        }

    }
}
