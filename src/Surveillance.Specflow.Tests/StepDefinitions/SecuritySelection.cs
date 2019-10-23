namespace Surveillance.Specflow.Tests.StepDefinitions
{
    using System.Collections.Generic;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Markets;

    /// <summary>
    /// The security selection.
    /// </summary>
    public class SecuritySelection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SecuritySelection"/> class.
        /// </summary>
        public SecuritySelection()
        {
            this.Securities = new Dictionary<string, SecurityMarketPair>
              {
                  { "Vodafone", this.Vodafone() },
                  { "Barclays", this.Barclays() },
                  { "BAE", this.Bae() },
                  { "AMD", this.Amd() },
                  { "Nvidia", this.Nvidia() },
                  { "Micron", this.Micron() },
                  { "UKGovtBond", this.UkGovernmentBond() },
                  { "UKGovtBondSecondaryMarket", this.UkGovernmentBondSecondaryMarket() }
              };
        }

        /// <summary>
        /// Gets or sets the securities.
        /// </summary>
        public IReadOnlyDictionary<string, SecurityMarketPair> Securities { get; set; }

        /// <summary>
        /// The advanced micro devices
        /// </summary>
        /// <returns>
        /// The <see cref="SecurityMarketPair"/>.
        /// </returns>
        private SecurityMarketPair Amd()
        {
            var identifiers = new InstrumentIdentifiers(
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
                "Advanced Micro Devices Inc",
                string.Empty);

            var financialInstrument = new FinancialInstrument(
                InstrumentTypes.Equity,
                identifiers,
                "AMD",
                "entspb",
                "USD",
                "Advanced Micro Devices Inc");

            var market = this.Nasdaq();

            return new SecurityMarketPair { Instrument = financialInstrument, Market = market };
        }

        /// <summary>
        /// The british aerospace
        /// </summary>
        /// <returns>
        /// The <see cref="SecurityMarketPair"/>.
        /// </returns>
        private SecurityMarketPair Bae()
        {
            var identifiers = new InstrumentIdentifiers(
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
                "BAE Systems Plc",
                string.Empty);

            var financialInstrument = new FinancialInstrument(
                InstrumentTypes.Equity,
                identifiers,
                "BAE",
                "entspb",
                "GBX",
                "BAE Systems Plc");

            var market = this.LondonStockExchange();

            return new SecurityMarketPair { Instrument = financialInstrument, Market = market };
        }

        /// <summary>
        /// The constructor for bank security.
        /// </summary>
        /// <returns>
        /// The <see cref="SecurityMarketPair"/>.
        /// </returns>
        private SecurityMarketPair Barclays()
        {
            var identifiers = new InstrumentIdentifiers(
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
                "Barclays Lon",
                string.Empty);

            var financialInstrument = new FinancialInstrument(
                InstrumentTypes.Equity,
                identifiers,
                "Barclays",
                "entspb",
                "GBX",
                "Barclays plc");

            var market = this.LondonStockExchange();

            return new SecurityMarketPair { Instrument = financialInstrument, Market = market };
        }

        /// <summary>
        /// The micron technology market.
        /// </summary>
        /// <returns>
        /// The <see cref="SecurityMarketPair"/>.
        /// </returns>
        private SecurityMarketPair Micron()
        {
            var identifiers = new InstrumentIdentifiers(
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
                "Micron Technology Inc",
                string.Empty);

            var financialInstrument = new FinancialInstrument(
                InstrumentTypes.Equity,
                identifiers,
                "Micron",
                "entspb",
                "USD",
                "Micron Technology Inc");

            var market = this.Nasdaq();

            return new SecurityMarketPair { Instrument = financialInstrument, Market = market };
        }

        /// <summary>
        /// The nasdaq exchange.
        /// </summary>
        /// <returns>
        /// The <see cref="Market"/>.
        /// </returns>
        private Market Nasdaq()
        {
            return new Market("0", "NASDAQ", "NASDAQ", MarketTypes.STOCKEXCHANGE);
        }

        /// <summary>
        /// The chip maker corporation.
        /// </summary>
        /// <returns>
        /// The <see cref="SecurityMarketPair"/>.
        /// </returns>
        private SecurityMarketPair Nvidia()
        {
            var identifiers = new InstrumentIdentifiers(
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
                "Nvidia Corp Inc",
                string.Empty);

            var financialInstrument = new FinancialInstrument(
                InstrumentTypes.Equity,
                identifiers,
                "Nvidia",
                "entspb",
                "USD",
                "Nvidia Corp Inc");

            var market = this.Nasdaq();

            return new SecurityMarketPair { Instrument = financialInstrument, Market = market };
        }

        /// <summary>
        /// The over the counter market.
        /// </summary>
        /// <returns>
        /// The <see cref="Market"/>.
        /// </returns>
        private Market OverTheCounter()
        {
            return new Market("0", "OTC", "OTC", MarketTypes.OTC);
        }

        /// <summary>
        /// The UK government bond.
        /// </summary>
        /// <returns>
        /// The <see cref="SecurityMarketPair"/>.
        /// </returns>
        private SecurityMarketPair UkGovernmentBond()
        {
            var identifiers = new InstrumentIdentifiers(
                "UKGovtBond",
                "RD01D",
                "RD01D",
                "UNITED KINGDOM OF GREAT BRITAI  0%32",
                string.Empty,
                "GB00B3D4VD98",
                "BBG0000VN0M0",
                "G924502U1",
                "UKTI 1.25 11/22/32 3MO Govt",
                string.Empty,
                "UKTI 1.25 11/22/32 3MO Govt",
                string.Empty);

            var financialInstrument = new FinancialInstrument(
                InstrumentTypes.Bond,
                identifiers,
                "UKGovtBond",
                "dbftfb",
                "GBX",
                "Government of United Kingdom");

            var market = this.OverTheCounter();

            return new SecurityMarketPair { Instrument = financialInstrument, Market = market };
        }

        /// <summary>
        /// The UK government bond secondary market.
        /// </summary>
        /// <returns>
        /// The <see cref="SecurityMarketPair"/>.
        /// </returns>
        private SecurityMarketPair UkGovernmentBondSecondaryMarket()
        {
            var identifiers = new InstrumentIdentifiers(
                "UKGovtBond",
                "RD01D",
                "RD01D",
                "UNITED KINGDOM OF GREAT BRITAI  0%32",
                string.Empty,
                "GB00B3D4VD98",
                "BBG0000VN0M0",
                "G924502U1",
                "UKTI 1.25 11/22/32 3MO Govt",
                string.Empty,
                "UKTI 1.25 11/22/32 3MO Govt",
                string.Empty);

            var financialInstrument = new FinancialInstrument(
                InstrumentTypes.Bond,
                identifiers,
                "UKGovtBond",
                "dbftfb",
                "GBX",
                "Government of United Kingdom");

            var market = this.Diversity();

            return new SecurityMarketPair { Instrument = financialInstrument, Market = market };
        }

        /// <summary>
        /// The diversity market.
        /// </summary>
        /// <returns>
        /// The <see cref="Market"/>.
        /// </returns>
        private Market Diversity()
        {
            return new Market("99", "Diversity", "Diversity", MarketTypes.STOCKEXCHANGE);
        }

        /// <summary>
        /// The european phone maker security.
        /// </summary>
        /// <returns>
        /// The <see cref="SecurityMarketPair"/>.
        /// </returns>
        private SecurityMarketPair Vodafone()
        {
            var identifiers = new InstrumentIdentifiers(
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
                "Vodafone Lon",
                string.Empty);

            var financialInstrument = new FinancialInstrument(
                InstrumentTypes.Equity,
                identifiers,
                "Vodafone",
                "entspb",
                "GBX",
                "Vodafone plc");

            var market = this.LondonStockExchange();

            return new SecurityMarketPair { Instrument = financialInstrument, Market = market };
        }

        /// <summary>
        /// The london stock exchange.
        /// </summary>
        /// <returns>
        /// The <see cref="Market"/>.
        /// </returns>
        private Market LondonStockExchange()
        {
            return new Market("0", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);
        }

        /// <summary>
        /// The security market pair.
        /// </summary>
        public class SecurityMarketPair
        {
            /// <summary>
            /// Gets or sets the instrument.
            /// </summary>
            public FinancialInstrument Instrument { get; set; }

            /// <summary>
            /// Gets or sets the market.
            /// </summary>
            public Market Market { get; set; }
        }
    }
}