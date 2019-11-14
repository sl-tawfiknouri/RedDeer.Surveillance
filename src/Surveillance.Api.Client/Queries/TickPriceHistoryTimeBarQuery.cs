using RedDeer.Surveillance.Api.Client.Dtos;
using RedDeer.Surveillance.Api.Client.Filters;
using RedDeer.Surveillance.Api.Client.Infrastructure;
using RedDeer.Surveillance.Api.Client.Nodes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RedDeer.Surveillance.Api.Client.Queries
{
    public class TickPriceHistoryTimeBarQuery 
        : Query<List<TickPriceHistoryTimeBarDto>>
    {
        public TickPriceHistoryTimeBarQuery()
        {
            this.Filter = new TickPriceHistoryTimeBarFilter<TickPriceHistoryTimeBarNode>(new TickPriceHistoryTimeBarNode(this));
        }

        public TickPriceHistoryTimeBarFilter<TickPriceHistoryTimeBarNode> Filter { get; }

        internal override async Task<List<TickPriceHistoryTimeBarDto>> HandleAsync(IRequest request, CancellationToken ct)
        {
            return await this.BuildAndPost<List<TickPriceHistoryTimeBarDto>>("tickPriceHistoryTimeBars", this.Filter, request, ct);
        }
    }
}
