using StructureMap;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Logging;
using Domain.Equity.Trading.Streams.Interfaces;
using Domain.Equity.Trading;
using Utilities.Network_IO.Websocket_Connections;
using Surveillance.Services;
using Utilities.Network_IO.Websocket_Hosts;
using Surveillance.Rules;
using Surveillance.Rules.BarredAssets;
using Surveillance.Rules.ProhibitedAssetTradingRule;
using Utilities.Network_IO.Websocket_Connections.Interfaces;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;
using Surveillance.Network_IO;
using Surveillance.Network_IO.Interfaces;

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
            For<IReddeerTradeService>().Use<ReddeerTradeService>();
            For<IRuleManager>().Use<RuleManager>();
            For<IProhibitedAssetsRepository>().Use<ProhibitedAssetsRepository>();
            For<IProhibitedAssetTradingRule>().Use<ProhibitedAssetTradingRule>();
            For<ISurveillanceNetworkExchangeFactory>().Use<SurveillanceNetworkExchangeFactory>();

            For<ISurveillanceTradeNetworkDuplexer>().Use<SurveillanceTradeNetworkDuplexer>();
            For<ISurveillanceEquityNetworkDuplexer>().Use<SurveillanceEquityNetworkDuplexer>();

            For<IWebsocketHostFactory>().Use<WebsocketHostFactory>();
            For<IWebsocketConnectionFactory>().Use<WebsocketConnectionFactory>();
            For(typeof(IUnsubscriberFactory<>)).Use(typeof(UnsubscriberFactory<>));
        }
    }
}
