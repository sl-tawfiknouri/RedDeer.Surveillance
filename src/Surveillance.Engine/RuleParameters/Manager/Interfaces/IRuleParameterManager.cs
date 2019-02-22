﻿using System.Threading.Tasks;
using Domain.Scheduling;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

namespace Surveillance.Engine.Rules.RuleParameters.Manager.Interfaces
{
    public interface IRuleParameterManager
    {
        Task<RuleParameterDto> RuleParameters(ScheduledExecution execution);
    }
}