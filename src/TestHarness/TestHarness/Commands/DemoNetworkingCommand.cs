namespace TestHarness.Commands
{
    using System;

    using TestHarness.Commands.Interfaces;
    using TestHarness.Engine.EquitiesGenerator.Interfaces;
    using TestHarness.Engine.OrderGenerator.Interfaces;
    using TestHarness.Factory.Interfaces;

    public class DemoNetworkingCommand : ICommand
    {
        private readonly IAppFactory _appFactory;

        private IEquityDataGenerator _equityProcess;

        private IOrderDataGenerator _tradingProcess;

        public DemoNetworkingCommand(IAppFactory appFactory)
        {
            this._appFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command)) return false;

            return string.Equals(command, "run demo networking", StringComparison.InvariantCultureIgnoreCase)
                   || string.Equals(command, "stop demo networking", StringComparison.InvariantCultureIgnoreCase);
        }

        public void Run(string command)
        {
            if (string.Equals(command, "run demo networking", StringComparison.InvariantCultureIgnoreCase)) this.Run();

            if (string.Equals(
                command,
                "stop demo networking",
                StringComparison.InvariantCultureIgnoreCase)) this.Stop();
        }

        private void Run()
        {
            var console = this._appFactory.Console;

            var equityStream = this._appFactory.StockExchangeStreamFactory.CreateDisplayable(console);

            this._equityProcess = this._appFactory.EquitiesProcessFactory.Create()
                .Regular(TimeSpan.FromMilliseconds(1000 * 6)).Finish();

            var tradeStream = this._appFactory.TradeOrderStreamFactory.CreateDisplayable(console);

            this._tradingProcess = this._appFactory.TradingFactory.Create().Heartbeat()
                .Irregular(TimeSpan.FromMilliseconds(800), 8).TradingFixedVolume(3).SetFilterNone().Finish();

            var spoofingTradeProcess = this._appFactory.TradingSpoofingFactory.Create();

            var cancelledTradeProcess = this._appFactory.TradingCancelledOrdersFactory.Create();

            // start updating equity data
            this._equityProcess.InitiateWalk(equityStream);

            // start updating trading data
            this._tradingProcess.InitiateTrading(equityStream, tradeStream);
            spoofingTradeProcess.InitiateTrading(equityStream, tradeStream);
            cancelledTradeProcess.InitiateTrading(equityStream, tradeStream);
        }

        private void Stop()
        {
            this._tradingProcess?.TerminateTrading();

            this._equityProcess?.TerminateWalk();
        }
    }
}