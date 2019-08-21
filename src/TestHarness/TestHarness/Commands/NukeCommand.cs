namespace TestHarness.Commands
{
    using System;
    using System.Threading;

    using TestHarness.Commands.Interfaces;
    using TestHarness.Factory.Interfaces;

    public class NukeCommand : ICommand
    {
        private readonly IAppFactory _appFactory;

        public NukeCommand(IAppFactory appFactory)
        {
            this._appFactory = appFactory;
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command)) return false;

            if (this._appFactory.DisableNuke) return false;

            return string.Equals(command, "Nuke", StringComparison.InvariantCultureIgnoreCase);
        }

        public void Run(string command)
        {
            if (this._appFactory.DisableNuke) return;

            var cts = new CancellationTokenSource(TimeSpan.FromMinutes(3));

            var queueNuked = false;
            try
            {
                this._appFactory.AwsQueueClient.PurgeQueue(
                    this._appFactory.Configuration.ScheduledRuleQueueName,
                    cts.Token);
                this._appFactory.AwsQueueClient.PurgeQueue(
                    this._appFactory.Configuration.ScheduleRuleDistributedWorkQueueName,
                    cts.Token);
                this._appFactory.AwsQueueClient.PurgeQueue(
                    this._appFactory.Configuration.CaseMessageQueueName,
                    cts.Token);

                this._appFactory.Console.WriteToUserFeedbackLine("QUEUE NUKE SUCCESSFUL!");
                queueNuked = true;
            }

#pragma warning disable 168
            catch (Exception e)
#pragma warning restore 168
            {
            }

            try
            {
                this._appFactory.AuroraRepository.DeleteTradingAndMarketData();

                if (queueNuked)
                    this._appFactory.Console.WriteToUserFeedbackLine("QUEUE NUKE SUCCESSFUL! AURORA NUKE SUCCESSFUL!");
                else this._appFactory.Console.WriteToUserFeedbackLine("QUEUE NUKE FAILED! AURORA NUKE SUCCESSFUL!");
            }

#pragma warning disable 168
            catch (Exception e)
#pragma warning restore 168
            {
                var outputText = queueNuked
                                     ? "QUEUE NUKE SUCCESSFUL! AURORA NUKE FAILED!"
                                     : "QUEUE NUKE FAILED! AURORA NUKE FAILED!";

                this._appFactory.Console.WriteToUserFeedbackLine(outputText);
            }
        }
    }
}