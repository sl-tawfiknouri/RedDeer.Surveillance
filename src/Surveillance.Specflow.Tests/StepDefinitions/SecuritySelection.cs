using DomainV2.Financial;
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
                { "Barclays", Barclays() }
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

            var market = new Market("0", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);

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

            var market = new Market("0", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);

            return new SecurityMarketPair
            {
                Instrument = financialInstrument,
                Market = market
            };
        }

        public class SecurityMarketPair
        {
            public FinancialInstrument Instrument { get; set; }
            public Market Market { get; set; }
        }
    }
}
