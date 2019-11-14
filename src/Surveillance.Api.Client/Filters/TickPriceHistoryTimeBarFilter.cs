using RedDeer.Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;

namespace RedDeer.Surveillance.Api.Client.Filters
{
    public class TickPriceHistoryTimeBarFilter<T> 
        : Filter<TickPriceHistoryTimeBarFilter<T>, T>
        where T : Parent
    {
        public TickPriceHistoryTimeBarFilter(T node)
            : base(node)
        {
        }

        public TickPriceHistoryTimeBarFilter<T> ArgumentRics(List<string> rics)
            => this.AddArgument("rics", rics);

        public TickPriceHistoryTimeBarFilter<T> ArgumentStartDateTime(DateTime startDateTime)
            => this.AddArgument("startDateTime", startDateTime);

        public TickPriceHistoryTimeBarFilter<T> ArgumentEndDateTime(DateTime endDateTime)
            => this.AddArgument("endDateTime", endDateTime);
    }
}
