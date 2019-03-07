using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Surveillance.Scheduling;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Aurora.Analytics.Interfaces;
using Surveillance.Engine.Rules.Analysis.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Factory.Interfaces;
using Surveillance.Engine.Rules.Analytics.Subscriber.Factory.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Queues.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Manager.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.Subscribers.Interfaces;

namespace Surveillance.Engine.Rules.Analysis
{
    /// <summary>
    /// Main entry point to the MAR Analysis Engine
    /// </summary>
    public class AnalysisEngine : IAnalysisEngine
    {
        private readonly IUniverseBuilder _universeBuilder;
        private readonly IUniversePlayerFactory _universePlayerFactory;
        private readonly IUniverseRuleSubscriber _ruleSubscriber;
        private readonly IUniverseAnalyticsSubscriberFactory _analyticsSubscriber;
        private readonly IUniverseAlertStreamFactory _alertStreamFactory;
        private readonly IUniverseAlertStreamSubscriberFactory _alertStreamSubscriberFactory;
        private readonly IUniverseDataRequestsSubscriberFactory _dataRequestSubscriberFactory;
        private readonly IUniversePercentageCompletionLogger _universeCompletionLogger;

        private readonly IRuleAnalyticsUniverseRepository _ruleAnalyticsRepository;
        private readonly IRuleAnalyticsAlertsRepository _alertsRepository;

        private readonly IQueueRuleUpdatePublisher _queueRuleUpdatePublisher;

        private readonly IRuleParameterManager _ruleParameterManager;
        private readonly IRuleParameterLeadingTimespanCalculator _leadingTimespanCalculator;
        
        private readonly ILogger<AnalysisEngine> _logger;

        public AnalysisEngine(
            IUniverseBuilder universeBuilder,
            IUniversePlayerFactory universePlayerFactory,
            IUniverseRuleSubscriber ruleSubscriber,
            IUniverseAnalyticsSubscriberFactory analyticsSubscriber,
            IUniverseAlertStreamFactory alertStreamFactory,
            IUniverseAlertStreamSubscriberFactory alertStreamSubscriberFactory,
            IUniverseDataRequestsSubscriberFactory dataRequestSubscriberFactory,
            IUniversePercentageCompletionLogger universeCompletionLogger,
            IRuleAnalyticsUniverseRepository ruleAnalyticsRepository,
            IRuleAnalyticsAlertsRepository alertsRepository,
            IQueueRuleUpdatePublisher queueRuleUpdatePublisher,
            IRuleParameterManager ruleParameterManager,
            IRuleParameterLeadingTimespanCalculator leadingTimespanCalculator,
            ILogger<AnalysisEngine> logger)
        {
            _universeBuilder = universeBuilder ?? throw new ArgumentNullException(nameof(universeBuilder));

            _universePlayerFactory =
                universePlayerFactory
                ?? throw new ArgumentNullException(nameof(universePlayerFactory));

            _ruleSubscriber = ruleSubscriber ?? throw new ArgumentNullException(nameof(ruleSubscriber));
            _analyticsSubscriber = analyticsSubscriber ?? throw new ArgumentNullException(nameof(analyticsSubscriber));
            _ruleAnalyticsRepository = ruleAnalyticsRepository ?? throw new ArgumentNullException(nameof(ruleAnalyticsRepository));
            _alertStreamFactory = alertStreamFactory ?? throw new ArgumentNullException(nameof(alertStreamFactory));
            _alertStreamSubscriberFactory = alertStreamSubscriberFactory ?? throw new ArgumentNullException(nameof(alertStreamSubscriberFactory));
            _alertsRepository = alertsRepository ?? throw new ArgumentNullException(nameof(alertsRepository));
            _queueRuleUpdatePublisher = queueRuleUpdatePublisher ?? throw new ArgumentNullException(nameof(queueRuleUpdatePublisher));
            _dataRequestSubscriberFactory = dataRequestSubscriberFactory ?? throw new ArgumentNullException(nameof(dataRequestSubscriberFactory));
            _universeCompletionLogger = universeCompletionLogger ?? throw new ArgumentNullException(nameof(universeCompletionLogger));

            _ruleParameterManager = ruleParameterManager ?? throw new ArgumentNullException(nameof(ruleParameterManager));
            _leadingTimespanCalculator = leadingTimespanCalculator ?? throw new ArgumentNullException(nameof(leadingTimespanCalculator));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Execute(ScheduledExecution execution, ISystemProcessOperationContext opCtx)
        {

            if (execution?.Rules == null
                || !execution.Rules.Any())
            {
                _logger.LogError($"{nameof(AnalysisEngine)} was executing a schedule that did not specify any rules to run");
                opCtx.EndEventWithError($"{nameof(AnalysisEngine)} was executing a schedule that did not specify any rules to run");
                return;
            }

            _logger.LogInformation($"{nameof(AnalysisEngine)} START OF UNIVERSE EXECUTION FOR {execution.CorrelationId}");

            var ruleParameters = await _ruleParameterManager.RuleParameters(execution);
            execution.LeadingTimespan = _leadingTimespanCalculator.LeadingTimespan(ruleParameters);
            var universe = await _universeBuilder.Summon(execution, opCtx);
            var player = _universePlayerFactory.Build();

            _universeCompletionLogger.InitiateTimeLogger(execution);
            _universeCompletionLogger.InitiateEventLogger(universe);
            player.Subscribe(_universeCompletionLogger);

            var dataRequestSubscriber = _dataRequestSubscriberFactory.Build(opCtx);
            var universeAlertSubscriber = _alertStreamSubscriberFactory.Build(opCtx.Id, execution.IsBackTest);
            var alertStream = _alertStreamFactory.Build();
            alertStream.Subscribe(universeAlertSubscriber);

            var ids = await _ruleSubscriber.SubscribeRules(execution, player, alertStream, dataRequestSubscriber, opCtx, ruleParameters);
            player.Subscribe(dataRequestSubscriber); // ensure this is registered after the rules so it will evaluate eschaton afterwards
            await RuleRunUpdateMessageSend(execution, ids);

            var universeAnalyticsSubscriber = _analyticsSubscriber.Build(opCtx.Id);
            player.Subscribe(universeAnalyticsSubscriber);

            _logger.LogInformation($"{nameof(AnalysisEngine)} START PLAYING UNIVERSE TO SUBSCRIBERS");
            player.Play(universe);
            _logger.LogInformation($"{nameof(AnalysisEngine)} STOPPED PLAYING UNIVERSE TO SUBSCRIBERS");

            universeAlertSubscriber.Flush();
            await _ruleAnalyticsRepository.Create(universeAnalyticsSubscriber.Analytics);
            await _alertsRepository.Create(universeAlertSubscriber.Analytics);

            SetOperationContextEndState(dataRequestSubscriber, opCtx);
            _logger.LogInformation($"{nameof(AnalysisEngine)} END OF UNIVERSE EXECUTION FOR {execution.CorrelationId}");

            await RuleRunUpdateMessageSend(execution, ids);
        }

        private async Task RuleRunUpdateMessageSend(ScheduledExecution execution, IReadOnlyCollection<string> ids)
        {
            if (ids == null)
            {
                return;
            }

            if (execution == null)
            {
                return;
            }

            if (!execution.IsBackTest)
            {
                return;
            }

            foreach (var id in ids)
            {
                await _queueRuleUpdatePublisher.Send(id);
            }
        }

        private void SetOperationContextEndState(
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            ISystemProcessOperationContext opCtx)
        {
            if (!dataRequestSubscriber?.SubmitRequests ?? true)
            {
                opCtx.EndEvent();
                return;
            }

            opCtx.EndEventWithMissingDataError();
        }
    }
}