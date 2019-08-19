namespace Surveillance.Api.DataAccess.Repositories
{
    using System;
    using System.Collections.Generic;

    using Surveillance.Api.DataAccess.Abstractions.Repositories;

    public class OrderQueryOptions : IOrderQueryOptions
    {
        public List<int> Directions { get; set; }

        public HashSet<string> ExcludeTraderIds { get; set; }

        public List<int> Ids { get; set; }

        public DateTime? PlacedDateFrom { get; set; }

        public DateTime? PlacedDateTo { get; set; }

        public List<string> ReddeerIds { get; set; }

        public List<int> Statuses { get; set; }

        public int? Take { get; set; }

        public HashSet<string> TraderIds { get; set; }

        public List<int> Types { get; set; }

        public string TzName { get; set; }
    }
}