using System.Collections.Generic;
using System.Threading.Tasks;

namespace Readible.Services
{
    public interface IDataContextService<T>
    {
        Task<IEnumerable<T>> Fetch();
        Task<int> Count();
        Task<T> Get(int id);
        Task<T> Get(string id);
        Task<T> Store(T data);
        Task<T> Update(int id, T data);
        Task<T> Update(string id, T data);
        Task<T> Delete(int id);
        Task<T> Delete(string id);
    }
}