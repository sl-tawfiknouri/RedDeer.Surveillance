namespace Surveillance.Engine.Rules.Universe.Subscribers.Interfaces
{
    using System.Collections.Generic;

    using Domain.Surveillance.Scheduling;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    /// <summary>
    /// The RuleSubscriber interface.
    /// </summary>
    public interface IRuleSubscriber
    {
        /// <summary>
        /// The collate subscriptions.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="ruleParameters">
        /// The rule parameters.
        /// </param>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        /// <param name="dataRequestSubscriber">
        /// The data request subscriber.
        /// </param>
        /// <param name="judgementService">
        /// The judgement service.
        /// </param>
        /// <param name="alertStream">
        /// The alert stream.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseEvent"/>.
        /// </returns>
        IReadOnlyCollection<IUniverseRule> CollateSubscriptions(
            ScheduledExecution execution,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext operationContext,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IJudgementService judgementService,
            IUniverseAlertStream alertStream);
    }
}