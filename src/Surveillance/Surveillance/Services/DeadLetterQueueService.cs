using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using Surveillance.Services.Interfaces;
using Utilities.Aws_IO.Interfaces;
using Timer = System.Timers.Timer;

namespace Surveillance.Services
{
    /// <summary>
    /// This service will check the dead letter queue and raise exceptions if it has any messages
    /// </summary>
    public class DeadLetterQueueService : IDeadLetterQueueService
    {
        private CancellationTokenSource _messageBusCts;
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private const int DeadLetterQueueCheckInMinutesFrequency = 15;

        private List<Timer> _timers;
        private readonly ILogger<DeadLetterQueueService> _logger;

        public DeadLetterQueueService(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            ILogger<DeadLetterQueueService> logger)
        {
            _awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            _awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            _timers = new List<Timer>();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initialise()
        {
            _messageBusCts = new CancellationTokenSource(60*1000);

            var distributeRuleQueue = _awsConfiguration.ScheduleRuleDistributedWorkDeadLetterQueueName;
            var scheduleRuleQueue = _awsConfiguration.ScheduledRuleDeadLetterQueueName;
            var caseMessageQueue = _awsConfiguration.CaseMessageDeadLetterQueueName;

            InitialiseForQueue(distributeRuleQueue);
            InitialiseForQueue(scheduleRuleQueue);
            InitialiseForQueue(caseMessageQueue);
        }

        private void InitialiseForQueue(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogError("DeadLetterQueueService asked to monitor a blank or empty queue name");
                return;
            }

            var timer = new Timer(DeadLetterQueueCheckInMinutesFrequency * 60 * 1000)
            {
                AutoReset = true,
                Interval = DeadLetterQueueCheckInMinutesFrequency * 60 * 1000
            };

            timer.Elapsed += (s,e) => TimerOnElapsed(s, e, name);
            timer.Start();

            _timers.Add(timer);
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e, string name)
        {
            try
            {
                var cancellationToken = new CancellationTokenSource(60 * 1000);
                var queueMessageCountTask = _awsQueueClient.QueueMessageCount(name, cancellationToken.Token);
                queueMessageCountTask.Wait(cancellationToken.Token);

                var result = queueMessageCountTask.Result;

                if (result != 0)
                {
                    _logger.LogError($"DeadLetterQueueService detected a letter on the queue {name}");
                }
            }
            catch (Exception a)
            {
                _logger.LogError("DeadLetterQueueService encountered an error ", a);
            }
        }

        public void Terminate()
        {
            _messageBusCts?.Cancel();
            _messageBusCts = null;

            if (_timers == null)
            {
                return;
            }

            foreach (var item in _timers)
            {
                item?.Stop();
            }

            _timers = new List<Timer>();
        }
    }
}
