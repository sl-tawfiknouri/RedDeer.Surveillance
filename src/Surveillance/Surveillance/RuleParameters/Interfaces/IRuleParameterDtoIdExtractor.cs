using System.Collections.Generic;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

namespace Surveillance.RuleParameters.Interfaces
{
    public interface IRuleParameterDtoIdExtractor
    {
        IReadOnlyCollection<string> ExtractIds(RuleParameterDto dtos);
    }
}