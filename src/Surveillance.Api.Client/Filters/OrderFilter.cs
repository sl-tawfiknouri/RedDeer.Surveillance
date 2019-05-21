using System;
using System.Collections.Generic;
using System.Text;
using RedDeer.Surveillance.Api.Client.Enums;
using RedDeer.Surveillance.Api.Client.Infrastructure;

namespace RedDeer.Surveillance.Api.Client.Filters
{
    public class OrderFilter<T> : Filter<OrderFilter<T>, T> where T : Parent
    {
        public OrderFilter(T node) : base(node) { }

        public OrderFilter<T> ArgumentIds(List<int> ids) => AddArgument("ids", ids);
        public OrderFilter<T> ArgumentTraderIds(List<string> traderIds) => AddArgument("traderIds", traderIds);
        public OrderFilter<T> ArgumentReddeerIds(List<string> reddeerIds) => AddArgument("reddeerIds", reddeerIds);
        public OrderFilter<T> ArgumentDirections(List<OrderDirection> directions) => AddArgument("directions", directions);
        public OrderFilter<T> ArgumentTypes(List<OrderType> types) => AddArgument("types", types);
        public OrderFilter<T> ArgumentPlacedDateFrom(DateTime dateTime) => AddArgument("placedDateFrom", dateTime);
        public OrderFilter<T> ArgumentPlacedDateTo(DateTime dateTime) => AddArgument("placedDateTo", dateTime);
        public OrderFilter<T> ArgumentTake(int count) => AddArgument("take", count);
        public OrderFilter<T> ArgumentTzName(string tzName) => AddArgument("tzName", tzName);
    }
}
