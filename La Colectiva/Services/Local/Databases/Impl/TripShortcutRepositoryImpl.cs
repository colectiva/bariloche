using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using La_Colectiva.Models.Api;
using La_Colectiva.Models.Local;
using Microsoft.Extensions.DependencyInjection;
using SQLite;

namespace La_Colectiva.Services.Local.Databases.Impl
{
    public class TripShortcutRepositoryImpl : TripShortcutRepository
    {

        private readonly DatabaseService dbService = App.Current.Services.GetRequiredService<DatabaseService>();
        private readonly DockRepository dockRepository = App.Current.Services.GetRequiredService<DockRepository>();
        private SQLiteAsyncConnection db => dbService.db;

        public TripShortcutRepositoryImpl() { }

        private async Task Init()
        {
            if (db != null) return;
            await dbService.Init();
        }

        public async Task<TripShortcutDto> GetById(string id)
        {
            await Init();
            return await db.Table<TripShortcutDto>().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<TripShortcutDto>> List()
        {
            await Init();

            var shortcuts = await db.Table<TripShortcutDto>()
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            if (!shortcuts.Any()) return shortcuts;
            
            var dockIds = shortcuts.Select(s => s.FromId)
                .Concat(shortcuts.Select(s => s.ToId))
                .Where(id => id > 0)
                .Distinct()
                .ToList();
            var dlist = await dockRepository.List();
            await dockRepository.PopulateVias(dlist);
            var docks = (dlist)
                .Where(d => dockIds.Contains(d.Id))
                .ToDictionary(d => d.Id);
            
            foreach (var shortcut in shortcuts)
            {
                if (docks.TryGetValue(shortcut.FromId, out var fromDock))
                    shortcut.From = fromDock;

                if (docks.TryGetValue(shortcut.ToId, out var toDock))
                    shortcut.To = toDock;
            }

            return shortcuts;
        }

        public async Task<TripShortcutDto> Save(TripShortcutDto data)
        {
            await Init();
            if (string.IsNullOrEmpty(data.Id) && data.FromId > 0 && data.ToId > 0)
            {
                data.Id = TripShortcutDto.BuildId(data.FromId, data.ToId);
            }
            data.CreatedAt = DateTime.UtcNow;
            await db.InsertOrReplaceAsync(data);
            return data;
        }

        public async Task<IEnumerable<TripShortcutDto>> Save(IEnumerable<TripShortcutDto> data)
        {
            foreach (var item in data)
            {
                await Save(item);
            }
            return data;
        }

        public async Task Delete(int fromId, int toId)
        {
            await Init();
            string id = TripShortcutDto.BuildId(fromId, toId);
            await db.DeleteAsync<TripShortcutDto>(id);
        }

        [Obsolete]
        public async Task<PageBuilder<TripShortcutDto>> Search(string q, int page, int size)
        {
            throw new NotImplementedException();
        }
    }
}
