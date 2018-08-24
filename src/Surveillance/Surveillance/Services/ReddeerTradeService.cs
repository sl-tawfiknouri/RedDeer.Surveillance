using Domain.Equity.Trading;
using Domain.Equity.Trading.Orders;
using Domain.Equity.Trading.Streams.Interfaces;
using Surveillance.Network_IO.RedDeer;
using Surveillance.Rules;
using System;

namespace Surveillance.Services
{
    public class ReddeerTradeService : IReddeerTradeService
    {
        private IReddeerTradeNetworkManager _networkManager;
        private IUnsubscriberFactory<TradeOrderFrame> _unsubscriberFactory;
        private IRuleManager _ruleManager;

        public ReddeerTradeService(
            IReddeerTradeNetworkManager networkManager,
            IUnsubscriberFactory<TradeOrderFrame> unsubscriberFactory,
            IRuleManager ruleManager)
        {
            _networkManager = networkManager ?? throw new ArgumentNullException(nameof(networkManager));
            _unsubscriberFactory = unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
            _ruleManager = ruleManager ?? throw new ArgumentNullException(nameof(ruleManager));
        }

        public void Initialise()
        {
            var stream = new TradeOrderStream<TradeOrderFrame>(_unsubscriberFactory);
            _networkManager.InitiateConnections(stream);

            _ruleManager.RegisterTradingRules(stream);
        }

        public void Dispose()
        {
            if (_networkManager != null)
            {
                _networkManager.TerminateConnections();
            }
        }
    }
}
