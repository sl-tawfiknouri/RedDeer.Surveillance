namespace SharedKernel.Files.Security.Interfaces
{
    using Domain.Core.Markets.Timebars;

    public interface ISecurityCsvToDtoMapper
    {
        int FailedParseTotal { get; set; }

        EquityInstrumentIntraDayTimeBar Map(FinancialInstrumentTimeBarCsv csv);
    }
}