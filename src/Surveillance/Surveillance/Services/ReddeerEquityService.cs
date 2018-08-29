using Domain.Equity.Trading;
using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Streams.Interfaces;
using Surveillance.Network_IO;
using Surveillance.Network_IO.Interfaces;
using Surveillance.Rules;
using Surveillance.Services.Interfaces;
using System;
using Utilities.Network_IO.Websocket_Hosts;

namespace Surveillance.Services
{
    public class ReddeerEquityService : IReddeerEquityService
    {
        private INetworkExchange _equityNetworkExchange;
        private ISurveillanceNetworkExchangeFactory _networkExchangeFactory;
        private IUnsubscriberFactory<ExchangeFrame> _equityUnsubscriberFactory;
        private IRuleManager _ruleManager;

        public ReddeerEquityService(
            ISurveillanceNetworkExchangeFactory networkExchangeFactory,
            IUnsubscriberFactory<ExchangeFrame> equityUnsubscriberFactory,
            IRuleManager ruleManager)
        {
            _networkExchangeFactory = 
                networkExchangeFactory
                ?? throw new ArgumentNullException(nameof(networkExchangeFactory));

            _equityUnsubscriberFactory =
                equityUnsubscriberFactory
                ?? throw new ArgumentNullException(nameof(equityUnsubscriberFactory));

            _ruleManager =
                ruleManager 
                ?? throw new ArgumentNullException(nameof(ruleManager));
        }

        public void Initialise()
        {
            HostEquityProcessingPipeline();
        }

        private void HostEquityProcessingPipeline()
        {
            var stream = new StockExchangeStream(_equityUnsubscriberFactory);
            var duplexer = new SurveillanceEquityNetworkDuplexer(stream);
            _equityNetworkExchange = _networkExchangeFactory.Create(duplexer);
            _equityNetworkExchange.Initialise("ws://0.0.0.0:9070");

            // equity stream only rules
            _ruleManager.RegisterEquityRules(stream);
        }

        public void Dispose()
        {
            if (_equityNetworkExchange != null)
            {
                _equityNetworkExchange.TerminateConnections();
            }
        }
    }
}
