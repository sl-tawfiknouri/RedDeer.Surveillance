namespace DomainV2.Equity.TimeBars.Interfaces
{
    public interface ISecurityCsvToDtoMapper
    {
        int FailedParseTotal { get; set; }
        EquityInstrumentIntraDayTimeBar Map(FinancialInstrumentTimeBarCsv csv);
    }
}