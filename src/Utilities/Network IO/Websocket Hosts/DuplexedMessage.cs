using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace Utilities.Network_IO.Websocket_Hosts
{
    public class DuplexedMessage : IDuplexedMessage
    {
        public DuplexedMessage()
        {
            Type = MessageType.Unknown;
            Message = string.Empty;
        }   
        
        public MessageType Type { get; set; }

        public string Message { get; set; }
    }
}
