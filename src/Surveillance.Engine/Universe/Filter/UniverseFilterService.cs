namespace Surveillance.Engine.Rules.Universe.Filter
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;

    using Domain.Core.Markets.Collections;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Scheduling;
    using Domain.Surveillance.Streams.Interfaces;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;

    public class UniverseFilterService : IUniverseFilterService
    {
        private readonly RuleFilter _accounts;

        private readonly RuleFilter _countries;

        private readonly RuleFilter _funds;

        private readonly IHighMarketCapFilter _highMarketCapFilter;

        private readonly RuleFilter _industries;

        private readonly ILogger<UniverseFilterService> _logger;

        private readonly RuleFilter _markets;

        private readonly RuleFilter _regions;

        private readonly RuleFilter _sectors;

        private readonly RuleFilter _strategies;

        private readonly RuleFilter _traders;

        private readonly ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>> _universeObservers;

        private readonly IUnsubscriberFactory<IUniverseEvent> _universeUnsubscriberFactory;

        public UniverseFilterService(
            IUnsubscriberFactory<IUniverseEvent> universeUnsubscriberFactory,
            IHighMarketCapFilter highMarketCapFilter,
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
            this._universeUnsubscriberFactory = universeUnsubscriberFactory
                                                ?? throw new ArgumentNullException(nameof(universeUnsubscriberFactory));

            this._highMarketCapFilter = highMarketCapFilter
                                        ?? throw new ArgumentNullException(nameof(highMarketCapFilter));
            

            this._accounts = accounts;
            this._traders = traders;
            this._markets = markets;
            this._funds = funds;
            this._strategies = strategies;
            this._sectors = sectors;
            this._industries = industries;
            this._regions = regions;
            this._countries = countries;

            this._universeObservers = new ConcurrentDictionary<IObserver<IUniverseEvent>, IObserver<IUniverseEvent>>();

            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Rules Rule { get; } = Rules.UniverseFilter;

        public string Version { get; } = Versioner.Version(0, 0);

        public void OnCompleted()
        {
            this._logger.LogInformation("has received OnCompleted() from the stream. Forwarding to observers.");

            foreach (var obs in this._universeObservers) obs.Value?.OnCompleted();
        }

        public void OnError(Exception error)
        {
            this._logger.LogError("OnError() received an exception", error);

            foreach (var obs in this._universeObservers) obs.Value?.OnError(error);
        }

        public void OnNext(IUniverseEvent value)
        {
            if (this.FilterOnAccount(value)) return;

            if (this.FilterOnTraders(value)) return;

            if (this.FilterOnMarkets(value)) return;

            if (this._highMarketCapFilter.Filter(value)) return;

            if (this.FilterOnFund(value)) return;

            if (this.FilterOnStrategy(value)) return;

            if (this.FilterOnSector(value)) return;

            if (this.FilterOnIndustry(value)) return;

            if (this.FilterOnRegion(value)) return;

            if (this.FilterOnCountry(value)) return;

            this._logger.LogInformation($"is not filtering event at {value.EventTime} with type {value.StateChange}");
            foreach (var obs in this._universeObservers) obs.Value?.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<IUniverseEvent> observer)
        {
            if (!this._universeObservers.ContainsKey(observer))
            {
                this._logger.LogInformation("subscribing a new observer");
                this._universeObservers.TryAdd(observer, observer);
            }

            return this._universeUnsubscriberFactory.Create(this._universeObservers, observer);
        }

        private bool FilterOnAccount(IUniverseEvent value)
        {
            return this.FilterOnOrderAttribute(
                value,
                this._accounts,
                i => i.OrderClientAccountAttributionId,
                "Accounts");
        }

        private bool FilterOnCountry(IUniverseEvent value)
        {
            return this.FilterOnOrderAttribute(value, this._countries, i => i.Instrument.CountryCode, "country");
        }

        private bool FilterOnFund(IUniverseEvent value)
        {
            return this.FilterOnOrderAttribute(value, this._funds, i => i.OrderFund, "fund");
        }

        private bool FilterOnIndustry(IUniverseEvent value)
        {
            return this.FilterOnOrderAttribute(value, this._industries, i => i.Instrument.IndustryCode, "industry");
        }

        private bool FilterOnMarketIntradayTick(IUniverseEvent value)
        {
            var exchFrame = (EquityIntraDayTimeBarCollection)value.UnderlyingEvent;
            if (exchFrame == null) return false;

            if (this._markets.Type == RuleFilterType.Include)
            {
                if (exchFrame?.Exchange?.MarketIdentifierCode == null) return true;

                var filter = !this._markets.Ids.Contains(
                                 exchFrame.Exchange.MarketIdentifierCode,
                                 StringComparer.InvariantCultureIgnoreCase);

                if (filter)
                    this._logger.LogInformation(
                        $"filtering out stock tick with id {exchFrame.Exchange.MarketIdentifierCode} at {exchFrame.Epoch}");

                return filter;
            }

            if (this._markets.Type == RuleFilterType.Exclude)
            {
                if (exchFrame?.Exchange?.MarketIdentifierCode == null) return false;

                var filter = this._markets.Ids.Contains(
                    exchFrame.Exchange.MarketIdentifierCode,
                    StringComparer.InvariantCultureIgnoreCase);

                if (filter)
                    this._logger.LogInformation(
                        $"filtering out stock tick with id {exchFrame.Exchange.MarketIdentifierCode} at {exchFrame.Epoch}");

                return filter;
            }

            return false;
        }

        private bool FilterOnMarkets(IUniverseEvent value)
        {
            if (this._markets == null) return false;

            if (this._markets.Type == RuleFilterType.None) return false;

            if (value == null) return false;

            if (value.StateChange != UniverseStateEvent.Order && value.StateChange != UniverseStateEvent.OrderPlaced
                                                              && value.StateChange
                                                              != UniverseStateEvent.EquityIntradayTick) return false;

            if (value.StateChange == UniverseStateEvent.EquityIntradayTick)
                return this.FilterOnMarketIntradayTick(value);

            if (value.StateChange == UniverseStateEvent.Order || value.StateChange == UniverseStateEvent.OrderPlaced)
                return this.FilterOnOrderAttribute(
                    value,
                    this._markets,
                    i => i?.Market?.MarketIdentifierCode,
                    "market");

            return false;
        }

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
                this._logger.LogInformation($"{filterName} filtering out order with id {frame.ReddeerOrderId}");

            return filterResult;
        }

        private bool FilterOnRegion(IUniverseEvent value)
        {
            return this.FilterOnOrderAttribute(value, this._regions, i => i.Instrument.RegionCode, "region");
        }

        private bool FilterOnSector(IUniverseEvent value)
        {
            return this.FilterOnOrderAttribute(value, this._sectors, i => i.Instrument.SectorCode, "sector");
        }

        private bool FilterOnStrategy(IUniverseEvent value)
        {
            return this.FilterOnOrderAttribute(value, this._strategies, i => i.OrderStrategy, "strategy");
        }

        private bool FilterOnTraders(IUniverseEvent value)
        {
            return this.FilterOnOrderAttribute(value, this._traders, i => i.OrderTraderId, "Trader");
        }
    }
}