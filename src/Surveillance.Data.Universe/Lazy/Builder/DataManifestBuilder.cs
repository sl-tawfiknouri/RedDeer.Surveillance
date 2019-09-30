namespace Surveillance.Data.Universe.Lazy.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Domain.Surveillance.Rules.Interfaces;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Lazy.Builder.Interfaces;
    using Surveillance.DataLayer.Aurora.Orders.Interfaces;

    /// <summary>
    /// The data manifest builder.
    /// </summary>
    public class DataManifestBuilder : IDataManifestBuilder
    {
        /// <summary>
        /// The orders repository.
        /// </summary>
        private readonly IOrdersRepository ordersRepository;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<IDataManifestBuilder> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataManifestBuilder"/> class.
        /// </summary>
        /// <param name="ordersRepository">
        /// The orders repository.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public DataManifestBuilder(
            IOrdersRepository ordersRepository,
            ILogger<IDataManifestBuilder> logger)
        {
            this.ordersRepository = ordersRepository ?? throw new ArgumentNullException(nameof(ordersRepository));
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
                // TODO
                // the empty data manifest
                this.logger.LogInformation($"No orders were found in the schedule passed into the data manifest builder");

                return null;
            }

            var subConstraints =
                ruleDataConstraints
                    ?.SelectMany(_ => _.Constraints)
                    ?.Where(_ => _ != null)
                    ?.ToList();

            if (subConstraints == null || !subConstraints.Any())
            {
                // TODO
                // the empty data manifest
                this.logger.LogInformation($"No rule constraints were passed into the data manifest builder");

                return null;
            }

            var bmllTimeBar = new Stack<BmllTimeBarQuery>();
            var factsetTimeBar = new Stack<FactSetTimeBarQuery>();
            var refinitiveTimeBar = new Stack<RefinitiveTimeBarQuery>();
            var unfilteredOrders = this.BuildUnfilteredOrdersQueries(execution);

            foreach (var sub in subConstraints)
            {
                var filteredOrders = orders.Where(sub.Predicate).ToList();

                switch (sub.Source)
                {
                    case DataSource.Any:

                    case DataSource.AnyInterday:

                    case DataSource.AnyIntraday:

                    case DataSource.Bmll:

                    case DataSource.Factset:

                    case DataSource.Markit:

                    case DataSource.None:

                    case DataSource.Refinitive:

                    default: break;
                }
            }

            return null;
        }

        /// <summary>
        /// The build unfiltered orders queries.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <returns>
        /// The <see cref="UnfilteredOrdersQuery"/>.
        /// </returns>
        private Stack<UnfilteredOrdersQuery> BuildUnfilteredOrdersQueries(ScheduledExecution execution)
        {
            var unfilteredOrders = new Stack<UnfilteredOrdersQuery>();

            var unfilteredQuery =
                new UnfilteredOrdersQuery(
                    execution.AdjustedTimeSeriesInitiation.DateTime,
                    execution.AdjustedTimeSeriesTermination.DateTime);

            unfilteredOrders.Push(unfilteredQuery);

            return unfilteredOrders;
        }
    }
}