using System;
using System.Collections.Generic;
using System.Linq;
using Surveillance.DataLayer.Stub;
using Surveillance.DataLayer.Stub.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.MarketEvents.Interfaces;

namespace Surveillance.Universe.MarketEvents
{
    public class MarketOpenCloseEventManager : IMarketOpenCloseEventManager
    {
        private readonly IMarketOpenCloseRepository _marketOpenCloseRepository;

        public MarketOpenCloseEventManager(IMarketOpenCloseRepository marketOpenCloseRepository)
        {
            _marketOpenCloseRepository =
                marketOpenCloseRepository
                ?? throw new ArgumentNullException(nameof(marketOpenCloseRepository));
        }

        public IReadOnlyCollection<IUniverseEvent> AllOpenCloseEvents(DateTime start, DateTime end)
        {
            var markets = _marketOpenCloseRepository.GetAll();

            return OpenCloseEvents(markets, start, end);
        }

        public IReadOnlyCollection<IUniverseEvent> OpenCloseEvents(
            IReadOnlyCollection<string> marketIds,
            DateTime start,
            DateTime end)
        {
            var markets = _marketOpenCloseRepository.Get(marketIds);

            return OpenCloseEvents(markets, start, end);
        }

        private IReadOnlyCollection<IUniverseEvent> OpenCloseEvents(
            IReadOnlyCollection<MarketOpenClose> markets,
            DateTime start,
            DateTime end)
        {

            var universeEvents = new List<IUniverseEvent>();

            foreach (var market in markets)
            {
                var marketOpeningSeconds = market.MarketOpen.TimeOfDay.TotalSeconds;
                var marketClosureSeconds = market.MarketClose.TimeOfDay.TotalSeconds;

                var openingOnFirstDay = new DateTime(start.Year, start.Month, start.Day);
                openingOnFirstDay = openingOnFirstDay.AddSeconds(marketOpeningSeconds);

                var closingOnFirstDay = new DateTime(start.Year, start.Month, start.Day);
                closingOnFirstDay = closingOnFirstDay.AddSeconds(marketClosureSeconds);
                
                var initialDayMarketHours = new MarketOpenClose(market.MarketId, openingOnFirstDay, closingOnFirstDay);
                AddInitialOpen(start, end, universeEvents, initialDayMarketHours, openingOnFirstDay);
                AddInitialClose(start, end, universeEvents, initialDayMarketHours, closingOnFirstDay);

                AddConcludingOpenAndClose(start, end, universeEvents, market, marketOpeningSeconds, marketClosureSeconds);

                var intraPeriodEvents = AddIntraMarketEvents(start, end, openingOnFirstDay, closingOnFirstDay, market);
                universeEvents.AddRange(intraPeriodEvents);
            }

            return universeEvents.OrderBy(ue => ue.EventTime).ToList();
        }

        private void AddInitialOpen(
            DateTime start,
            DateTime end,
            List<IUniverseEvent> universeEvents,
            MarketOpenClose market,
            DateTime openingOnFirstDay)
        {
            var initialOpen = AddBoundaryMarketEvent(start, end, market, openingOnFirstDay, UniverseStateEvent.StockMarketOpen);
            if (initialOpen != null)
            {
                universeEvents.Add(initialOpen);
            }
        }

        private void AddInitialClose(
            DateTime start,
            DateTime end,
            List<IUniverseEvent> universeEvents,
            MarketOpenClose market,
            DateTime closingOnFirstDay)
        {
            var initialClose = AddBoundaryMarketEvent(start, end, market, closingOnFirstDay, UniverseStateEvent.StockMarketClose);
            if (initialClose != null)
            {
                universeEvents.Add(initialClose);
            }
        }

        private void AddConcludingOpenAndClose(
            DateTime start,
            DateTime end,
            List<IUniverseEvent> universeEvents,
            MarketOpenClose market,
            double marketOpensInSecondsFromMidnight,
            double marketClosureInSecondsFromMidnight)
        {
            if (!(Math.Floor((end.Date - start.Date).TotalDays) > 0))
            {
                return;
            }

            var openingOnLastDay = new DateTime(end.Year, end.Month, end.Day);
            openingOnLastDay = openingOnLastDay.AddSeconds(marketOpensInSecondsFromMidnight);

            var closingOnLastDay = new DateTime(end.Year, end.Month, end.Day);
            closingOnLastDay = closingOnLastDay.AddSeconds(marketClosureInSecondsFromMidnight);

            var closingDayMarketHours = new MarketOpenClose(market.MarketId, openingOnLastDay, closingOnLastDay);

            var finalOpen =
                AddBoundaryMarketEvent(
                    start,
                    end,
                    closingDayMarketHours,
                    openingOnLastDay,
                    UniverseStateEvent.StockMarketOpen);

            if (finalOpen != null)
            {
                universeEvents.Add(finalOpen);
            }

            var finalClose =
                AddBoundaryMarketEvent(
                    start,
                    end,
                    closingDayMarketHours,
                    closingOnLastDay, 
                    UniverseStateEvent.StockMarketClose);

            if (finalClose != null)
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
            if (boundaryEventTime <= start || boundaryEventTime >= end)
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
            MarketOpenClose openClose)
        {
            var universeEvents = new List<IUniverseEvent>();
            
            var days = Math.Floor((end - start).TotalDays) - 1; // subtract last day as we handle that explicitly
            for (var x = 1; x <= days; x++)
            {
                var open = initialOpening.AddDays(x);
                var close = initialClosure.AddDays(x);
                var marketHoursOnDay = new MarketOpenClose(openClose.MarketId, open, close);

                var openingEvent = new UniverseEvent(UniverseStateEvent.StockMarketOpen, open, marketHoursOnDay);
                var closingEvent = new UniverseEvent(UniverseStateEvent.StockMarketClose, close, marketHoursOnDay);

                universeEvents.Add(openingEvent);
                universeEvents.Add(closingEvent);
            }

            return universeEvents;
        }
    }
}