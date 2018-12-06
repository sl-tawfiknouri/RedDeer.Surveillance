using System;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.RuleParameters.OrganisationalFactors;
using Surveillance.Rules.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.OrganisationalFactors.Interfaces;

namespace Surveillance.Universe.OrganisationalFactors
{
    public class OrganisationalFactorBroker : IOrganisationalFactorBroker
    {
        private readonly object _traderLock = new object();
        private readonly object _strategyLock = new object();
        private readonly object _accountLock = new object();

        /// <summary>
        /// don't pass trades into the clone source
        /// </summary>
        private readonly IUniverseCloneableRule _cloneSource;
        private readonly IReadOnlyCollection<ClientOrganisationalFactors> _factors;
        private readonly bool _aggregateNonFactorableIntoOwnCategory;

        private readonly IUniverseRule _noneFactor;
        private readonly IDictionary<string, IUniverseRule> _traderFactors;
        private readonly IDictionary<string, IUniverseRule> _portfolioManagerFactors;
        private readonly IDictionary<string, IUniverseRule> _fundFactors;
        private readonly IDictionary<string, IUniverseRule> _strategyFactors;

        private readonly ILogger<OrganisationalFactorBroker> _logger;

        public OrganisationalFactorBroker(
            IUniverseCloneableRule cloneSource,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory,
            ILogger<OrganisationalFactorBroker> logger)
        {
            _cloneSource = cloneSource ?? throw new ArgumentNullException(nameof(cloneSource));
            _noneFactor = (IUniverseRule)_cloneSource.Clone();
            _factors = FactorGuard(factors);

            _aggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;
            
            _traderFactors = new Dictionary<string, IUniverseRule>();
            _portfolioManagerFactors = new Dictionary<string, IUniverseRule>();
            _fundFactors = new Dictionary<string, IUniverseRule>();
            _strategyFactors = new Dictionary<string, IUniverseRule>();

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private IReadOnlyCollection<ClientOrganisationalFactors> FactorGuard(
            IReadOnlyCollection<ClientOrganisationalFactors> factors)
        {
            if (factors == null
                || !factors.Any())
            {
                return new HashSet<ClientOrganisationalFactors>(new[] { ClientOrganisationalFactors.None });
            }

            return factors;
        }

        public void OnCompleted()
        {
            _cloneSource.OnCompleted();
        }

        public void OnError(Exception error)
        {
            _cloneSource.OnError(error);
        }

        public void OnNext(IUniverseEvent value)
        {
            if (value == null)
            {
                return;
            }

            if (value.StateChange != UniverseStateEvent.TradeReddeer
                && value.StateChange != UniverseStateEvent.TradeReddeerSubmitted)
            {
                _cloneSource.OnNext(value);
                _noneFactor.OnNext(value);

                foreach (var rule in _traderFactors)
                    rule.Value?.OnNext(value);

                foreach (var rule in _portfolioManagerFactors)
                    rule.Value?.OnNext(value);

                foreach (var rule in _fundFactors)
                    rule.Value?.OnNext(value);

                foreach (var rule in _strategyFactors)
                    rule.Value?.OnNext(value);

                return;
            }

            if (_factors.Contains(ClientOrganisationalFactors.None))
            {
                _noneFactor.OnNext(value);
            }

            if (_factors.Contains(ClientOrganisationalFactors.Trader))
            {
                TraderFactor(value);
            }

            if (_factors.Contains(ClientOrganisationalFactors.Fund))
            {
                FundFactor(value);
            }

            if (_factors.Contains(ClientOrganisationalFactors.Strategy))
            {
                StrategyFactor(value);
            }

            if (_factors.Contains(ClientOrganisationalFactors.PortfolioManager))
            {
                _logger.LogInformation("OrganisationalFactorBroker passed a portfolio manager organisational factor which is not currently supported");
            }

            if (_factors.Contains(ClientOrganisationalFactors.Unknown))
            {
                _logger.LogInformation("OrganisationalFactorBroker passed a unknown organisational factor which is not currently supported");
            }
        }

        private void TraderFactor(IUniverseEvent value)
        {
            var data = (Order)value.UnderlyingEvent;

            if (data == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(data.OrderTraderId)
                && !_aggregateNonFactorableIntoOwnCategory)
            {
                return;
            }

            lock (_traderLock)
            {
                if (string.IsNullOrWhiteSpace(data.TraderId))
                {
                    data.TraderId = string.Empty;
                }

                if (!_traderFactors.ContainsKey(data.TraderId))
                {
                    var kvp = new KeyValuePair<string, IUniverseRule>(data.TraderId, (IUniverseRule)_cloneSource.Clone());
                    _traderFactors.Add(kvp);
                }

                if (_traderFactors.ContainsKey(data.TraderId))
                {
                    _traderFactors.TryGetValue(data.TraderId, out var rule);
                    rule?.OnNext(value);
                }
            }
        }

        private void StrategyFactor(IUniverseEvent value)
        {
            var data = (Order)value.UnderlyingEvent;

            if (data == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(data.OrderStrategy)
                && !_aggregateNonFactorableIntoOwnCategory)
            {
                return;
            }

            lock (_strategyLock)
            {
                var orderStrategy = data.OrderStrategy;
                if (string.IsNullOrWhiteSpace(orderStrategy))
                {
                    orderStrategy = string.Empty;
                }

                if (!_strategyFactors.ContainsKey(orderStrategy))
                {
                    var kvp = new KeyValuePair<string, IUniverseRule>(orderStrategy, (IUniverseRule)_cloneSource.Clone());
                    _strategyFactors.Add(kvp);
                }

                if (_strategyFactors.ContainsKey(orderStrategy))
                {
                    _strategyFactors.TryGetValue(orderStrategy, out var rule);
                    rule?.OnNext(value);
                }
            }
        }

        private void FundFactor(IUniverseEvent value)
        {
            var data = (Order)value.UnderlyingEvent;

            if (data == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(data.AccountId)
                && !_aggregateNonFactorableIntoOwnCategory)
            {
                return;
            }

            lock (_accountLock)
            {
                if (string.IsNullOrWhiteSpace(data.AccountId))
                {
                    data.AccountId = string.Empty;
                }

                if (!_fundFactors.ContainsKey(data.AccountId))
                {
                    var kvp = new KeyValuePair<string, IUniverseRule>(data.AccountId, (IUniverseRule)_cloneSource.Clone());
                    _fundFactors.Add(kvp);
                }

                if (_fundFactors.ContainsKey(data.AccountId))
                {
                    _fundFactors.TryGetValue(data.AccountId, out var rule);
                    rule?.OnNext(value);
                }
            }
        }

        public DomainV2.Scheduling.Rules Rule => _cloneSource.Rule;
        public string Version => _cloneSource.Version;
    }
}
