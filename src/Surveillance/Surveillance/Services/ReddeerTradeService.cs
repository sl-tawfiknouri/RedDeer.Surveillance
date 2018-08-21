using Domain.Equity.Trading;
using Domain.Equity.Trading.Orders;
using Domain.Equity.Trading.Streams.Interfaces;
using Surveillance.Network_IO.RedDeer;
using System;

namespace Surveillance.Services
{
    public class ReddeerTradeService : IReddeerTradeService
    {
        private IReddeerTradeNetworkManager _networkManager;
        private IUnsubscriberFactory<TradeOrderFrame> _unsubscriberFactory;

        public ReddeerTradeService(
            IReddeerTradeNetworkManager networkManager,
            IUnsubscriberFactory<TradeOrderFrame> unsubscriberFactory)
        {
            _networkManager = networkManager ?? throw new ArgumentNullException(nameof(networkManager));
            _unsubscriberFactory = unsubscriberFactory ?? throw new ArgumentNullException(nameof(unsubscriberFactory));
        }

        public void Initialise()
        {
            var stream = new TradeOrderStream(_unsubscriberFactory);
            _networkManager.InitiateConnections(stream);

            // hook post stream work into trade order stream
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
