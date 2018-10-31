using System;
using System.Globalization;
using System.IO;
using System.Linq;
using TestHarness.Commands.Interfaces;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Engine.EquitiesStorage.Interfaces;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Engine.OrderStorage.Interfaces;
using TestHarness.Factory.Interfaces;
using TestHarness.Network_IO.Interfaces;

namespace TestHarness.Commands
{
    public class DemoDataGenerationCommand : ICommand
    {
        public const string FileDirectory = "DataGenerationStorageMarket";
        public const string TradeFileDirectory = "DataGenerationStorageTrades";

        private readonly IAppFactory _appFactory;
        private INetworkManager _networkManager;
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
                    .TradingNormalDistributionVolume(4)
                    .FilterNone()
                    .Finish();

            _networkManager =
                _appFactory
                    .NetworkManagerFactory
                    .CreateWebsockets();

            var equitiesDirectory = Path.Combine(Directory.GetCurrentDirectory(), FileDirectory);

            _equitiesFileStorageProcess = _appFactory
                .EquitiesFileStorageProcessFactory
                .Create(equitiesDirectory);

            var tradeDirectory = Path.Combine(Directory.GetCurrentDirectory(), TradeFileDirectory);

             _orderFileStorageProcess = _appFactory
                .OrderFileStorageProcessFactory
                .Build(tradeDirectory);

            // start networking processes
            var connectionEstablished = _networkManager.InitiateAllNetworkConnections();

            if (!connectionEstablished)
            {
                console.WriteToUserFeedbackLine("Failed to establish network connections. Aborting run data generation.");
                return;
            }

            connectionEstablished = _networkManager.AttachTradeOrderSubscriberToStream(tradeStream);
            if (!connectionEstablished)
            {
                console.WriteToUserFeedbackLine("Failed to establish trade network connections. Aborting run data generation.");
                return;
            }

            connectionEstablished = _networkManager.AttachStockExchangeSubscriberToStream(equityStream);
            if (!connectionEstablished)
            {
                console.WriteToUserFeedbackLine("Failed to establish stock market network connections. Aborting run data generation.");
                return;
            }

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
            _networkManager?.TerminateAllNetworkConnections();
            _tradingProcess?.TerminateTrading();
            _equityProcess?.TerminateWalk();
        }
    }
}