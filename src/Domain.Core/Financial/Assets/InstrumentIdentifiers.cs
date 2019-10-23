namespace Domain.Core.Financial.Assets
{
    using System;

    public struct InstrumentIdentifiers
    {
        public InstrumentIdentifiers(
            string id,
            string reddeerId,
            string reddeerEnrichmentId,
            string clientIdentifier,
            string sedol,
            string isin,
            string figi,
            string cusip,
            string exchangeSymbol,
            string lei,
            string bloombergTicker,
            string ric)
            : this(
                id,
                reddeerId,
                reddeerEnrichmentId,
                clientIdentifier,
                sedol,
                isin,
                figi,
                cusip,
                exchangeSymbol,
                lei,
                bloombergTicker,
                ric,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty)
        {
        }

        public InstrumentIdentifiers(
            string id,
            string reddeerId,
            string reddeerEnrichmentId,
            string clientIdentifier,
            string sedol,
            string isin,
            string figi,
            string cusip,
            string exchangeSymbol,
            string lei,
            string bloombergTicker,
            string ric,
            string underlyingSedol,
            string underlyingIsin,
            string underlyingFigi,
            string underlyingCusip,
            string underlyingLei,
            string underlyingExchangeSymbol,
            string underlyingBloombergTicker,
            string underlyingClientIdentifier,
            string underlyingRic)
        {
            this.Id = id ?? string.Empty;
            this.ReddeerId = reddeerId ?? string.Empty;
            this.ReddeerEnrichmentId = reddeerEnrichmentId ?? string.Empty;
            this.ClientIdentifier = clientIdentifier ?? string.Empty;
            this.Sedol = sedol ?? string.Empty;
            this.Isin = isin ?? string.Empty;
            this.Figi = figi ?? string.Empty;
            this.Cusip = cusip ?? string.Empty;
            this.ExchangeSymbol = exchangeSymbol ?? string.Empty;
            this.Lei = lei ?? string.Empty;
            this.BloombergTicker = bloombergTicker ?? string.Empty;
            this.Ric = ric ?? string.Empty;

            this.UnderlyingSedol = underlyingSedol ?? string.Empty;
            this.UnderlyingIsin = underlyingIsin ?? string.Empty;
            this.UnderlyingFigi = underlyingFigi ?? string.Empty;
            this.UnderlyingCusip = underlyingCusip ?? string.Empty;
            this.UnderlyingLei = underlyingLei ?? string.Empty;
            this.UnderlyingExchangeSymbol = underlyingExchangeSymbol ?? string.Empty;
            this.UnderlyingBloombergTicker = underlyingBloombergTicker ?? string.Empty;
            this.UnderlyingClientIdentifier = underlyingClientIdentifier ?? string.Empty;
            this.UnderlyingRic = underlyingRic ?? string.Empty;
        }

        public static InstrumentIdentifiers Null()
        {
            return new InstrumentIdentifiers(
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty);
        }

        /// <summary>
        ///     Primary key id
        /// </summary>
        public string Id { get; set; }

        public string ReddeerId { get; set; }

        public string ReddeerEnrichmentId { get; set; }

        public string ClientIdentifier { get; set; }

        public string Sedol { get; set; }

        public string Isin { get; set; }

        public string Figi { get; set; }

        public string Cusip { get; set; }

        public string ExchangeSymbol { get; set; }

        public string Lei { get; set; }

        public string BloombergTicker { get; set; }

        public string Ric { get; set; }

        public string UnderlyingSedol { get; set; }

        public string UnderlyingIsin { get; set; }

        public string UnderlyingFigi { get; set; }

        public string UnderlyingCusip { get; set; }

        public string UnderlyingLei { get; set; }

        public string UnderlyingExchangeSymbol { get; set; }

        public string UnderlyingBloombergTicker { get; set; }

        public string UnderlyingClientIdentifier { get; set; }

        public string UnderlyingRic { get; set; }

        public override int GetHashCode()
        {
            if (string.IsNullOrWhiteSpace(this.ReddeerId)) return 0;

            return this.ReddeerId.ToLower().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is InstrumentIdentifiers)) return false;

            var otherId = (InstrumentIdentifiers)obj;

            if (!string.IsNullOrWhiteSpace(this.ReddeerId) && string.Equals(
                    this.ReddeerId,
                    otherId.ReddeerId,
                    StringComparison.InvariantCultureIgnoreCase)) return true;

            if (!string.IsNullOrWhiteSpace(this.ClientIdentifier) && string.Equals(
                    this.ClientIdentifier,
                    otherId.ClientIdentifier,
                    StringComparison.InvariantCultureIgnoreCase)) return true;

            if (!string.IsNullOrWhiteSpace(this.Sedol) && string.Equals(
                    this.Sedol,
                    otherId.Sedol,
                    StringComparison.InvariantCultureIgnoreCase)) return true;

            if (!string.IsNullOrWhiteSpace(this.Figi) && string.Equals(
                    this.Figi,
                    otherId.Figi,
                    StringComparison.InvariantCultureIgnoreCase)) return true;

            if (!string.IsNullOrWhiteSpace(this.Cusip) && string.Equals(
                    this.Cusip,
                    otherId.Cusip,
                    StringComparison.InvariantCultureIgnoreCase)) return true;

            if (!string.IsNullOrWhiteSpace(this.Isin) && string.Equals(
                    this.Isin,
                    otherId.Isin,
                    StringComparison.InvariantCultureIgnoreCase)) return true;

            if (!string.IsNullOrWhiteSpace(this.BloombergTicker) && string.Equals(
                    this.BloombergTicker,
                    otherId.BloombergTicker,
                    StringComparison.InvariantCultureIgnoreCase)) return true;

            if (!string.IsNullOrWhiteSpace(this.Ric) && string.Equals(
                    this.Ric,
                    otherId.Ric,
                    StringComparison.InvariantCultureIgnoreCase)) return true;

            return false;
        }

        /// <summary>
        ///     Don't show reddeer id otherwise it'll be exposed to clients
        /// </summary>
        public override string ToString()
        {
            return
                $"Client Id: {this.ClientIdentifier} | Sedol {this.Sedol} | Isin {this.Isin} | Figi {this.Figi} | Cusip {this.Cusip} | Exchange Symbol {this.ExchangeSymbol} | Lei {this.Lei} | Bloomberg Ticker {this.BloombergTicker} | Ric {this.Ric}";
        }
    }
}