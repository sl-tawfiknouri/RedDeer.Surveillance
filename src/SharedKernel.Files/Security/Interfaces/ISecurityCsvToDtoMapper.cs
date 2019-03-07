using Domain.Core.Markets.Timebars;

namespace SharedKernel.Files.Security.Interfaces
{
    public interface ISecurityCsvToDtoMapper
    {
        int FailedParseTotal { get; set; }
        EquityInstrumentIntraDayTimeBar Map(FinancialInstrumentTimeBarCsv csv);
    }
}