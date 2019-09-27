namespace Surveillance.Engine.Rules.Universe.OrganisationalFactors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Rules;
    using Domain.Surveillance.Rules.Interfaces;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Surveillance.Data.Universe;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces;

    /// <summary>
    /// The organizational factor broker service.
    /// </summary>
    public class OrganisationalFactorBrokerService : IOrganisationalFactorBrokerService
    {
        /// <summary>
        /// The account lock.
        /// </summary>
        private readonly object accountLock = new object();

        /// <summary>
        /// The aggregate non factorable into own category.
        /// </summary>
        private readonly bool aggregateNonFactorableIntoOwnCategory;

        /// <summary>
        ///     don't pass trades into the clone source
        /// </summary>
        private readonly IUniverseCloneableRule cloneSource;

        /// <summary>
        /// The factors.
        /// </summary>
        private readonly IReadOnlyCollection<ClientOrganisationalFactors> factors;

        /// <summary>
        /// The fund factors.
        /// </summary>
        private readonly IDictionary<string, IUniverseRule> fundFactors;

        /// <summary>
        /// The none factor.
        /// </summary>
        private readonly IUniverseCloneableRule noneFactor;

        /// <summary>
        /// The portfolio manager factors.
        /// </summary>
        private readonly IDictionary<string, IUniverseRule> portfolioManagerFactors;

        /// <summary>
        /// The strategy factors.
        /// </summary>
        private readonly IDictionary<string, IUniverseRule> strategyFactors;

        /// <summary>
        /// The strategy lock.
        /// </summary>
        private readonly object strategyLock = new object();

        /// <summary>
        /// The trader factors.
        /// </summary>
        private readonly IDictionary<string, IUniverseRule> traderFactors;

        /// <summary>
        /// The trader lock.
        /// </summary>
        private readonly object traderLock = new object();

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<OrganisationalFactorBrokerService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrganisationalFactorBrokerService"/> class.
        /// </summary>
        /// <param name="cloneSource">
        /// The clone source.
        /// </param>
        /// <param name="factors">
        /// The factors.
        /// </param>
        /// <param name="aggregateNonFactorableIntoOwnCategory">
        /// The aggregate non factorable into own category.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public OrganisationalFactorBrokerService(
            IUniverseCloneableRule cloneSource,
            IReadOnlyCollection<ClientOrganisationalFactors> factors,
            bool aggregateNonFactorableIntoOwnCategory,
            ILogger<OrganisationalFactorBrokerService> logger)
        {
            this.cloneSource = cloneSource ?? throw new ArgumentNullException(nameof(cloneSource));
            this.noneFactor = this.cloneSource.Clone(new FactorValue(ClientOrganisationalFactors.None, string.Empty));
            this.factors = this.FactorGuard(factors);

            this.aggregateNonFactorableIntoOwnCategory = aggregateNonFactorableIntoOwnCategory;

            this.traderFactors = new Dictionary<string, IUniverseRule>();
            this.portfolioManagerFactors = new Dictionary<string, IUniverseRule>();
            this.fundFactors = new Dictionary<string, IUniverseRule>();
            this.strategyFactors = new Dictionary<string, IUniverseRule>();

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The rule.
        /// </summary>
        public Rules Rule => this.cloneSource.Rule;

        /// <summary>
        /// The version.
        /// </summary>
        public string Version => this.cloneSource.Version;

        /// <summary>
        /// The data constraints.
        /// </summary>
        /// <returns>
        /// The <see cref="IRuleDataConstraint"/>.
        /// </returns>
        public IRuleDataConstraint DataConstraints()
        {
            return RuleDataConstraint.Empty().Case;
        }

        /// <summary>
        /// The on completed.
        /// </summary>
        public void OnCompleted()
        {
            this.logger.LogInformation("received OnCompleted() event from the universe stream");
            this.cloneSource.OnCompleted();
        }

        /// <summary>
        /// The on error.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        public void OnError(Exception error)
        {
            this.logger.LogError("received OnError() event from the universe stream");
            this.cloneSource.OnError(error);
        }

        /// <summary>
        /// The on next.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void OnNext(IUniverseEvent value)
        {
            if (value == null) return;

            if (value.StateChange != UniverseStateEvent.Order && value.StateChange != UniverseStateEvent.OrderPlaced)
            {
                this.logger.LogInformation(
                    $"received an event that was not an order. No brokering to perform. {value.EventTime} of type {value.StateChange}");

                this.cloneSource.OnNext(value);
                this.noneFactor.OnNext(value);

                foreach (var rule in this.traderFactors)
                    rule.Value?.OnNext(value);

                foreach (var rule in this.portfolioManagerFactors)
                    rule.Value?.OnNext(value);

                foreach (var rule in this.fundFactors)
                    rule.Value?.OnNext(value);

                foreach (var rule in this.strategyFactors)
                    rule.Value?.OnNext(value);

                return;
            }

            if (this.factors.Contains(ClientOrganisationalFactors.None))
            {
                this.logger.LogInformation("has a none organisational factor so passing onto next");
                this.noneFactor.OnNext(value);
            }

            if (this.factors.Contains(ClientOrganisationalFactors.Trader))
            {
                this.logger.LogInformation("has a trader organisational factor passing to trade factoring");
                this.TraderFactor(value);
            }

            if (this.factors.Contains(ClientOrganisationalFactors.Fund))
            {
                this.logger.LogInformation("has a fund organisational factor so passing to fund factoring");
                this.FundFactor(value);
            }

            if (this.factors.Contains(ClientOrganisationalFactors.Strategy))
            {
                this.logger.LogInformation("has a strategy organisational factor so passing to strategy factoring");
                this.StrategyFactor(value);
            }

            if (this.factors.Contains(ClientOrganisationalFactors.PortfolioManager))
                this.logger.LogInformation(
                    "passed a portfolio manager organisational factor which is not currently supported");

            if (this.factors.Contains(ClientOrganisationalFactors.Unknown))
                this.logger.LogInformation("passed a unknown organisational factor which is not currently supported");
        }

        /// <summary>
        /// The factor guard.
        /// </summary>
        /// <param name="factors">
        /// The factors.
        /// </param>
        /// <returns>
        /// The <see cref="IReadOnlyCollection"/>.
        /// </returns>
        private IReadOnlyCollection<ClientOrganisationalFactors> FactorGuard(
            IReadOnlyCollection<ClientOrganisationalFactors> factors)
        {
            if (factors == null || !factors.Any())
                return new HashSet<ClientOrganisationalFactors>(new[] { ClientOrganisationalFactors.None });

            return factors;
        }

        /// <summary>
        /// The fund factor.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        private void FundFactor(IUniverseEvent value)
        {
            var data = (Order)value.UnderlyingEvent;

            if (data == null) return;

            if (string.IsNullOrWhiteSpace(data.OrderFund)
                && string.IsNullOrWhiteSpace(data.OrderClientAccountAttributionId)
                && !this.aggregateNonFactorableIntoOwnCategory) return;

            lock (this.accountLock)
            {
                var orderFund = data.OrderFund;

                if (string.IsNullOrWhiteSpace(orderFund))
                    orderFund = data.OrderClientAccountAttributionId ?? string.Empty;

                if (string.IsNullOrWhiteSpace(orderFund)) orderFund = string.Empty;

                if (!this.fundFactors.ContainsKey(orderFund))
                {
                    var kvp = new KeyValuePair<string, IUniverseRule>(
                        orderFund,
                        this.cloneSource.Clone(new FactorValue(ClientOrganisationalFactors.Fund, orderFund)));

                    this.fundFactors.Add(kvp);
                }

                if (this.fundFactors.ContainsKey(orderFund))
                {
                    this.logger.LogInformation(
                        $"has a fund organisational factor and found a rule for fund {orderFund}");
                    this.fundFactors.TryGetValue(orderFund, out var rule);
                    rule?.OnNext(value);
                }
                else
                {
                    this.logger.LogInformation(
                        $"has a fund organisational factor but could not find a rule for {orderFund}");
                }
            }
        }

        /// <summary>
        /// The strategy factor.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        private void StrategyFactor(IUniverseEvent value)
        {
            var data = (Order)value.UnderlyingEvent;

            if (data == null) return;

            if (string.IsNullOrWhiteSpace(data.OrderStrategy) && !this.aggregateNonFactorableIntoOwnCategory) return;

            lock (this.strategyLock)
            {
                var orderStrategy = data.OrderStrategy;
                if (string.IsNullOrWhiteSpace(orderStrategy)) orderStrategy = string.Empty;

                if (!this.strategyFactors.ContainsKey(orderStrategy))
                {
                    var kvp = new KeyValuePair<string, IUniverseRule>(
                        orderStrategy,
                        this.cloneSource.Clone(new FactorValue(ClientOrganisationalFactors.Strategy, orderStrategy)));

                    this.strategyFactors.Add(kvp);
                }

                if (this.strategyFactors.ContainsKey(orderStrategy))
                {
                    this.logger.LogInformation(
                        $"has a strategy organisational factor and found a rule for strategy {orderStrategy}");
                    this.strategyFactors.TryGetValue(orderStrategy, out var rule);
                    rule?.OnNext(value);
                }
                else
                {
                    this.logger.LogInformation(
                        $"has a strategy organisational factor and could not find a rule for strategy {orderStrategy}");
                }
            }
        }

        /// <summary>
        /// The trader factor.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        private void TraderFactor(IUniverseEvent value)
        {
            var data = (Order)value.UnderlyingEvent;

            if (data == null) return;

            if (string.IsNullOrWhiteSpace(data.OrderTraderId) && !this.aggregateNonFactorableIntoOwnCategory) return;

            lock (this.traderLock)
            {
                var orderTraderId = data.OrderTraderId;

                if (string.IsNullOrWhiteSpace(orderTraderId)) orderTraderId = string.Empty;

                if (!this.traderFactors.ContainsKey(orderTraderId))
                {
                    var kvp = new KeyValuePair<string, IUniverseRule>(
                        orderTraderId,
                        this.cloneSource.Clone(new FactorValue(ClientOrganisationalFactors.Trader, orderTraderId)));

                    this.traderFactors.Add(kvp);
                }

                if (this.traderFactors.ContainsKey(orderTraderId))
                {
                    this.traderFactors.TryGetValue(orderTraderId, out var rule);

                    this.logger.LogInformation(
                        $"has a trader organisational factor and found a rule for order trader id {orderTraderId}. Brokering.");
                    rule?.OnNext(value);
                }
                else
                {
                    this.logger.LogInformation(
                        $"has a trader organisational factor but could not find a factored rule to pass onto for trader {orderTraderId}. Not brokering.");
                }
            }
        }
    }
}