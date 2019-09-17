namespace RedDeer.Surveillance.Api.Client.Queries
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using RedDeer.Surveillance.Api.Client.Dtos;
    using RedDeer.Surveillance.Api.Client.Filters;
    using RedDeer.Surveillance.Api.Client.Infrastructure;
    using RedDeer.Surveillance.Api.Client.Nodes;

    /// <summary>
    /// The broker query.
    /// </summary>
    public class BrokerQuery : Query<List<BrokerDto>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrokerQuery"/> class.
        /// </summary>
        public BrokerQuery()
        {
            this.Filter = new BrokerFilter<BrokerNode>(new BrokerNode(this));
        }

        /// <summary>
        /// Gets the filter.
        /// </summary>
        public BrokerFilter<BrokerNode> Filter { get; }

        /// <summary>
        /// The handle async.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        internal override async Task<List<BrokerDto>> HandleAsync(IRequest request, CancellationToken cancellationToken)
        {
            return await this.BuildAndPost<List<BrokerDto>>("brokers", this.Filter, request, cancellationToken);
        }
    }
}