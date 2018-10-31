using System;
using System.Linq;
using TestHarness.Commands.Interfaces;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Factory.Interfaces;
using TestHarness.Network_IO.Interfaces;

namespace TestHarness.Commands.Market_Abuse_Commands
{
    /// <summary>
    /// Edition 2 of the cancellation command
    /// Prior edition injected cancelled orders into the
    /// on going data generation process this one injects into
    /// historic data only and calls run schedule rule after the injection
    /// </summary>
    public class Cancellation2Command : ICommand
    {
        private readonly object _lock = new object();
        private readonly IAppFactory _appFactory;

        private INetworkManager _networkManager;
        private IEquitiesDataGenerationMarkovProcess _equityProcess;
        private IOrderDataGenerator _tradingProcess;

        public Cancellation2Command(IAppFactory appFactory)
        {
            _appFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            return command.ToLower().Contains("run cancellation2 trade");
        }

        public void Run(string command)
        {
            lock (_lock)
            {
                var fromDate = new DateTime(2018, 03, 01);
                var toDate = new DateTime(2018, 03, 02);
                const string market = "xlon";

                var console = _appFactory.Console;
                var apiRepository = _appFactory.SecurityApiRepository;
                var marketApiRepository = _appFactory.MarketApiRepository;
                
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
                        .Finish();

                var cancelledProcess =
                    _appFactory
                        .TradingCancelled2Factory
                        .Build(fromDate, "B188SR5", "3163836");

                //_networkManager =
                //    _appFactory
                //        .NetworkManagerFactory
                //        .CreateWebsockets();

                // start networking processes
                //var connectionEstablished = _networkManager.InitiateAllNetworkConnections();

                //if (!connectionEstablished)
                //{
                //    console.WriteToUserFeedbackLine("Failed to establish network connections. Aborting run data generation.");
                //    return;
                //}

                //connectionEstablished = _networkManager.AttachTradeOrderSubscriberToStream(tradeStream);
                //if (!connectionEstablished)
                //{
                //    console.WriteToUserFeedbackLine("Failed to establish trade network connections. Aborting run data generation.");
                //    return;
                //}

                //connectionEstablished = _networkManager.AttachStockExchangeSubscriberToStream(equityStream);
                //if (!connectionEstablished)
                //{
                //    console.WriteToUserFeedbackLine("Failed to establish stock market network connections. Aborting run data generation.");
                //    return;
                //}

                equityStream.Subscribe(cancelledProcess);
                cancelledProcess.InitiateTrading(equityStream, tradeStream);
               // _tradingProcess.InitiateTrading(equityStream, tradeStream);
                _equityProcess.InitiateWalk(equityStream, marketData, priceApiResult);
            }
        }
    }
}
