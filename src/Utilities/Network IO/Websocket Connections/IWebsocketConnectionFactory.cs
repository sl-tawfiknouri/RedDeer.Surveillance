namespace Utilities.Network_IO.Websocket_Connections
{
    public interface IWebsocketConnectionFactory
    {
       IConnectionWebsocket Build(string connection);
    }
}