using System;
using Polly;
using PollyFacade.Policies.Interfaces;

namespace PollyFacade.Policies
{
    public class PolicyFactory : IPolicyFactory
    {
        public IPolicy<T> PolicyTimeoutGeneric<T>(TimeSpan timeout, Func<T, bool> predicate, int retries, TimeSpan retryWait)
        {
            if (retries < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(retries));
            }

            var timeoutPolicy = Policy.TimeoutAsync<T>(timeout);

            var retryPolicy =
                Policy
                    .Handle<Exception>()
                    .OrResult<T>(predicate)
                    .WaitAndRetryAsync(retries, i => retryWait);

            var policyWrap = Policy.WrapAsync(timeoutPolicy, retryPolicy);

            return new PollyPolicy<T>(policyWrap);
        }
    }
}
