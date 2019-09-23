namespace Surveillance.Engine.Rules.RuleParameters.Interfaces
{
    using System.Collections.Generic;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

    public interface IRuleParameterDtoIdExtractor
    {
        IReadOnlyCollection<string> ExtractIds(RuleParameterDto dtos);
    }
}