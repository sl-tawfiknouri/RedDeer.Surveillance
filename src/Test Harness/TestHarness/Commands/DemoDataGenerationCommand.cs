﻿using System;
using System.Globalization;
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
                var apiRepository = _appFactory.SecurityApiRepository;
                var marketApiRepository = _appFactory.MarketApiRepository;

                var cmd = command.ToLower();
                cmd = cmd.Replace("run data generation", string.Empty).Trim();
                var splitCmd = cmd.Split(' ');

                var rawFromDate = (splitCmd.Take(1)).FirstOrDefault();
                var rawToDate = splitCmd.Skip(1).Take(1).FirstOrDefault();
                var market = splitCmd.Skip(2).Take(1).FirstOrDefault();

                var fromSuccess = DateTime.TryParse(rawFromDate, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out var fromDate);
                var toSuccess = DateTime.TryParse(rawToDate, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out var toDate);
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

                // ok so...what's next? I think it;s the actual processor tbh we have the market data and the exchange rates
                // we'll pass both into initiate trading

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
