namespace Surveillance.Engine.Rules.Universe
{
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

    /// <summary>
    ///     Generates a universe from the existing data set
    /// </summary>
    public class UniverseBuilder : IUniverseBuilder
    {
        private readonly IOrdersToAllocatedOrdersProjector _allocateOrdersProjector;

        private readonly IReddeerMarketRepository _auroraMarketRepository;

        private readonly IOrdersRepository _auroraOrdersRepository;

        private readonly ILogger<UniverseBuilder> _logger;

        private readonly IMarketOpenCloseEventService _marketService;

        private readonly IUniverseSortComparer _universeSorter;

        public UniverseBuilder(
            IOrdersRepository auroraOrdersRepository,
            IOrdersToAllocatedOrdersProjector allocateOrdersProjector,
            IReddeerMarketRepository auroraMarketRepository,
            IMarketOpenCloseEventService marketService,
            IUniverseSortComparer universeSorter,
            ILogger<UniverseBuilder> logger)
        {
            this._auroraOrdersRepository =
                auroraOrdersRepository ?? throw new ArgumentNullException(nameof(auroraOrdersRepository));
            this._allocateOrdersProjector = allocateOrdersProjector
                                            ?? throw new ArgumentNullException(nameof(allocateOrdersProjector));
            this._auroraMarketRepository =
                auroraMarketRepository ?? throw new ArgumentNullException(nameof(auroraMarketRepository));
            this._marketService = marketService ?? throw new ArgumentNullException(nameof(marketService));
            this._universeSorter = universeSorter ?? throw new ArgumentNullException(nameof(universeSorter));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Crack the cosmic egg and unscramble your reality
        /// </summary>
        public async Task<IUniverse> Summon(ScheduledExecution execution, ISystemProcessOperationContext opCtx)
        {
            return await this.Summon(
                       execution,
                       opCtx,
                       true,
                       true,
                       execution.TimeSeriesInitiation,
                       execution.TimeSeriesTermination);
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
                this._logger.LogError(
                    $"had a null execution or rule parameters and therefore null data sets for {opCtx.Id} operation context");
                return new Universe(null);
            }

            this._logger.LogInformation("fetching aurora trade data");
            var projectedTrades = await this.TradeDataFetchAurora(execution, opCtx);
            this._logger.LogInformation("completed fetching aurora trade data");

            this._logger.LogInformation("fetching aurora trade allocation data");
            var projectedTradesAllocations = await this._allocateOrdersProjector.DecorateOrders(projectedTrades);
            this._logger.LogInformation("completed fetching aurora trade allocation data");

            this._logger.LogInformation("fetching intraday for equities");
            var intradayEquityBars = await this.MarketEquityIntraDayDataFetchAurora(execution, opCtx);
            this._logger.LogInformation("completed fetching intraday for equities");

            this._logger.LogInformation("fetching inter day for equities");
            var interDayEquityBars = await this.MarketEquityInterDayDataFetchAurora(execution, opCtx);
            this._logger.LogInformation("completed fetching inter day for equities");

            this._logger.LogInformation("fetching universe event data");
            var universe = await this.UniverseEvents(
                               execution,
                               projectedTradesAllocations,
                               intradayEquityBars,
                               interDayEquityBars,
                               includeGenesis,
                               includeEschaton,
                               realUniverseEpoch,
                               futureUniverseEpoch);

            this._logger.LogInformation("completed fetching universe event data");

            this._logger.LogInformation("returning a new universe");
            return new Universe(universe);
        }

        private List<IUniverseEvent> FilterOutTradesInFutureEpoch(
            List<IUniverseEvent> events,
            DateTimeOffset? futureUniverseEpoch)
        {
            if (events == null || !events.Any()) return events;

            if (futureUniverseEpoch == null) return events;

            var filteredEvents = events.Where(_ => !_.StateChange.IsOrderType() || _.EventTime <= futureUniverseEpoch)
                .ToList();

            return filteredEvents;
        }

        private async Task<IReadOnlyCollection<EquityInterDayTimeBarCollection>> MarketEquityInterDayDataFetchAurora(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx)
        {
            var equities = await this._auroraMarketRepository.GetEquityInterDay(
                               execution.TimeSeriesInitiation.Subtract(execution.LeadingTimespan ?? TimeSpan.Zero).Date,
                               execution.TimeSeriesTermination.Date,
                               opCtx);

            return equities ?? new List<EquityInterDayTimeBarCollection>();
        }

        private async Task<IReadOnlyCollection<EquityIntraDayTimeBarCollection>> MarketEquityIntraDayDataFetchAurora(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx)
        {
            var equities = await this._auroraMarketRepository.GetEquityIntraday(
                               execution.TimeSeriesInitiation.Subtract(execution.LeadingTimespan ?? TimeSpan.Zero).Date,
                               execution.TimeSeriesTermination.Date,
                               opCtx);

            return equities ?? new List<EquityIntraDayTimeBarCollection>();
        }

        private async Task<IReadOnlyCollection<Order>> TradeDataFetchAurora(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx)
        {
            var trades = await this._auroraOrdersRepository.Get(
                             execution.TimeSeriesInitiation.Date,
                             execution.TimeSeriesTermination.Date,
                             opCtx);

            return trades ?? new List<Order>();
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
            var tradeSubmittedEvents = trades.Where(tr => tr != null).Select(
                    tr => new UniverseEvent(UniverseStateEvent.OrderPlaced, tr.PlacedDate.GetValueOrDefault(), tr))
                .ToArray();

            var tradeStatusChangedOnEvents = trades.Where(tr => tr != null)
                .Where(tr => !(tr.OrderStatus() == OrderStatus.Booked && tr.PlacedDate == tr.MostRecentDateEvent()))
                .Select(tr => new UniverseEvent(UniverseStateEvent.Order, tr.MostRecentDateEvent(), tr)).ToArray();

            var tradeFilledEvents = trades.Where(tr => tr != null).Where(tr => tr.OrderStatus() == OrderStatus.Filled)
                .Where(tr => tr.FilledDate != null).Select(
                    tr => new UniverseEvent(UniverseStateEvent.OrderFilled, tr.FilledDate.Value, tr)).ToArray();

            var intradayEquityEvents = equityIntradayUpdates
                .Select(exch => new UniverseEvent(UniverseStateEvent.EquityIntradayTick, exch.Epoch, exch)).ToArray();

            var interDayEquityEvents = equityInterDayUpdates
                .Select(exch => new UniverseEvent(UniverseStateEvent.EquityInterDayTick, exch.Epoch, exch)).ToArray();

            var marketEvents = await this._marketService.AllOpenCloseEvents(
                                   execution.TimeSeriesInitiation.DateTime,
                                   execution.TimeSeriesTermination.DateTime);

            var intraUniversalHistoryEvents = new List<IUniverseEvent>();
            intraUniversalHistoryEvents.AddRange(tradeSubmittedEvents);
            intraUniversalHistoryEvents.AddRange(tradeStatusChangedOnEvents);
            intraUniversalHistoryEvents.AddRange(tradeFilledEvents);
            intraUniversalHistoryEvents.AddRange(intradayEquityEvents);
            intraUniversalHistoryEvents.AddRange(interDayEquityEvents);
            intraUniversalHistoryEvents.AddRange(marketEvents);
            intraUniversalHistoryEvents = this.FilterOutTradesInFutureEpoch(
                intraUniversalHistoryEvents,
                futureUniverseEpoch);
            var orderedIntraUniversalHistory =
                intraUniversalHistoryEvents.OrderBy(ihe => ihe, this._universeSorter).ToList();

            var universeEvents = new List<IUniverseEvent>();

            if (includeGenesis)
            {
                var genesis = new UniverseEvent(
                    UniverseStateEvent.Genesis,
                    execution.TimeSeriesInitiation.DateTime,
                    execution);
                var primordialEpoch = new UniverseEvent(
                    UniverseStateEvent.EpochPrimordialUniverse,
                    execution.TimeSeriesInitiation.DateTime,
                    execution);
                universeEvents.Add(genesis);
                universeEvents.Add(primordialEpoch);
            }

            if (realUniverseEpoch != null && realUniverseEpoch >= execution.TimeSeriesInitiation
                                          && realUniverseEpoch <= execution.TimeSeriesTermination)
            {
                var realUniverseEpochEvent = new UniverseEvent(
                    UniverseStateEvent.EpochRealUniverse,
                    realUniverseEpoch.GetValueOrDefault().DateTime,
                    execution);
                universeEvents.Add(realUniverseEpochEvent);
            }

            if (futureUniverseEpoch != null && futureUniverseEpoch >= execution.TimeSeriesInitiation
                                            && futureUniverseEpoch <= execution.TimeSeriesTermination)
            {
                var futureUniverseEpochEvent = new UniverseEvent(
                    UniverseStateEvent.EpochFutureUniverse,
                    futureUniverseEpoch.GetValueOrDefault().DateTime,
                    execution);
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

            universeEvents = universeEvents.OrderBy(ue => ue, this._universeSorter).ToList();

            return universeEvents;
        }
    }
}