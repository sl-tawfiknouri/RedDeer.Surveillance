namespace Domain.Financial.Interfaces
{
    public interface ICfiInstrumentTypeMapper
    {
        InstrumentTypes MapCfi(string cfi);
    }
}