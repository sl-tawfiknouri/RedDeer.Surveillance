﻿using System;
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

namespace TestHarness.Commands.Market_Abuse_Commands
{
    public class Spoofing2Command : ICommand
    {
        public const string FileDirectory = "DataGenerationStorageMarketSpoofingCmd";
        public const string TradeFileDirectory = "DataGenerationStorageTradesSpoofingCmd";

        private readonly object _lock = new object();
        private readonly IAppFactory _appFactory;

        private INetworkManager _networkManager;
        private IEquitiesDataGenerationMarkovProcess _equityProcess;
        private IOrderDataGenerator _tradingProcess;
        private IEquityDataStorage _equitiesFileStorageProcess;
        private IOrderFileStorageProcess _orderFileStorageProcess;


        public Spoofing2Command(IAppFactory appFactory)
        {
            _appFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            return command.ToLower().Contains("run spoofing trades");
        }

        public void Run(string command)
        {
            lock (_lock)
            {
                var console = _appFactory.Console;
                var apiRepository = _appFactory.SecurityApiRepository;
                var marketApiRepository = _appFactory.MarketApiRepository;

                var cmd = command.ToLower();
                cmd = cmd.Replace("run spoofing trades", string.Empty).Trim();
                var splitCmd = cmd.Split(' ');

                var rawFromDate = splitCmd.FirstOrDefault();
                var market = splitCmd.Skip(1).FirstOrDefault();
                var trades = splitCmd.Skip(2).FirstOrDefault();
                var saveMarketCsv = splitCmd.Skip(3).FirstOrDefault();
                var saveTradeCsv = splitCmd.Skip(4).FirstOrDefault();
                var sedols = splitCmd.Skip(5).ToList();

                var fromSuccess = DateTime.TryParse(rawFromDate, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out var fromDate);
                var tradesSuccess =
                    string.Equals(trades, "trade", StringComparison.InvariantCultureIgnoreCase)
                    || string.Equals(trades, "notrade", StringComparison.InvariantCultureIgnoreCase);
                var marketSuccess = !string.IsNullOrWhiteSpace(market);
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

                if (!marketSuccess)
                {
                    console.WriteToUserFeedbackLine($"Did not understand market of {market}");
                    return;
                }

                if (!tradesSuccess)
                {
                    console.WriteToUserFeedbackLine($"Did not understand trades of {trades}. Can be 'trade' or 'notrade'");
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

                if (!sedols.Any())
                {
                    console.WriteToUserFeedbackLine($"Did not understand any of the sedol arguments provided");
                    return;
                }

                var isHeartbeatingTask = apiRepository.Heartbeating();
                isHeartbeatingTask.Wait();

                if (!isHeartbeatingTask.Result)
                {
                    console.WriteToUserFeedbackLine("Could not connect to the security api on the client service");
                    return;
                }

                var priceApiTask = apiRepository.Get(fromDate, fromDate.Date.AddDays(1).AddSeconds(-1), market);
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

                var auroraRepository = _appFactory.AuroraRepository;
                auroraRepository.DeleteTradingAndMarketDataForMarketOnDate(market, fromDate);

                var equitiesDirectory = Path.Combine(Directory.GetCurrentDirectory(), FileDirectory);

                _equitiesFileStorageProcess = _appFactory
                    .EquitiesFileStorageProcessFactory
                    .Create(equitiesDirectory);

                var tradeDirectory = Path.Combine(Directory.GetCurrentDirectory(), TradeFileDirectory);

                _orderFileStorageProcess = _appFactory
                    .OrderFileStorageProcessFactory
                    .Build(tradeDirectory);

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
                        .FilterSedol(sedols)
                        .Finish();

                var spoofingProcess =
                    _appFactory
                        .SpoofingV2Factory
                        .Build(sedols);

                _networkManager =
                    _appFactory
                        .NetworkManagerFactory
                        .CreateWebsockets();

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

                equityStream.Subscribe(spoofingProcess);
                spoofingProcess.InitiateTrading(equityStream, tradeStream);

                if (string.Equals(trades, "trade", StringComparison.InvariantCultureIgnoreCase))
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
        }
    }
}