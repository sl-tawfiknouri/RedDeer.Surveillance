namespace Utilities.Network_IO.Websocket_Hosts.Interfaces
{
    public interface IDuplexedMessage
    {
        string Message { get; set; }
        MessageType Type { get; set; }
    }
}