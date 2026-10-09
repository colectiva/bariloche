using La_Colectiva.Models.Api;
using La_Colectiva.Models.Classic;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace La_Colectiva.Services.Local.Databases.Impl
{
    public class ViaRepositoryImpl : ViaRepository
    {
        private SQLiteAsyncConnection db;
        public ViaRepositoryImpl() { }

        public bool NeedsUpdate(ViaDto local, ViaDto retrieved)
        {
            return !local.UpdatedAt.Equals(retrieved.UpdatedAt);
        }

        public async Task<ViaDto> GetById(int id) {
            await Init();
            return await db.Table<ViaDto>().Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<List<ViaDto>> List()
        {
            await Init();
            var query = db.Table<ViaDto>();
            return await query.ToListAsync();
        }

        public async Task<ViaDto> Save(ViaDto data)
        {
            await Init();
            var local = await GetById(data.Id);
            if (local != null && !NeedsUpdate(local, data)) return local;

            await db.InsertOrReplaceAsync(data);
            return data;
        }

        public async Task<IEnumerable<ViaDto>> Save(IEnumerable<ViaDto> data)
        {
            await Init();
            await db.InsertOrReplaceAsync(data);
            return data;
        }

        public async Task<PageBuilder<ViaDto>> Search(string q, int page, int size) {
            await Init();
            var query = db.Table<ViaDto>()
                .Where(x => (x.Name.ToUpper().Contains(q.ToUpper())));
            int totalElements = await query.CountAsync();
            var items = await query
                .Skip(page * size)
                .Take(size)
                .ToListAsync();
            return new PageBuilder<ViaDto>()
                .Content(items)
                .Page(page)
                .Size(size)
                .TotalElements(totalElements);
        }

        private async Task Init() {
            if (db != null) return;
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "app_data.db3");
            db = new SQLiteAsyncConnection(dbPath);
            await db.CreateTableAsync<ViaDto>();
        }



    }
}
