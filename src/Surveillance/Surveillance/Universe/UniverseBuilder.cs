using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainV2.Equity.TimeBars;
using DomainV2.Financial;
using DomainV2.Scheduling;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Market.Interfaces;
using Surveillance.DataLayer.Aurora.Trade.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.MarketEvents.Interfaces;

namespace Surveillance.Universe
{
    /// <summary>
    /// Generates a universe from the existing data set
    /// </summary>
    public class UniverseBuilder : IUniverseBuilder
    {
        private readonly IOrdersRepository _auroraOrdersRepository;
        private readonly IReddeerMarketRepository _auroraMarketRepository;
        private readonly IMarketOpenCloseEventManager _marketManager;
        private readonly IUniverseSortComparer _universeSorter;
        private readonly ILogger<UniverseBuilder> _logger;

        public UniverseBuilder(
            IOrdersRepository auroraOrdersRepository,
            IReddeerMarketRepository auroraMarketRepository,
            IMarketOpenCloseEventManager marketManager,
            IUniverseSortComparer universeSorter,
            ILogger<UniverseBuilder> logger)
        {
            _auroraOrdersRepository = auroraOrdersRepository ?? throw new ArgumentNullException(nameof(auroraOrdersRepository));
            _auroraMarketRepository = auroraMarketRepository ?? throw new ArgumentNullException(nameof(auroraMarketRepository));
            _marketManager = marketManager ?? throw new ArgumentNullException(nameof(marketManager));
            _universeSorter = universeSorter ?? throw new ArgumentNullException(nameof(universeSorter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crack the cosmic egg and unscramble your reality
        /// </summary>
        public async Task<IUniverse> Summon(ScheduledExecution execution, ISystemProcessOperationContext opCtx)
        {
            if (execution == null)
            {
                _logger.LogError($"UniverseBuilder had a null execution and therefore null data sets for {opCtx.Id} operation context");
                return new Universe(null, null, null);
            }

            _logger.LogInformation($"UniverseBuilder fetching aurora trade data");
            var projectedTrades = await TradeDataFetchAurora(execution, opCtx);
            _logger.LogInformation($"UniverseBuilder completed fetching aurora trade data");

            _logger.LogInformation($"UniverseBuilder completed fetching market data for equities");
            var exchangeFrames = await MarketEquityDataFetchAurora(execution, opCtx);
            _logger.LogInformation($"UniverseBuilder completed fetching market data for equities");

            _logger.LogInformation($"UniverseBuilder fetching universe event data");
            var universe = await UniverseEvents(execution, projectedTrades, exchangeFrames);
            _logger.LogInformation($"UniverseBuilder completed fetching universe event data");

            _logger.LogInformation($"UniverseBuilder returning a new universe");
            return new Universe(projectedTrades, exchangeFrames, universe);
        }

        private async Task<IReadOnlyCollection<Order>> TradeDataFetchAurora(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx)
        {
            var trades =
                await _auroraOrdersRepository.Get(
                    execution.TimeSeriesInitiation.Date,
                    execution.TimeSeriesTermination.Date,
                    opCtx);

            return trades ?? new List<Order>();
        }

        private async Task<IReadOnlyCollection<MarketTimeBarCollection>> MarketEquityDataFetchAurora(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx)
        {
            var equities =
                await _auroraMarketRepository.GetEquityIntraday(
                    execution.TimeSeriesInitiation.Date,
                    execution.TimeSeriesTermination.Date,
                    opCtx);

            return equities ?? new List<MarketTimeBarCollection>();
        }

        private async Task<IReadOnlyCollection<IUniverseEvent>> UniverseEvents(
            ScheduledExecution execution,
            IReadOnlyCollection<Order> trades,
            IReadOnlyCollection<MarketTimeBarCollection> equityIntradayUpdates)
        {
            var tradeSubmittedEvents =
                trades
                    .Where(tr => tr != null)
                    .Select(tr => new UniverseEvent(UniverseStateEvent.OrderPlaced, tr.OrderPlacedDate.GetValueOrDefault(), tr))
                    .ToArray();

            var tradeStatusChangedOnEvents =
                trades
                    .Where(tr => tr != null)
                    .Where(tr => ! (tr.OrderStatus() == OrderStatus.Booked && tr.OrderPlacedDate == tr.MostRecentDateEvent()))
                    .Select(tr => new UniverseEvent(UniverseStateEvent.Order, tr.MostRecentDateEvent(), tr))
                    .ToArray();

            var exchangeEvents =
                equityIntradayUpdates
                    .Select(exch => new UniverseEvent(UniverseStateEvent.EquityIntradayTick, exch.Epoch, exch))
                    .ToArray();

            var marketEvents =
                await _marketManager
                    .AllOpenCloseEvents(
                        execution.TimeSeriesInitiation.DateTime,
                        execution.TimeSeriesTermination.DateTime);

            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, execution.TimeSeriesInitiation.DateTime, execution);

            var intraUniversalHistoryEvents = new List<IUniverseEvent>();
            intraUniversalHistoryEvents.AddRange(tradeSubmittedEvents);
            intraUniversalHistoryEvents.AddRange(tradeStatusChangedOnEvents);
            intraUniversalHistoryEvents.AddRange(exchangeEvents);
            intraUniversalHistoryEvents.AddRange(marketEvents);
            var orderedIntraUniversalHistory = intraUniversalHistoryEvents.OrderBy(ihe => ihe, _universeSorter).ToList();

            var universeEvents = new List<IUniverseEvent> {genesis};
            universeEvents.AddRange(orderedIntraUniversalHistory);

            var youngestEventInUniverse = orderedIntraUniversalHistory.Any()
                ? orderedIntraUniversalHistory.Max(i => i.EventTime)
                : execution.TimeSeriesTermination.DateTime;
            var eschatonDate = youngestEventInUniverse > execution.TimeSeriesTermination.DateTime
                ? youngestEventInUniverse
                : execution.TimeSeriesTermination.DateTime;
            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, eschatonDate, execution);
            universeEvents.Add(eschaton);

            return universeEvents;
        }
    }
}