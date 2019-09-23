namespace SharedKernel.Files.Security.Interfaces
{
    using Domain.Core.Markets.Timebars;

    public interface IDtoToSecurityCsvMapper
    {
        int FailedMapTotal { get; set; }

        FinancialInstrumentTimeBarCsv Map(EquityInstrumentIntraDayTimeBar equityInstrumentIntraDayTimeBar);
    }
}