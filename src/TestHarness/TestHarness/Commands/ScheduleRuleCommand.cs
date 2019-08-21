namespace TestHarness.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Domain.Surveillance.Scheduling;
    using Domain.Surveillance.Scheduling.Interfaces;

    using Infrastructure.Network.Aws.Interfaces;

    using TestHarness.Commands.Interfaces;
    using TestHarness.Configuration.Interfaces;
    using TestHarness.Display.Interfaces;
    using TestHarness.Factory.Interfaces;

    /// <summary>
    ///     Schedule rule execution for all rules between two dates
    /// </summary>
    public class ScheduleRuleCommand : ICommand
    {
        private readonly IAwsQueueClient _awsQueueClient;

        private readonly INetworkConfiguration _configuration;

        private readonly IConsole _console;

        private readonly IScheduledExecutionMessageBusSerialiser _serialiser;

        public ScheduleRuleCommand(IAppFactory appFactory)
        {
            this._console = appFactory.Console;
            this._awsQueueClient = appFactory.AwsQueueClient;
            this._serialiser = appFactory.ScheduledExecutionSerialiser;
            this._configuration = appFactory.Configuration;
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command)) return false;

            return command.Contains("run schedule rule");
        }

        public void Run(string command)
        {
            if (!command.Contains("run schedule rule")) return;

            var removedCommand = command.Replace("run schedule rule", string.Empty).Trim();
            var dates = removedCommand.Split(' ');

            if (dates.Length != 2)
            {
                // write out error to screen
                this._console.WriteToUserFeedbackLine(
                    $"Malformed command format at schedule rule command {command}. Expected 'run schedule rule 01/01/2018 12/01/2018'");

                return;
            }

            if (!DateTime.TryParse(dates[0], out var initialDate))
            {
                this._console.WriteToUserFeedbackLine($"{initialDate} could not be parsed as a date");

                return;
            }

            if (!DateTime.TryParse(dates[1], out var terminationDate))
            {
                this._console.WriteToUserFeedbackLine($"{terminationDate} could not be parsed as a date");

                return;
            }

            terminationDate = this.OffsetTerminationDate(terminationDate);

            if (initialDate > terminationDate)
            {
                this._console.WriteToUserFeedbackLine("Initiation precedes Termination date. Invalid input.");

                return;
            }

            initialDate = DateTime.SpecifyKind(initialDate, DateTimeKind.Utc);
            terminationDate = DateTime.SpecifyKind(terminationDate, DateTimeKind.Utc);

            var allRulesList = this.GetAllRules();

            var scheduledExecution = new ScheduledExecution
                                         {
                                             Rules = allRulesList,
                                             TimeSeriesInitiation = initialDate,
                                             TimeSeriesTermination = terminationDate
                                         };

            var message = this._serialiser.SerialiseScheduledExecution(scheduledExecution);
            var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
            this._awsQueueClient.SendToQueue(this._configuration.ScheduledRuleQueueName, message, cts.Token);
        }

        private List<RuleIdentifier> GetAllRules()
        {
            var allRules = Enum.GetValues(typeof(Rules));
            var allRulesList = new List<Rules>();

            foreach (var item in allRules) allRulesList.Add((Rules)item);

            return allRulesList.Select(arl => new RuleIdentifier { Rule = arl, Ids = new string[0] }).ToList();
        }

        private DateTime OffsetTerminationDate(DateTime terminationDate)
        {
            terminationDate = terminationDate.AddDays(1);
            terminationDate = terminationDate.AddMilliseconds(-1);

            return terminationDate;
        }
    }
}