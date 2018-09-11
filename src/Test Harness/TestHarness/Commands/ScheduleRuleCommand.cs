using System;
using System.Collections.Generic;
using System.Threading;
using Domain.Scheduling;
using Domain.Scheduling.Interfaces;
using TestHarness.Commands.Interfaces;
using TestHarness.Configuration.Interfaces;
using TestHarness.Display;
using TestHarness.Factory.Interfaces;
using Utilities.Aws_IO.Interfaces;

namespace TestHarness.Commands
{
    /// <summary>
    /// Schedule rule execution for all rules between two dates
    /// </summary>
    public class ScheduleRuleCommand : ICommand
    {
        private readonly IConsole _console;
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IScheduledExecutionMessageBusSerialiser _serialiser;
        private readonly INetworkConfiguration _configuration;

        public ScheduleRuleCommand(IAppFactory appFactory)
        {
            _console = appFactory.Console;
            _awsQueueClient = appFactory.AwsQueueClient;
            _serialiser = appFactory.ScheduledExecutionSerialiser;
            _configuration = appFactory.Configuration;
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            return command.Contains("run schedule rule");
        }

        public void Run(string command)
        {
            if (!command.Contains("run schedule rule"))
            {
                return;
            }

            var removedCommand = command.Replace("run schedule rule", string.Empty).Trim();
            var dates = removedCommand.Split(' ');

            if (dates.Length != 2)
            {
                // write out error to screen
                _console.WriteToUserFeedbackLine($"Malformed command format at schedule rule command {command}. Expected 'run schedule rule 01/01/2018 12/01/2018'");

                return;
            }

            if (!DateTime.TryParse(dates[0], out var initialDate))
            {
                _console.WriteToUserFeedbackLine($"{initialDate} could not be parsed as a date");

                return;
            }

            if (!DateTime.TryParse(dates[1], out var terminationDate))
            {
                _console.WriteToUserFeedbackLine($"{terminationDate} could not be parsed as a date");

                return;
            }

            terminationDate = OffsetTerminationDate(terminationDate);
            var allRulesList = GetAllRules();

            var scheduledExecution = new ScheduledExecution
            {
                Rules = allRulesList,
                TimeSeriesInitiation = initialDate,
                TimeSeriesTermination = terminationDate
            };

            var message = _serialiser.SerialiseScheduledExecution(scheduledExecution);
            var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
            _awsQueueClient.SendToQueue(_configuration.ScheduledRuleQueueName, message, cts.Token);
        }

        private DateTime OffsetTerminationDate(DateTime terminationDate)
        {
            terminationDate = terminationDate.AddDays(1);
            terminationDate = terminationDate.AddMilliseconds(-1);

            return terminationDate;
        }

        private List<Rules> GetAllRules()
        {
            var allRules = Enum.GetValues(typeof(Rules));
            var allRulesList = new List<Rules>();

            foreach (var item in allRules)
            {
                allRulesList.Add((Rules)item);
            }

            return allRulesList;
        }
    }
}