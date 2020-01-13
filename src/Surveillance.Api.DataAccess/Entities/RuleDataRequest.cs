using Surveillance.Api.DataAccess.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.DataAccess.Entities
{
    public class RuleDataRequest : IRuleDataRequest
    {
        public int Id { get; set; }
        public int SystemProcessOperationRuleRunId { get; set; }
    }
}
