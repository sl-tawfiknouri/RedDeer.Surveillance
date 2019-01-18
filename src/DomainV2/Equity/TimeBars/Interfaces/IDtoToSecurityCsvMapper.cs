﻿namespace DomainV2.Equity.TimeBars.Interfaces
{
    public interface IDtoToSecurityCsvMapper
    {
        int FailedMapTotal { get; set; }
        FinancialInstrumentTimeBarCsv Map(EquityInstrumentIntraDayTimeBar equityInstrumentIntraDayTimeBar);
    }
}