using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Scheduling;
using Domain.Scheduling.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Api.RuleParameter.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.Scheduler.Interfaces;
using Utilities.Aws_IO.Interfaces;

namespace Surveillance.Scheduler
{
    public class ReddeerSmartRuleScheduler : IReddeerSmartRuleScheduler
    {
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private readonly IScheduledExecutionMessageBusSerialiser _messageBusSerialiser;
        private readonly IRuleParameterApiRepository _ruleParameterApiRepository;
        private readonly IRuleParameterToRulesMapper _ruleParameterMapper;

        private readonly ILogger<ReddeerSmartRuleScheduler> _logger;
        private CancellationTokenSource _messageBusCts;

        public ReddeerSmartRuleScheduler(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            IScheduledExecutionMessageBusSerialiser messageBusSerialiser,
            IRuleParameterApiRepository ruleParameterApiRepository,
            IRuleParameterToRulesMapper ruleParameterMapper,
            ILogger<ReddeerSmartRuleScheduler> logger)
        {
            _awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            _awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            _messageBusSerialiser = messageBusSerialiser ?? throw new ArgumentNullException(nameof(messageBusSerialiser));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _ruleParameterApiRepository =
                ruleParameterApiRepository
                ?? throw new ArgumentNullException(nameof(ruleParameterApiRepository));

            _ruleParameterMapper =
                ruleParameterMapper
                ?? throw new ArgumentNullException(nameof(ruleParameterMapper));
        }
        
        public void Initiate()
        {
            _messageBusCts?.Cancel();

            _messageBusCts = new CancellationTokenSource();

            _awsQueueClient.SubscribeToQueueAsync(
                _awsConfiguration.ScheduledRuleQueueName,
                async (s1, s2) => { await ExecuteNonDistributedMessage(s1, s2); },
                _messageBusCts.Token);
        }

        public void Terminate()
        {
            _messageBusCts?.Cancel();
            _messageBusCts = null;
        }

        public async Task ExecuteNonDistributedMessage(string messageId, string messageBody)
        {
            _logger.LogInformation($"ReddeerRuleScheduler read message {messageId} with body {messageBody} from {_awsConfiguration.ScheduledRuleQueueName}");

            var execution = _messageBusSerialiser.DeserialisedScheduledExecution(messageBody);

            if (execution == null)
            {
                _logger.LogError($"ReddeerRuleScheduler was unable to deserialise the message {messageId}");
            }

            if (execution?.Rules == null
                || !execution.Rules.Any())
            {
                return;
            }

            foreach (var rule in execution.Rules)
            {
                await ScheduleRule(rule, execution);
            }
        }

        private async Task ScheduleRule(Domain.Scheduling.Rules rule, ScheduledExecution execution)
        {
            var distributedExecution = new ScheduledExecution
            {
                Rules = new List<Domain.Scheduling.Rules> {rule},
                TimeSeriesInitiation = execution.TimeSeriesInitiation,
                TimeSeriesTermination = execution.TimeSeriesTermination
            };

            var serialisedDistributedExecution =
                _messageBusSerialiser.SerialiseScheduledExecution(distributedExecution);

            _logger.LogDebug($"Reddeer Smart Rule Scheduler - dispatching distribute message to queue - {serialisedDistributedExecution}");

            await _awsQueueClient.SendToQueue(
                _awsConfiguration.ScheduleRuleDistributedWorkQueueName,
                serialisedDistributedExecution,
                _messageBusCts.Token);
        }
    }
}
