namespace DomainV2.Equity.TimeBars.Interfaces
{
    public interface ISecurityCsvToDtoMapper
    {
        int FailedParseTotal { get; set; }
        FinancialInstrumentTimeBar Map(SecurityTickCsv csv);
    }
}