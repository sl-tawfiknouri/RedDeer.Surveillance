namespace SharedKernel.Contracts.Queues
{
    using Newtonsoft.Json;

    using SharedKernel.Contracts.Queues.Interfaces;

    public class ThirdPartyDataRequestSerialiser : IThirdPartyDataRequestSerialiser
    {
        public ThirdPartyDataRequestMessage Deserialise(string message)
        {
            return JsonConvert.DeserializeObject<ThirdPartyDataRequestMessage>(message);
        }

        public string Serialise(ThirdPartyDataRequestMessage message)
        {
            return JsonConvert.SerializeObject(message);
        }
    }
}