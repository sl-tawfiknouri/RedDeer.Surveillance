using Surveillance.Api.DataAccess.Abstractions.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.DataAccess.Repositories
{
    public class OrderQueryOptions : IOrderQueryOptions
    {
        public List<int> Ids { get; set; }
        public int? Take { get; set; }
        public List<string> TraderIds { get; set; }
        public List<string> ReddeerIds { get; set; }
        public string PlacedDateFrom { get; set; }
        public string PlacedDateTo { get; set; }
    }
}
