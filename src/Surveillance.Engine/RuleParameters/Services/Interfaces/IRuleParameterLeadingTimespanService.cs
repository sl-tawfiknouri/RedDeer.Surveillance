using System;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

namespace Surveillance.Engine.Rules.RuleParameters.Services.Interfaces
{
    public interface IRuleParameterLeadingTimespanService
    {
        TimeSpan LeadingTimespan(RuleParameterDto dto);
        TimeSpan TrailingTimeSpan(RuleParameterDto dto);
    }
}