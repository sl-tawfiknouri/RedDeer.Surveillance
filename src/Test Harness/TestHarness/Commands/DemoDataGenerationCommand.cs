using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TestHarness.Commands.Interfaces;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Engine.EquitiesStorage.Interfaces;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Engine.OrderStorage.Interfaces;
using TestHarness.Factory.Interfaces;

namespace TestHarness.Commands
{
    public class DemoDataGenerationCommand : ICommand
    {
        public const string FileDirectory = "DataGenerationStorageMarket";
        public const string TradeFileDirectory = "DataGenerationStorageTrades";

        private readonly IAppFactory _appFactory;
        private IEquitiesDataGenerationMarkovProcess _equityProcess;
        private IOrderDataGenerator _tradingProcess;
        private IEquityDataStorage _equitiesFileStorageProcess;
        private IOrderFileStorageProcess _orderFileStorageProcess;

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

            return command.ToLower().Contains("run data generation") 
                   || command.ToLower().Contains("stop data generation");
        }

        public void Run(string command)
        {
            lock (_lock)
            {
                if (command.ToLower().Contains("run data generation"))
                {
                    _Run(command);
                }

                if (command.ToLower().Contains("stop data generation"))
                {
                    Stop();
                }
            }
        }

        private void _Run(string command)
        {
            // command splitter
            var console = _appFactory.Console;
            var apiRepository = _appFactory.SecurityApiRepository;
            var marketApiRepository = _appFactory.MarketApiRepository;
            
            var cmd = command.ToLower();
            cmd = cmd.Replace("run data generation", string.Empty).Trim();
            var splitCmd = cmd.Split(' ');

            var rawFromDate = splitCmd.FirstOrDefault();
            var rawToDate = splitCmd.Skip(1).FirstOrDefault();
            var market = splitCmd.Skip(2).FirstOrDefault();
            var trade = splitCmd.Skip(3).FirstOrDefault();
            var saveMarketCsv = splitCmd.Skip(4).FirstOrDefault();
            var saveTradeCsv = splitCmd.Skip(5).FirstOrDefault();

            var fromSuccess = DateTime.TryParse(rawFromDate, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out var fromDate);
            var toSuccess = DateTime.TryParse(rawToDate, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out var toDate);
            var marketSuccess = !string.IsNullOrWhiteSpace(market);
            var tradeSuccess =
                string.Equals(trade, "trades", StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(trade, "notrades", StringComparison.InvariantCultureIgnoreCase);
            var saveMarketCsvSuccess =
                string.Equals(saveMarketCsv, "marketcsv", StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(saveMarketCsv, "nomarketcsv", StringComparison.InvariantCultureIgnoreCase);
            var saveTradeCsvSuccess =
                string.Equals(saveTradeCsv, "tradecsv", StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(saveTradeCsv, "notradecsv", StringComparison.InvariantCultureIgnoreCase);


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

            if (!tradeSuccess)
            {
                console.WriteToUserFeedbackLine($"Did not understand value for whether to include trades. Options are 'trades' or 'notrades'. No spaces.");
                return;
            }

            if (!saveMarketCsvSuccess)
            {
                console.WriteToUserFeedbackLine($"Did not understand the save to csv value. Options are 'market' or 'nomarketcsv'. No spaces.");
                return;
            }

            if (!saveTradeCsvSuccess)
            {
                console.WriteToUserFeedbackLine($"Did not understand the save to csv value. Options are 'trade' or 'notradecsv'. No spaces.");
                return;
            }           

            var isHeartbeatingTask = apiRepository.Heartbeating();
            isHeartbeatingTask.Wait();

            if (!isHeartbeatingTask.Result)
            {
                console.WriteToUserFeedbackLine("Could not connect to the security api on the client service");
                return;
            }

            var priceApiTask = apiRepository.Get(fromDate, toDate, market);
            priceApiTask.Wait();
            var priceApiResult = priceApiTask.Result;

            if (priceApiResult == null
                || (!priceApiResult.SecurityPrices?.Any() ?? true))
            {
                console.WriteToUserFeedbackLine("Could not find any results on the security api for the provided query");
                return;
            }

            var marketApiHeartbeatTask = marketApiRepository.HeartBeating();
            marketApiHeartbeatTask.Wait();

            if (!marketApiHeartbeatTask.Result)
            {
                console.WriteToUserFeedbackLine("Could not connect to the market api on the client service");
                return;
            }

            var marketApiTask = marketApiRepository.Get();
            marketApiTask.Wait();
            var marketApiResult = marketApiTask.Result;

            if (marketApiResult == null
                || marketApiResult.Count == 0)
            {
                console.WriteToUserFeedbackLine("Could not find any results for the market api on the client service");
                return;
            }

            var marketData = marketApiResult.FirstOrDefault(ap =>
                string.Equals(ap.Code, market, StringComparison.InvariantCultureIgnoreCase));

            if (marketData == null)
            {
                console.WriteToUserFeedbackLine("Could not find any relevant results for the market api on the client service");
                return;
            }

            var equityStream =
                _appFactory
                    .StockExchangeStreamFactory
                    .CreateDisplayable(console);

            _equityProcess =
                _appFactory
                    .EquitiesDataGenerationProcessFactory
                    .Build();

            var tradeStream =
                _appFactory
                    .TradeOrderStreamFactory
                    .CreateDisplayable(console);

            _tradingProcess =
                _appFactory
                    .TradingFactory
                    .Create()
                    .MarketUpdate()
                    .TradingFixedVolume(10)
                    .FilterSedol(Ftse100SedolList())
                    .Finish();

            var equitiesDirectory = Path.Combine(Directory.GetCurrentDirectory(), FileDirectory);

            _equitiesFileStorageProcess = _appFactory
                .EquitiesFileStorageProcessFactory
                .Create(equitiesDirectory);

            var tradeDirectory = Path.Combine(Directory.GetCurrentDirectory(), TradeFileDirectory);

             _orderFileStorageProcess = _appFactory
                .OrderFileStorageProcessFactory
                .Build(tradeDirectory);


            if (string.Equals(trade, "trades", StringComparison.InvariantCultureIgnoreCase))
            {
                _tradingProcess.InitiateTrading(equityStream, tradeStream);
            }

            if (string.Equals(saveMarketCsv, "marketcsv", StringComparison.InvariantCultureIgnoreCase))
            {
                _equitiesFileStorageProcess.Initiate(equityStream);
            }

            if (string.Equals(saveTradeCsv, "tradecsv", StringComparison.InvariantCultureIgnoreCase))
            {
                tradeStream.Subscribe(_orderFileStorageProcess);
            }

            _equityProcess.InitiateWalk(equityStream, marketData, priceApiResult);
        }

        private void Stop()
        {
            _tradingProcess?.TerminateTrading();
            _equityProcess?.TerminateWalk();
        }

        // restrict test harness demo data to ftse 100 companies
        private IReadOnlyCollection<string> Ftse100SedolList()
        {
            return new List<string>
            {
                "B1YW440",
                "0673123",
                "B02J639",
                "B1XZS82",
                "0045614",
                "0053673",
                "0989529",
                "BVYVFW2",
                "0216238",
                "0263494",
                "3134865",
                "0081180",
                "B02L3W3",
                "BH0P3Z9",
                "0798059",
                "0287580",
                "0136701",
                "3091357",
                "B0744B3",
                "3174300",

                "3121522",
                "B033F22",
                "B9895B7",
                "BD6K457",
                "0182704",
                "BYZWX76",
                "0242493",
                "0237400",
                "BY9D0Y1",
                "B7KR2P8",
                "B71N6K8",
                "B19NLV4",
                "BFYFZP5",
                "B2QPKJ1",
                "0925288",
                "B4T3BW6",
                "B5VQMV6",
                "0405207",
                "B1VZ0M2",
                "B0LCW08",


                "BVZHXQ9",
                "0540528",
                "0454492",
                "BMJ6DW5",
                "BHJYC05",
                "3163836",
                "B5M6XQ7",
                "3398649",
                "BZ4BQC7",
                "3319521",
                "BYW0PQ6",
                "0560399",
                "0870612",
                "B0SWJX3",
                "3127489",
                "BZ1G432",
                "BD8YWM0",
                "B1CRLC4",
                "0604316",
                "BDR05C0",

                "3208986",
                "B7FC076",
                "B3MBS74",
                "BWXC0Z1",
                "0677608",
                "0682538",
                "0709954",
                "B03MLX2",
                "B03MM40",
                "B24CGK7",
                "B2B0DG9",
                "B082RF1",
                "BGDT3G2",
                "0718875",
                "B63H849",
                "B7T7721",
                "BKKMKR2",
                "B8C3BL0",
                "B019KW7",
                "0240549",


                "BLDYK61",
                "B5ZN1N8",
                "B1FH8J7",
                "0922320",
                "0822011",
                "B1WY233",
                "B1RR840",
                "BWFGQN1",
                "0790873",
                "0766937",
                "0408284",
                "BF8Q6K6",
                "0878230",
                "0884709",
                "B11LJN4",
                "B10RZP7",
                "B39J2M4",
                "BH4HKS3",
                "B1KJJ40",
                "B5N0P84",
                "B8KF9B4"
            };
        }
    }
}