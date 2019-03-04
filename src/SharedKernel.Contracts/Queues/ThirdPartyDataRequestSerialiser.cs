using Domain.DTO.Interfaces;
using Newtonsoft.Json;

namespace Domain.DTO
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
