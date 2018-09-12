﻿using System;
using System.IO;
using System.Linq;
using TestHarness.Commands.Interfaces;
using TestHarness.Factory.Interfaces;

namespace TestHarness.Commands
{
    public class DemoMarketEquityFileNetworkingCommand : ICommand
    {
        public static string FileDirectory = "Play Files";

        private readonly IAppFactory _appFactory;

        public DemoMarketEquityFileNetworkingCommand(IAppFactory appFactory)
        {
            _appFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            if (command.ToLower().Contains("stop demo equity market file networking"))
            {
                return true;
            }

            if (!command.ToLower().Contains("run demo equity market file networking"))
            {
                return false;
            }

            var fileSegment = command.ToLower().Replace("run demo equity market file networking ", string.Empty);
            fileSegment = fileSegment?.Trim();

            if (string.IsNullOrWhiteSpace(fileSegment))
            {
                return false;
            }

            var playFileDirectory = Path.Combine(Directory.GetCurrentDirectory(), FileDirectory);
            var playFileFullPath = Path.Combine(playFileDirectory, fileSegment);

            if (!File.Exists(playFileFullPath))
            {
                return false;
            }

            var fileExtension = playFileFullPath.Split('.').Reverse().FirstOrDefault();

            if (string.Equals(fileExtension, "csv", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public void Run(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return;
            }

            var hasRunDemoMarketEquityFileCommandSegment = command.ToLower().Contains("run demo equity market file networking");
            if (hasRunDemoMarketEquityFileCommandSegment)
            {
                RunDemo(command);
                return;
            }

            var hasStopDemoMarketEquityFileCommandSegment = command.ToLower().Contains("stop demo equity market file networking");
            if (hasStopDemoMarketEquityFileCommandSegment)
            {
                StopDemo();
                return;
            }
        }

        private void RunDemo(string command)
        {
        }

        private void StopDemo()
        {
        }
    }
}
