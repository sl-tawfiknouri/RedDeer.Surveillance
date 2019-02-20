using System;
using System.Threading.Tasks;
using DataSynchroniser.Api.Policies.Interfaces;
using Polly.Wrap;

namespace DataSynchroniser.Api.Policies
{
    public class PollyPolicy<T> : IPolicy<T>
    {
        private readonly AsyncPolicyWrap<T> _policy;

        public PollyPolicy(AsyncPolicyWrap<T>policy)
        {
            _policy = policy ?? throw new ArgumentNullException(nameof(policy));
        }

        public async Task<T> ExecuteAsync(Func<Task<T>> action)
        {
            return await _policy.ExecuteAsync(action);
        }
    }
}
