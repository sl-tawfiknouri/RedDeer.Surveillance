using DomainV2.Financial.Interfaces;

namespace DomainV2.Financial
{
    public class FinancialInstrument : IFinancialInstrument
    {
        public InstrumentTypes Type { get; set; }
        public InstrumentIdentifiers Identifiers { get; set; }

        /// <summary>
        /// Name of the instrument
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Classification Financial Instrument code
        /// </summary>
        public string Cfi { get; set; }

        // derivatives
        public string UnderlyingName { get; set; }
        public string UnderlyingCfi { get; set; }
    }
}
