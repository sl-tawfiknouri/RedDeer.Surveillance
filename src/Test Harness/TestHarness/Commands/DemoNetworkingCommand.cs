﻿using System;
using TestHarness.Commands.Interfaces;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Engine.OrderGenerator;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;
using TestHarness.Factory.Interfaces;
using TestHarness.Network_IO;

namespace TestHarness.Commands
{
    public class DemoNetworkingCommand : ICommand
    {
        private IAppFactory _appFactory;
        private IEquityDataGenerator _equityProcess;
        private IOrderDataGenerator _tradeProcess;
        private INetworkManager _networkManager;

        private object _lock = new object();

        public DemoNetworkingCommand(IAppFactory appFactory)
        {
            _appFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            return
                string.Equals(command, "run demo networking", StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(command, "stop demo networking", StringComparison.InvariantCultureIgnoreCase);
        }

        public void Run(string command)
        {
            if (string.Equals(command, "run demo networking", StringComparison.InvariantCultureIgnoreCase))
            {
                Run();
            }

            if (string.Equals(command, "stop demo networking", StringComparison.InvariantCultureIgnoreCase))
            {
                Stop();
            }
        }

        private void Run()
        {
            var console = _appFactory.Console;

            var equityStream =
                _appFactory
                .StockExchangeStreamFactory
                .CreateDisplayable(console);

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

            _tradeProcess =
                _appFactory
                .TradingFactory
                .Create()
                .Heartbeat()
                .Irregular(TimeSpan.FromMilliseconds(600), 8)
                .TradingFixedVolume(3)
                .Finish();

            var prohibitedTradeProcess = new TradingHeartbeatProhibitedSecuritiesProcess(
                _appFactory.ProhibitedSecurityHeartbeat,
                _appFactory.Logger,
                new StubTradeStrategy());

            _networkManager =
                _appFactory
                .NetworkManagerFactory
                .CreateWebsockets();

            // start networking processes
            var connectionEstablished = _networkManager.InitiateNetworkConnections();

            if (!connectionEstablished)
            {
                console.WriteToUserFeedbackLine("Failed to establish network connections. Aborting run demo networking.");
                return;
            }

            connectionEstablished = _networkManager.AttachTradeOrderSubscriberToStream(tradeStream);

            if (!connectionEstablished)
            {
                console.WriteToUserFeedbackLine("Failed to establish network connections. Aborting run demo networking.");
                return;
            }

            // start updating equity data
            _equityProcess.InitiateWalk(equityStream);

            // start updating trading data
            _tradeProcess.InitiateTrading(equityStream, tradeStream);
            prohibitedTradeProcess.InitiateTrading(equityStream, tradeStream);
        }

        private void Stop()
        {
            if (_networkManager != null)
            {
                _networkManager.TerminateNetworkConnections();
            }

            if (_tradeProcess != null)
            {
                _tradeProcess.TerminateTrading();
            }

            if (_equityProcess != null)
            {
                _equityProcess.TerminateWalk();
            }
        }
    }
}