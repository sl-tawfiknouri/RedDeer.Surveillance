using System;
using Surveillance.Factories;
using Surveillance.Rules.HighProfits.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.Multiverse.Interfaces;

// ReSharper disable AssignNullToNotNullAttribute
namespace Surveillance.Rules.HighProfits
{
    /// <summary>
    /// Calculate the profits from the trade
    /// </summary>
    public class HighProfitsRule : IHighProfitRule
    {
        private readonly IHighProfitStreamRule _streamRule;
        private readonly IMarketCloseMultiverseTransformer _multiverseTransformer;

        public HighProfitsRule(
            IHighProfitStreamRule streamRule,
            IMarketCloseMultiverseTransformer multiverseTransformer)
        {
            _streamRule = streamRule ?? throw new ArgumentNullException(nameof(streamRule));
            _multiverseTransformer =
                multiverseTransformer
                ?? throw new ArgumentNullException(nameof(multiverseTransformer));
        }

        public void OnCompleted()
        {
            _streamRule.OnCompleted();
            _multiverseTransformer.OnCompleted();
        }

        public void OnError(Exception error)
        {
            _streamRule.OnError(error);
            _multiverseTransformer.OnError(error);
        }

        public void OnNext(IUniverseEvent value)
        {
            _streamRule.OnNext(value);
            _multiverseTransformer.OnNext(value);
        }

        public Domain.Scheduling.Rules Rule { get; } = Domain.Scheduling.Rules.HighProfits;
        public string Version { get; } = HighProfitRuleFactory.Version;

        public object Clone()
        {
            return new HighProfitsRule(
                (IHighProfitStreamRule)_streamRule.Clone(),
                (IMarketCloseMultiverseTransformer)_multiverseTransformer.Clone());
        }
    }
}