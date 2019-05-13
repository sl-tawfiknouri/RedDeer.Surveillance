using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.DataAccess.Abstractions.Repositories
{
    public interface IOrderQueryOptions
    {
        List<int> Ids { get; }
        int? Take { get; }
        List<string> TraderIds { get; }
        List<string> ReddeerIds { get; }
        string PlacedDateFrom { get; }
        string PlacedDateTo { get; }
    }
}
