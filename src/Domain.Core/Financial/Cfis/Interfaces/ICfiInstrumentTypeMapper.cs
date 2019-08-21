namespace Domain.Core.Financial.Cfis.Interfaces
{
    using Domain.Core.Financial.Assets;

    public interface ICfiInstrumentTypeMapper
    {
        InstrumentTypes MapCfi(string cfi);
    }
}