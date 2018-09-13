using System;
using System.IO;
using System.Linq;
using TestHarness.Commands.Interfaces;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Factory.Interfaces;
using TestHarness.Network_IO.Interfaces;

namespace TestHarness.Commands
{
    public class DemoTradeFileNetworkingCommand : ICommand
    {
        public static string FileDirectory = "Play Trade Files";

        private readonly IAppFactory _appFactory;
        private IOrderDataGenerator _tradeFileProcessor;
        private INetworkManager _networkManager;

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

            command = command.ToLowerInvariant();

            var hasStopDemoTradeFileCommandSegment = command.Contains("stop demo trade networking file");
            if (hasStopDemoTradeFileCommandSegment)
            {
                return true;
            }

            var hasDemoTradeFileCommandSegment = command.Contains("run demo trade networking file");
            if (!hasDemoTradeFileCommandSegment)
            {
                return false;
            }

            var fileSegment = command.Replace("run demo trade networking file ", string.Empty);
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

            var prohibitedTradeProcess = _appFactory
                .TradingProhibitedSecurityFactory
                .Create();

            var spoofingTradeProcess = _appFactory
                .TradingSpoofingFactory
                .Create();

            var equityStream = _appFactory.StockExchangeStreamFactory.Create();

            _networkManager =
                _appFactory
                    .NetworkManagerFactory
                    .CreateWebsockets();

            // start networking processes
            var connectionEstablished = _networkManager.InitiateAllNetworkConnections();

            if (!connectionEstablished)
            {
                console.WriteToUserFeedbackLine("Failed to establish network connections. Aborting run demo networking.");
                return;
            }

            connectionEstablished = _networkManager.AttachTradeOrderSubscriberToStream(tradeStream);

            if (!connectionEstablished)
            {
                console.WriteToUserFeedbackLine("Failed to establish trade network connections. Aborting run demo networking.");
                return;
            }

            connectionEstablished = _networkManager.AttachStockExchangeSubscriberToStream(equityStream);

            if (!connectionEstablished)
            {
                console.WriteToUserFeedbackLine("Failed to establish stock market network connections. Aborting run demo networking.");
                return;
            }

            // start updating trading data
            _tradeFileProcessor.InitiateTrading(equityStream, tradeStream);

            // start ad hoc heartbeat driven commands
            prohibitedTradeProcess.InitiateTrading(equityStream, tradeStream);
            spoofingTradeProcess.InitiateTrading(equityStream, tradeStream);
        }

        private void SetTradingFileProcessor(string command)
        {
            var fileSegment = command.Replace("run demo trade networking file ", string.Empty);
            fileSegment = fileSegment?.Trim();
            var playFileDirectory = Path.Combine(Directory.GetCurrentDirectory(), FileDirectory);
            var playFileFullPath = Path.Combine(playFileDirectory, fileSegment);

            _tradeFileProcessor = _appFactory.TradingFileRelayProcessFactory.Build(playFileFullPath);
        }

        private void StopDemo()
        {
            _tradeFileProcessor?.TerminateTrading();
            _networkManager?.TerminateAllNetworkConnections();
        }
    }
}