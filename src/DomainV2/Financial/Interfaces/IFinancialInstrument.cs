namespace DomainV2.Financial.Interfaces
{
    public interface IFinancialInstrument
    {
        string Cfi { get; set; }
        InstrumentIdentifiers Identifiers { get; set; }
        string Name { get; set; }
        InstrumentTypes Type { get; set; }
        string UnderlyingCfi { get; set; }
        string UnderlyingName { get; set; }
    }
}