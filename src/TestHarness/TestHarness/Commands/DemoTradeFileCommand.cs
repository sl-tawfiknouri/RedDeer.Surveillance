namespace TestHarness.Commands
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using TestHarness.Commands.Interfaces;
    using TestHarness.Engine.OrderGenerator.Interfaces;
    using TestHarness.Factory.Interfaces;

    public class DemoTradeFileCommand : ICommand
    {
        public static string FileDirectory = "PlayTradeFiles";

        private readonly IAppFactory _appFactory;

        private IOrderDataGenerator _tradeFileProcessor;

        public DemoTradeFileCommand(IAppFactory appFactory)
        {
            this._appFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command)) return false;

            var hasStopDemoTradeFileCommandSegment = command.ToLowerInvariant().Contains("stop demo trade file");
            if (hasStopDemoTradeFileCommandSegment) return true;

            var hasDemoTradeFileCommandSegment = command.ToLowerInvariant().Contains("run demo trade file");
            if (!hasDemoTradeFileCommandSegment) return false;

            var fileSegment = Regex.Replace(command, "run demo trade file ", string.Empty, RegexOptions.IgnoreCase);
            fileSegment = fileSegment.Trim();

            if (string.IsNullOrWhiteSpace(fileSegment)) return false;

            var playFileDirectory = Path.Combine(Directory.GetCurrentDirectory(), FileDirectory);
            var playFileFullPath = Path.Combine(playFileDirectory, fileSegment);

            if (!File.Exists(playFileFullPath)) return false;

            var fileExtension = playFileFullPath.Split('.').Reverse().FirstOrDefault();

            if (string.Equals(fileExtension, "csv", StringComparison.InvariantCultureIgnoreCase)) return true;

            return false;
        }

        public void Run(string command)
        {
            if (string.IsNullOrWhiteSpace(command)) return;

            var hasDemoTradeFileCommandSegment = command.ToLower().Contains("run demo trade file");
            if (hasDemoTradeFileCommandSegment)
            {
                this.RunDemo(command);
                return;
            }

            var hasStopDemoTradeFileCommandSegment = command.ToLower().Contains("stop demo trade file");
            if (hasStopDemoTradeFileCommandSegment) this.StopDemo();
        }

        private void RunDemo(string command)
        {
            this.SetTradingFileProcessor(command);
            var console = this._appFactory.Console;

            var tradeStream = this._appFactory.TradeOrderStreamFactory.CreateDisplayable(console);

            var spoofingTradeProcess = this._appFactory.TradingSpoofingFactory.Create();

            var cancelledTradeProcess = this._appFactory.TradingCancelledOrdersFactory.Create();

            var equityStream = this._appFactory.StockExchangeStreamFactory.Create();

            // start updating trading data
            this._tradeFileProcessor.InitiateTrading(equityStream, tradeStream);

            // start ad hoc heartbeat driven commands
            spoofingTradeProcess.InitiateTrading(equityStream, tradeStream);
            cancelledTradeProcess.InitiateTrading(equityStream, tradeStream);
        }

        private void SetTradingFileProcessor(string command)
        {
            var fileSegment = Regex.Replace(command, "run demo trade file ", string.Empty, RegexOptions.IgnoreCase);
            fileSegment = fileSegment.Trim();
            var playFileDirectory = Path.Combine(Directory.GetCurrentDirectory(), FileDirectory);
            var playFileFullPath = Path.Combine(playFileDirectory, fileSegment);

            this._tradeFileProcessor = this._appFactory.TradingFileDataImportProcessFactory.Build(playFileFullPath);
        }

        private void StopDemo()
        {
            this._tradeFileProcessor?.TerminateTrading();
        }
    }
}