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
    public class DatabaseServiceImpl : DatabaseService
    {
        public SQLiteAsyncConnection db { get; private set; }
        public DatabaseServiceImpl()
        {

        }
        public async Task Init()
        {
            if (db != null) return;
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "app_data.db3");
            db = new SQLiteAsyncConnection(dbPath);

            await db.CreateTableAsync<DockDto>();
            await db.CreateTableAsync<ViaDto>();
            await db.CreateTableAsync<BookmarkDockDto>();
        }
        public async Task DropData()
        {
            await Init();

            await db.DropTableAsync<DockDto>();
            await db.DropTableAsync<ViaDto>();
            await db.DropTableAsync<BookmarkDockDto>();

            await db.CreateTableAsync<DockDto>();
            await db.CreateTableAsync<ViaDto>();
            await db.CreateTableAsync<BookmarkDockDto>();

            return;
        }
    }
}
