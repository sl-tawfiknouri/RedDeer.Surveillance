﻿using Domain.Scheduling;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Universe.Subscribers.Interfaces
{
    public interface IHighProfitsSubscriber
    {
        void HighProfitsRule(
            ScheduledExecution execution,
            IUniversePlayer player,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream);
    }
}