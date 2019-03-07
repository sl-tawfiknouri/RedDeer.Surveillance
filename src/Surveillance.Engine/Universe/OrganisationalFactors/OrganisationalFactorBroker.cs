using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Trading.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces;

namespace Surveillance.Engine.Rules.Universe.OrganisationalFactors
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

        private readonly IUniverseCloneableRule _noneFactor;
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
            _noneFactor = _cloneSource.Clone(new FactorValue(ClientOrganisationalFactors.None, string.Empty));
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
            _logger.LogInformation($"received OnCompleted() event from the universe stream");
            _cloneSource.OnCompleted();
        }

        public void OnError(Exception error)
        {
            _logger.LogError($"received OnError() event from the universe stream");
            _cloneSource.OnError(error);
        }

        public void OnNext(IUniverseEvent value)
        {
            if (value == null)
            {
                return;
            }

            if (value.StateChange != UniverseStateEvent.Order
                && value.StateChange != UniverseStateEvent.OrderPlaced)
            {
                _logger.LogInformation($"received an event that was not an order. No brokering to perform. {value.EventTime} of type {value.StateChange}");

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
                _logger.LogInformation($"has a none organisational factor so passing onto next");
                _noneFactor.OnNext(value);
            }

            if (_factors.Contains(ClientOrganisationalFactors.Trader))
            {
                _logger.LogInformation($"has a trader organisational factor passing to trade factoring");
                TraderFactor(value);
            }

            if (_factors.Contains(ClientOrganisationalFactors.Fund))
            {
                _logger.LogInformation($"has a fund organisational factor so passing to fund factoring");
                FundFactor(value);
            }

            if (_factors.Contains(ClientOrganisationalFactors.Strategy))
            {
                _logger.LogInformation($"has a strategy organisational factor so passing to strategy factoring");
                StrategyFactor(value);
            }

            if (_factors.Contains(ClientOrganisationalFactors.PortfolioManager))
            {
                _logger.LogInformation($"passed a portfolio manager organisational factor which is not currently supported");
            }

            if (_factors.Contains(ClientOrganisationalFactors.Unknown))
            {
                _logger.LogInformation($"passed a unknown organisational factor which is not currently supported");
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
                var orderTraderId = data.OrderTraderId;

                if (string.IsNullOrWhiteSpace(orderTraderId))
                {
                    orderTraderId = string.Empty;
                }

                if (!_traderFactors.ContainsKey(orderTraderId))
                {
                    var kvp = new KeyValuePair<string, IUniverseRule>(
                        orderTraderId,
                        _cloneSource.Clone(new FactorValue(ClientOrganisationalFactors.Trader, orderTraderId)));

                    _traderFactors.Add(kvp);
                }

                if (_traderFactors.ContainsKey(orderTraderId))
                {
                    _traderFactors.TryGetValue(orderTraderId, out var rule);

                    _logger.LogInformation($"has a trader organisational factor and found a rule for order trader id {orderTraderId}. Brokering.");
                    rule?.OnNext(value);
                }
                else
                {
                    _logger.LogInformation($"has a trader organisational factor but could not find a factored rule to pass onto for trader {orderTraderId}. Not brokering.");
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
                    var kvp = new KeyValuePair<string, IUniverseRule>(
                        orderStrategy,
                        _cloneSource.Clone(new FactorValue(ClientOrganisationalFactors.Strategy, orderStrategy)));

                    _strategyFactors.Add(kvp);
                }

                if (_strategyFactors.ContainsKey(orderStrategy))
                {
                    _logger.LogInformation($"has a strategy organisational factor and found a rule for strategy {orderStrategy}");
                    _strategyFactors.TryGetValue(orderStrategy, out var rule);
                    rule?.OnNext(value);
                }
                else
                {
                    _logger.LogInformation($"has a strategy organisational factor and could not find a rule for strategy {orderStrategy}");
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

            if ((string.IsNullOrWhiteSpace(data.OrderFund) 
                 && string.IsNullOrWhiteSpace(data.OrderClientAccountAttributionId))
                && !_aggregateNonFactorableIntoOwnCategory)
            {
                return;
            }

            lock (_accountLock)
            {
                var orderFund = data.OrderFund;

                if (string.IsNullOrWhiteSpace(orderFund))
                {
                    // promote client account attribution ids to fund level
                    orderFund = data.OrderClientAccountAttributionId ?? string.Empty;
                }

                if (string.IsNullOrWhiteSpace(orderFund))
                {
                    orderFund = string.Empty;
                }

                if (!_fundFactors.ContainsKey(orderFund))
                {
                    var kvp = new KeyValuePair<string, IUniverseRule>(
                        orderFund,
                        _cloneSource.Clone(new FactorValue(ClientOrganisationalFactors.Fund, orderFund)));

                    _fundFactors.Add(kvp);
                }

                if (_fundFactors.ContainsKey(orderFund))
                {
                    _logger.LogInformation($"has a fund organisational factor and found a rule for fund {orderFund}");
                    _fundFactors.TryGetValue(orderFund, out var rule);
                    rule?.OnNext(value);
                }
                else
                {
                    _logger.LogInformation($"has a fund organisational factor but could not find a rule for {orderFund}");
                }
            }
        }

        public Domain.Surveillance.Scheduling.Rules Rule => _cloneSource.Rule;
        public string Version => _cloneSource.Version;
    }
}
