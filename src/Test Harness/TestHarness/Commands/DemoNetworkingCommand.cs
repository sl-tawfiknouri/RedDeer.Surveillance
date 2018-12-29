using System;
using TestHarness.Commands.Interfaces;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Factory.Interfaces;

namespace TestHarness.Commands
{
    public class DemoNetworkingCommand : ICommand
    {
        private readonly IAppFactory _appFactory;
        private IEquityDataGenerator _equityProcess;
        private IOrderDataGenerator _tradingProcess;

        public DemoNetworkingCommand(IAppFactory appFactory)
        {
            _appFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            return
                string.Equals(command, "run demo networking", StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(command, "stop demo networking", StringComparison.InvariantCultureIgnoreCase);
        }

        public void Run(string command)
        {
            if (string.Equals(command, "run demo networking", StringComparison.InvariantCultureIgnoreCase))
            {
                Run();
            }

            if (string.Equals(command, "stop demo networking", StringComparison.InvariantCultureIgnoreCase))
            {
                Stop();
            }
        }

        private void Run()
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
                .Regular(TimeSpan.FromMilliseconds(1000 * 6))
                .Finish();

            var tradeStream =
                _appFactory
                .TradeOrderStreamFactory
                .CreateDisplayable(console);

            _tradingProcess =
                _appFactory
                .TradingFactory
                .Create()
                .Heartbeat()
                .Irregular(TimeSpan.FromMilliseconds(800), 8)
                .TradingFixedVolume(3)
                .FilterNone()
                .Finish();

            var spoofingTradeProcess = _appFactory
                .TradingSpoofingFactory
                .Create();

            var cancelledTradeProcess = _appFactory
                .TradingCancelledOrdersFactory
                .Create();

            // start updating equity data
            _equityProcess.InitiateWalk(equityStream);

            // start updating trading data
            _tradingProcess.InitiateTrading(equityStream, tradeStream);
            spoofingTradeProcess.InitiateTrading(equityStream, tradeStream);
            cancelledTradeProcess.InitiateTrading(equityStream, tradeStream);
        }

        private void Stop()
        {
            _tradingProcess?.TerminateTrading();

            _equityProcess?.TerminateWalk();
        }
    }
}