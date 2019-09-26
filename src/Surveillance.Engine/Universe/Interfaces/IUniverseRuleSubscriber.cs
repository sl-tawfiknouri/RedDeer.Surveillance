namespace Surveillance.Engine.Rules.Universe.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Domain.Surveillance.Scheduling;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;

    /// <summary>
    /// The UniverseRuleSubscriber interface.
    /// </summary>
    public interface IUniverseRuleSubscriber
    {
        /// <summary>
        /// The subscribe rules.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="player">
        /// The player.
        /// </param>
        /// <param name="alertStream">
        /// The alert stream.
        /// </param>
        /// <param name="dataRequestSubscriber">
        /// The data request subscriber.
        /// </param>
        /// <param name="judgementService">
        /// The judgement service.
        /// </param>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        /// <param name="ruleParameters">
        /// The rule parameters.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<IReadOnlyCollection<string>> SubscribeRules(
            ScheduledExecution execution,
            IUniversePlayer player,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IJudgementService judgementService,
            ISystemProcessOperationContext operationContext,
            RuleParameterDto ruleParameters);
    }
}