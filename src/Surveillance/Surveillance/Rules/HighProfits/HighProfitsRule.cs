using System;
using Surveillance.Rules.HighProfits.Interfaces;
using Surveillance.Universe.Interfaces;

// ReSharper disable AssignNullToNotNullAttribute
namespace Surveillance.Rules.HighProfits
{
    /// <summary>
    /// Calculate the profits from the trade
    /// </summary>
    public class HighProfitsRule : IHighProfitRule
    {
        private readonly IHighProfitStreamRule _streamRule;
        private readonly IHighProfitStreamRule _marketCloseRule;

        public HighProfitsRule(IHighProfitStreamRule streamRule, IHighProfitMarketClosureRule marketCloseRule)
        {
            _streamRule = streamRule ?? throw new ArgumentNullException(nameof(streamRule));
            _marketCloseRule = marketCloseRule ?? throw new ArgumentNullException(nameof(marketCloseRule));
        }

        public void OnCompleted()
        {
            _streamRule.OnCompleted();
            _marketCloseRule.OnCompleted();
        }

        public void OnError(Exception error)
        {
            _streamRule.OnError(error);
            _marketCloseRule.OnError(error);
        }

        public void OnNext(IUniverseEvent value)
        {
             _streamRule.OnNext(value);
            // todo multiverse rewrite stream and pass it onto the market closure rule =)
            _marketCloseRule.OnNext(value);
        }

        public Domain.Scheduling.Rules Rule { get; } = Domain.Scheduling.Rules.HighProfits;
        public string Version { get; } = Versioner.Version(1, 0);
    }
}