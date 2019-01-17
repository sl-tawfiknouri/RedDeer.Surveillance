using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Api.MarketOpenClose.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.MarketEvents.Interfaces;

namespace Surveillance.Universe.MarketEvents
{
    public class MarketOpenCloseEventManager : IMarketOpenCloseEventManager
    {
        private readonly IMarketOpenCloseApiCachingDecoratorRepository _marketOpenCloseRepository;
        private readonly ILogger<MarketOpenCloseEventManager> _logger;

        public MarketOpenCloseEventManager(
            IMarketOpenCloseApiCachingDecoratorRepository marketOpenCloseRepository,
            ILogger<MarketOpenCloseEventManager> logger)
        {
            _marketOpenCloseRepository =
                marketOpenCloseRepository
                ?? throw new ArgumentNullException(nameof(marketOpenCloseRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IReadOnlyCollection<IUniverseEvent>> AllOpenCloseEvents(DateTime start, DateTime end)
        {
            _logger.LogInformation($"MarketOpenCloseEventManager fetching market events from {start} to {end}");

            var markets = await _marketOpenCloseRepository.Get();
            var exchangeMarkets = markets.Select(m => new ExchangeMarket(m)).ToList();

            var extendedStart = start.AddDays(-1);
            var extendedEnd = end.AddDays(1);

            var response = OpenCloseEvents(exchangeMarkets, extendedStart, extendedEnd);

            _logger.LogInformation($"MarketOpenCloseEventManager completed fetching market events from {start} to {end} and found {response?.Count} market open/close events");

            return response;
        }

        private IReadOnlyCollection<IUniverseEvent> OpenCloseEvents(
            IReadOnlyCollection<ExchangeMarket> markets,
            DateTime start,
            DateTime end)
        {
            var universeEvents = new List<IUniverseEvent>();

            foreach (var market in markets)
            {
                var marketEvents = SetOpenCloseEventsForMarket(market, start, end);
                universeEvents.AddRange(marketEvents);
            }

            universeEvents = TrimTimeline(start, end, universeEvents);
            return universeEvents.OrderBy(ue => ue.EventTime).ToList();
        }

        private List<IUniverseEvent> SetOpenCloseEventsForMarket(
            ExchangeMarket market,
            DateTime start,
            DateTime end)
        {
            var localStart = TimeZoneInfo.ConvertTime(start, market.TimeZone);
            var localEnd = TimeZoneInfo.ConvertTime(end, market.TimeZone);

            var runLength = Math.Ceiling((localEnd - localStart).TotalDays);

            var events = new List<IUniverseEvent>();
            for (var i = 0; i <= runLength; i++)
            {
                var startInLocal = localStart.Date.AddDays(i).Add(market.MarketOpenTime);
                var closeInLocal = localStart.Date.AddDays(i).Add(market.MarketCloseTime);
                var marketOpenClose = new MarketOpenClose(market.Code, startInLocal.ToUniversalTime(), closeInLocal.ToUniversalTime());

                if (startInLocal >= localStart
                    && startInLocal <= localEnd)
                {
                    events.Add(new UniverseEvent(UniverseStateEvent.ExchangeOpen, startInLocal.ToUniversalTime(), marketOpenClose));
                }

                if (closeInLocal >= localStart
                    && closeInLocal <= localEnd)
                {
                    events.Add(new UniverseEvent(UniverseStateEvent.ExchangeClose, closeInLocal.ToUniversalTime(), marketOpenClose));
                }
            }

            return events;
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
    }
}