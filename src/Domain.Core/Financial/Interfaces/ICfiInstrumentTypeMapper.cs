namespace Domain.Core.Financial.Interfaces
{
    public interface ICfiInstrumentTypeMapper
    {
        InstrumentTypes MapCfi(string cfi);
    }
}