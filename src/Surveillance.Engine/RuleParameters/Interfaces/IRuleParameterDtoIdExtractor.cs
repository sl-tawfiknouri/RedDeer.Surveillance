using System.Collections.Generic;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

namespace Surveillance.Engine.Rules.RuleParameters.Interfaces
{
    public interface IRuleParameterDtoIdExtractor
    {
        IReadOnlyCollection<string> ExtractIds(RuleParameterDto dtos);
    }
}