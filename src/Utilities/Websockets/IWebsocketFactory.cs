namespace Utilities.Websockets
{
    public interface IWebsocketFactory
    {
       IWebsocket Build(string connection);
    }
}