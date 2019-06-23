using System;
using System.Collections.Generic;

namespace Surveillance.Api.DataAccess.Abstractions.Repositories
{
    public interface IOrderQueryOptions
    {
        List<int> Ids { get; }
        int? Take { get; }
        List<string> TraderIds { get; }
        List<string> ReddeerIds { get; }
        List<int> Statuses { get; set; }
        List<int> Directions { get; set; }
        List<int> Types { get; set; }
        DateTime? PlacedDateFrom { get; }
        DateTime? PlacedDateTo { get; }
        string TzName { get; }
    }
}
