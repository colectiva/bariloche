using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Services.Local.Databases
{
    public interface DatabaseService {
        SQLiteAsyncConnection db { get; }
        Task Init();
        Task DropData();
    }
}
