namespace Domain.Core.Financial.Interfaces
{
    public interface IFinancialInstrument
    {
        string Name { get; set; }
        string Cfi { get; set; }
        string IssuerIdentifier { get; set; }
        InstrumentIdentifiers Identifiers { get; set; }
        InstrumentTypes Type { get; set; }
        string UnderlyingName { get; set; }
        string UnderlyingCfi { get; set; }
        string UnderlyingIssuerIdentifier { get; set; }
    }
}