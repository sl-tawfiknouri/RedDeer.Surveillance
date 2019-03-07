using Domain.Core.Financial.Assets;

namespace Domain.Core.Financial.Cfis.Interfaces
{
    public interface ICfiInstrumentTypeMapper
    {
        InstrumentTypes MapCfi(string cfi);
    }
}