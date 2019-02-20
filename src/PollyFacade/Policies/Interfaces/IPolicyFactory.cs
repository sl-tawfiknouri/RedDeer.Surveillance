using System;

namespace PollyFacade.Policies.Interfaces
{
    public interface IPolicyFactory
    {
        IPolicy<T> PolicyTimeoutGeneric<T>(TimeSpan timeout, Func<T, bool> predicate, int retries, TimeSpan retryWait);
    }
}