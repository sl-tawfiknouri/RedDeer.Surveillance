﻿namespace TestHarness.Commands.Market_Abuse_Commands
{
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

    public class HighVolumeCommand : ICommand
    {
        public const string FileDirectory = "DataGenerationStorageMarketHighVolumeCmd";

        public const string TradeFileDirectory = "DataGenerationStorageTradesHighVolumeCmd";

        private readonly IAppFactory _appFactory;

        private readonly object _lock = new object();

        private IEquityDataStorage _equitiesFileStorageProcess;

        private IEquitiesDataGenerationMarkovProcess _equityProcess;

        private IOrderFileStorageProcess _orderFileStorageProcess;

        private IOrderDataGenerator _tradingProcess;

        public HighVolumeCommand(IAppFactory appFactory)
        {
            this._appFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command)) return false;

            return command.ToLower().Contains("run high volume trades");
        }

        public void Run(string command)
        {
            lock (this._lock)
            {
                var console = this._appFactory.Console;
                var apiRepository = this._appFactory.SecurityApiRepository;
                var marketApiRepository = this._appFactory.MarketApiRepository;

                var cmd = command.ToLower();
                cmd = cmd.Replace("run high volume trades", string.Empty).Trim();
                var splitCmd = cmd.Split(' ');

                var rawFromDate = splitCmd.FirstOrDefault();
                var market = splitCmd.Skip(1).FirstOrDefault();
                var trades = splitCmd.Skip(2).FirstOrDefault();
                var saveMarketCsv = splitCmd.Skip(3).FirstOrDefault();
                var saveTradeCsv = splitCmd.Skip(4).FirstOrDefault();
                var sedols = splitCmd.Skip(5).ToList();

                var fromSuccess = DateTime.TryParse(
                    rawFromDate,
                    CultureInfo.CurrentCulture,
                    DateTimeStyles.AssumeLocal,
                    out var fromDate);
                var tradesSuccess = string.Equals(trades, "trade", StringComparison.InvariantCultureIgnoreCase)
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
                    console.WriteToUserFeedbackLine(
                        $"Did not understand trades of {trades}. Can be 'trade' or 'notrade'");
                    return;
                }

                if (!saveMarketCsvSuccess)
                {
                    console.WriteToUserFeedbackLine(
                        "Did not understand the save to csv value. Options are \'market\' or \'nomarketcsv\'. No spaces.");
                    return;
                }

                if (!saveTradeCsvSuccess)
                {
                    console.WriteToUserFeedbackLine(
                        "Did not understand the save to csv value. Options are \'trade\' or \'notradecsv\'. No spaces.");
                    return;
                }

                if (!sedols.Any())
                {
                    console.WriteToUserFeedbackLine("Did not understand any of the sedol arguments provided");
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

                if (priceApiResult == null || (!priceApiResult.SecurityPrices?.Any() ?? true))
                {
                    console.WriteToUserFeedbackLine(
                        "Could not find any results on the security api for the provided query");
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

                if (marketApiResult == null || marketApiResult.Count == 0)
                {
                    console.WriteToUserFeedbackLine(
                        "Could not find any results for the market api on the client service");
                    return;
                }

                var marketData = marketApiResult.FirstOrDefault(
                    ap => string.Equals(ap.Code, market, StringComparison.InvariantCultureIgnoreCase));

                if (marketData == null)
                {
                    console.WriteToUserFeedbackLine(
                        "Could not find any relevant results for the market api on the client service");
                    return;
                }

                var auroraRepository = this._appFactory.AuroraRepository;
                auroraRepository.DeleteTradingAndMarketDataForMarketOnDate(market, fromDate);

                var equitiesDirectory = Path.Combine(Directory.GetCurrentDirectory(), FileDirectory);

                this._equitiesFileStorageProcess =
                    this._appFactory.EquitiesFileStorageProcessFactory.Create(equitiesDirectory);

                var tradeDirectory = Path.Combine(Directory.GetCurrentDirectory(), TradeFileDirectory);

                this._orderFileStorageProcess = this._appFactory.OrderFileStorageProcessFactory.Build(tradeDirectory);

                var equityStream = this._appFactory.StockExchangeStreamFactory.CreateDisplayable(console);

                this._equityProcess = this._appFactory.EquitiesDataGenerationProcessFactory.Build();

                var tradeStream = this._appFactory.TradeOrderStreamFactory.CreateDisplayable(console);

                this._tradingProcess = this._appFactory.TradingFactory.Create().MarketUpdate()
                    .TradingNormalDistributionVolume(4).SetFilterSedol(sedols, false).Finish();

                var highVolumeTradingProcess = this._appFactory.TradingHighVolumeFactory.Build(sedols);

                equityStream.Subscribe(highVolumeTradingProcess);
                highVolumeTradingProcess.InitiateTrading(equityStream, tradeStream);

                if (string.Equals(trades, "trade", StringComparison.InvariantCultureIgnoreCase))
                    this._tradingProcess.InitiateTrading(equityStream, tradeStream);

                if (string.Equals(saveMarketCsv, "marketcsv", StringComparison.InvariantCultureIgnoreCase))
                    this._equitiesFileStorageProcess.Initiate(equityStream);

                if (string.Equals(saveTradeCsv, "tradecsv", StringComparison.InvariantCultureIgnoreCase))
                    tradeStream.Subscribe(this._orderFileStorageProcess);

                this._equityProcess.InitiateWalk(equityStream, marketData, priceApiResult);
            }
        }
    }
}