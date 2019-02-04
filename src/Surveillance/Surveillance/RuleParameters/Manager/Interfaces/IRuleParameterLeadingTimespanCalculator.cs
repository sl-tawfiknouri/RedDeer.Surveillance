using System;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

namespace Surveillance.RuleParameters.Manager.Interfaces
{
    public interface IRuleParameterLeadingTimespanCalculator
    {
        TimeSpan LeadingTimespan(RuleParameterDto dto);
    }
}