using Surveillance.Api.DataAccess.Abstractions.Repositories;
using System.Collections.Generic;

namespace Surveillance.Api.DataAccess.Repositories
{
    public class OrderQueryOptions : IOrderQueryOptions
    {
        public List<int> Ids { get; set; }
        public int? Take { get; set; }
        public HashSet<string> TraderIds { get; set; }
        public HashSet<string> NotTraderIds { get; set; }
        public List<string> ReddeerIds { get; set; }
        public List<int> Statuses { get; set; }
        public List<int> Directions { get; set; }
        public List<int> Types { get; set; }
        public string PlacedDateFrom { get; set; }
        public string PlacedDateTo { get; set; }
        public string TzName { get; set; }
    }
}
