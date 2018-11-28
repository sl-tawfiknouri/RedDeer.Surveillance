using System;
using System.Collections.Generic;
using Domain.Scheduling;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Universe.Subscribers.Interfaces
{
    public interface IRuleSubscriber
    {
        IReadOnlyCollection<IObserver<IUniverseEvent>> CollateSubscriptions(
            ScheduledExecution execution,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream);
    }
}
