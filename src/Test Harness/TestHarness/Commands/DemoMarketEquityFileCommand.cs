﻿using System;
using System.IO;
using System.Linq;
using TestHarness.Commands.Interfaces;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Factory.Interfaces;

namespace TestHarness.Commands
{
    public class DemoMarketEquityFileCommand : ICommand
    {
        public static string FileDirectory = "Play Files";

        private readonly IAppFactory _appFactory;
        private IEquityDataGenerator _fileProcessor;

        public DemoMarketEquityFileCommand(IAppFactory appFactory)
        {
            _appFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            if (command.ToLower().Contains("stop demo equity market file"))
            {
                return true;
            }

            if (!command.ToLower().Contains("run demo equity market file"))
            {
                return false;
            }

            var fileSegment = command.ToLower().Replace("run demo equity market file ", string.Empty);
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

            var hasRunDemoMarketEquityFileCommandSegment = command.ToLower().Contains("run demo equity market file");
            if (hasRunDemoMarketEquityFileCommandSegment)
            {
                RunDemo(command);
            }

            var hasStopDemoMarketEquityFileCommandSegment = command.ToLower().Contains("stop demo equity market file");
            if (hasStopDemoMarketEquityFileCommandSegment)
            {
                StopDemo();
                return;
            }
        }

        private void RunDemo(string command)
        {
            var console = _appFactory.Console;
            var equityStream = _appFactory.StockExchangeStreamFactory.CreateDisplayable(console);
            var filePath = GetEquityFilePath(command);
            _fileProcessor = _appFactory.EquitiesFileRelayProcessFactory.Create(filePath);

            _fileProcessor.InitiateWalk(equityStream);
        }

        private string GetEquityFilePath(string command)
        {
            var fileSegment = command.ToLower().Replace("run demo equity market file ", string.Empty);
            fileSegment = fileSegment?.Trim();
            var playFileDirectory = Path.Combine(Directory.GetCurrentDirectory(), FileDirectory);
            var playFileFullPath = Path.Combine(playFileDirectory, fileSegment);

            return playFileFullPath;
        }

        private void StopDemo()
        {
            _fileProcessor?.TerminateWalk();
        }
    }
}
