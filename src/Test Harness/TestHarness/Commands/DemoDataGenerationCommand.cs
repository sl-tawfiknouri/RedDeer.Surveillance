using System;
using System.Linq;
using TestHarness.Commands.Interfaces;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Factory.Interfaces;

namespace TestHarness.Commands
{
    public class DemoDataGenerationCommand : ICommand
    {
        private readonly IAppFactory _appFactory;
        private IEquityDataGenerator _equityProcess;
        private IOrderDataGenerator _tradingProcess;

        private readonly object _lock = new object();

        public DemoDataGenerationCommand(IAppFactory appFactory)
        {
            _appFactory = appFactory;
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            return command.ToLower().Contains("run data generation");
        }

        public void Run(string command)
        {
            lock (_lock)
            {
                // command splitter
                var console = _appFactory.Console;

                var cmd = command.ToLower();
                cmd = cmd.Replace("run data generation", string.Empty);
                var splitCmd = cmd.Split(' ');

                var rawFromDate = (splitCmd.Take(1)).FirstOrDefault();
                var rawToDate = splitCmd.Skip(1).Take(1).FirstOrDefault();
                var market = splitCmd.Skip(2).Take(1).FirstOrDefault();

                var fromSuccess = DateTime.TryParse(rawFromDate, out var fromDate);
                var toSuccess = DateTime.TryParse(rawToDate, out var toDate);
                var marketSuccess = !string.IsNullOrWhiteSpace(market);

                if (!fromSuccess)
                {
                    console.WriteToUserFeedbackLine($"Did not understand from date of {rawFromDate}");
                    return;
                }

                if (!toSuccess)
                {
                    console.WriteToUserFeedbackLine($"Did not understand to date of {rawToDate}");
                    return;
                }

                if (!marketSuccess)
                {
                    console.WriteToUserFeedbackLine($"Did not understand market of {market}");
                    return;
                }


                // heartbeat API to check



                // if heart beated, go and make the real request =)






                var equityStream =
                    _appFactory
                        .StockExchangeStreamFactory
                        .CreateDisplayable(console);

                // this is what we will be replacing
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

                _tradingProcess =
                    _appFactory
                        .TradingFactory
                        .Create()
                        .MarketUpdate()
                        .TradingFixedVolume(2)
                        .Finish();

                _tradingProcess.InitiateTrading(equityStream, tradeStream);
            }
        }
    }
}
