using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Readible.Hub
{
    public interface IDataContextHub<T>
    {
        Task Store(T data);
        Task Update(T data);
        Task Delete(int id);
    }

    public interface IDataContextUserHub<T>
    {
        Task Store(string connectId, T data);
        Task Update(string connectId, T data);
        Task Delete(string connectId, int id);
    }
}
