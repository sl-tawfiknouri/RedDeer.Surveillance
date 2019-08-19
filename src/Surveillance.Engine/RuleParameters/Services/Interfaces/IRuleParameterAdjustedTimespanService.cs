namespace Surveillance.Engine.Rules.RuleParameters.Services.Interfaces
{
    using System;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

    public interface IRuleParameterAdjustedTimespanService
    {
        TimeSpan LeadingTimespan(RuleParameterDto dto);

        TimeSpan TrailingTimeSpan(RuleParameterDto dto);
    }
}