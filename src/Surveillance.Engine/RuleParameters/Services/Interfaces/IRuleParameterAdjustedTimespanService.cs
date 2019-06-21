using System;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

namespace Surveillance.Engine.Rules.RuleParameters.Services.Interfaces
{
    public interface IRuleParameterAdjustedTimespanService
    {
        TimeSpan LeadingTimespan(RuleParameterDto dto);
        TimeSpan TrailingTimeSpan(RuleParameterDto dto);
    }
}