namespace Utilities.Network_IO.Websocket_Hosts.Interfaces
{
    public interface IWebsocketHostFactory
    {
        IWebsocketHost Build(string location);
    }
}