using System;
using System.Threading;
using TestHarness.Commands.Interfaces;
using TestHarness.Factory.Interfaces;

namespace TestHarness.Commands
{
    public class NukeCommand : ICommand
    {
        private readonly IAppFactory _appFactory;

        public NukeCommand(IAppFactory appFactory)
        {
            _appFactory = appFactory;
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            if (_appFactory.DisableNuke)
            {
                return false;
            }

            return string.Equals(command, "Nuke", StringComparison.InvariantCultureIgnoreCase);
        }

        public void Run(string command)
        {
            if (_appFactory.DisableNuke)
            {
                return;
            }

            var cts = new CancellationTokenSource(TimeSpan.FromMinutes(3));

            var queueNuked = false;
            try
            {
                _appFactory.AwsQueueClient.PurgeQueue(_appFactory.Configuration.ScheduledRuleQueueName, cts.Token);
                _appFactory.AwsQueueClient.PurgeQueue(_appFactory.Configuration.ScheduleRuleDistributedWorkQueueName,
                    cts.Token);
                _appFactory.AwsQueueClient.PurgeQueue(_appFactory.Configuration.CaseMessageQueueName, cts.Token);

                _appFactory.Console.WriteToUserFeedbackLine("QUEUE NUKE SUCCESSFUL!");
                queueNuked = true;
            }
#pragma warning disable 168
            catch (Exception e)
#pragma warning restore 168
            {
                //
            }

            try
            {
                _appFactory.AuroraRepository.DeleteTradingAndMarketData();

                if (queueNuked)
                {
                    _appFactory.Console.WriteToUserFeedbackLine("QUEUE NUKE SUCCESSFUL! AURORA NUKE SUCCESSFUL!");
                }
                else
                {
                    _appFactory.Console.WriteToUserFeedbackLine("QUEUE NUKE FAILED! AURORA NUKE SUCCESSFUL!");
                }
            }
#pragma warning disable 168
            catch (Exception e)
#pragma warning restore 168
            {
                var outputText = queueNuked
                    ? "QUEUE NUKE SUCCESSFUL! AURORA NUKE FAILED!"
                    : "QUEUE NUKE FAILED! AURORA NUKE FAILED!";

                _appFactory.Console.WriteToUserFeedbackLine(outputText);
            }
        }
    }
}
