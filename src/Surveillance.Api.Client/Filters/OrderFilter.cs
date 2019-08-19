namespace RedDeer.Surveillance.Api.Client.Filters
{
    using System;
    using System.Collections.Generic;

    using RedDeer.Surveillance.Api.Client.Enums;
    using RedDeer.Surveillance.Api.Client.Infrastructure;

    public class OrderFilter<T> : Filter<OrderFilter<T>, T>
        where T : Parent
    {
        public OrderFilter(T node)
            : base(node)
        {
        }

        public OrderFilter<T> ArgumentDirections(List<OrderDirection> directions)
        {
            return this.AddArgument("directions", directions);
        }

        public OrderFilter<T> ArgumentExcludeTraderIds(HashSet<string> excludeTraderIds)
        {
            return this.AddArgument("excludeTraderIds", excludeTraderIds);
        }

        public OrderFilter<T> ArgumentIds(List<int> ids)
        {
            return this.AddArgument("ids", ids);
        }

        public OrderFilter<T> ArgumentPlacedDateFrom(DateTime dateTime)
        {
            return this.AddArgument("placedDateFrom", dateTime);
        }

        public OrderFilter<T> ArgumentPlacedDateTo(DateTime dateTime)
        {
            return this.AddArgument("placedDateTo", dateTime);
        }

        public OrderFilter<T> ArgumentReddeerIds(List<string> reddeerIds)
        {
            return this.AddArgument("reddeerIds", reddeerIds);
        }

        public OrderFilter<T> ArgumentStatuses(List<OrderStatus> statuses)
        {
            return this.AddArgument("statuses", statuses);
        }

        public OrderFilter<T> ArgumentTake(int count)
        {
            return this.AddArgument("take", count);
        }

        public OrderFilter<T> ArgumentTraderIds(HashSet<string> traderIds)
        {
            return this.AddArgument("traderIds", traderIds);
        }

        public OrderFilter<T> ArgumentTypes(List<OrderType> types)
        {
            return this.AddArgument("types", types);
        }

        public OrderFilter<T> ArgumentTzName(string tzName)
        {
            return this.AddArgument("tzName", tzName);
        }
    }
}