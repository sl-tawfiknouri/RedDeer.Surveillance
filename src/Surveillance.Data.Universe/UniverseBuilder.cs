namespace Surveillance.Data.Universe
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
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Data.Universe.MarketEvents.Interfaces;
    using Surveillance.Data.Universe.Trades.Interfaces;
    using Surveillance.DataLayer.Aurora.Market.Interfaces;
    using Surveillance.DataLayer.Aurora.Orders.Interfaces;

    /// <summary>
    ///     Generates a universe from the existing data set
    /// </summary>
    public class UniverseBuilder : IUniverseBuilder
    {
        /// <summary>
        /// The allocate orders projector.
        /// </summary>
        private readonly IOrdersToAllocatedOrdersProjector allocateOrdersProjector;

        /// <summary>
        /// The market repository.
        /// </summary>
        private readonly IReddeerMarketRepository marketRepository;

        /// <summary>
        /// The orders repository.
        /// </summary>
        private readonly IOrdersRepository ordersRepository;

        /// <summary>
        /// The market service.
        /// </summary>
        private readonly IMarketOpenCloseEventService marketService;

        /// <summary>
        /// The universe sorter.
        /// </summary>
        private readonly IUniverseSortComparer universeSorter;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<UniverseBuilder> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UniverseBuilder"/> class.
        /// </summary>
        /// <param name="ordersRepository">
        /// The orders repository.
        /// </param>
        /// <param name="allocateOrdersProjector">
        /// The allocate orders projector.
        /// </param>
        /// <param name="marketRepository">
        /// The market repository.
        /// </param>
        /// <param name="marketService">
        /// The market service.
        /// </param>
        /// <param name="universeSorter">
        /// The universe sorter.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public UniverseBuilder(
            IOrdersRepository ordersRepository,
            IOrdersToAllocatedOrdersProjector allocateOrdersProjector,
            IReddeerMarketRepository marketRepository,
            IMarketOpenCloseEventService marketService,
            IUniverseSortComparer universeSorter,
            ILogger<UniverseBuilder> logger)
        {
            this.ordersRepository =
                ordersRepository ?? throw new ArgumentNullException(nameof(ordersRepository));
            this.allocateOrdersProjector = allocateOrdersProjector
                                            ?? throw new ArgumentNullException(nameof(allocateOrdersProjector));
            this.marketRepository =
                marketRepository ?? throw new ArgumentNullException(nameof(marketRepository));
            this.marketService = marketService ?? throw new ArgumentNullException(nameof(marketService));
            this.universeSorter = universeSorter ?? throw new ArgumentNullException(nameof(universeSorter));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crack the cosmic egg and unscramble your reality
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="operationContext">
        /// The operation Context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<IUniverse> Summon(ScheduledExecution execution, ISystemProcessOperationContext operationContext)
        {
            return await this.Summon(
                       execution,
                       operationContext,
                       true,
                       true,
                       execution.TimeSeriesInitiation,
                       execution.TimeSeriesTermination);
        }

        /// <summary>
        /// The summon.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        /// <param name="includeGenesis">
        /// The include genesis.
        /// </param>
        /// <param name="includeEschaton">
        /// The include eschaton.
        /// </param>
        /// <param name="realUniverseEpoch">
        /// The real universe epoch.
        /// </param>
        /// <param name="futureUniverseEpoch">
        /// The future universe epoch.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<IUniverse> Summon(
            ScheduledExecution execution,
            ISystemProcessOperationContext operationContext,
            bool includeGenesis,
            bool includeEschaton,
            DateTimeOffset? realUniverseEpoch,
            DateTimeOffset? futureUniverseEpoch)
        {
            if (execution == null)
            {
                this.logger.LogError(
                    $"had a null execution or rule parameters and therefore null data sets for {operationContext.Id} operation context");
                return new Universe(null);
            }

            this.logger.LogInformation("fetching aurora trade data");
            var projectedTrades = await this.TradeDataFetchAurora(execution, operationContext);
            this.logger.LogInformation("completed fetching aurora trade data");

            this.logger.LogInformation("fetching aurora trade allocation data");
            var projectedTradesAllocations = await this.allocateOrdersProjector.DecorateOrders(projectedTrades);
            this.logger.LogInformation("completed fetching aurora trade allocation data");

            this.logger.LogInformation("fetching intraday for equities");
            var intradayEquityBars = await this.MarketEquityIntraDayDataFetchAurora(execution, operationContext);
            this.logger.LogInformation("completed fetching intraday for equities");

            this.logger.LogInformation("fetching inter day for equities");
            var interDayEquityBars = await this.MarketEquityInterDayDataFetchAurora(execution, operationContext);
            this.logger.LogInformation("completed fetching inter day for equities");

            var marketOpenClose = await this.marketService.AllOpenCloseEvents(
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime);

            this.logger.LogInformation("fetching universe event data");
            var universe = this.PackageUniverse(
                               execution,
                               projectedTradesAllocations,
                               intradayEquityBars,
                               interDayEquityBars,
                               marketOpenClose,
                               includeGenesis,
                               includeEschaton,
                               realUniverseEpoch,
                               futureUniverseEpoch);

            this.logger.LogInformation("completed fetching universe event data");

            this.logger.LogInformation("returning a new universe");

            return universe;
        }

        /// <summary>
        /// The universe events.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="trades">
        /// The trades.
        /// </param>
        /// <param name="equityIntradayUpdates">
        /// The equity intraday updates.
        /// </param>
        /// <param name="equityInterDayUpdates">
        /// The equity inter day updates.
        /// </param>
        /// <param name="marketEvents">
        /// The market events
        /// </param>
        /// <param name="includeGenesis">
        /// The include genesis.
        /// </param>
        /// <param name="includeEschaton">
        /// The include eschaton.
        /// </param>
        /// <param name="realUniverseEpoch">
        /// The real universe epoch.
        /// </param>
        /// <param name="futureUniverseEpoch">
        /// The future universe epoch.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public IUniverse PackageUniverse(
            ScheduledExecution execution,
            IReadOnlyCollection<Order> trades,
            IReadOnlyCollection<EquityIntraDayTimeBarCollection> equityIntradayUpdates,
            IReadOnlyCollection<EquityInterDayTimeBarCollection> equityInterDayUpdates,
            IReadOnlyCollection<IUniverseEvent> marketEvents,
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
                intraUniversalHistoryEvents.OrderBy(ihe => ihe, this.universeSorter).ToList();

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

            universeEvents = universeEvents.OrderBy(ue => ue, this.universeSorter).ToList();

            return new Universe(universeEvents);
        }

        /// <summary>
        /// The filter out trades in future epoch.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="futureUniverseEpoch">
        /// The future universe epoch.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseEvent"/>.
        /// </returns>
        private List<IUniverseEvent> FilterOutTradesInFutureEpoch(
            List<IUniverseEvent> events,
            DateTimeOffset? futureUniverseEpoch)
        {
            if (events == null || !events.Any())
            {
                return events;
            }

            if (futureUniverseEpoch == null)
            {
                return events;
            }

            var filteredEvents = 
                events.Where(_ => !_.StateChange.IsOrderType() || _.EventTime <= futureUniverseEpoch)
                .ToList();

            return filteredEvents;
        }

        /// <summary>
        /// The market equity inter day data fetch aurora.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<IReadOnlyCollection<EquityInterDayTimeBarCollection>> MarketEquityInterDayDataFetchAurora(
            ScheduledExecution execution,
            ISystemProcessOperationContext operationContext)
        {
            var startDate = execution.TimeSeriesInitiation.Subtract(execution.LeadingTimespan ?? TimeSpan.Zero).Date;

            var equities = await this.marketRepository.GetEquityInterDay(
                               startDate,
                               execution.TimeSeriesTermination.Date,
                               operationContext);

            return equities ?? new List<EquityInterDayTimeBarCollection>();
        }

        /// <summary>
        /// The market equity intra day data fetch aurora.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<IReadOnlyCollection<EquityIntraDayTimeBarCollection>> MarketEquityIntraDayDataFetchAurora(
            ScheduledExecution execution,
            ISystemProcessOperationContext operationContext)
        {
            var startDate = execution.TimeSeriesInitiation.Subtract(execution.LeadingTimespan ?? TimeSpan.Zero).Date;

            var equities = 
                await this.marketRepository.GetEquityIntraday(
                    startDate,
                    execution.TimeSeriesTermination.Date,
                    operationContext);

            return equities ?? new List<EquityIntraDayTimeBarCollection>();
        }

        /// <summary>
        /// The trade data fetch aurora.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<IReadOnlyCollection<Order>> TradeDataFetchAurora(
            ScheduledExecution execution,
            ISystemProcessOperationContext operationContext)
        {
            var trades = await this.ordersRepository.Get(
                             execution.TimeSeriesInitiation.Date,
                             execution.TimeSeriesTermination.Date,
                             operationContext);

            return trades ?? new List<Order>();
        }
    }
}