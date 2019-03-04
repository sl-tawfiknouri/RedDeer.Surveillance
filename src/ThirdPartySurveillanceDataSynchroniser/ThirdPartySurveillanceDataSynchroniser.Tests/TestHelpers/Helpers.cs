namespace DataSynchroniser.Tests.TestHelpers
{
    public static class Helpers
    {
        public static FinancialInstrument FinancialInstrument()
        {
            var instrumentIdentifiers =
                new InstrumentIdentifiers
                {
                    Id = "1",
                    ReddeerId = "1",
                    ReddeerEnrichmentId = "1",
                    ClientIdentifier = "abc",
                    Sedol = "BSJC723",
                    Isin = "US8825081040",
                    Cusip = "882508104"
                };

            return new FinancialInstrument(
                InstrumentTypes.Equity,
                instrumentIdentifiers,
                "TEXAS INSTRUMENTS INCORPORATED  COM",
                "entspb",
                "USD",
                "Texas Instruments Incorporated");
        }
    }
}
