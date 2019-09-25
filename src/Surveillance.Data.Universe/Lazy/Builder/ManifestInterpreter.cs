namespace Surveillance.Data.Universe.Lazy.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Domain.Core.Markets.Collections;
    using Domain.Core.Trading.Orders;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Data.Universe.Lazy.Builder.Interfaces;
    using Surveillance.DataLayer.Aurora.Orders.Interfaces;

    /// <summary>
    /// The manifest interpreter.
    /// </summary>
    public class ManifestInterpreter
    {
        /// <summary>
        /// The universe builder.
        /// </summary>
        private readonly IUniverseBuilder universeBuilder;

        /// <summary>
        /// The aurora orders repository.
        /// </summary>
        private readonly IOrdersRepository ordersRepository;

        /// <summary>
        /// The system process operation context.
        /// </summary>
        private readonly ISystemProcessOperationContext systemProcessOperationContext;

        /// <summary>
        /// The has set current time universal central time.
        /// </summary>
        private bool hasSetCurrentTimeUtc = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestInterpreter"/> class.
        /// </summary>
        /// <param name="dataManifest">
        /// The data manifest.
        /// </param>
        /// <param name="universeBuilder">
        /// The universe builder.
        /// </param>
        /// <param name="ordersRepository">
        /// The orders repository.
        /// </param>
        /// <param name="systemProcessOperationContext">
        /// The system operation context.
        /// </param>
        public ManifestInterpreter(
            IDataManifest dataManifest,
            IUniverseBuilder universeBuilder,
            IOrdersRepository ordersRepository,
            ISystemProcessOperationContext systemProcessOperationContext)
        {
            this.DataManifest = dataManifest ?? throw new ArgumentNullException(nameof(dataManifest));
            this.universeBuilder = universeBuilder ?? throw new ArgumentNullException(nameof(universeBuilder));
            this.ordersRepository = ordersRepository ?? throw new ArgumentNullException(nameof(ordersRepository));
            this.systemProcessOperationContext = systemProcessOperationContext ?? throw new ArgumentNullException(nameof(systemProcessOperationContext));
        }

        /// <summary>
        /// Gets the data manifest.
        /// </summary>
        public IDataManifest DataManifest { get; }

        /// <summary>
        /// Gets or sets the current time universal central time.
        /// </summary>
        private DateTime CurrentTimeUtc { get; set; }

        /// <summary>
        /// The play forward.
        /// </summary>
        /// <param name="span">
        /// The span.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverse"/>.
        /// </returns>
        public async Task<IUniverse> PlayForward(TimeSpan span)
        {
            if (!this.hasSetCurrentTimeUtc)
            {
                this.CurrentTimeUtc = this.DataManifest.Execution.AdjustedTimeSeriesInitiation.DateTime.ToUniversalTime();
                this.hasSetCurrentTimeUtc = true;
            }

            var orders = await this.ScanOrders(span);
            var equityIntradayTimeBars = this.ScanIntraDayTimeBars(span);
            var equityInterdayTimeBars = this.ScanInterDayTimeBars(span);
            var marketOpenClose = this.ScanMarketOpenClose(span);

            // ReSharper disable once PossibleInvalidOperationException
            var includeGenesis = this.CurrentTimeUtc == this.DataManifest.Execution.AdjustedTimeSeriesInitiation.DateTime.ToUniversalTime();
            var includeEschaton = this.CurrentTimeUtc.Add(span) >= this.DataManifest.Execution.AdjustedTimeSeriesTermination;

            var universe = this.universeBuilder.PackageUniverse(
                this.DataManifest.Execution,
                orders,
                equityIntradayTimeBars,
                equityInterdayTimeBars,
                marketOpenClose,
                includeGenesis,
                includeEschaton,
                this.DataManifest.Execution.TimeSeriesInitiation,
                this.DataManifest.Execution.TimeSeriesTermination);

            this.CurrentTimeUtc = this.CurrentTimeUtc.Add(span);

            return universe;
        }

        /// <summary>
        /// The scan orders.
        /// </summary>
        /// <param name="span">
        /// The span.
        /// </param>
        /// <returns>
        /// The <see cref="Order"/>.
        /// </returns>
        private async Task<IReadOnlyCollection<Order>> ScanOrders(TimeSpan span)
        {
            if (this.DataManifest.UnfilteredOrders == null
                || !this.DataManifest.UnfilteredOrders.Any())
            {
                return new Order[0];
            }

            var scanStack = new Stack<UnfilteredOrdersQuery>();
            var scanEnd = this.CurrentTimeUtc.Add(span);

            while (this.DataManifest.UnfilteredOrders.Any()
                   && this.DataManifest.UnfilteredOrders.Peek().StartUtc <= scanEnd)
            {
                scanStack.Push(this.DataManifest.UnfilteredOrders.Pop());  
            }
            
            var queriedStack = new Stack<UnfilteredOrdersQuery>();
            var queriedOrders = new List<Order>();

            while (scanStack.Any())
            {
                var query = scanStack.Pop();
                var queryEnd = query.EndUtc < scanEnd ? query.EndUtc : scanEnd;
                var orders = await this.ordersRepository.Get(query.StartUtc, queryEnd, this.systemProcessOperationContext);
                queriedOrders.AddRange(orders);

                if (query.EndUtc >= scanEnd)
                {
                    var cpy = new UnfilteredOrdersQuery(scanEnd, query.EndUtc);
                    queriedStack.Push(cpy);
                }
            }

            while (queriedStack.Any())
            {
                this.DataManifest.UnfilteredOrders.Push(queriedStack.Pop());
            }

            return queriedOrders;
        }

        /// <summary>
        /// The scan intra day time bars.
        /// </summary>
        /// <returns>
        /// The <see cref="EquityIntraDayTimeBarCollection"/>.
        /// </returns>
        private IReadOnlyCollection<EquityIntraDayTimeBarCollection> ScanIntraDayTimeBars(TimeSpan span)
        {
            return new EquityIntraDayTimeBarCollection[0];
        }

        /// <summary>
        /// The scan inter day time bars.
        /// </summary>
        /// <returns>
        /// The <see cref="EquityInterDayTimeBarCollection"/>.
        /// </returns>
        private IReadOnlyCollection<EquityInterDayTimeBarCollection> ScanInterDayTimeBars(TimeSpan span)
        {
            return new EquityInterDayTimeBarCollection[0];
        }

        /// <summary>
        /// The scan market open close.
        /// </summary>
        /// <returns>
        /// The <see cref="IUniverseEvent"/>.
        /// </returns>
        private IReadOnlyCollection<IUniverseEvent> ScanMarketOpenClose(TimeSpan span)
        {
            return new IUniverseEvent[0];
        }
    }
}
