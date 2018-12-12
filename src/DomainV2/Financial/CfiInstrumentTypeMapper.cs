﻿using System.Linq;
using DomainV2.Financial.Interfaces;

namespace DomainV2.Financial
{
    public class CfiInstrumentTypeMapper : ICfiInstrumentTypeMapper
    {
        public InstrumentTypes MapCfi(string cfi)
        {
            if (string.IsNullOrWhiteSpace(cfi))
            {
                return InstrumentTypes.None;
            }

            cfi = cfi.ToLower();

            if (cfi.Take(1).ToString() == "e")
            {
                return InstrumentTypes.Equity;
            }

            if (cfi.Take(2).ToString() == "db")
            {
                return InstrumentTypes.Bond;
            }

            if (cfi.Take(2).ToString() == "oc")
            {
                return InstrumentTypes.OptionCall;
            }

            if (cfi.Take(2).ToString() == "op")
            {
                return InstrumentTypes.OptionPut;
            }

            return InstrumentTypes.Unknown;
        }
    }
}
