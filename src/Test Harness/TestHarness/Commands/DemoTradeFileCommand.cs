using System;
using System.IO;
using System.Linq;
using TestHarness.Commands.Interfaces;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Factory.Interfaces;

namespace TestHarness.Commands
{
    public class DemoTradeFileCommand : ICommand
    {
        public static string FileDirectory = "PlayTradeFiles";

        private readonly IAppFactory _appFactory;
        private IOrderDataGenerator _tradeFileProcessor;

        public DemoTradeFileCommand(IAppFactory appFactory)
        {
            _appFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            command = command.ToLowerInvariant();

            var hasStopDemoTradeFileCommandSegment = command.Contains("stop demo trade file");
            if (hasStopDemoTradeFileCommandSegment)
            {
                return true;
            }

            var hasDemoTradeFileCommandSegment = command.Contains("run demo trade file");
            if (!hasDemoTradeFileCommandSegment)
            {
                return false;
            }

            var fileSegment = command.Replace("run demo trade file ", string.Empty);
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

            var hasDemoTradeFileCommandSegment = command.ToLower().Contains("run demo trade file");
            if (hasDemoTradeFileCommandSegment)
            {
                RunDemo(command);
                return;
            }

            var hasStopDemoTradeFileCommandSegment = command.ToLower().Contains("stop demo trade file");
            if (hasStopDemoTradeFileCommandSegment)
            {
                StopDemo();
                return;
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

            var prohibitedTradeProcess = _appFactory
                .TradingProhibitedSecurityFactory
                .Create();

            var spoofingTradeProcess = _appFactory
                .TradingSpoofingFactory
                .Create();

            var equityStream = _appFactory.StockExchangeStreamFactory.Create();

            // start updating trading data
            _tradeFileProcessor.InitiateTrading(equityStream, tradeStream);

            // start ad hoc heartbeat driven commands
            prohibitedTradeProcess.InitiateTrading(equityStream, tradeStream);
            spoofingTradeProcess.InitiateTrading(equityStream, tradeStream);
        }

        private void SetTradingFileProcessor(string command)
        {
            var fileSegment = command.ToLower().Replace("run demo trade file ", string.Empty);
            fileSegment = fileSegment?.Trim();
            var playFileDirectory = Path.Combine(Directory.GetCurrentDirectory(), FileDirectory);
            var playFileFullPath = Path.Combine(playFileDirectory, fileSegment);

            _tradeFileProcessor = _appFactory.TradingFileRelayProcessFactory.Build(playFileFullPath);
        }

        private void StopDemo()
        {
            _tradeFileProcessor?.TerminateTrading();
        }
    }
}