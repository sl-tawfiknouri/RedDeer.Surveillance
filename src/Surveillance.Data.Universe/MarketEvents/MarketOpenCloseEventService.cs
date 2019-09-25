namespace Surveillance.Data.Universe.MarketEvents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Data.Universe.MarketEvents.Interfaces;
    using Surveillance.Reddeer.ApiClient.MarketOpenClose.Interfaces;

    /// <summary>
    /// The market open close event service.
    /// </summary>
    public class MarketOpenCloseEventService : IMarketOpenCloseEventService
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<MarketOpenCloseEventService> logger;

        /// <summary>
        /// The market open close repository.
        /// </summary>
        private readonly IMarketOpenCloseApiCachingDecorator marketOpenCloseRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketOpenCloseEventService"/> class.
        /// </summary>
        /// <param name="marketOpenCloseRepository">
        /// The market open close repository.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public MarketOpenCloseEventService(
            IMarketOpenCloseApiCachingDecorator marketOpenCloseRepository,
            ILogger<MarketOpenCloseEventService> logger)
        {
            this.marketOpenCloseRepository = marketOpenCloseRepository
                                              ?? throw new ArgumentNullException(nameof(marketOpenCloseRepository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The all open close events.
        /// </summary>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="end">
        /// The end.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<IReadOnlyCollection<IUniverseEvent>> AllOpenCloseEvents(DateTime start, DateTime end)
        {
            this.logger.LogInformation($"fetching market events from {start} to {end}");

            var markets = await this.marketOpenCloseRepository.GetAsync();
            var exchangeMarkets = markets.Select(m => new ExchangeMarket(m)).ToList();

            var extendedStart = start.AddDays(-1);
            var extendedEnd = end.AddDays(1);

            var response = this.OpenCloseEvents(exchangeMarkets, extendedStart, extendedEnd);

            this.logger.LogInformation(
                $"completed fetching market events from {start} to {end} and found {response?.Count} market open/close events");

            return response;
        }

        /// <summary>
        /// The open close events.
        /// </summary>
        /// <param name="markets">
        /// The markets.
        /// </param>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="end">
        /// The end.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseEvent"/>.
        /// </returns>
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

        /// <summary>
        /// The set open close events for market.
        /// </summary>
        /// <param name="market">
        /// The market.
        /// </param>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="end">
        /// The end.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        private IList<IUniverseEvent> SetOpenCloseEventsForMarket(ExchangeMarket market, DateTime start, DateTime end)
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
                {
                    events.Add(
                        new UniverseEvent(
                            UniverseStateEvent.ExchangeOpen,
                            startInLocal.ToUniversalTime(),
                            marketOpenClose));
                }

                if (closeInLocal >= localStart && closeInLocal <= localEnd)
                {
                    events.Add(
                        new UniverseEvent(
                            UniverseStateEvent.ExchangeClose,
                            closeInLocal.ToUniversalTime(),
                            marketOpenClose));
                }
            }

            return events;
        }

        /// <summary>
        /// The trim timeline.
        /// </summary>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="end">
        /// The end.
        /// </param>
        /// <param name="universeEvents">
        /// The universe events.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private List<IUniverseEvent> TrimTimeline(DateTime start, DateTime end, List<IUniverseEvent> universeEvents)
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