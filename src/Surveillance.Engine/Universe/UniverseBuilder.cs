using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Core.Markets.Collections;
using Domain.Core.Trading.Orders;
using Domain.Surveillance.Scheduling;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Aurora.Market.Interfaces;
using Surveillance.DataLayer.Aurora.Orders.Interfaces;
using Surveillance.Engine.Rules.Trades.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.MarketEvents.Interfaces;

namespace Surveillance.Engine.Rules.Universe
{
    /// <summary>
    /// Generates a universe from the existing data set
    /// </summary>
    public class UniverseBuilder : IUniverseBuilder
    {
        private readonly IOrdersRepository _auroraOrdersRepository;
        private readonly IOrdersToAllocatedOrdersProjector _allocateOrdersProjector;
        private readonly IReddeerMarketRepository _auroraMarketRepository;
        private readonly IMarketOpenCloseEventService _marketService;
        private readonly IUniverseSortComparer _universeSorter;
        private readonly ILogger<UniverseBuilder> _logger;

        public UniverseBuilder(
            IOrdersRepository auroraOrdersRepository,
            IOrdersToAllocatedOrdersProjector allocateOrdersProjector,
            IReddeerMarketRepository auroraMarketRepository,
            IMarketOpenCloseEventService marketService,
            IUniverseSortComparer universeSorter,
            ILogger<UniverseBuilder> logger)
        {
            _auroraOrdersRepository = auroraOrdersRepository ?? throw new ArgumentNullException(nameof(auroraOrdersRepository));
            _allocateOrdersProjector = allocateOrdersProjector ?? throw new ArgumentNullException(nameof(allocateOrdersProjector));
            _auroraMarketRepository = auroraMarketRepository ?? throw new ArgumentNullException(nameof(auroraMarketRepository));
            _marketService = marketService ?? throw new ArgumentNullException(nameof(marketService));
            _universeSorter = universeSorter ?? throw new ArgumentNullException(nameof(universeSorter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crack the cosmic egg and unscramble your reality
        /// </summary>
        public async Task<IUniverse> Summon(ScheduledExecution execution, ISystemProcessOperationContext opCtx)
        {
            return await Summon(execution, opCtx, true, true, execution.TimeSeriesInitiation, execution.TimeSeriesTermination);
        }

        public async Task<IUniverse> Summon(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            bool includeGenesis,
            bool includeEschaton,
            DateTimeOffset? realUniverseEpoch,
            DateTimeOffset? futureUniverseEpoch)
        {
            if (execution == null)
            {
                _logger.LogError($"had a null execution or rule parameters and therefore null data sets for {opCtx.Id} operation context");
                return new Universe(null);
            }

            _logger.LogInformation($"fetching aurora trade data");
            var projectedTrades = await TradeDataFetchAurora(execution, opCtx);
            _logger.LogInformation($"completed fetching aurora trade data");

            _logger.LogInformation($"fetching aurora trade allocation data");
            var projectedTradesAllocations = await _allocateOrdersProjector.DecorateOrders(projectedTrades);
            _logger.LogInformation($"completed fetching aurora trade allocation data");

            _logger.LogInformation($"fetching intraday for equities");
            var intradayEquityBars = await MarketEquityIntraDayDataFetchAurora(execution, opCtx);
            _logger.LogInformation($"completed fetching intraday for equities");

            _logger.LogInformation($"fetching inter day for equities");
            var interDayEquityBars = await MarketEquityInterDayDataFetchAurora(execution, opCtx);
            _logger.LogInformation($"completed fetching inter day for equities");

            _logger.LogInformation($"fetching universe event data");
            var universe =
                await UniverseEvents(
                    execution,
                    projectedTradesAllocations,
                    intradayEquityBars,
                    interDayEquityBars, 
                    includeGenesis,
                    includeEschaton,
                    realUniverseEpoch,
                    futureUniverseEpoch);

            _logger.LogInformation($"completed fetching universe event data");

            _logger.LogInformation($"returning a new universe");
            return new Universe(universe);
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

        private async Task<IReadOnlyCollection<EquityIntraDayTimeBarCollection>> MarketEquityIntraDayDataFetchAurora(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx)
        {
            var equities =
                await _auroraMarketRepository.GetEquityIntraday(
                    execution.TimeSeriesInitiation.Subtract(execution.LeadingTimespan ?? TimeSpan.Zero).Date,
                    execution.TimeSeriesTermination.Date,
                    opCtx);

            return equities ?? new List<EquityIntraDayTimeBarCollection>();
        }

        private async Task<IReadOnlyCollection<EquityInterDayTimeBarCollection>> MarketEquityInterDayDataFetchAurora(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx)
        {
            var equities =
                await _auroraMarketRepository.GetEquityInterDay(
                    execution.TimeSeriesInitiation.Subtract(execution.LeadingTimespan ?? TimeSpan.Zero).Date,
                    execution.TimeSeriesTermination.Date,
                    opCtx);

            return equities ?? new List<EquityInterDayTimeBarCollection>();
        }

        private async Task<IReadOnlyCollection<IUniverseEvent>> UniverseEvents(
            ScheduledExecution execution,
            IReadOnlyCollection<Order> trades,
            IReadOnlyCollection<EquityIntraDayTimeBarCollection> equityIntradayUpdates,
            IReadOnlyCollection<EquityInterDayTimeBarCollection> equityInterDayUpdates,
            bool includeGenesis,
            bool includeEschaton,
            DateTimeOffset? realUniverseEpoch,
            DateTimeOffset? futureUniverseEpoch)
        {
            var tradeSubmittedEvents =
                trades
                    .Where(tr => tr != null)
                    .Select(tr => new UniverseEvent(UniverseStateEvent.OrderPlaced, tr.PlacedDate.GetValueOrDefault(), tr))
                    .ToArray();

            var tradeStatusChangedOnEvents =
                trades
                    .Where(tr => tr != null)
                    .Where(tr => ! (tr.OrderStatus() == OrderStatus.Booked && tr.PlacedDate == tr.MostRecentDateEvent()))
                    .Select(tr => new UniverseEvent(UniverseStateEvent.Order, tr.MostRecentDateEvent(), tr))
                    .ToArray();

            var tradeFilledEvents =
                trades
                    .Where(tr => tr != null)
                    .Where(tr => tr.OrderStatus() == OrderStatus.Filled)
                    .Where(tr => tr.FilledDate != null)
                    .Select(tr => new UniverseEvent(UniverseStateEvent.OrderFilled, tr.FilledDate.Value, tr))
                    .ToArray();

            var intradayEquityEvents =
                equityIntradayUpdates
                    .Select(exch => new UniverseEvent(UniverseStateEvent.EquityIntradayTick, exch.Epoch, exch))
                    .ToArray();

            var interDayEquityEvents =
                equityInterDayUpdates
                    .Select(exch => new UniverseEvent(UniverseStateEvent.EquityInterDayTick, exch.Epoch, exch))
                    .ToArray();

            var marketEvents =
                await _marketService
                    .AllOpenCloseEvents(
                        execution.TimeSeriesInitiation.DateTime,
                        execution.TimeSeriesTermination.DateTime);

            var intraUniversalHistoryEvents = new List<IUniverseEvent>();
            intraUniversalHistoryEvents.AddRange(tradeSubmittedEvents);
            intraUniversalHistoryEvents.AddRange(tradeStatusChangedOnEvents);
            intraUniversalHistoryEvents.AddRange(tradeFilledEvents);
            intraUniversalHistoryEvents.AddRange(intradayEquityEvents);
            intraUniversalHistoryEvents.AddRange(interDayEquityEvents);
            intraUniversalHistoryEvents.AddRange(marketEvents);
            var orderedIntraUniversalHistory = intraUniversalHistoryEvents.OrderBy(ihe => ihe, _universeSorter).ToList();

            var universeEvents = new List<IUniverseEvent>();

            if (includeGenesis)
            {
                var genesis = new UniverseEvent(UniverseStateEvent.Genesis, execution.TimeSeriesInitiation.DateTime, execution);
                var primordialEpoch = new UniverseEvent(UniverseStateEvent.EpochPrimordialUniverse, execution.TimeSeriesInitiation.DateTime, execution);
                universeEvents.Add(genesis);
                universeEvents.Add(primordialEpoch);
            }

            if (realUniverseEpoch != null 
                && realUniverseEpoch >= execution.TimeSeriesInitiation 
                && realUniverseEpoch <= execution.TimeSeriesTermination)
            {
                var realUniverseEpochEvent = new UniverseEvent(UniverseStateEvent.EpochRealUniverse, realUniverseEpoch.GetValueOrDefault().DateTime, execution);
                universeEvents.Add(realUniverseEpochEvent);
            }

            if (futureUniverseEpoch != null
                && futureUniverseEpoch >= execution.TimeSeriesInitiation
                && futureUniverseEpoch <= execution.TimeSeriesTermination)
            {
                var futureUniverseEpochEvent = new UniverseEvent(UniverseStateEvent.EpochFutureUniverse, futureUniverseEpoch.GetValueOrDefault().DateTime, execution);
                universeEvents.Add(futureUniverseEpochEvent);
            }

            universeEvents.AddRange(orderedIntraUniversalHistory);

            var youngestEventInUniverse = orderedIntraUniversalHistory.Any()
                ? orderedIntraUniversalHistory.Max(i => i.EventTime)
                : execution.TimeSeriesTermination.DateTime;
            var eschatonDate = youngestEventInUniverse > execution.TimeSeriesTermination.DateTime
                ? youngestEventInUniverse
                : execution.TimeSeriesTermination.DateTime;

            if (includeEschaton)
            {
                var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, eschatonDate, execution);
                universeEvents.Add(eschaton);
            }

            universeEvents = universeEvents.OrderBy(ue => ue, _universeSorter).ToList();

            return universeEvents;
        }
    }
}