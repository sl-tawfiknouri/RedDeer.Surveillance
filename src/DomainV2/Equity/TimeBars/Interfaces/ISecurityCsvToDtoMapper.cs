namespace Domain.Equity.TimeBars.Interfaces
{
    public interface ISecurityCsvToDtoMapper
    {
        int FailedParseTotal { get; set; }
        EquityInstrumentIntraDayTimeBar Map(FinancialInstrumentTimeBarCsv csv);
    }
}