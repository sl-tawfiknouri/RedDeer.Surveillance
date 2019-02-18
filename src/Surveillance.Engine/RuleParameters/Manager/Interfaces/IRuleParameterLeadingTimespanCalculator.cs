using System;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

namespace Surveillance.Engine.Rules.RuleParameters.Manager.Interfaces
{
    public interface IRuleParameterLeadingTimespanCalculator
    {
        TimeSpan LeadingTimespan(RuleParameterDto dto);
    }
}