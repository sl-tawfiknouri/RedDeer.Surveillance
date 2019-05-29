using System;
using System.IO;
using TestHarness.Commands.Interfaces;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Engine.EquitiesStorage.Interfaces;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Factory.Interfaces;

namespace TestHarness.Commands
{
    public class DemoCsvCommand : ICommand
    {
        public const string FileDirectory = "EquitiesFilesStorage";

        private readonly IAppFactory _appFactory;
        private IEquityDataGenerator _equityProcess;
        private IOrderDataGenerator _tradingProcess;
        private IEquityDataStorage _equitiesFileStorageProcess;

        private readonly object _lock = new object();

        public DemoCsvCommand(IAppFactory appFactory)
        {
            _appFactory = appFactory;
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            return
                string.Equals(command, "run demo csv", StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(command, "stop demo csv", StringComparison.InvariantCultureIgnoreCase);
        }

        public void Run(string command)
        {
            lock (_lock)
            {
                if (string.Equals(command, "run demo csv", StringComparison.InvariantCultureIgnoreCase))
                {
                    RunDemo();
                }

                if (string.Equals(command, "stop demo csv", StringComparison.InvariantCultureIgnoreCase))
                {
                    StopDemo();
                }
            }
        }

        private void RunDemo()
        {
            var console = _appFactory.Console;

            var equityStream = 
                _appFactory
                .StockExchangeStreamFactory
                .CreateDisplayable(console);

            _equityProcess = 
                _appFactory
                .EquitiesProcessFactory
                .Create()
                .Regular(TimeSpan.FromMilliseconds(10 * 1000)) // ?? 
                .Finish();

            var tradeStream = 
                _appFactory
                .TradeOrderStreamFactory
                .CreateDisplayable(console);

            _tradingProcess =
                _appFactory
                .TradingFactory
                .Create()
                .MarketUpdate()
                .TradingFixedVolume(2)
                .SetFilterNone()
                .Finish();

            var spoofingTradeProcess = _appFactory
                .TradingSpoofingFactory
                .Create();

            var cancelledTradeProcess = _appFactory
                .TradingCancelledOrdersFactory
                .Create();

            var directory = Path.Combine(Directory.GetCurrentDirectory(), FileDirectory);

            _equitiesFileStorageProcess =_appFactory
                .EquitiesFileStorageProcessFactory
                .Create(directory);
            
            // start saving equity data
            _equitiesFileStorageProcess.Initiate(equityStream);

            // start updating equity data
            _equityProcess.InitiateWalk(equityStream);

            // start updating trading data
            _tradingProcess.InitiateTrading(equityStream, tradeStream);

            // start ad hoc heartbeat driven commands
            spoofingTradeProcess.InitiateTrading(equityStream, tradeStream);
            cancelledTradeProcess.InitiateTrading(equityStream, tradeStream);
        }

        private void StopDemo()
        {
            _tradingProcess?.TerminateTrading();
            _equityProcess?.TerminateWalk();
            _equitiesFileStorageProcess?.Terminate();
        }
    }
}
