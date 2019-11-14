namespace Surveillance.Engine.Rules.Universe.Filter
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;

    using Domain.Core.Markets.Collections;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Rules;
    using Domain.Surveillance.Rules.Interfaces;
    using Domain.Surveillance.Scheduling;
    using Domain.Surveillance.Streams.Interfaces;

    using Microsoft.Extensions.Logging;

    using Surveillance.Data.Universe;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    /// <summary>
    /// The universe filter service.
    /// </summary>
    public class UniverseFilterService : IUniverseFilterService
    {
        /// <summary>
        /// The accounts.
        /// </summary>
        private readonly RuleFilter accounts;

        /// <summary>
        /// The countries.
        /// </summary>
        private readonly RuleFilter countries;

        /// <summary>
        /// The rule that is being filtered by this service
        /// </summary>
        private readonly IUniverseRule filteredRule;

        /// <summary>
        /// The funds.
        /// </summary>
        private readonly RuleFilter funds;

        /// <summary>
        /// The high market cap filter.
        /// </summary>
        private readonly IHighMarketCapFilter highMarketCapFilter;

        /// <summary>
        /// The industries.
        /// </summary>
        private readonly RuleFilter industries;

        /// <summary>
        /// The markets.
        /// </summary>
        private readonly RuleFilter markets;

        /// <summary>
        /// The regions.
        /// </summary>
        private readonly RuleFilter regions;

        /// <summary>
        /// The sectors.
        /// </summary>
        private readonly RuleFilter sectors;

        /// <summary>
        /// The strategies.
        /// </summary>
        private readonly RuleFilter strategies;

        /// <summary>
        /// The traders.
        /// </summary>
        private readonly RuleFilter traders;

        /// <summary>
        /// The universe observers.
        /// </summary>
        private readonly ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>> universeObservers;

        /// <summary>
        /// The universe un subscriber factory.
        /// </summary>
        private readonly IUnsubscriberFactory<IUniverseEvent> universeUnsubscriberFactory;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<UniverseFilterService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UniverseFilterService"/> class.
        /// </summary>
        /// <param name="universeUnsubscriberFactory">
        /// The universe un subscriber factory.
        /// </param>
        /// <param name="highMarketCapFilter">
        /// The high market cap filter.
        /// </param>
        /// <param name="accounts">
        /// The accounts.
        /// </param>
        /// <param name="traders">
        /// The traders.
        /// </param>
        /// <param name="markets">
        /// The markets.
        /// </param>
        /// <param name="funds">
        /// The funds.
        /// </param>
        /// <param name="strategies">
        /// The strategies.
        /// </param>
        /// <param name="sectors">
        /// The sectors.
        /// </param>
        /// <param name="industries">
        /// The industries.
        /// </param>
        /// <param name="regions">
        /// The regions.
        /// </param>
        /// <param name="countries">
        /// The countries.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public UniverseFilterService(
            IUnsubscriberFactory<IUniverseEvent> universeUnsubscriberFactory,
            IHighMarketCapFilter highMarketCapFilter,
            IUniverseRule filteredRule,
            RuleFilter accounts,
            RuleFilter traders,
            RuleFilter markets,
            RuleFilter funds,
            RuleFilter strategies,
            RuleFilter sectors,
            RuleFilter industries,
            RuleFilter regions,
            RuleFilter countries,
            ILogger<UniverseFilterService> logger)
        {
            this.universeUnsubscriberFactory =
                universeUnsubscriberFactory ?? throw new ArgumentNullException(nameof(universeUnsubscriberFactory));
            this.highMarketCapFilter = 
                highMarketCapFilter ?? throw new ArgumentNullException(nameof(highMarketCapFilter));
            this.filteredRule = filteredRule;
            this.accounts = accounts;
            this.traders = traders;
            this.markets = markets;
            this.funds = funds;
            this.strategies = strategies;
            this.sectors = sectors;
            this.industries = industries;
            this.regions = regions;
            this.countries = countries;
            this.universeObservers = new ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>>();
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets the rule.
        /// </summary>
        public Rules Rule { get; } = Rules.UniverseFilter;

        /// <summary>
        /// Gets the version.
        /// </summary>
        public string Version { get; } = Versioner.Version(0, 0);

        /// <summary>
        /// The data constraints.
        /// </summary>
        /// <returns>
        /// The <see cref="IRuleDataConstraint"/>.
        /// </returns>
        public IRuleDataConstraint DataConstraints()
        {
            return this.filteredRule.DataConstraints();
        }

        /// <summary>
        /// The on completed.
        /// </summary>
        public void OnCompleted()
        {
            this.logger.LogInformation("has received OnCompleted() from the stream. Forwarding to observers.");

            foreach (var obs in this.universeObservers) obs.Value?.OnCompleted();
        }

        /// <summary>
        /// The on error.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        public void OnError(Exception error)
        {
            this.logger.LogError("OnError() received an exception", error);

            foreach (var obs in this.universeObservers) obs.Value?.OnError(error);
        }

        /// <summary>
        /// The on next.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void OnNext(IUniverseEvent value)
        {
            if (this.FilterOnAccount(value)) return;

            if (this.FilterOnTraders(value)) return;

            if (this.FilterOnMarkets(value)) return;

            if (this.highMarketCapFilter.Filter(value)) return;

            if (this.FilterOnFund(value)) return;

            if (this.FilterOnStrategy(value)) return;

            if (this.FilterOnSector(value)) return;

            if (this.FilterOnIndustry(value)) return;

            if (this.FilterOnRegion(value)) return;

            if (this.FilterOnCountry(value)) return;

            this.logger.LogInformation($"is not filtering event at {value.EventTime} with type {value.StateChange}");
            foreach (var obs in this.universeObservers) obs.Value?.OnNext(value);
        }

        /// <summary>
        /// The subscribe.
        /// </summary>
        /// <param name="observer">
        /// The observer.
        /// </param>
        /// <returns>
        /// The <see cref="IDisposable"/>.
        /// </returns>
        public IDisposable Subscribe(IObserver<IUniverseEvent> observer)
        {
            if (!this.universeObservers.ContainsKey(observer))
            {
                this.logger.LogInformation("subscribing a new observer");
                this.universeObservers.TryAdd(observer, observer);
            }

            return this.universeUnsubscriberFactory.Create(this.universeObservers, observer);
        }

        /// <summary>
        /// The filter on account.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool FilterOnAccount(IUniverseEvent value)
        {
            return this.FilterOnOrderAttribute(
                value,
                this.accounts,
                i => i.OrderClientAccountAttributionId,
                "Accounts");
        }

        /// <summary>
        /// The filter on country.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool FilterOnCountry(IUniverseEvent value)
        {
            return this.FilterOnOrderAttribute(value, this.countries, i => i.Instrument.CountryCode, "country");
        }

        /// <summary>
        /// The filter on fund.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool FilterOnFund(IUniverseEvent value)
        {
            return this.FilterOnOrderAttribute(value, this.funds, i => i.OrderFund, "fund");
        }

        /// <summary>
        /// The filter on industry.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool FilterOnIndustry(IUniverseEvent value)
        {
            return this.FilterOnOrderAttribute(value, this.industries, i => i.Instrument.IndustryCode, "industry");
        }

        /// <summary>
        /// The filter on market intraday tick.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool FilterOnMarketIntradayTick(IUniverseEvent value)
        {
            var exchFrame = (EquityIntraDayTimeBarCollection)value.UnderlyingEvent;
            if (exchFrame == null) return false;

            if (this.markets.Type == RuleFilterType.Include)
            {
                if (exchFrame?.Exchange?.MarketIdentifierCode == null) return true;

                var filter = !this.markets.Ids.Contains(
                                 exchFrame.Exchange.MarketIdentifierCode,
                                 StringComparer.InvariantCultureIgnoreCase);

                if (filter)
                    this.logger.LogInformation(
                        $"filtering out stock tick with id {exchFrame.Exchange.MarketIdentifierCode} at {exchFrame.Epoch}");

                return filter;
            }

            if (this.markets.Type == RuleFilterType.Exclude)
            {
                if (exchFrame?.Exchange?.MarketIdentifierCode == null) return false;

                var filter = this.markets.Ids.Contains(
                    exchFrame.Exchange.MarketIdentifierCode,
                    StringComparer.InvariantCultureIgnoreCase);

                if (filter)
                    this.logger.LogInformation(
                        $"filtering out stock tick with id {exchFrame.Exchange.MarketIdentifierCode} at {exchFrame.Epoch}");

                return filter;
            }

            return false;
        }

        /// <summary>
        /// The filter on markets.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool FilterOnMarkets(IUniverseEvent value)
        {
            if (this.markets == null) return false;

            if (this.markets.Type == RuleFilterType.None) return false;

            if (value == null) return false;

            if (value.StateChange != UniverseStateEvent.Order && value.StateChange != UniverseStateEvent.OrderPlaced
                                                              && value.StateChange
                                                              != UniverseStateEvent.EquityIntraDayTick) return false;

            if (value.StateChange == UniverseStateEvent.EquityIntraDayTick)
                return this.FilterOnMarketIntradayTick(value);

            if (value.StateChange == UniverseStateEvent.Order || value.StateChange == UniverseStateEvent.OrderPlaced)
                return this.FilterOnOrderAttribute(
                    value,
                    this.markets,
                    i => i?.Market?.MarketIdentifierCode,
                    "market");

            return false;
        }

        /// <summary>
        /// The filter on order attribute.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <param name="propertyLens">
        /// The property lens.
        /// </param>
        /// <param name="filterName">
        /// The filter name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool FilterOnOrderAttribute(
            IUniverseEvent value,
            RuleFilter filter,
            Func<Order, string> propertyLens,
            string filterName)
        {
            if (filter == null) return false;

            if (filter.Type == RuleFilterType.None) return false;

            if (value == null) return false;

            if (value.StateChange != UniverseStateEvent.Order
                && value.StateChange != UniverseStateEvent.OrderPlaced) return false;

            var frame = (Order)value.UnderlyingEvent;

            if (frame == null) return false;

            var filterResult = false;

            switch (filter.Type)
            {
                case RuleFilterType.Include:
                    filterResult = !filter.Ids.Contains(propertyLens(frame), StringComparer.InvariantCultureIgnoreCase);
                    break;
                case RuleFilterType.Exclude:
                    filterResult = filter.Ids.Contains(propertyLens(frame), StringComparer.InvariantCultureIgnoreCase);
                    break;
            }

            if (filterResult)
                this.logger.LogInformation($"{filterName} filtering out order with id {frame.ReddeerOrderId}");

            return filterResult;
        }

        /// <summary>
        /// The filter on region.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool FilterOnRegion(IUniverseEvent value)
        {
            return this.FilterOnOrderAttribute(value, this.regions, i => i.Instrument.RegionCode, "region");
        }

        /// <summary>
        /// The filter on sector.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool FilterOnSector(IUniverseEvent value)
        {
            return this.FilterOnOrderAttribute(value, this.sectors, i => i.Instrument.SectorCode, "sector");
        }

        /// <summary>
        /// The filter on strategy.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool FilterOnStrategy(IUniverseEvent value)
        {
            return this.FilterOnOrderAttribute(value, this.strategies, i => i.OrderStrategy, "strategy");
        }

        /// <summary>
        /// The filter on traders.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool FilterOnTraders(IUniverseEvent value)
        {
            return this.FilterOnOrderAttribute(value, this.traders, i => i.OrderTraderId, "Trader");
        }
    }
}