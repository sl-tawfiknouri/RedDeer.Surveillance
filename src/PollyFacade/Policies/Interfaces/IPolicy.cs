namespace PollyFacade.Policies.Interfaces
{
    using System;
    using System.Threading.Tasks;

    public interface IPolicy<T>
    {
        Task<T> ExecuteAsync(Func<Task<T>> action);
    }
}