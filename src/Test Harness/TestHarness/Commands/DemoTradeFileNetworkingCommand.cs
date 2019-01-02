using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TestHarness.Commands.Interfaces;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Factory.Interfaces;

namespace TestHarness.Commands
{
    public class DemoTradeFileNetworkingCommand : ICommand
    {
        public static string FileDirectory = "PlayTradeFiles";

        private readonly IAppFactory _appFactory;
        private IOrderDataGenerator _tradeFileProcessor;
        public DemoTradeFileNetworkingCommand(IAppFactory appFactory)
        {
            _appFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            var hasStopDemoTradeFileCommandSegment = command.ToLowerInvariant().Contains("stop demo trade networking file");
            if (hasStopDemoTradeFileCommandSegment)
            {
                return true;
            }

            var hasDemoTradeFileCommandSegment = command.ToLowerInvariant().Contains("run demo trade networking file");
            if (!hasDemoTradeFileCommandSegment)
            {
                return false;
            }

            var fileSegment = Regex.Replace(command, "run demo trade networking file ", string.Empty, RegexOptions.IgnoreCase);
            fileSegment = fileSegment.Trim();

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

            var fileExtension = playFileFullPath.Split('.').Reverse().First();

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

            var hasDemoTradeFileCommandSegment = command.Contains("run demo trade networking file");
            if (hasDemoTradeFileCommandSegment)
            {
                RunDemo(command);
            }

            var hasStopDemoTradeFileCommandSegment = command.Contains("stop demo trade networking file");
            if (hasStopDemoTradeFileCommandSegment)
            {
                StopDemo();
            }
        }

        private void RunDemo(string command)
        {
            SetTradingFileProcessor(command);
            var console = _appFactory.Console;
            
            var tradeStream =
                _appFactory
                    .TradeOrderStreamFactory
                    .CreateDisplayable(console);

            var spoofingTradeProcess = _appFactory
                .TradingSpoofingFactory
                .Create();

            var cancelledTradeProcess = _appFactory
                .TradingCancelledOrdersFactory
                .Create();

            var equityStream = _appFactory.StockExchangeStreamFactory.Create();

            // start updating trading data
            _tradeFileProcessor.InitiateTrading(equityStream, tradeStream);

            // start ad hoc heartbeat driven commands
            spoofingTradeProcess.InitiateTrading(equityStream, tradeStream);
            cancelledTradeProcess.InitiateTrading(equityStream, tradeStream);
        }

        private void SetTradingFileProcessor(string command)
        {
            var fileSegment = Regex.Replace(command, "run demo trade networking file ", string.Empty, RegexOptions.IgnoreCase);
            fileSegment = fileSegment.Trim();
            var playFileDirectory = Path.Combine(Directory.GetCurrentDirectory(), FileDirectory);
            var playFileFullPath = Path.Combine(playFileDirectory, fileSegment);

            _tradeFileProcessor = _appFactory.TradingFileDataImportProcessFactory.Build(playFileFullPath);
        }

        private void StopDemo()
        {
            _tradeFileProcessor?.TerminateTrading();
        }
    }
}