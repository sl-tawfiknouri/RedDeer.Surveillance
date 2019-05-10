using Surveillance.Api.Client.Infrastructure;
using Surveillance.Api.Client.Nodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.Client.Queries
{
    public abstract class OrderArgumentsQuery<Z, T> : Query<OrderArgumentsQuery<Z, T>, T> where Z : class
    {
        public Z ArgumentIds(List<int> ids) => AddArgument("ids", ids) as Z;
        public Z ArgumentTraderIds(List<string> traderIds) => AddArgument("traderIds", traderIds) as Z;
        public Z ArgumentReddeerIds(List<string> reddeerIds) => AddArgument("reddeerIds", reddeerIds) as Z;
        public Z ArgumentPlacedDateFrom(DateTime dateTime) => AddArgument("placedDateFrom", dateTime) as Z;
        public Z ArgumentPlacedDateTo(DateTime dateTime) => AddArgument("placedDateTo", dateTime) as Z;
        public Z ArgumentTake(int count) => AddArgument("take", count) as Z;
    }
}
