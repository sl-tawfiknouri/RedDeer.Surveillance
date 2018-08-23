namespace Utilities.Network_IO.Websocket_Connections.Interfaces
{
    public interface IWebsocketConnectionFactory
    {
       IConnectionWebsocket Build(string connection);
    }
}