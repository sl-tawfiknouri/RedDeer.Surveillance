namespace Surveillance.Engine.Rules.Analysis
{
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
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Queues.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Services.Interfaces;
    using Surveillance.Engine.Rules.Rules.Cancellation;
    using Surveillance.Engine.Rules.Rules.Cancellation.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.Lazy.Interfaces;
    using Surveillance.Engine.Rules.Universe.Subscribers.Interfaces;

    /// <summary>
    ///     Main entry point to the MAR Analysis Engine
    /// </summary>
    public class AnalysisEngine : IAnalysisEngine
    {
        private readonly IRuleAnalyticsAlertsRepository _alertsRepository;

        private readonly IUniverseAlertStreamFactory _alertStreamFactory;

        private readonly IUniverseAlertStreamSubscriberFactory _alertStreamSubscriberFactory;

        private readonly IUniverseAnalyticsSubscriberFactory _analyticsSubscriber;

        private readonly IUniverseDataRequestsSubscriberFactory _dataRequestSubscriberFactory;

        private readonly IJudgementServiceFactory _judgementServiceFactory;

        private readonly ILogger<AnalysisEngine> _logger;

        private readonly IQueueRuleUpdatePublisher _queueRuleUpdatePublisher;

        private readonly ITaskReSchedulerService _reschedulerService;

        private readonly IRuleAnalyticsUniverseRepository _ruleAnalyticsRepository;

        private readonly IRuleCancellation _ruleCancellation;

        private readonly IRuleParameterService _ruleParameterService;

        private readonly IUniverseRuleSubscriber _ruleSubscriber;

        private readonly IRuleParameterAdjustedTimespanService _timespanService;

        private readonly IUniversePercentageCompletionLogger _universeCompletionLogger;

        private readonly ILazyTransientUniverseFactory _universeFactory;

        private readonly IUniversePlayerFactory _universePlayerFactory;

        public AnalysisEngine(
            IUniversePlayerFactory universePlayerFactory,
            IUniverseRuleSubscriber ruleSubscriber,
            IUniverseAnalyticsSubscriberFactory analyticsSubscriber,
            IUniverseAlertStreamFactory alertStreamFactory,
            IJudgementServiceFactory judgementServiceFactory,
            IUniverseAlertStreamSubscriberFactory alertStreamSubscriberFactory,
            IUniverseDataRequestsSubscriberFactory dataRequestSubscriberFactory,
            IUniversePercentageCompletionLogger universeCompletionLogger,
            IRuleAnalyticsUniverseRepository ruleAnalyticsRepository,
            IRuleAnalyticsAlertsRepository alertsRepository,
            IQueueRuleUpdatePublisher queueRuleUpdatePublisher,
            IRuleParameterService ruleParameterService,
            IRuleParameterAdjustedTimespanService adjustedTimespanService,
            ILazyTransientUniverseFactory universeFactory,
            IRuleCancellation ruleCancellation,
            ITaskReSchedulerService reschedulerService,
            ILogger<AnalysisEngine> logger)
        {
            this._universePlayerFactory =
                universePlayerFactory ?? throw new ArgumentNullException(nameof(universePlayerFactory));

            this._ruleSubscriber = ruleSubscriber ?? throw new ArgumentNullException(nameof(ruleSubscriber));
            this._analyticsSubscriber =
                analyticsSubscriber ?? throw new ArgumentNullException(nameof(analyticsSubscriber));
            this._ruleAnalyticsRepository = ruleAnalyticsRepository
                                            ?? throw new ArgumentNullException(nameof(ruleAnalyticsRepository));
            this._alertStreamFactory =
                alertStreamFactory ?? throw new ArgumentNullException(nameof(alertStreamFactory));
            this._judgementServiceFactory = judgementServiceFactory
                                            ?? throw new ArgumentNullException(nameof(judgementServiceFactory));
            this._alertStreamSubscriberFactory = alertStreamSubscriberFactory
                                                 ?? throw new ArgumentNullException(
                                                     nameof(alertStreamSubscriberFactory));
            this._alertsRepository = alertsRepository ?? throw new ArgumentNullException(nameof(alertsRepository));
            this._queueRuleUpdatePublisher = queueRuleUpdatePublisher
                                             ?? throw new ArgumentNullException(nameof(queueRuleUpdatePublisher));
            this._dataRequestSubscriberFactory = dataRequestSubscriberFactory
                                                 ?? throw new ArgumentNullException(
                                                     nameof(dataRequestSubscriberFactory));
            this._universeCompletionLogger = universeCompletionLogger
                                             ?? throw new ArgumentNullException(nameof(universeCompletionLogger));

            this._ruleParameterService =
                ruleParameterService ?? throw new ArgumentNullException(nameof(ruleParameterService));
            this._timespanService = adjustedTimespanService
                                    ?? throw new ArgumentNullException(nameof(adjustedTimespanService));
            this._universeFactory = universeFactory ?? throw new ArgumentNullException(nameof(universeFactory));
            this._ruleCancellation = ruleCancellation ?? throw new ArgumentNullException(nameof(ruleCancellation));
            this._reschedulerService =
                reschedulerService ?? throw new ArgumentNullException(nameof(reschedulerService));

            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Execute(ScheduledExecution execution, ISystemProcessOperationContext opCtx)
        {
            if (execution?.Rules == null || !execution.Rules.Any())
            {
                this._logger.LogError("was executing a schedule that did not specify any rules to run");
                opCtx.EndEventWithError("was executing a schedule that did not specify any rules to run");
                return;
            }

            this._logger.LogInformation($"START OF UNIVERSE EXECUTION FOR {execution.CorrelationId}");
            this.LogExecutionParameters(execution, opCtx);

            var cts = new CancellationTokenSource();
            var ruleCancellation = new CancellableRule(execution, cts);
            this._ruleCancellation.Subscribe(ruleCancellation);

            var ruleParameters = await this._ruleParameterService.RuleParameters(execution);
            execution.LeadingTimespan = this._timespanService.LeadingTimespan(ruleParameters);
            execution.TrailingTimespan = this._timespanService.TrailingTimeSpan(ruleParameters);

            var player = this._universePlayerFactory.Build(cts.Token);

            this._universeCompletionLogger.InitiateTimeLogger(execution);
            player.Subscribe(this._universeCompletionLogger);

            var dataRequestSubscriber = this._dataRequestSubscriberFactory.Build(opCtx);
            var universeAlertSubscriber = this._alertStreamSubscriberFactory.Build(opCtx.Id, execution.IsBackTest);
            var judgementService = this._judgementServiceFactory.Build();
            var alertStream = this._alertStreamFactory.Build();
            alertStream.Subscribe(universeAlertSubscriber);

            var ids = await this._ruleSubscriber.SubscribeRules(
                          execution,
                          player,
                          alertStream,
                          dataRequestSubscriber,
                          judgementService,
                          opCtx,
                          ruleParameters);
            player.Subscribe(
                dataRequestSubscriber); // ensure this is registered after the rules so it will evaluate eschaton afterwards
            this.RuleRunUpdateMessageSend(execution, ids);

            if (this.GuardForBackTestIntoFutureExecution(execution))
            {
                this.SetFailedBackTestDueToFutureExecution(opCtx, execution, ruleCancellation, ids);
                return;
            }

            if (execution.AdjustedTimeSeriesTermination.Date >= DateTime.UtcNow.Date)
                await this._reschedulerService.RescheduleFutureExecution(execution);

            var universeAnalyticsSubscriber = this._analyticsSubscriber.Build(opCtx.Id);
            player.Subscribe(universeAnalyticsSubscriber);

            this._logger.LogInformation("START PLAYING UNIVERSE TO SUBSCRIBERS");
            var lazyUniverse = this._universeFactory.Build(execution, opCtx);
            player.Play(lazyUniverse);
            this._logger.LogInformation("STOPPED PLAYING UNIVERSE TO SUBSCRIBERS");

            if (cts.IsCancellationRequested)
            {
                this.SetRuleCancelledState(opCtx, execution, ruleCancellation, ids);
                return;
            }

            universeAlertSubscriber.Flush();
            await this._ruleAnalyticsRepository.Create(universeAnalyticsSubscriber.Analytics);
            await this._alertsRepository.Create(universeAlertSubscriber.Analytics);
            dataRequestSubscriber.DispatchIfSubmitRequest();
            judgementService.PassJudgement();

            this.SetOperationContextEndState(dataRequestSubscriber, opCtx);

            this._logger.LogInformation("calling rule run update message send");
            this.RuleRunUpdateMessageSend(execution, ids);
            this._logger.LogInformation("completed rule run update message send");

            this._ruleCancellation.Unsubscribe(ruleCancellation);
            this._logger.LogInformation($"END OF UNIVERSE EXECUTION FOR {execution.CorrelationId}");
        }

        private bool GuardForBackTestIntoFutureExecution(ScheduledExecution execution)
        {
            if (execution == null)
                return false;

            if (!execution.IsBackTest)
                return false;

            return execution.AdjustedTimeSeriesTermination.Date > DateTime.UtcNow.Date;
        }

        private void LogExecutionParameters(ScheduledExecution execution, ISystemProcessOperationContext opCtx)
        {
            var executionJson = JsonConvert.SerializeObject(execution);
            var opCtxJson = JsonConvert.SerializeObject(opCtx);
            this._logger.LogInformation($"analysis execute received json {executionJson} for opCtx {opCtxJson}");
        }

        private void RuleRunUpdateMessageSend(ScheduledExecution execution, IReadOnlyCollection<string> ids)
        {
            if (execution == null)
            {
                this._logger.LogInformation("execution was null not sending rule run update message");
                return;
            }

            if (!execution.IsBackTest)
            {
                this._logger.LogInformation(
                    $"execution with correlation id {execution.CorrelationId} was not a back test not sending rule run update message");
                return;
            }

            if (ids == null)
            {
                this._logger.LogInformation(
                    $"no ids for rule run with correlation id {execution.CorrelationId} not submitting update message");
                return;
            }

            foreach (var id in ids)
            {
                this._logger.LogInformation(
                    $"submitting rule update message for correlation id {execution.CorrelationId} and test parameter id {id}");
                this._queueRuleUpdatePublisher.Send(id).Wait();
            }

            if (!ids.Any())
                this._logger.LogError(
                    $"could not submit rule update message for correlation id {execution.CorrelationId} as there were no ids");
        }

        private void SetFailedBackTestDueToFutureExecution(
            ISystemProcessOperationContext opCtx,
            ScheduledExecution execution,
            CancellableRule ruleCancellation,
            IReadOnlyCollection<string> ids)
        {
            opCtx.EndEventWithError("Set back test to end some time in the future");
            this._logger.LogInformation(
                $"End of universe execution for {execution.CorrelationId} - back test had illegal future dates");

            this._ruleCancellation.Unsubscribe(ruleCancellation);

            this._logger.LogInformation("calling rule run update message send");
            this.RuleRunUpdateMessageSend(execution, ids);
            this._logger.LogInformation("completed rule run update message send");
        }

        private void SetOperationContextEndState(
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            ISystemProcessOperationContext opCtx)
        {
            if (!dataRequestSubscriber?.SubmitRequests ?? true)
            {
                this._logger.LogInformation("ending operation context event");
                opCtx.EndEvent();
                return;
            }

            this._logger.LogInformation("ending operating context event with missing data error");
            opCtx.EndEventWithMissingDataError();
        }

        private void SetRuleCancelledState(
            ISystemProcessOperationContext opCtx,
            ScheduledExecution execution,
            CancellableRule ruleCancellation,
            IReadOnlyCollection<string> ids)
        {
            opCtx.EndEventWithError("USER CANCELLED RUN");
            this._logger.LogInformation(
                $"END OF UNIVERSE EXECUTION FOR {execution.CorrelationId} - USER CANCELLED RUN");

            this._ruleCancellation.Unsubscribe(ruleCancellation);

            this._logger.LogInformation("calling rule run update message send");
            this.RuleRunUpdateMessageSend(execution, ids);
            this._logger.LogInformation("completed rule run update message send");
        }
    }
}