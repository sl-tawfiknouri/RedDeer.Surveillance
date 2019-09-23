namespace Surveillance.Engine.Rules.Universe.MarketEvents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.MarketEvents.Interfaces;
    using Surveillance.Reddeer.ApiClient.MarketOpenClose.Interfaces;

    public class MarketOpenCloseEventService : IMarketOpenCloseEventService
    {
        private readonly ILogger<MarketOpenCloseEventService> _logger;

        private readonly IMarketOpenCloseApiCachingDecorator _marketOpenCloseRepository;

        public MarketOpenCloseEventService(
            IMarketOpenCloseApiCachingDecorator marketOpenCloseRepository,
            ILogger<MarketOpenCloseEventService> logger)
        {
            this._marketOpenCloseRepository = marketOpenCloseRepository
                                              ?? throw new ArgumentNullException(nameof(marketOpenCloseRepository));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IReadOnlyCollection<IUniverseEvent>> AllOpenCloseEvents(DateTime start, DateTime end)
        {
            this._logger.LogInformation($"fetching market events from {start} to {end}");

            var markets = await this._marketOpenCloseRepository.Get();
            var exchangeMarkets = markets.Select(m => new ExchangeMarket(m)).ToList();

            var extendedStart = start.AddDays(-1);
            var extendedEnd = end.AddDays(1);

            var response = this.OpenCloseEvents(exchangeMarkets, extendedStart, extendedEnd);

            this._logger.LogInformation(
                $"completed fetching market events from {start} to {end} and found {response?.Count} market open/close events");

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
                var marketEvents = this.SetOpenCloseEventsForMarket(market, start, end);
                universeEvents.AddRange(marketEvents);
            }

            universeEvents = this.TrimTimeline(start, end, universeEvents);
            return universeEvents.OrderBy(ue => ue.EventTime).ToList();
        }

        private List<IUniverseEvent> SetOpenCloseEventsForMarket(ExchangeMarket market, DateTime start, DateTime end)
        {
            var localStart = TimeZoneInfo.ConvertTime(start, market.TimeZone);
            var localEnd = TimeZoneInfo.ConvertTime(end, market.TimeZone);

            var runLength = Math.Ceiling((localEnd - localStart).TotalDays);

            var events = new List<IUniverseEvent>();
            for (var i = 0; i <= runLength; i++)
            {
                var startInLocal = localStart.Date.AddDays(i).Add(market.MarketOpenTime);
                var closeInLocal = localStart.Date.AddDays(i).Add(market.MarketCloseTime);
                var marketOpenClose = new MarketOpenClose(
                    market.Code,
                    startInLocal.ToUniversalTime(),
                    closeInLocal.ToUniversalTime());

                if (startInLocal >= localStart && startInLocal <= localEnd)
                    events.Add(
                        new UniverseEvent(
                            UniverseStateEvent.ExchangeOpen,
                            startInLocal.ToUniversalTime(),
                            marketOpenClose));

                if (closeInLocal >= localStart && closeInLocal <= localEnd)
                    events.Add(
                        new UniverseEvent(
                            UniverseStateEvent.ExchangeClose,
                            closeInLocal.ToUniversalTime(),
                            marketOpenClose));
            }

            return events;
        }

        private List<IUniverseEvent> TrimTimeline(DateTime start, DateTime end, List<IUniverseEvent> universeEvents)
        {
            if (universeEvents == null) return new List<IUniverseEvent>();

            universeEvents = universeEvents.Where(ue => ue != null).Where(ue => ue.EventTime.Date >= start.Date)
                .Where(ue => ue.EventTime.Date <= end.Date).ToList();

            return universeEvents;
        }
    }
}