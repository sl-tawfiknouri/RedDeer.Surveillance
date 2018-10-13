using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Surveillance.DataLayer.Api.MarketOpenClose.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.MarketEvents.Interfaces;

namespace Surveillance.Universe.MarketEvents
{
    public class MarketOpenCloseEventManager : IMarketOpenCloseEventManager
    {
        private readonly IMarketOpenCloseApiCachingDecoratorRepository _marketOpenCloseRepository;

        public MarketOpenCloseEventManager(IMarketOpenCloseApiCachingDecoratorRepository marketOpenCloseRepository)
        {
            _marketOpenCloseRepository =
                marketOpenCloseRepository
                ?? throw new ArgumentNullException(nameof(marketOpenCloseRepository));
        }

        public async Task<IReadOnlyCollection<IUniverseEvent>> AllOpenCloseEvents(DateTime start, DateTime end)
        {
            var markets = await _marketOpenCloseRepository.Get();
            var exchangeMarkets = markets.Select(m => new ExchangeMarket(m)).ToList();

            return OpenCloseEvents(exchangeMarkets, start, end);
        }

        private IReadOnlyCollection<IUniverseEvent> OpenCloseEvents(
            IReadOnlyCollection<ExchangeMarket> markets,
            DateTime start,
            DateTime end)
        {
            var universeEvents = new List<IUniverseEvent>();

            start = start.ToUniversalTime();
            end = end.ToUniversalTime();

            foreach (var market in markets)
            {
                var marketOpeningSeconds = market.MarketOpenTime.TotalSeconds;
                var marketClosureSeconds = market.MarketCloseTime.TotalSeconds;

                var openingOnFirstDay = SetMarketEventTime(market, start, marketOpeningSeconds);
                var closingOnFirstDay = SetMarketEventTime(market, start, marketClosureSeconds);

                var initialDayMarketHours = new MarketOpenClose(market.Code, openingOnFirstDay, closingOnFirstDay);
                AddInitialOpen(start, end, universeEvents, initialDayMarketHours, openingOnFirstDay, market);
                AddInitialClose(start, end, universeEvents, initialDayMarketHours, closingOnFirstDay, market);

                AddConcludingOpenAndClose(start, end, universeEvents, market, marketOpeningSeconds, marketClosureSeconds);

                var intraPeriodEvents = AddIntraMarketEvents(start, end, openingOnFirstDay, closingOnFirstDay, market);
                universeEvents.AddRange(intraPeriodEvents);
            }

            universeEvents = TrimTimeline(start, end, universeEvents);

            return universeEvents.OrderBy(ue => ue.EventTime).ToList();
        }

        private List<IUniverseEvent> TrimTimeline(
            DateTime start,
            DateTime end,
            List<IUniverseEvent> universeEvents)
        {
            if (universeEvents == null)
            {
                return new List<IUniverseEvent>();
            }

            universeEvents =
                universeEvents
                    .Where(ue => ue != null)
                    .Where(ue => ue.EventTime.Date >= start.Date)
                    .Where(ue => ue.EventTime.Date <= end.Date)
                    .ToList();

            return universeEvents;
        }

        private DateTime SetMarketEventTime(ExchangeMarket exchangeMarket, DateTime start, double marketEventSeconds)
        {
            var marketEvent = exchangeMarket.DateTime(start.ToUniversalTime());
            marketEvent = marketEvent.AddSeconds(marketEventSeconds);

            return marketEvent.ToUniversalTime().DateTime;
        }

        private void AddInitialOpen(
            DateTime start,
            DateTime end,
            List<IUniverseEvent> universeEvents,
            MarketOpenClose market,
            DateTime openingOnFirstDay,
            ExchangeMarket exchange)
        {
            var initialOpen = AddBoundaryMarketEvent(start, end, market, openingOnFirstDay, UniverseStateEvent.StockMarketOpen);

            if (initialOpen != null && exchange.IsOpenOnDay(openingOnFirstDay))
            {
                universeEvents.Add(initialOpen);
            }
        }

        private void AddInitialClose(
            DateTime start,
            DateTime end,
            List<IUniverseEvent> universeEvents,
            MarketOpenClose market,
            DateTime closingOnFirstDay,
            ExchangeMarket exchange)
        {
            var initialClose = AddBoundaryMarketEvent(start, end, market, closingOnFirstDay, UniverseStateEvent.StockMarketClose);

            if (initialClose != null && exchange.IsOpenOnDay(closingOnFirstDay))
            {
                universeEvents.Add(initialClose);
            }
        }

        private void AddConcludingOpenAndClose(
            DateTime start,
            DateTime end,
            List<IUniverseEvent> universeEvents,
            ExchangeMarket market,
            double marketOpensInSecondsFromMidnight,
            double marketClosureInSecondsFromMidnight)
        {
            if (!(Math.Floor((end.Date - start.Date).TotalDays) > 0))
            {
                return;
            }
            
            var openingOnLastDay = market.DateTime(end);
            openingOnLastDay = openingOnLastDay.AddSeconds(marketOpensInSecondsFromMidnight);
            var openingOnLastDayUniversal = openingOnLastDay.ToUniversalTime().DateTime;

            var closingOnLastDay = market.DateTime(end);
            closingOnLastDay = closingOnLastDay.AddSeconds(marketClosureInSecondsFromMidnight);
            var closingOnLastDayUniversal = closingOnLastDay.ToUniversalTime().DateTime;

            var closingDayMarketHours = new MarketOpenClose(market.Code, openingOnLastDayUniversal, closingOnLastDayUniversal);

            var finalOpen = AddBoundaryMarketEvent(start, end, closingDayMarketHours, openingOnLastDayUniversal, UniverseStateEvent.StockMarketOpen);
            if (finalOpen != null && market.IsOpenOnDay(openingOnLastDay.DateTime))
            {
                universeEvents.Add(finalOpen);
            }

            var finalClose = AddBoundaryMarketEvent(start, end, closingDayMarketHours, closingOnLastDayUniversal, UniverseStateEvent.StockMarketClose);
            if (finalClose != null && market.IsOpenOnDay(closingOnLastDay.DateTime))
            {
                universeEvents.Add(finalClose);
            }
        }

        private IUniverseEvent AddBoundaryMarketEvent(
            DateTime start,
            DateTime end,
            MarketOpenClose market,
            DateTime boundaryEventTime,
            UniverseStateEvent boundaryType)
        {
            if (boundaryEventTime <= start
                || boundaryEventTime >= end)
            {
                return null;
            }

            var boundaryEvent = new UniverseEvent(boundaryType, boundaryEventTime, market);

            return boundaryEvent;
        }

        /// <summary>
        /// Open close events on days that are not the initial nor concluding day
        /// </summary>
        private IReadOnlyCollection<IUniverseEvent> AddIntraMarketEvents(
            DateTime start,
            DateTime end,
            DateTime initialOpening,
            DateTime initialClosure,
            ExchangeMarket openClose)
        {
            var universeEvents = new List<IUniverseEvent>();
            
            var days = Math.Floor((end - start).TotalDays) - 1; // subtract last day as we handle that explicitly
            for (var x = 1; x <= days; x++)
            {
                var open = initialOpening.AddDays(x);
                var close = initialClosure.AddDays(x);
                var marketHoursOnDay = new MarketOpenClose(openClose.Code, open, close);
                var localTime = TimeZoneInfo.ConvertTimeFromUtc(open, openClose.TimeZone);

                if (openClose.IsOpenOnDay(localTime))
                {
                    var openingEvent = new UniverseEvent(UniverseStateEvent.StockMarketOpen, open, marketHoursOnDay);
                    var closingEvent = new UniverseEvent(UniverseStateEvent.StockMarketClose, close, marketHoursOnDay);
                    universeEvents.Add(openingEvent);
                    universeEvents.Add(closingEvent);
                }
            }

            return universeEvents;
        }
    }
}