using Domain.Core.Markets.Timebars;

namespace SharedKernel.Files.Security.Interfaces
{
    public interface IDtoToSecurityCsvMapper
    {
        int FailedMapTotal { get; set; }
        FinancialInstrumentTimeBarCsv Map(EquityInstrumentIntraDayTimeBar equityInstrumentIntraDayTimeBar);
    }
}