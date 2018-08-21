using Domain.Equity.Trading;
using Domain.Equity.Trading.Orders;
using Microsoft.Extensions.Logging;
using Utilities.Network_IO.Websocket_Hosts;

namespace Surveillance.Network_IO.RedDeer
{
    /// <summary>
    /// Manages network comms for 
    /// </summary>
    public class ReddeerTradeNetworkManager : NetworkManager<TradeOrderStream, TradeOrderFrame>, IReddeerTradeNetworkManager
    {
        public ReddeerTradeNetworkManager(
            ILogger<NetworkManager<TradeOrderStream, TradeOrderFrame>> logger,
            IWebsocketHostFactory websocketHostFactory) 
            : base(logger, websocketHostFactory, "ws://0.0.0.0:9069", "Reddeer Trade Network Manager")
        {
        }
    }
}