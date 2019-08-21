namespace TestHarness.Commands
{
    using System;

    using TestHarness.Commands.Interfaces;
    using TestHarness.Engine.EquitiesGenerator.Interfaces;
    using TestHarness.Engine.OrderGenerator.Interfaces;
    using TestHarness.Factory.Interfaces;

    public class DemoCommand : ICommand
    {
        private readonly IAppFactory _appFactory;

        private readonly object _lock = new object();

        private IEquityDataGenerator _equityProcess;

        private IOrderDataGenerator _tradingProcess;

        public DemoCommand(IAppFactory appFactory)
        {
            this._appFactory = appFactory;
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command)) return false;

            return string.Equals(command, "run demo", StringComparison.InvariantCultureIgnoreCase) || string.Equals(
                       command,
                       "stop demo",
                       StringComparison.InvariantCultureIgnoreCase);
        }

        public void Run(string command)
        {
            lock (this._lock)
            {
                if (string.Equals(command, "run demo", StringComparison.InvariantCultureIgnoreCase)) this.RunDemo();

                if (string.Equals(command, "stop demo", StringComparison.InvariantCultureIgnoreCase)) this.StopDemo();
            }
        }

        private void RunDemo()
        {
            var console = this._appFactory.Console;

            var equityStream = this._appFactory.StockExchangeStreamFactory.CreateDisplayable(console);

            this._equityProcess = this._appFactory.EquitiesProcessFactory.Create()
                .Regular(TimeSpan.FromMilliseconds(300)).Finish();

            var tradeStream = this._appFactory.TradeOrderStreamFactory.CreateDisplayable(console);

            this._tradingProcess = this._appFactory.TradingFactory.Create().MarketUpdate().TradingFixedVolume(2)
                .SetFilterNone().Finish();

            var spoofingTradeProcess = this._appFactory.TradingSpoofingFactory.Create();

            var cancelledTradeProcess = this._appFactory.TradingCancelledOrdersFactory.Create();

            // start updating equity data
            this._equityProcess.InitiateWalk(equityStream);

            // start updating trading data
            this._tradingProcess.InitiateTrading(equityStream, tradeStream);

            // start ad hoc heartbeat driven commands
            spoofingTradeProcess.InitiateTrading(equityStream, tradeStream);
            cancelledTradeProcess.InitiateTrading(equityStream, tradeStream);
        }

        private void StopDemo()
        {
            this._tradingProcess?.TerminateTrading();
            this._equityProcess?.TerminateWalk();
        }
    }
}