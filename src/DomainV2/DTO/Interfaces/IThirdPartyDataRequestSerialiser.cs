namespace DomainV2.DTO.Interfaces
{
    public interface IThirdPartyDataRequestSerialiser
    {
        ThirdPartyDataRequestMessage Deserialise(string message);
        string Serialise(ThirdPartyDataRequestMessage message);
    }
}