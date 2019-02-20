using System;
using System.Threading.Tasks;

namespace DataSynchroniser.Api.Policies.Interfaces
{
    public interface IPolicy<T>
    {
        Task<T> ExecuteAsync(Func<Task<T>> action);
    }
}
