using Domain.Core.Financial;
using Domain.Core.Financial.Markets;
using System.Collections.Generic;

namespace Surveillance.Specflow.Tests.StepDefinitions
{
    /// <summary>
    /// static list of securities for specflow testing
    /// </summary>
    public class SecuritySelection
    {
        public SecuritySelection()
        {
            Securities = new Dictionary<string, SecurityMarketPair>
            {
                { "Vodafone", Vodafone() },
                { "Barclays", Barclays() },
                { "BAE", Bae() },
                { "AMD", Amd() },
                { "Nvidia", Nvidia() },
                { "Micron", Micron() },
            };
        }

        public IReadOnlyDictionary<string, SecurityMarketPair> Securities { get; set; }

        private SecurityMarketPair Vodafone()
        {
            var identifiers =
                new InstrumentIdentifiers(
                    "Voda",
                    "RD00D",
                    "RD00D",
                    "vodafone ln",
                    "BH4HKS3",
                    "GB00BH4HKS39",
                    "BBG000N8D298", 
                    string.Empty,
                    "VOD",
                    "213800WP8QQ8YTL6MH84",
                    "Vodafone Lon");

            var financialInstrument =
                new FinancialInstrument(
                    InstrumentTypes.Equity,
                    identifiers,
                    "Vodafone", 
                    "entspb",
                    "GBX", 
                    "Vodafone plc");

            var market = Xlon();

            return new SecurityMarketPair
            {
                Instrument = financialInstrument,
                Market = market
            };
        }

        private SecurityMarketPair Barclays()
        {
            var identifiers =
                new InstrumentIdentifiers(
                    "Barclays",
                    "RD01D",
                    "RD01D",
                    "Barclays ln",
                    "3134865",
                    "GB0031348658",
                    "BBG000NC8KY7",
                    string.Empty,
                    "BARC",
                    "G5GSEF7VJP5I7OUK5573",
                    "Barclays Lon");

            var financialInstrument =
                new FinancialInstrument(
                    InstrumentTypes.Equity,
                    identifiers,
                    "Barclays",
                    "entspb",
                    "GBX",
                    "Barclays plc");

            var market = Xlon();

            return new SecurityMarketPair
            {
                Instrument = financialInstrument,
                Market = market
            };
        }

        private SecurityMarketPair Bae()
        {
            var identifiers =
                new InstrumentIdentifiers(
                    "BAE",
                    "RD01D",
                    "RD01D",
                    "BAE Systems Ln",
                    "0263494",
                    "GB0002634946",
                    "BBG000H342V1",
                    string.Empty,
                    "BAE",
                    "549300D72VDDRVGKSH48",
                    "BAE Systems Plc");

            var financialInstrument =
                new FinancialInstrument(
                    InstrumentTypes.Equity,
                    identifiers,
                    "BAE",
                    "entspb",
                    "GBX",
                    "BAE Systems Plc");

            var market = Xlon();

            return new SecurityMarketPair
            {
                Instrument = financialInstrument,
                Market = market
            };
        }

        private Market Xlon()
        {
            return new Market("0", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);
        }

        private SecurityMarketPair Amd()
        {
            var identifiers =
                new InstrumentIdentifiers(
                    "AMD",
                    "RD01D",
                    "RD01D",
                    "Advanced Micro Devices nasdaq",
                    "2007849",
                    "US0079031078",
                    "BBG00KFWP7B3",
                    string.Empty,
                    "AMD",
                    string.Empty,
                    "Advanced Micro Devices Inc");

            var financialInstrument =
                new FinancialInstrument(
                    InstrumentTypes.Equity,
                    identifiers,
                    "AMD",
                    "entspb",
                    "USD",
                    "Advanced Micro Devices Inc");

            var market = Nasdaq();

            return new SecurityMarketPair
            {
                Instrument = financialInstrument,
                Market = market
            };
        }

        private SecurityMarketPair Nvidia()
        {
            var identifiers =
                new InstrumentIdentifiers(
                    "Nvidia",
                    "RD01D",
                    "RD01D",
                    "Nvidia Corp nasdaq",
                    "2379504",
                    "US67066G1040",
                    "BBG00JSC5XF7",
                    string.Empty,
                    "Nvidia",
                    string.Empty,
                    "Nvidia Corp Inc");

            var financialInstrument =
                new FinancialInstrument(
                    InstrumentTypes.Equity,
                    identifiers,
                    "Nvidia",
                    "entspb",
                    "USD",
                    "Nvidia Corp Inc");

            var market = Nasdaq();

            return new SecurityMarketPair
            {
                Instrument = financialInstrument,
                Market = market
            };
        }
        
        private SecurityMarketPair Micron()
        {
            var identifiers =
                new InstrumentIdentifiers(
                    "Micron Technology Inc",
                    "RD01D",
                    "RD01D",
                    "Micron Technology nasdaq",
                    "2588184",
                    "US5951121038",
                    "BBG000DQNYH9",
                    string.Empty,
                    "Micron",
                    string.Empty,
                    "Micron Technology Inc");

            var financialInstrument =
                new FinancialInstrument(
                    InstrumentTypes.Equity,
                    identifiers,
                    "Micron",
                    "entspb",
                    "USD",
                    "Micron Technology Inc");

            var market = Nasdaq();

            return new SecurityMarketPair
            {
                Instrument = financialInstrument,
                Market = market
            };
        }

        private Market Nasdaq()
        {
            return new Market("0", "NASDAQ", "NASDAQ", MarketTypes.STOCKEXCHANGE);
        }

        public class SecurityMarketPair
        {
            public FinancialInstrument Instrument { get; set; }
            public Market Market { get; set; }
        }
    }
}
