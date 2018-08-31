using Newtonsoft.Json;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace Utilities.Network_IO.Websocket_Hosts
{
    public class DuplexMessageFactory : IDuplexMessageFactory
    {
        public IDuplexedMessage Create<T>(MessageType type, T value)
        {
            var serialiserSettings = new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
            };

            return new DuplexedMessage
            {
                Type = type,
                Message = JsonConvert.SerializeObject(value, serialiserSettings)
            };
        }
    }
}
