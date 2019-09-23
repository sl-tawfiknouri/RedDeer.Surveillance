namespace Domain.Core.Financial.Assets.Interfaces
{
    public interface IFinancialInstrument
    {
        string Cfi { get; set; }

        string CountryCode { get; set; }

        InstrumentIdentifiers Identifiers { get; set; }

        string IndustryCode { get; set; }

        string IssuerIdentifier { get; set; }

        string Name { get; set; }

        string RegionCode { get; set; }

        string SectorCode { get; set; }

        InstrumentTypes Type { get; set; }

        string UnderlyingCfi { get; set; }

        string UnderlyingIssuerIdentifier { get; set; }

        string UnderlyingName { get; set; }
    }
}