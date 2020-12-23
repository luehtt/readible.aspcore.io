using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Readible.services.hub
{
    public interface IHubContextService<T>
    {
        Task Store(T data);
        Task Update(T data);
        Task Delete(int id);
    }
}
