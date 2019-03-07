using Newtonsoft.Json;
using SharedKernel.Contracts.Queues.Interfaces;

namespace SharedKernel.Contracts.Queues
{
    public class ThirdPartyDataRequestSerialiser : IThirdPartyDataRequestSerialiser
    {
        public string Serialise(ThirdPartyDataRequestMessage message)
        {
            return JsonConvert.SerializeObject(message);
        }

        public ThirdPartyDataRequestMessage Deserialise(string message)
        {
            return JsonConvert.DeserializeObject<ThirdPartyDataRequestMessage>(message);
        }
    }
}
