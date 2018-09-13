using Domain.Equity.Streams.Interfaces;
using Domain.Scheduling;
using Domain.Scheduling.Interfaces;
using Domain.Streams;
using StructureMap;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Logging;
using Utilities.Network_IO.Websocket_Connections;
using Surveillance.Services;
using Utilities.Network_IO.Websocket_Hosts;
using Surveillance.Rules;
using Utilities.Network_IO.Websocket_Connections.Interfaces;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;
using Surveillance.Network_IO;
using Surveillance.Network_IO.Interfaces;
using Surveillance.Services.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.Factories;
using Surveillance.Recorders.Interfaces;
using Surveillance.Recorders;
using Surveillance.Recorders.Projectors;
using Surveillance.Recorders.Projectors.Interfaces;
using Surveillance.Rules.Interfaces;
using Surveillance.Rules.ProhibitedAssets;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;
using Surveillance.Rules.Spoofing.Interfaces;
using Surveillance.Rules.Spoofing;
using Surveillance.Rules.ProhibitedAssets.Interfaces;
using Surveillance.Scheduler;
using Surveillance.Scheduler.Interfaces;
using Surveillance.Universe;
using Surveillance.Universe.Interfaces;

namespace Surveillance
{
    public class SurveillanceRegistry : Registry
    {
        public SurveillanceRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            For(typeof(ILoggerFactory)).Use(loggerFactory);
            For(typeof(ILogger<>)).Use(typeof(Logger<>));

            For<IMediator>().Use<Mediator>();
            For<IRuleManager>().Use<RuleManager>();
            For<IRuleBreachFactory>().Use<RuleBreachFactory>();
            For<IOriginFactory>().Use<OriginFactory>();

            For<IProhibitedAssetsRepository>().Use<ProhibitedAssetsRepository>();
            For<IProhibitedAssetTradingRule>().Use<ProhibitedAssetTradingRule>();
            For<ISpoofingRule>().Use<SpoofingRule>();

            For<ISurveillanceNetworkExchangeFactory>().Use<SurveillanceNetworkExchangeFactory>();

            For<IReddeerTradeService>().Use<ReddeerTradeService>();

            For<ISurveillanceNetworkDuplexer>().Use<SurveillanceNetworkDuplexer>();

            For<IRedDeerTradeRecorder>().Use<RedDeerTradeRecorder>();
            For<IReddeerTradeFormatProjector>().Use<ReddeerTradeFormatProjector>();

            For<ITradingHistory>().Use<TradingHistory>();
            For<ITradingHistoryStack>().Use<TradingHistoryStack>();

            For<IRedDeerStockExchangeRecorder>().Use<RedDeerStubStockExchangeRecorder>();
            For<IRedDeerStockExchangeRecorder>().Use<RedDeerStockExchangeRecorder>();
            For<IReddeerMarketExchangeFormatProjector>().Use<ReddeerMarketExchangeFormatProjector>();

            For<IWebsocketHostFactory>().Use<WebsocketHostFactory>();
            For<IWebsocketConnectionFactory>().Use<WebsocketConnectionFactory>();
            For(typeof(IUnsubscriberFactory<>)).Use(typeof(UnsubscriberFactory<>));

            For<IReddeerRuleScheduler>().Use<ReddeerRuleScheduler>();
            For<ISpoofingRuleFactory>().Use<SpoofingRuleFactory>();
            For<IUniversePlayerFactory>().Use<UniversePlayerFactory>();

            For<IUniverse>().Use<Universe.Universe>();
            For<IUniverseBuilder>().Use<UniverseBuilder>();
            For<IUniverseEvent>().Use<UniverseEvent>();
            For<IUniversePlayer>().Use<UniversePlayer>();

            For<IScheduledExecutionMessageBusSerialiser>().Use<ScheduledExecutionMessageBusSerialiser>();
        }
    }
}