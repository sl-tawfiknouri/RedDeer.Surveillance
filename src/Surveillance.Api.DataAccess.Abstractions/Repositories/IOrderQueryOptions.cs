﻿using System;
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
        List<int> Statuses { get; set; }
        List<int> Directions { get; set; }
        List<int> Types { get; set; }
        string PlacedDateFrom { get; }
        string PlacedDateTo { get; }
        string TzName { get; }
    }
}