using Surveillance.Api.DataAccess.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.DataAccess.Entities
{
    public class Aggregation : IAggregation
    {
        public string Key { get; set; }
        public int Count { get; set; }
    }
}
