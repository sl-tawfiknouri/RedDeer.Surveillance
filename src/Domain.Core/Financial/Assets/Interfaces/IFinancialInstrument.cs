namespace Domain.Core.Financial.Assets.Interfaces
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

        string SectorCode { get; set; }
        string IndustryCode { get; set; }
        string RegionCode { get; set; }
        string CountryCode { get; set; }
    }
}