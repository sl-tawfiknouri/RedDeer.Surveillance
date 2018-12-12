namespace DomainV2.Financial.Interfaces
{
    public interface ICfiInstrumentTypeMapper
    {
        InstrumentTypes MapCfi(string cfi);
    }
}