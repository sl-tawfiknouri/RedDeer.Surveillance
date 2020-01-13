namespace Surveillance.Data.Universe.Lazy.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Rules.Interfaces;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Data.Universe.Lazy.Builder.Interfaces;
    using Surveillance.Data.Universe.MarketEvents.Interfaces;
    using Surveillance.Data.Universe.Refinitiv.Interfaces;
    using Surveillance.Data.Universe.Trades.Interfaces;
    using Surveillance.DataLayer.Aurora.Market.Interfaces;
    using Surveillance.DataLayer.Aurora.Orders.Interfaces;

    /// <summary>
    /// The data manifest builder.
    /// </summary>
    public class DataManifestBuilder : IDataManifestBuilder
    {
        /// <summary>
        /// The universe builder.
        /// </summary>
        private readonly IUniverseBuilder universeBuilder;

        /// <summary>
        /// The orders repository.
        /// </summary>
        private readonly IOrdersRepository ordersRepository;

        /// <summary>
        /// The allocate orders projector.
        /// </summary>
        private readonly IOrdersToAllocatedOrdersProjector allocateOrdersProjector;

        /// <summary>
        /// The market open close event service.
        /// </summary>
        private readonly IMarketOpenCloseEventService marketOpenCloseEventService;

        /// <summary>
        /// The market repository.
        /// </summary>
        private readonly IReddeerMarketRepository marketRepository;

        /// <summary>
        /// The time line continuum.
        /// </summary>
        private readonly ITimeLineContinuum timeLineContinuum;

        private readonly IRefinitivTickPriceHistoryApi refinitivTickPriceHistoryApi;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<IDataManifestBuilder> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataManifestBuilder"/> class.
        /// </summary>
        /// <param name="universeBuilder">
        /// The universe builder.
        /// </param>
        /// <param name="ordersRepository">
        /// The orders repository.
        /// </param>
        /// <param name="marketRepository">
        /// The market repository.
        /// </param>
        /// <param name="marketOpenCloseEventService">
        /// The market open close hours service.
        /// </param>
        /// <param name="timeLineContinuum">
        /// The time line continuum.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public DataManifestBuilder(
            IUniverseBuilder universeBuilder,
            IOrdersRepository ordersRepository,
            IOrdersToAllocatedOrdersProjector allocateOrdersProjector,
            IReddeerMarketRepository marketRepository,
            IMarketOpenCloseEventService marketOpenCloseEventService,
            ITimeLineContinuum timeLineContinuum,
            IRefinitivTickPriceHistoryApi refinitivTickPriceHistoryApi,
            ILogger<IDataManifestBuilder> logger)
        {
            this.universeBuilder = 
                universeBuilder ?? throw new ArgumentNullException(nameof(universeBuilder));
            this.ordersRepository = 
                ordersRepository ?? throw new ArgumentNullException(nameof(ordersRepository));
            this.allocateOrdersProjector =
                allocateOrdersProjector ?? throw new ArgumentNullException(nameof(allocateOrdersProjector));
            this.marketRepository = 
                marketRepository ?? throw new ArgumentNullException(nameof(marketRepository));
            this.marketOpenCloseEventService = 
                marketOpenCloseEventService ?? throw new ArgumentNullException(nameof(marketOpenCloseEventService));
            this.timeLineContinuum = timeLineContinuum ?? throw new ArgumentNullException(nameof(timeLineContinuum));
            this.refinitivTickPriceHistoryApi = refinitivTickPriceHistoryApi ?? throw new ArgumentNullException(nameof(refinitivTickPriceHistoryApi));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The build.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="ruleDataConstraints">
        /// The rule data constraints.
        /// </param>
        /// <param name="systemProcessOperationContext">
        /// The system process operation context.
        /// </param>
        /// <returns>
        /// The <see cref="IDataManifestInterpreter"/>.
        /// </returns>
        public async Task<IDataManifestInterpreter> Build(
            ScheduledExecution execution,
            IReadOnlyCollection<IRuleDataConstraint> ruleDataConstraints,
            ISystemProcessOperationContext systemProcessOperationContext)
        {
            var orders = 
                await this.ordersRepository.Get(
                     execution.AdjustedTimeSeriesInitiation.DateTime, 
                     execution.AdjustedTimeSeriesTermination.DateTime,
                     systemProcessOperationContext);

            if (orders == null || !orders.Any())
            {
                this.logger.LogInformation($"No orders were found in the schedule passed into the data manifest builder");
                var dataManifestInterpreter = this.EmptyManifestInterpreter(execution, systemProcessOperationContext);

                return dataManifestInterpreter;
            }

            var subConstraints =
                ruleDataConstraints
                    ?.SelectMany(_ => _.Constraints)
                    ?.Where(_ => _ != null && _.Source != DataSource.None)
                    ?.ToList();

            if (subConstraints == null || !subConstraints.Any())
            {
                this.logger.LogInformation($"No rule constraints were passed into the data manifest builder");
                var dataManifestInterpreter = this.EmptyManifestInterpreter(execution, systemProcessOperationContext);

                return dataManifestInterpreter;
            }

            var bmllTimeBar = new List<BmllTimeBarQuery>();
            var factsetTimeBar = new List<FactSetTimeBarQuery>();
            var refinitivIntraDayTimeBar = new List<RefinitivIntraDayTimeBarQuery>();
            var refinitivInterDayTimeBar = new List<RefinitivInterDayTimeBarQuery>();
            var unfilteredOrders = this.BuildUnfilteredOrdersQueries(execution);

            foreach (var sub in subConstraints)
            {
                // updates relevant list by reference
                this.MapSubConstraintToQuery(sub, orders, bmllTimeBar, factsetTimeBar, refinitivIntraDayTimeBar, refinitivInterDayTimeBar);
            }

            bmllTimeBar = this.timeLineContinuum.Merge(bmllTimeBar.Distinct().ToList()).ToList();
            factsetTimeBar = this.timeLineContinuum.Merge(factsetTimeBar.Distinct().ToList()).ToList();
            refinitivIntraDayTimeBar = this.timeLineContinuum.Merge(refinitivIntraDayTimeBar.Distinct().ToList()).ToList();
            refinitivInterDayTimeBar = this.timeLineContinuum.Merge(refinitivInterDayTimeBar.Distinct().ToList()).ToList();
            unfilteredOrders = this.timeLineContinuum.Merge(unfilteredOrders.Distinct().ToList()).ToList();

            var manifest = this.BuildManifest(
                execution,
                unfilteredOrders,
                bmllTimeBar,
                factsetTimeBar,
                refinitivIntraDayTimeBar,
                refinitivInterDayTimeBar);

            var interpreter =
                new DataManifestInterpreter(
                    manifest,
                    this.universeBuilder,
                    this.ordersRepository,
                    this.allocateOrdersProjector,
                    systemProcessOperationContext,
                    this.marketOpenCloseEventService,
                    this.marketRepository,
                    this.refinitivTickPriceHistoryApi);

            return interpreter;
        }

        /// <summary>
        /// The empty manifest interpreter.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="systemProcessOperationContext">
        /// The system process operation context.
        /// </param>
        /// <returns>
        /// The <see cref="IDataManifestInterpreter"/>.
        /// </returns>
        private IDataManifestInterpreter EmptyManifestInterpreter(
            ScheduledExecution execution,
            ISystemProcessOperationContext systemProcessOperationContext)
        {
            var dataManifest =
                new DataManifest(
                    execution,
                    new Stack<UnfilteredOrdersQuery>(),
                    new Stack<BmllTimeBarQuery>(),
                    new Stack<FactSetTimeBarQuery>(),
                    new Stack<RefinitivIntraDayTimeBarQuery>(),
                    new Stack<RefinitivInterDayTimeBarQuery>());

            return new DataManifestInterpreter(
                dataManifest,
                this.universeBuilder,
                this.ordersRepository,
                this.allocateOrdersProjector,
                systemProcessOperationContext,
                this.marketOpenCloseEventService,
                this.marketRepository,
                this.refinitivTickPriceHistoryApi);
        }

        /// <summary>
        /// The map sub constraint to query.
        /// </summary>
        /// <param name="sub">
        /// The sub.
        /// </param>
        /// <param name="orders">
        /// The orders.
        /// </param>
        /// <param name="bmllTimeBar">
        /// The time bar.
        /// </param>
        /// <param name="factsetTimeBar">
        /// The fact set time bar.
        /// </param>
        /// <param name="refinitivIntraDayTimeBar">
        /// The refinitiv intra day set time bar.
        /// </param>
        /// <param name="refinitivInterDayTimeBar">
        /// The refinitiv inter day set time bar.
        /// </param>
        private void MapSubConstraintToQuery(
            IRuleDataSubConstraint sub,
            IReadOnlyCollection<Order> orders,
            List<BmllTimeBarQuery> bmllTimeBar,
            List<FactSetTimeBarQuery> factsetTimeBar,
            List<RefinitivIntraDayTimeBarQuery> refinitivIntraDayTimeBar,
            List<RefinitivInterDayTimeBarQuery> refinitivInterDayTimeBar)
        {
            var filteredOrders = orders.Where(sub.Predicate).ToList();

            switch (sub.Source)
            {
                case DataSource.Any:
                    this.logger.LogError("Any data source called in data manifest builder by an engine component - currently unsupported");
                    break;
                case DataSource.AnyInterday:
                    var interdayQueries = this.TimeBarAdd(
                        filteredOrders,
                        sub,
                        (a, b, c) => new FactSetTimeBarQuery(a, b, c));
                    factsetTimeBar.AddRange(interdayQueries);
                    break;
                case DataSource.AnyIntraday:
                    var intradayQueries = this.TimeBarAdd(
                        filteredOrders,
                        sub,
                        (a, b, c) => new BmllTimeBarQuery(a, b, c));
                    bmllTimeBar.AddRange(intradayQueries);
                    break;
                case DataSource.Bmll:
                    var bmllTimeBarQueries = this.TimeBarAdd(
                        filteredOrders,
                        sub,
                        (a, b, c) => new BmllTimeBarQuery(a, b, c));
                    bmllTimeBar.AddRange(bmllTimeBarQueries);
                    break;
                case DataSource.Factset:
                    var factSetTimeBarQueries = this.TimeBarAdd(
                        filteredOrders,
                        sub,
                        (a, b, c) => new FactSetTimeBarQuery(a, b, c));
                    factsetTimeBar.AddRange(factSetTimeBarQueries);
                    break;
                case DataSource.Markit:
                    this.logger.LogError("Markit data requests are being issued without support in the data manifest builder");
                    break;
                case DataSource.None:
                    break;
                case DataSource.RefinitivIntraday:
                    var refinitivIntraDayTimeBarQueries = this.TimeBarAdd(
                        filteredOrders,
                        sub,
                        (a, b, c) => new RefinitivIntraDayTimeBarQuery(a, b, c));
                    refinitivIntraDayTimeBar.AddRange(refinitivIntraDayTimeBarQueries);
                    break;
                case DataSource.RefinitivInterday:
                    var refinitivInterDayTimeBarQueries = this.TimeBarAdd(
                        filteredOrders,
                        sub,
                        (a, b, c) => new RefinitivInterDayTimeBarQuery(a, b, c));
                    refinitivInterDayTimeBar.AddRange(refinitivInterDayTimeBarQueries);
                    break;
                case DataSource.NoPrices:
                    break;
                default:
                    this.logger.LogError($"Argument out of range for data source {sub.Source}");
                    break;
            }
        }

        /// <summary>
        /// The build manifest.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="orders">
        /// The orders.
        /// </param>
        /// <param name="bmllQueries">
        /// The queries.
        /// </param>
        /// <param name="factSetQueries">
        /// The fact set queries.
        /// </param>
        /// <param name="refinitivIntraDayQueries">
        /// The refinitiv intra day queries.
        /// </param>
        /// <param name="refinitivInterDayQueries">
        /// The refinitiv inter day queries.
        /// </param>
        /// <returns>
        /// The <see cref="IDataManifest"/>.
        /// </returns>
        private IDataManifest BuildManifest(
            ScheduledExecution execution,
            IList<UnfilteredOrdersQuery> orders,
            IList<BmllTimeBarQuery> bmllQueries,
            IList<FactSetTimeBarQuery> factSetQueries,
            IList<RefinitivIntraDayTimeBarQuery> refinitivIntraDayQueries,
            IList<RefinitivInterDayTimeBarQuery> refinitivInterDayQueries)
        {
            var stackedOrders = new Stack<UnfilteredOrdersQuery>();
            orders = orders?.OrderByDescending(_ => _.StartUtc)?.ToList() ?? new List<UnfilteredOrdersQuery>();
            foreach (var order in orders)
            {
                stackedOrders.Push(order);
            }

            var stackedBmllQueries = new Stack<BmllTimeBarQuery>();
            bmllQueries = bmllQueries?.OrderByDescending(_ => _.StartUtc)?.ToList() ?? new List<BmllTimeBarQuery>();
            foreach (var bmllQuery in bmllQueries)
            {
                stackedBmllQueries.Push(bmllQuery);   
            }

            var stackedFactSetQueries = new Stack<FactSetTimeBarQuery>();
            factSetQueries = factSetQueries?.OrderByDescending(_ => _.StartUtc)?.ToList() ?? new List<FactSetTimeBarQuery>();
            foreach (var factSetQuery in factSetQueries)
            {
                stackedFactSetQueries.Push(factSetQuery);
            }

            var stackedRefinitivIntraDayQueries = new Stack<RefinitivIntraDayTimeBarQuery>();
            refinitivIntraDayQueries = refinitivIntraDayQueries?.OrderByDescending(_ => _.StartUtc)?.ToList() ?? new List<RefinitivIntraDayTimeBarQuery>();
            foreach (var refinitivQuery in refinitivIntraDayQueries)
            {
                stackedRefinitivIntraDayQueries.Push(refinitivQuery);
            }

            var stackedRefinitivInterDayQueries = new Stack<RefinitivInterDayTimeBarQuery>();
            refinitivInterDayQueries = refinitivInterDayQueries?.OrderByDescending(_ => _.StartUtc)?.ToList() ?? new List<RefinitivInterDayTimeBarQuery>();
            foreach (var refinitivQuery in refinitivInterDayQueries)
            {
                stackedRefinitivInterDayQueries.Push(refinitivQuery);
            }

            return new DataManifest(
                execution,
                stackedOrders,
                stackedBmllQueries,
                stackedFactSetQueries,
                stackedRefinitivIntraDayQueries,
                stackedRefinitivInterDayQueries);
        }

        /// <summary>
        /// The time bar add.
        /// </summary>
        /// <param name="filteredOrders">
        /// The filtered orders.
        /// </param>
        /// <param name="subConstraint">
        /// The sub constraint.
        /// </param>
        /// <param name="func">
        /// The function.
        /// </param>
        /// <typeparam name="T">
        /// Query type
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        private IList<T> TimeBarAdd<T>(
            IList<Order> filteredOrders,
            IRuleDataSubConstraint subConstraint,
            Func<DateTime, DateTime, InstrumentIdentifiers, T> func)
        {
            var timeBars = new List<T>();

            if (filteredOrders == null || !filteredOrders.Any())
            {
                return timeBars;
            }

            foreach (var order in filteredOrders)
            {
                if (order?.PlacedDate == null)
                {
                    continue;
                }

                var query = func(
                    order.PlacedDate.Value - subConstraint.BackwardOffset,
                    order.MostRecentDateEvent() + subConstraint.ForwardOffset,
                    order.Instrument.Identifiers);

                timeBars.Add(query);
            }

            return timeBars;
        }
        
        /// <summary>
        /// The build unfiltered orders queries.
        /// This could be extended to be where must pass one filter predicate
        /// And then just become a list of primary keys which is chunked through
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <returns>
        /// The <see cref="UnfilteredOrdersQuery"/>.
        /// </returns>
        private IList<UnfilteredOrdersQuery> BuildUnfilteredOrdersQueries(ScheduledExecution execution)
        {
            var unfilteredOrders = new List<UnfilteredOrdersQuery>();

            var unfilteredQuery =
                new UnfilteredOrdersQuery(
                    execution.AdjustedTimeSeriesInitiation.DateTime,
                    execution.AdjustedTimeSeriesTermination.DateTime);

            unfilteredOrders.Add(unfilteredQuery);

            return unfilteredOrders;
        }
    }
}