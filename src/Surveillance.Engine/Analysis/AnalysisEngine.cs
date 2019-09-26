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
    using Surveillance.Data.Universe.Lazy.Interfaces;
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
    using Surveillance.Engine.Rules.Universe.Subscribers.Interfaces;

    /// <summary>
    ///     Main entry point to the MAR Analysis Engine
    /// </summary>
    public class AnalysisEngine : IAnalysisEngine
    {
        /// <summary>
        /// The alerts repository - in process of being discontinued with judgements.
        /// </summary>
        private readonly IRuleAnalyticsAlertsRepository alertsRepository;

        /// <summary>
        /// The alert stream factory - should be discontinued once judgements complete.
        /// </summary>
        private readonly IUniverseAlertStreamFactory alertStreamFactory;

        /// <summary>
        /// The alert stream subscriber factory.
        /// </summary>
        private readonly IUniverseAlertStreamSubscriberFactory alertStreamSubscriberFactory;

        /// <summary>
        /// The analytics subscriber for recording salient rule run data.
        /// </summary>
        private readonly IUniverseAnalyticsSubscriberFactory analyticsSubscriber;

        /// <summary>
        /// The data request subscriber factory.
        /// </summary>
        private readonly IUniverseDataRequestsSubscriberFactory dataRequestSubscriberFactory;

        /// <summary>
        /// The judgement service factory.
        /// </summary>
        private readonly IJudgementServiceFactory judgementServiceFactory;

        /// <summary>
        /// The logger for the analysis engine.
        /// </summary>
        private readonly ILogger<AnalysisEngine> logger;

        /// <summary>
        /// The queue rule update publisher.
        /// </summary>
        private readonly IQueueRuleUpdatePublisher queueRuleUpdatePublisher;

        /// <summary>
        /// The rescheduler service.
        /// </summary>
        private readonly ITaskReSchedulerService reschedulerService;

        /// <summary>
        /// The rule analytics repository.
        /// </summary>
        private readonly IRuleAnalyticsUniverseRepository ruleAnalyticsRepository;

        /// <summary>
        /// The rule cancellation singleton (per rule run).
        /// </summary>
        private readonly IRuleCancellation ruleCancellation;

        /// <summary>
        /// The rule parameter service.
        /// </summary>
        private readonly IRuleParameterService ruleParameterService;

        /// <summary>
        /// The rule subscriber.
        /// </summary>
        private readonly IUniverseRuleSubscriber ruleSubscriber;

        /// <summary>
        /// The _timespan service.
        /// </summary>
        private readonly IRuleParameterAdjustedTimespanService timespanService;

        /// <summary>
        /// The _universe completion logger.
        /// </summary>
        private readonly IUniversePercentageCompletionLogger universeCompletionLogger;

        /// <summary>
        /// The _universe factory.
        /// </summary>
        private readonly ILazyTransientUniverseFactory universeFactory;

        /// <summary>
        /// The _universe player factory.
        /// </summary>
        private readonly IUniversePlayerFactory universePlayerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisEngine"/> class.
        /// </summary>
        /// <param name="universePlayerFactory">
        /// The universe player factory.
        /// </param>
        /// <param name="ruleSubscriber">
        /// The rule subscriber.
        /// </param>
        /// <param name="analyticsSubscriber">
        /// The analytics subscriber.
        /// </param>
        /// <param name="alertStreamFactory">
        /// The alert stream factory.
        /// </param>
        /// <param name="judgementServiceFactory">
        /// The judgement service factory.
        /// </param>
        /// <param name="alertStreamSubscriberFactory">
        /// The alert stream subscriber factory.
        /// </param>
        /// <param name="dataRequestSubscriberFactory">
        /// The data request subscriber factory.
        /// </param>
        /// <param name="universeCompletionLogger">
        /// The universe completion logger.
        /// </param>
        /// <param name="ruleAnalyticsRepository">
        /// The rule analytics repository.
        /// </param>
        /// <param name="alertsRepository">
        /// The alerts repository.
        /// </param>
        /// <param name="queueRuleUpdatePublisher">
        /// The queue rule update publisher.
        /// </param>
        /// <param name="ruleParameterService">
        /// The rule parameter service.
        /// </param>
        /// <param name="adjustedTimespanService">
        /// The adjusted timespan service.
        /// </param>
        /// <param name="universeFactory">
        /// The universe factory.
        /// </param>
        /// <param name="ruleCancellation">
        /// The rule cancellation.
        /// </param>
        /// <param name="reschedulerService">
        /// The reschedule service.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
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
            this.universePlayerFactory =
                universePlayerFactory ?? throw new ArgumentNullException(nameof(universePlayerFactory));
            this.ruleSubscriber = 
                ruleSubscriber ?? throw new ArgumentNullException(nameof(ruleSubscriber));
            this.analyticsSubscriber =
                analyticsSubscriber ?? throw new ArgumentNullException(nameof(analyticsSubscriber));
            this.ruleAnalyticsRepository =
                ruleAnalyticsRepository ?? throw new ArgumentNullException(nameof(ruleAnalyticsRepository));
            this.alertStreamFactory =
                alertStreamFactory ?? throw new ArgumentNullException(nameof(alertStreamFactory));
            this.judgementServiceFactory = 
                judgementServiceFactory ?? throw new ArgumentNullException(nameof(judgementServiceFactory));
            this.alertStreamSubscriberFactory =
                alertStreamSubscriberFactory ?? throw new ArgumentNullException(nameof(alertStreamSubscriberFactory));
            this.alertsRepository =
                alertsRepository ?? throw new ArgumentNullException(nameof(alertsRepository));
            this.queueRuleUpdatePublisher =
                queueRuleUpdatePublisher ?? throw new ArgumentNullException(nameof(queueRuleUpdatePublisher));
            this.dataRequestSubscriberFactory =
                dataRequestSubscriberFactory ?? throw new ArgumentNullException(nameof(dataRequestSubscriberFactory));
            this.universeCompletionLogger =
                universeCompletionLogger ?? throw new ArgumentNullException(nameof(universeCompletionLogger));
            this.ruleParameterService =
                ruleParameterService ?? throw new ArgumentNullException(nameof(ruleParameterService));
            this.timespanService =
                adjustedTimespanService ?? throw new ArgumentNullException(nameof(adjustedTimespanService));
            this.universeFactory =
                universeFactory ?? throw new ArgumentNullException(nameof(universeFactory));
            this.ruleCancellation =
                ruleCancellation ?? throw new ArgumentNullException(nameof(ruleCancellation));
            this.reschedulerService =
                reschedulerService ?? throw new ArgumentNullException(nameof(reschedulerService));
            this.logger =
                logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The execute analysis engine
        /// This is the top level function for trade analysis
        /// within the surveillance engine
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Execute(ScheduledExecution execution, ISystemProcessOperationContext operationContext)
        {
            if (execution?.Rules == null || !execution.Rules.Any())
            {
                this.logger.LogError("was executing a schedule that did not specify any rules to run");
                operationContext.EndEventWithError("was executing a schedule that did not specify any rules to run");

                return;
            }

            this.logger.LogInformation($"START OF UNIVERSE EXECUTION FOR {execution.CorrelationId}");
            this.LogExecutionParameters(execution, operationContext);

            var cts = new CancellationTokenSource();
            var cancellableRule = new CancellableRule(execution, cts);
            this.ruleCancellation.Subscribe(cancellableRule);

            var ruleParameters = await this.ruleParameterService.RuleParameters(execution);
            execution.LeadingTimespan = this.timespanService.LeadingTimespan(ruleParameters);
            execution.TrailingTimespan = this.timespanService.TrailingTimeSpan(ruleParameters);

            var player = this.universePlayerFactory.Build(cts.Token);

            this.universeCompletionLogger.InitiateTimeLogger(execution);
            player.Subscribe(this.universeCompletionLogger);

            var dataRequestSubscriber = this.dataRequestSubscriberFactory.Build(operationContext);
            var universeAlertSubscriber = this.alertStreamSubscriberFactory.Build(operationContext.Id, execution.IsBackTest);
            var judgementService = this.judgementServiceFactory.Build();
            var alertStream = this.alertStreamFactory.Build();
            alertStream.Subscribe(universeAlertSubscriber);

            var ids = await this.ruleSubscriber.SubscribeRules(
                          execution,
                          player,
                          alertStream,
                          dataRequestSubscriber,
                          judgementService,
                          operationContext,
                          ruleParameters);

            player.Subscribe(dataRequestSubscriber); // ensure this is registered after the rules so it will evaluate eschaton afterwards
            this.RuleRunUpdateMessageSend(execution, ids);

            if (this.GuardForBackTestIntoFutureExecution(execution))
            {
                this.SetFailedBackTestDueToFutureExecution(operationContext, execution, cancellableRule, ids);
                return;
            }

            if (execution.AdjustedTimeSeriesTermination.Date >= DateTime.UtcNow.Date)
            {
                await this.reschedulerService.RescheduleFutureExecution(execution);
            }

            var universeAnalyticsSubscriber = this.analyticsSubscriber.Build(operationContext.Id);
            player.Subscribe(universeAnalyticsSubscriber);

            this.logger.LogInformation("START PLAYING UNIVERSE TO SUBSCRIBERS");

            // pass through rule subscriptions here :)
            // no
            // pass in a data manifest here, and build it before

            // execution + rule subs to make a data manifest


            var lazyUniverse = this.universeFactory.Build(execution, operationContext);
            player.Play(lazyUniverse);


            this.logger.LogInformation("STOPPED PLAYING UNIVERSE TO SUBSCRIBERS");

            if (cts.IsCancellationRequested)
            {
                this.SetRuleCancelledState(operationContext, execution, cancellableRule, ids);
                return;
            }

            // post rule execution analysis
            universeAlertSubscriber.Flush();
            await this.ruleAnalyticsRepository.Create(universeAnalyticsSubscriber.Analytics);
            await this.alertsRepository.Create(universeAlertSubscriber.Analytics);
            dataRequestSubscriber.DispatchIfSubmitRequest();
            judgementService.PassJudgement();

            this.SetOperationContextEndState(dataRequestSubscriber, operationContext);

            this.logger.LogInformation("calling rule run update message send");
            this.RuleRunUpdateMessageSend(execution, ids);
            this.logger.LogInformation("completed rule run update message send");

            this.ruleCancellation.Unsubscribe(cancellableRule);
            this.logger.LogInformation($"END OF UNIVERSE EXECUTION FOR {execution.CorrelationId}");
        }

        /// <summary>
        /// The guard for back test into future execution.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool GuardForBackTestIntoFutureExecution(ScheduledExecution execution)
        {
            if (execution == null)
            {
                return false;
            }

            if (!execution.IsBackTest)
            {
                return false;
            }

            return execution.AdjustedTimeSeriesTermination.Date > DateTime.UtcNow.Date;
        }

        /// <summary>
        /// The log execution parameters to info log.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        private void LogExecutionParameters(ScheduledExecution execution, ISystemProcessOperationContext operationContext)
        {
            var executionJson = JsonConvert.SerializeObject(execution);
            var operationContextJson = JsonConvert.SerializeObject(operationContext);

            this.logger.LogInformation($"analysis execute received json {executionJson} for opCtx {operationContextJson}");
        }

        /// <summary>
        /// The rule run update message send
        /// This is for relaying updates to the client service.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="ids">
        /// The ids.
        /// </param>
        private void RuleRunUpdateMessageSend(ScheduledExecution execution, IReadOnlyCollection<string> ids)
        {
            if (execution == null)
            {
                this.logger.LogInformation("execution was null not sending rule run update message");
                return;
            }

            if (!execution.IsBackTest)
            {
                this.logger.LogInformation($"execution with correlation id {execution.CorrelationId} was not a back test not sending rule run update message");
                return;
            }

            if (ids == null)
            {
                this.logger.LogInformation($"no ids for rule run with correlation id {execution.CorrelationId} not submitting update message");
                return;
            }

            foreach (var id in ids)
            {
                this.logger.LogInformation($"submitting rule update message for correlation id {execution.CorrelationId} and test parameter id {id}");
                this.queueRuleUpdatePublisher.Send(id).Wait();
            }

            if (!ids.Any())
            {
                this.logger.LogError($"could not submit rule update message for correlation id {execution.CorrelationId} as there were no ids");
            }
        }

        /// <summary>
        /// The set failed back test due to future execution.
        /// </summary>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="cancellableRule">
        /// The cancellable rule.
        /// </param>
        /// <param name="ids">
        /// The ids.
        /// </param>
        private void SetFailedBackTestDueToFutureExecution(
            ISystemProcessOperationContext operationContext,
            ScheduledExecution execution,
            CancellableRule cancellableRule,
            IReadOnlyCollection<string> ids)
        {
            operationContext.EndEventWithError("Set back test to end some time in the future");
            this.logger.LogInformation($"End of universe execution for {execution.CorrelationId} - back test had illegal future dates");

            this.ruleCancellation.Unsubscribe(cancellableRule);

            this.logger.LogInformation("calling rule run update message send");
            this.RuleRunUpdateMessageSend(execution, ids);
            this.logger.LogInformation("completed rule run update message send");
        }

        /// <summary>
        /// The set operation context end state.
        /// </summary>
        /// <param name="dataRequestSubscriber">
        /// The data request subscriber.
        /// </param>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        private void SetOperationContextEndState(
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            ISystemProcessOperationContext operationContext)
        {
            if (!dataRequestSubscriber?.SubmitRequests ?? true)
            {
                this.logger.LogInformation("ending operation context event");
                operationContext.EndEvent();
                return;
            }

            this.logger.LogInformation("ending operating context event with missing data error");
            operationContext.EndEventWithMissingDataError();
        }

        /// <summary>
        /// The set rule cancelled state.
        /// </summary>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="cancellableRule">
        /// The cancellable rule.
        /// </param>
        /// <param name="ids">
        /// The ids.
        /// </param>
        private void SetRuleCancelledState(
            ISystemProcessOperationContext operationContext,
            ScheduledExecution execution,
            CancellableRule cancellableRule,
            IReadOnlyCollection<string> ids)
        {
            operationContext.EndEventWithError("USER CANCELLED RUN");
            this.logger.LogInformation($"END OF UNIVERSE EXECUTION FOR {execution.CorrelationId} - USER CANCELLED RUN");

            this.ruleCancellation.Unsubscribe(cancellableRule);

            this.logger.LogInformation("calling rule run update message send");
            this.RuleRunUpdateMessageSend(execution, ids);
            this.logger.LogInformation("completed rule run update message send");
        }
    }
}