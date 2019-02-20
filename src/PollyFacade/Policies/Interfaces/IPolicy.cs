using System;
using System.Threading.Tasks;

namespace PollyFacade.Policies.Interfaces
{
    public interface IPolicy<T>
    {
        Task<T> ExecuteAsync(Func<Task<T>> action);
    }
}
