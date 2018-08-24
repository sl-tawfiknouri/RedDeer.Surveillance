namespace Utilities.Network_IO.Websocket_Hosts.Interfaces
{
    public interface IDuplexMessageFactory
    {
        IDuplexedMessage Create<T>(MessageType type, T value);
    }
}