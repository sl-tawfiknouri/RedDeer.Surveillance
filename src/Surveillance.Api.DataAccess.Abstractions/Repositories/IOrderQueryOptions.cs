namespace Surveillance.Api.DataAccess.Abstractions.Repositories
{
    using System;
    using System.Collections.Generic;

    public interface IOrderQueryOptions
    {
        List<int> Directions { get; set; }

        HashSet<string> ExcludeTraderIds { get; }

        List<int> Ids { get; }

        DateTime? PlacedDateFrom { get; }

        DateTime? PlacedDateTo { get; }

        List<string> ReddeerIds { get; }

        List<int> Statuses { get; set; }

        int? Take { get; }

        HashSet<string> TraderIds { get; }

        List<int> Types { get; set; }

        string TzName { get; }
    }
}