namespace Utilities.Network_IO.Websocket_Hosts
{
    public interface IWebsocketHostFactory
    {
        IWebsocketHost Build(string location);
    }
}