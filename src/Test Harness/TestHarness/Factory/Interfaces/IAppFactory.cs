﻿using Domain.Scheduling.Interfaces;
using NLog;
using TestHarness.Commands.Interfaces;
using TestHarness.Configuration.Interfaces;
using TestHarness.Display;
using TestHarness.Engine.Heartbeat.Interfaces;
using TestHarness.Factory.EquitiesFactory.Interfaces;
using TestHarness.Factory.NetworkFactory.Interfaces;
using TestHarness.Factory.TradeCancelledFactory.Interfaces;
using TestHarness.Factory.TradingFactory.Interfaces;
using TestHarness.Factory.TradingProhibitedSecurityFactory.Interfaces;
using TestHarness.Factory.TradingSpoofingFactory.Interfaces;
using TestHarness.State.Interfaces;
using Utilities.Aws_IO.Interfaces;

namespace TestHarness.Factory.Interfaces
{
    public interface IAppFactory
    {
        ILogger Logger { get; }

        ICommandManager CommandManager { get; }

        ICommandManifest CommandManifest { get; }

        IProgramState State { get; }

        IConsole Console { get; }

        IPulsatingHeartbeat ProhibitedSecurityHeartbeat { get; }

        IPulsatingHeartbeat SpoofedTradeHeartbeat { get; }

        IPulsatingHeartbeat CancelTradeHeartbeat { get; }

        IEquitiesProcessFactory EquitiesProcessFactory { get; }

        IStockExchangeStreamFactory StockExchangeStreamFactory { get; }

        INetworkManagerFactory NetworkManagerFactory { get; }

        ITradingFactory TradingFactory { get; }

        ITradeOrderStreamFactory TradeOrderStreamFactory { get; }

        ITradingProhibitedSecurityProcessFactory TradingProhibitedSecurityFactory { get; }
        ITradingSpoofingProcessFactory TradingSpoofingFactory { get; }
        ITradingFileRelayProcessFactory TradingFileRelayProcessFactory { get; }
        ITradingCancelledFactory TradingCancelledOrdersFactory { get; }
        IAwsQueueClient AwsQueueClient { get; }
        IScheduledExecutionMessageBusSerialiser ScheduledExecutionSerialiser { get; }
        INetworkConfiguration Configuration { get; }
        IEquitiesFileRelayProcessFactory EquitiesFileRelayProcessFactory { get; }
    }
}