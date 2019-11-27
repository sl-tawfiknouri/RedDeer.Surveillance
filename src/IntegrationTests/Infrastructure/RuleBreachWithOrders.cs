using Surveillance.Api.DataAccess.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedDeer.Surveillance.IntegrationTests.Infrastructure
{
    public class RuleBreachWithOrders
    {
        public IRuleBreach RuleBreach { get; set; }
        public List<IOrder> Orders { get; set; }
    }
}
