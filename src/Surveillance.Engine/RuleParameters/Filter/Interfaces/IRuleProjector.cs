﻿namespace Surveillance.Engine.Rules.RuleParameters.Filter.Interfaces
{
    public interface IRuleProjector
    {
        RuleFilter Project(RedDeer.Contracts.SurveillanceService.Api.RuleParameter.Filter filter);
    }
}