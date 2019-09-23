namespace Domain.Core.Financial.Cfis
{
    using System.Linq;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Financial.Cfis.Interfaces;

    public class CfiInstrumentTypeMapper : ICfiInstrumentTypeMapper
    {
        public InstrumentTypes MapCfi(string cfi)
        {
            if (string.IsNullOrWhiteSpace(cfi)) return InstrumentTypes.None;

            cfi = cfi.ToLower();

            if (new string(cfi.Take(1).ToArray()) == "e") return InstrumentTypes.Equity;

            if (new string(cfi.Take(2).ToArray()) == "db") return InstrumentTypes.Bond;

            if (new string(cfi.Take(2).ToArray()) == "oc") return InstrumentTypes.OptionCall;

            if (new string(cfi.Take(2).ToArray()) == "op") return InstrumentTypes.OptionPut;

            return InstrumentTypes.Unknown;
        }
    }
}