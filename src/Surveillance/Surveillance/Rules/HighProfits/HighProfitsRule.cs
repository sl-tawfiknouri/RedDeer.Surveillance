using System;
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
            _multiverseTransformer.OnCompleted();
            _streamRule.OnCompleted();
        }

        public void OnError(Exception error)
        {
            _multiverseTransformer.OnError(error);
            _streamRule.OnError(error);
        }

        public void OnNext(IUniverseEvent value)
        {
            _multiverseTransformer.OnNext(value);
            _streamRule.OnNext(value);
        }

        public Domain.Scheduling.Rules Rule { get; } = Domain.Scheduling.Rules.HighProfits;
        public string Version { get; } = Versioner.Version(2, 0);
    }
}