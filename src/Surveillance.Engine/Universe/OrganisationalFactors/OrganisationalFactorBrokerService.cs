namespace Surveillance.Engine.Rules.Universe.OrganisationalFactors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Surveillance.Data.Universe;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces;

    public class OrganisationalFactorBrokerService : IOrganisationalFactorBrokerService
    {
        private readonly object _accountLock = new object();

        private readonly bool _aggregateNonFactorableIntoOwnCategory;

        /// <summary>
        ///     don't pass trades into the clone source
        /// </summary>
        private readonly IUniverseCloneableRule _cloneSource;

        private readonly IReadOnlyCollection<ClientOrganisationalFactors> _factors;

        private readonly IDictionary<string, IUniverseRule> _fundFactors;

        private readonly ILogger<OrganisationalFactorBrokerService> _logger;

        private readonly IUniverseCloneableRule _noneFactor;

        private readonly IDictionary<string, IUniverseRule> _portfolioManagerFactors;

        private readonly IDictionary<string, IUniverseRule> _strategyFactors;

        private readonly object _strategyLock = new object();

        private readonly IDictionary<string, IUniverseRule> _traderFactors;

        private readonly object _traderLock = new object();

        public OrganisationalFactorBrokerService(
            IUniverseCloneableRule cloneSource,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory,
            ILogger<OrganisationalFactorBrokerService> logger)
        {
            this._cloneSource = cloneSource ?? throw new ArgumentNullException(nameof(cloneSource));
            this._noneFactor = this._cloneSource.Clone(new FactorValue(ClientOrganisationalFactors.None, string.Empty));
            this._factors = this.FactorGuard(factors);

            this._aggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;

            this._traderFactors = new Dictionary<string, IUniverseRule>();
            this._portfolioManagerFactors = new Dictionary<string, IUniverseRule>();
            this._fundFactors = new Dictionary<string, IUniverseRule>();
            this._strategyFactors = new Dictionary<string, IUniverseRule>();

            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Rules Rule => this._cloneSource.Rule;

        public string Version => this._cloneSource.Version;

        public void OnCompleted()
        {
            this._logger.LogInformation("received OnCompleted() event from the universe stream");
            this._cloneSource.OnCompleted();
        }

        public void OnError(Exception error)
        {
            this._logger.LogError("received OnError() event from the universe stream");
            this._cloneSource.OnError(error);
        }

        public void OnNext(IUniverseEvent value)
        {
            if (value == null) return;

            if (value.StateChange != UniverseStateEvent.Order && value.StateChange != UniverseStateEvent.OrderPlaced)
            {
                this._logger.LogInformation(
                    $"received an event that was not an order. No brokering to perform. {value.EventTime} of type {value.StateChange}");

                this._cloneSource.OnNext(value);
                this._noneFactor.OnNext(value);

                foreach (var rule in this._traderFactors)
                    rule.Value?.OnNext(value);

                foreach (var rule in this._portfolioManagerFactors)
                    rule.Value?.OnNext(value);

                foreach (var rule in this._fundFactors)
                    rule.Value?.OnNext(value);

                foreach (var rule in this._strategyFactors)
                    rule.Value?.OnNext(value);

                return;
            }

            if (this._factors.Contains(ClientOrganisationalFactors.None))
            {
                this._logger.LogInformation("has a none organisational factor so passing onto next");
                this._noneFactor.OnNext(value);
            }

            if (this._factors.Contains(ClientOrganisationalFactors.Trader))
            {
                this._logger.LogInformation("has a trader organisational factor passing to trade factoring");
                this.TraderFactor(value);
            }

            if (this._factors.Contains(ClientOrganisationalFactors.Fund))
            {
                this._logger.LogInformation("has a fund organisational factor so passing to fund factoring");
                this.FundFactor(value);
            }

            if (this._factors.Contains(ClientOrganisationalFactors.Strategy))
            {
                this._logger.LogInformation("has a strategy organisational factor so passing to strategy factoring");
                this.StrategyFactor(value);
            }

            if (this._factors.Contains(ClientOrganisationalFactors.PortfolioManager))
                this._logger.LogInformation(
                    "passed a portfolio manager organisational factor which is not currently supported");

            if (this._factors.Contains(ClientOrganisationalFactors.Unknown))
                this._logger.LogInformation("passed a unknown organisational factor which is not currently supported");
        }

        private IReadOnlyCollection<ClientOrganisationalFactors> FactorGuard(
            IReadOnlyCollection<ClientOrganisationalFactors> factors)
        {
            if (factors == null || !factors.Any())
                return new HashSet<ClientOrganisationalFactors>(new[] { ClientOrganisationalFactors.None });

            return factors;
        }

        private void FundFactor(IUniverseEvent value)
        {
            var data = (Order)value.UnderlyingEvent;

            if (data == null) return;

            if (string.IsNullOrWhiteSpace(data.OrderFund)
                && string.IsNullOrWhiteSpace(data.OrderClientAccountAttributionId)
                && !this._aggregateNonFactorableIntoOwnCategory) return;

            lock (this._accountLock)
            {
                var orderFund = data.OrderFund;

                if (string.IsNullOrWhiteSpace(orderFund))
                    orderFund = data.OrderClientAccountAttributionId ?? string.Empty;

                if (string.IsNullOrWhiteSpace(orderFund)) orderFund = string.Empty;

                if (!this._fundFactors.ContainsKey(orderFund))
                {
                    var kvp = new KeyValuePair<string, IUniverseRule>(
                        orderFund,
                        this._cloneSource.Clone(new FactorValue(ClientOrganisationalFactors.Fund, orderFund)));

                    this._fundFactors.Add(kvp);
                }

                if (this._fundFactors.ContainsKey(orderFund))
                {
                    this._logger.LogInformation(
                        $"has a fund organisational factor and found a rule for fund {orderFund}");
                    this._fundFactors.TryGetValue(orderFund, out var rule);
                    rule?.OnNext(value);
                }
                else
                {
                    this._logger.LogInformation(
                        $"has a fund organisational factor but could not find a rule for {orderFund}");
                }
            }
        }

        private void StrategyFactor(IUniverseEvent value)
        {
            var data = (Order)value.UnderlyingEvent;

            if (data == null) return;

            if (string.IsNullOrWhiteSpace(data.OrderStrategy) && !this._aggregateNonFactorableIntoOwnCategory) return;

            lock (this._strategyLock)
            {
                var orderStrategy = data.OrderStrategy;
                if (string.IsNullOrWhiteSpace(orderStrategy)) orderStrategy = string.Empty;

                if (!this._strategyFactors.ContainsKey(orderStrategy))
                {
                    var kvp = new KeyValuePair<string, IUniverseRule>(
                        orderStrategy,
                        this._cloneSource.Clone(new FactorValue(ClientOrganisationalFactors.Strategy, orderStrategy)));

                    this._strategyFactors.Add(kvp);
                }

                if (this._strategyFactors.ContainsKey(orderStrategy))
                {
                    this._logger.LogInformation(
                        $"has a strategy organisational factor and found a rule for strategy {orderStrategy}");
                    this._strategyFactors.TryGetValue(orderStrategy, out var rule);
                    rule?.OnNext(value);
                }
                else
                {
                    this._logger.LogInformation(
                        $"has a strategy organisational factor and could not find a rule for strategy {orderStrategy}");
                }
            }
        }

        private void TraderFactor(IUniverseEvent value)
        {
            var data = (Order)value.UnderlyingEvent;

            if (data == null) return;

            if (string.IsNullOrWhiteSpace(data.OrderTraderId) && !this._aggregateNonFactorableIntoOwnCategory) return;

            lock (this._traderLock)
            {
                var orderTraderId = data.OrderTraderId;

                if (string.IsNullOrWhiteSpace(orderTraderId)) orderTraderId = string.Empty;

                if (!this._traderFactors.ContainsKey(orderTraderId))
                {
                    var kvp = new KeyValuePair<string, IUniverseRule>(
                        orderTraderId,
                        this._cloneSource.Clone(new FactorValue(ClientOrganisationalFactors.Trader, orderTraderId)));

                    this._traderFactors.Add(kvp);
                }

                if (this._traderFactors.ContainsKey(orderTraderId))
                {
                    this._traderFactors.TryGetValue(orderTraderId, out var rule);

                    this._logger.LogInformation(
                        $"has a trader organisational factor and found a rule for order trader id {orderTraderId}. Brokering.");
                    rule?.OnNext(value);
                }
                else
                {
                    this._logger.LogInformation(
                        $"has a trader organisational factor but could not find a factored rule to pass onto for trader {orderTraderId}. Not brokering.");
                }
            }
        }
    }
}