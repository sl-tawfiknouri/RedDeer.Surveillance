using System;

namespace DataSynchroniser.Api.Policies.Interfaces
{
    public interface IPolicyFactory
    {
        IPolicy<T> PolicyTimeoutGeneric<T>(TimeSpan timeout, Func<T, bool> predicate, int retries, TimeSpan retryWait);
    }
}