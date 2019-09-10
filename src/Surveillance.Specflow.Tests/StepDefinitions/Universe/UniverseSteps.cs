namespace Surveillance.Specflow.Tests.StepDefinitions.Universe
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Financial.Money;
    using Domain.Core.Markets.Collections;
    using Domain.Core.Markets.Timebars;
    using Domain.Core.Trading.Orders;

    using Surveillance.Engine.Rules.Universe;
    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.MarketEvents;
    using Surveillance.Specflow.Tests.StepDefinitions.InterdayTrade;
    using Surveillance.Specflow.Tests.StepDefinitions.IntradayTrade;
    using Surveillance.Specflow.Tests.StepDefinitions.Orders;

    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

    [Binding]
    public sealed class UniverseSteps
    {
        private readonly ScenarioContext _scenarioContext;

        private readonly SecuritySelection _securitySelection;

        private readonly UniverseSelectionState _universeSelectionState;

        public UniverseSteps(
            ScenarioContext scenarioContext,
            UniverseSelectionState universeSelectionState,
            SecuritySelection securitySelection)
        {
            this._scenarioContext = scenarioContext;
            this._universeSelectionState = universeSelectionState;
            this._securitySelection = securitySelection;
        }

        [Given(@"I have the orders for a universe from (.*) to (.*) :")]
        public void GivenIHaveTheOrders(string from, string to, Table orderTable)
        {
            var eventList = new List<IUniverseEvent>();

            var fromDate = DateTime.Parse(from);
            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, fromDate, new object());

            var toDate = DateTime.Parse(to).AddDays(1).AddMilliseconds(-1);
            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, toDate, new object());

            eventList.Add(genesis);

            for (var i = 0; i <= toDate.Subtract(fromDate).Days; i++)
            {
                var closeEventXlon = new UniverseEvent(
                    UniverseStateEvent.ExchangeClose,
                    fromDate.Date.AddDays(i).AddHours(16),
                    new MarketOpenClose(
                        "XLON",
                        fromDate.Date.AddDays(i).AddHours(8),
                        fromDate.Date.AddDays(i).AddHours(16)));

                eventList.Add(closeEventXlon);

                var closeEventNasdaq = new UniverseEvent(
                    UniverseStateEvent.ExchangeClose,
                    fromDate.Date.AddDays(i).AddHours(23),
                    new MarketOpenClose(
                        "NASDAQ",
                        fromDate.Date.AddDays(i).AddHours(15),
                        fromDate.Date.AddDays(i).AddHours(23)));

                eventList.Add(closeEventNasdaq);
            }

            var orderParams = orderTable.CreateSet<OrderParameters>();

            foreach (var row in orderParams)
            {
                var orderEvent = this.MapRowToOrderEvent(row);

                if (orderEvent == null)
                    continue;

                foreach (var universeEvent in orderEvent)
                    eventList.Add(universeEvent);
            }

            eventList.Add(eschaton);
            eventList = eventList.OrderBy(i => i.EventTime).ToList();

            var universe = this.Build(eventList);

            this._universeSelectionState.SelectedUniverse = universe;
        }

        [Given(@"With the interday market data :")]
        public void GivenWithTheInterdayMarketData(Table interdayMarketDataTable)
        {
            if (this._universeSelectionState.SelectedUniverse == null || interdayMarketDataTable == null)
            {
                this._scenarioContext.Pending();
                return;
            }

            var eventList = new List<IUniverseEvent>();
            var interdayMarketDataParams = interdayMarketDataTable.CreateSet<InterdayMarketDataParameters>();
            foreach (var row in interdayMarketDataParams)
            {
                var proRow = this.MapRowToInterdayMarketDataEvent(row);

                if (proRow == null) continue;

                eventList.Add(proRow);
            }

            var otherEvents = this._universeSelectionState.SelectedUniverse.UniverseEvents.ToList();
            otherEvents.AddRange(eventList);

            var comparer = new UniverseEventComparer();
            var orderedUniverse = otherEvents.OrderBy(x => x, comparer).ToList();

            this._universeSelectionState.SelectedUniverse = new Universe(orderedUniverse);
        }

        [Given(@"With the intraday market data :")]
        public void GivenWithTheIntradayMarketData(Table intradayMarketDataTable)
        {
            if (this._universeSelectionState.SelectedUniverse == null || intradayMarketDataTable == null)
            {
                this._scenarioContext.Pending();
                return;
            }

            var eventList = new List<IUniverseEvent>();
            var intradayMarketDataParams = intradayMarketDataTable.CreateSet<IntradayMarketDataParameters>();
            foreach (var row in intradayMarketDataParams)
            {
                var proRow = this.MapRowToIntradayMarketDataEvent(row);

                if (proRow == null) continue;

                eventList.Add(proRow);
            }

            var otherEvents = this._universeSelectionState.SelectedUniverse.UniverseEvents.ToList();
            otherEvents.AddRange(eventList);

            var comparer = new UniverseEventComparer();
            var orderedUniverse = otherEvents.OrderBy(x => x, comparer).ToList();

            this._universeSelectionState.SelectedUniverse = new Universe(orderedUniverse);
        }

        private Universe Build(IReadOnlyCollection<IUniverseEvent> universeEvents)
        {
            return new Universe(universeEvents);
        }

        private IUniverseEvent MapRowToInterdayMarketDataEvent(InterdayMarketDataParameters marketDataParam)
        {
            if (marketDataParam == null) return null;

            if (string.IsNullOrWhiteSpace(marketDataParam.SecurityName)
                || !this._securitySelection.Securities.ContainsKey(marketDataParam.SecurityName))
            {
                this._scenarioContext.Pending();
                return null;
            }

            var security = this._securitySelection.Securities[marketDataParam.SecurityName];
            var open = this.MapToMoney(marketDataParam.Open, marketDataParam.Currency);
            var close = this.MapToMoney(marketDataParam.Close, marketDataParam.Currency);
            var high = this.MapToMoney(marketDataParam.High, marketDataParam.Currency);
            var low = this.MapToMoney(marketDataParam.Low, marketDataParam.Currency);
            var intradayPrices = new IntradayPrices(open, close, high, low);

            var dailySummary = new DailySummaryTimeBar(
                marketDataParam.MarketCap,
                marketDataParam.Currency,
                intradayPrices,
                marketDataParam.ListedSecurities,
                new Volume(marketDataParam.DailyVolume.GetValueOrDefault(0)),
                marketDataParam.Epoch);

            var marketData = new EquityInstrumentInterDayTimeBar(
                security.Instrument,
                dailySummary,
                marketDataParam.Epoch,
                security.Market);

            var timeBarCollection = new EquityInterDayTimeBarCollection(
                security.Market,
                marketDataParam.Epoch,
                new[] { marketData });
            var universeEvent = new UniverseEvent(
                UniverseStateEvent.EquityInterDayTick,
                marketDataParam.Epoch,
                timeBarCollection);

            return universeEvent;
        }

        private IUniverseEvent MapRowToIntradayMarketDataEvent(IntradayMarketDataParameters marketDataParam)
        {
            if (marketDataParam == null) return null;

            if (string.IsNullOrWhiteSpace(marketDataParam.SecurityName)
                || !this._securitySelection.Securities.ContainsKey(marketDataParam.SecurityName))
            {
                this._scenarioContext.Pending();
                return null;
            }

            if (marketDataParam.Bid == null || marketDataParam.Ask == null || marketDataParam.Price == null)
            {
                this._scenarioContext.Pending();
                return null;
            }

            var security = this._securitySelection.Securities[marketDataParam.SecurityName];
            var bid = this.MapToMoney(marketDataParam.Bid, marketDataParam.Currency);
            var ask = this.MapToMoney(marketDataParam.Ask, marketDataParam.Currency);
            var price = this.MapToMoney(marketDataParam.Price, marketDataParam.Currency);
            var volume = new Volume(marketDataParam.Volume.GetValueOrDefault(0));

            var intradayPrices = new SpreadTimeBar(bid.Value, ask.Value, price.Value, volume);

            var marketData = new EquityInstrumentIntraDayTimeBar(
                security.Instrument,
                intradayPrices,
                null,
                marketDataParam.Epoch,
                security.Market);

            var timeBarCollection = new EquityIntraDayTimeBarCollection(
                security.Market,
                marketDataParam.Epoch,
                new[] { marketData });
            var universeEvent = new UniverseEvent(
                UniverseStateEvent.EquityIntradayTick,
                marketDataParam.Epoch,
                timeBarCollection);

            return universeEvent;
        }

        private IUniverseEvent[] MapRowToOrderEvent(OrderParameters orderParam)
        {
            if (orderParam == null) return null;

            if (string.IsNullOrWhiteSpace(orderParam.SecurityName)
                || !this._securitySelection.Securities.ContainsKey(orderParam.SecurityName))
            {
                this._scenarioContext.Pending();
                return null;
            }

            var security = this._securitySelection.Securities[orderParam.SecurityName];

            var orderLimitPrice = orderParam.LimitPrice != null
                                      ? new Money(orderParam.LimitPrice, orderParam.Currency)
                                      : (Money?)null;

            var orderAveragePrice = orderParam.AverageFillPrice != null
                                        ? new Money(orderParam.AverageFillPrice, orderParam.Currency)
                                        : (Money?)null;

            var orderType = orderParam.LimitPrice != null ? OrderTypes.LIMIT : OrderTypes.MARKET;

            var order = new Order(
                security.Instrument,
                security.Market,
                0,
                orderParam.OrderId,
                orderParam.PlacedDate,
                null,
                null,
                null,
                orderParam.PlacedDate,
                orderParam.BookedDate,
                orderParam.AmendedDate,
                orderParam.RejectedDate,
                orderParam.CancelledDate,
                orderParam.FilledDate,
                orderType,
                orderParam.Direction,
                new Currency(orderParam.Currency),
                null,
                OrderCleanDirty.NONE,
                null,
                orderLimitPrice,
                orderAveragePrice,
                orderParam.OrderedVolume,
                orderParam.FilledVolume,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                OptionEuropeanAmerican.NONE,
                null);

            var universeEventOrder = new UniverseEvent(UniverseStateEvent.Order, order.MostRecentDateEvent(), order);
            var universeEventOrderPlaced = new UniverseEvent(
                UniverseStateEvent.OrderPlaced,
                order.PlacedDate.GetValueOrDefault(),
                order);
            var universeEventOrderFilled = new UniverseEvent(
                UniverseStateEvent.OrderFilled,
                order.FilledDate.GetValueOrDefault(),
                order);

            return new[] { universeEventOrder, universeEventOrderPlaced, universeEventOrderFilled };
        }

        private Money? MapToMoney(decimal? dec, string currency)
        {
            return dec != null ? (Money?)new Money(dec, currency) : null;
        }
    }
}