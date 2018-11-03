using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RedDeer.Contracts.SurveillanceService.Api.Markets;
using TestHarness.Commands.Interfaces;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Engine.Plans;
using TestHarness.Factory.Interfaces;
using TestHarness.Network_IO.Interfaces;

namespace TestHarness.Commands.Market_Abuse_Commands
{
    public class LayeringCommand : ICommand
    {
        private readonly object _lock = new object();
        private readonly IAppFactory _appFactory;

        private INetworkManager _networkManager;
        private IEquitiesDataGenerationMarkovProcess _equityProcess;
        private IOrderDataGenerator _tradingProcess;


        public LayeringCommand(IAppFactory appFactory)
        {
            _appFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            return command.ToLower().Contains("run layering trades");
        }

        public void Run(string command)
        {
            lock (_lock)
            {
                var console = _appFactory.Console;
                var apiRepository = _appFactory.SecurityApiRepository;
                var marketApiRepository = _appFactory.MarketApiRepository;

                var cmd = command.ToLower();
                cmd = cmd.Replace("run layering trades", string.Empty).Trim();
                var splitCmd = cmd.Split(' ');

                var rawFromDate = splitCmd.FirstOrDefault();
                var market = splitCmd.Skip(1).FirstOrDefault();
                var trades = splitCmd.Skip(2).FirstOrDefault();
                var sedols = splitCmd.Skip(3).ToList();

                var fromSuccess = DateTime.TryParse(rawFromDate, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out var fromDate);
                var tradesSuccess =
                    string.Equals(trades, "trade", StringComparison.InvariantCultureIgnoreCase)
                    || string.Equals(trades, "notrade", StringComparison.InvariantCultureIgnoreCase);
                var marketSuccess = !string.IsNullOrWhiteSpace(market);

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

                var plans = Plan(sedols, marketData, fromDate.Date);
                
                var auroraRepository = _appFactory.AuroraRepository;
                auroraRepository.DeleteTradingAndMarketDataForMarketOnDate(market, fromDate);

                var equityStream =
                    _appFactory
                        .StockExchangeStreamFactory
                        .CreateDisplayable(console);

                _equityProcess =
                    _appFactory
                        .EquitiesDataGenerationProcessFactory
                        .Build(plans);

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

                var layeringProcess =
                    _appFactory
                        .LayeringProcessFactory
                        .Build(plans);

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

                equityStream.Subscribe(layeringProcess);
                layeringProcess.InitiateTrading(equityStream, tradeStream);

                if (string.Equals(trades, "trade", StringComparison.InvariantCultureIgnoreCase))
                {
                    _tradingProcess.InitiateTrading(equityStream, tradeStream);
                }

                _equityProcess.InitiateWalk(equityStream, marketData, priceApiResult);
            }
        }

        private IReadOnlyCollection<DataGenerationPlan> Plan(
            IReadOnlyCollection<string> sedols,
            ExchangeDto dto,
            DateTime from)
        {
            if (!sedols?.Any() ?? true)
            {
                return new DataGenerationPlan[0];
            }

            var result = new List<DataGenerationPlan>();

            var i = 1;
            foreach (var sedol in sedols)
            {
                

                var openTime = from.Add(dto.MarketOpenTime).AddMinutes(i * 30);
                var intervalInstruction =
                    new IntervalEquityPriceInstruction(
                        sedol,
                        openTime.TimeOfDay,
                        openTime.TimeOfDay.Add(TimeSpan.FromMinutes(10)),
                        TimeSpan.FromMinutes(1),
                        from.Date.Add(openTime.TimeOfDay),
                        from.Date.Add(openTime.TimeOfDay.Add(TimeSpan.FromMinutes(10))),
                        PriceManipulation.Increase,
                        0.01m);

                var newPlan = new DataGenerationPlan(sedol,  intervalInstruction);
                result.Add(newPlan);
                i++;
            }

            return result;
        }
    }
}
