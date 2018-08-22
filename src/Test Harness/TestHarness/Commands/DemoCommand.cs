using System;
using TestHarness.Commands.Interfaces;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Factory.Interfaces;

namespace TestHarness.Commands
{
    public class DemoCommand : ICommand
    {
        private IAppFactory _appFactory;
        private IEquityDataGenerator _equityProcess;
        private IOrderDataGenerator _tradeProcess;

        private object _lock = new object();

        public DemoCommand(IAppFactory appFactory)
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
                string.Equals(command, "run demo", StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(command, "stop demo", StringComparison.InvariantCultureIgnoreCase);
        }

        public void Run(string command)
        {
            lock (_lock)
            {
                if (string.Equals(command, "run demo", StringComparison.InvariantCultureIgnoreCase))
                {
                    RunDemo();
                }

                if (string.Equals(command, "stop demo", StringComparison.InvariantCultureIgnoreCase))
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
                .Regular(TimeSpan.FromMilliseconds(300))
                .Finish();

            var tradeStream = 
                _appFactory
                .TradeOrderStreamFactory
                .CreateDisplayable(console);

            _tradeProcess =
                _appFactory
                .TradingFactory
                .Create()
                .Heartbeat()
                .Irregular(TimeSpan.FromMilliseconds(600), 8)
                .TradingFixedVolume(3)
                .Finish();

            // start updating equity data
            _equityProcess.InitiateWalk(equityStream);

            // start updating trading data
            _tradeProcess.InitiateTrading(equityStream, tradeStream);
        }

        private void StopDemo()
        {
            if (_tradeProcess != null)
            {
                _tradeProcess.TerminateTrading();
            }

            if (_equityProcess != null)
            {
                _equityProcess.TerminateWalk();
            }
        }
    }
}
