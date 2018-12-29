using DomainV2.DTO.Interfaces;
using Newtonsoft.Json;

namespace DomainV2.DTO
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
