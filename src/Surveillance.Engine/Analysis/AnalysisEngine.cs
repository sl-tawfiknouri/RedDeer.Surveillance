using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Surveillance.Scheduling;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Aurora.Analytics.Interfaces;
using Surveillance.Engine.Rules.Analysis.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Factory.Interfaces;
using Surveillance.Engine.Rules.Analytics.Subscriber.Factory.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Queues.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Services.Interfaces;
using Surveillance.Engine.Rules.Rules.Cancellation;
using Surveillance.Engine.Rules.Rules.Cancellation.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.Lazy.Interfaces;
using Surveillance.Engine.Rules.Universe.Subscribers.Interfaces;

namespace Surveillance.Engine.Rules.Analysis
{
    /// <summary>
    /// Main entry point to the MAR Analysis Engine
    /// </summary>
    public class AnalysisEngine : IAnalysisEngine
    {
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

        private readonly IRuleParameterService _ruleParameterService;
        private readonly IRuleParameterLeadingTimespanService _leadingTimespanService;
        private readonly ILazyTransientUniverseFactory _universeFactory;
        private readonly IRuleCancellation _ruleCancellation;

        private readonly ILogger<AnalysisEngine> _logger;

        public AnalysisEngine(
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
            IRuleParameterService ruleParameterService,
            IRuleParameterLeadingTimespanService leadingTimespanService,
            ILazyTransientUniverseFactory universeFactory,
            IRuleCancellation ruleCancellation,
            ILogger<AnalysisEngine> logger)
        {
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

            _ruleParameterService = ruleParameterService ?? throw new ArgumentNullException(nameof(ruleParameterService));
            _leadingTimespanService = leadingTimespanService ?? throw new ArgumentNullException(nameof(leadingTimespanService));
            _universeFactory = universeFactory ?? throw new ArgumentNullException(nameof(universeFactory));
            _ruleCancellation = ruleCancellation ?? throw new ArgumentNullException(nameof(ruleCancellation));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Execute(ScheduledExecution execution, ISystemProcessOperationContext opCtx)
        {
            if (execution?.Rules == null
                || !execution.Rules.Any())
            {
                _logger.LogError($"was executing a schedule that did not specify any rules to run");
                opCtx.EndEventWithError($"was executing a schedule that did not specify any rules to run");
                return;
            }
            
            _logger.LogInformation($"START OF UNIVERSE EXECUTION FOR {execution.CorrelationId}");
            var executionJson = JsonConvert.SerializeObject(execution);
            var opCtxJson = JsonConvert.SerializeObject(opCtx);
            _logger.LogInformation($"analysis execute received json {executionJson} for opCtx {opCtxJson}");

            var cts = new CancellationTokenSource();
            var ruleCancellation = new CancellableRule(execution, cts);
            _ruleCancellation.Subscribe(ruleCancellation);

            var ruleParameters = await _ruleParameterService.RuleParameters(execution);
            execution.LeadingTimespan = _leadingTimespanService.LeadingTimespan(ruleParameters);
            var player = _universePlayerFactory.Build(cts.Token);

            _universeCompletionLogger.InitiateTimeLogger(execution);
            player.Subscribe(_universeCompletionLogger);

            var dataRequestSubscriber = _dataRequestSubscriberFactory.Build(opCtx);
            var universeAlertSubscriber = _alertStreamSubscriberFactory.Build(opCtx.Id, execution.IsBackTest);
            var alertStream = _alertStreamFactory.Build();
            alertStream.Subscribe(universeAlertSubscriber);

            var ids = await _ruleSubscriber.SubscribeRules(execution, player, alertStream, dataRequestSubscriber, opCtx, ruleParameters);
            player.Subscribe(dataRequestSubscriber); // ensure this is registered after the rules so it will evaluate eschaton afterwards
            RuleRunUpdateMessageSend(execution, ids);

            var universeAnalyticsSubscriber = _analyticsSubscriber.Build(opCtx.Id);
            player.Subscribe(universeAnalyticsSubscriber);

            _logger.LogInformation($"START PLAYING UNIVERSE TO SUBSCRIBERS");
            var lazyUniverse = _universeFactory.Build(execution, opCtx);
            player.Play(lazyUniverse);
            _logger.LogInformation($"STOPPED PLAYING UNIVERSE TO SUBSCRIBERS");

            if (cts.IsCancellationRequested)
            {
                opCtx.EndEventWithError("USER CANCELLED RUN");
                _logger.LogInformation($"END OF UNIVERSE EXECUTION FOR {execution.CorrelationId} - USER CANCELLED RUN");

                _ruleCancellation.Unsubscribe(ruleCancellation);

                _logger.LogInformation($"calling rule run update message send");
                RuleRunUpdateMessageSend(execution, ids);
                _logger.LogInformation($"completed rule run update message send");

                return;
            }

            universeAlertSubscriber.Flush();
            await _ruleAnalyticsRepository.Create(universeAnalyticsSubscriber.Analytics);
            await _alertsRepository.Create(universeAlertSubscriber.Analytics);
            dataRequestSubscriber.DispatchIfSubmitRequest();

            SetOperationContextEndState(dataRequestSubscriber, opCtx);
 
            _logger.LogInformation($"calling rule run update message send");
            RuleRunUpdateMessageSend(execution, ids);
            _logger.LogInformation($"completed rule run update message send");

            _ruleCancellation.Unsubscribe(ruleCancellation);
            _logger.LogInformation($"END OF UNIVERSE EXECUTION FOR {execution.CorrelationId}");
        }

        private void RuleRunUpdateMessageSend(ScheduledExecution execution, IReadOnlyCollection<string> ids)
        {
            if (execution == null)
            {
                _logger.LogInformation($"execution was null not sending rule run update message");
                return;
            }

            if (!execution.IsBackTest)
            {
                _logger.LogInformation($"execution with correlation id {execution.CorrelationId} was not a back test not sending rule run update message");
                return;
            }

            if (ids == null)
            {
                _logger.LogInformation($"no ids for rule run with correlation id {execution.CorrelationId} not submitting update message");
                return;
            }

            foreach (var id in ids)
            {
                _logger.LogInformation($"submitting rule update message for correlation id {execution.CorrelationId} and test parameter id {id}");
                _queueRuleUpdatePublisher.Send(id).Wait();
            }

            if (!ids.Any())
            {
                _logger.LogError($"could not submit rule update message for correlation id {execution.CorrelationId} as there were no ids");
            }
        }

        private void SetOperationContextEndState(
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            ISystemProcessOperationContext opCtx)
        {
            if (!dataRequestSubscriber?.SubmitRequests ?? true)
            {
                _logger.LogInformation($"ending operation context event");
                opCtx.EndEvent();
                return;
            }

            _logger.LogInformation($"ending operating context event with missing data error");
            opCtx.EndEventWithMissingDataError();
        }
    }
}