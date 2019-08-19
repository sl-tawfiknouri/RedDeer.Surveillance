namespace PollyFacade.Policies.Interfaces
{
    using System;

    public interface IPolicyFactory
    {
        IPolicy<T> PolicyTimeoutGeneric<T>(TimeSpan timeout, Func<T, bool> predicate, int retries, TimeSpan retryWait);
    }
}