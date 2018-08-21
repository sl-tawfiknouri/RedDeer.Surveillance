using StructureMap;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Logging;
using Domain.Equity.Trading.Streams.Interfaces;
using Domain.Equity.Trading;
using Utilities.Websockets;

namespace Surveillance
{
    public class SurveillanceRegistry : Registry
    {
        public SurveillanceRegistry()
        {
            var loggerFactory = new NLogLoggerFactory();
            For(typeof(ILoggerFactory)).Use(loggerFactory);
            For(typeof(ILogger<>)).Use(typeof(Logger<>));


            For<IWebsocketFactory>().Use<WebsocketFactory>();
            For(typeof(IUnsubscriberFactory<>)).Use(typeof(UnsubscriberFactory<>));
        }
    }
}
