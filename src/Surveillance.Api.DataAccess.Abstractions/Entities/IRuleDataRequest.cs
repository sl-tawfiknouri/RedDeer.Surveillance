using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    public interface IRuleDataRequest
    {
        int Id { get; set; }
        int SystemProcessOperationRuleRunId { get; set; }
    }
}
