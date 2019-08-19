namespace PollyFacade.Policies
{
    using System;
    using System.Threading.Tasks;

    using Polly.Wrap;

    using PollyFacade.Policies.Interfaces;

    public class PollyPolicy<T> : IPolicy<T>
    {
        private readonly AsyncPolicyWrap<T> _policy;

        public PollyPolicy(AsyncPolicyWrap<T> policy)
        {
            this._policy = policy ?? throw new ArgumentNullException(nameof(policy));
        }

        public async Task<T> ExecuteAsync(Func<Task<T>> action)
        {
            return await this._policy.ExecuteAsync(action);
        }
    }
}