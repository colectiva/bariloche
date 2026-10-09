using La_Colectiva.Models.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Services.Local.Databases
{
    public interface LocalRepository<T, ID> {

        Task<T> Save(T data);
        Task<IEnumerable<T>> Save(IEnumerable<T> data);

        Task<T> GetById(ID id);

        Task<PageBuilder<T>> Search(string q, int page, int size);
        Task<List<T>> List();

    }
}
