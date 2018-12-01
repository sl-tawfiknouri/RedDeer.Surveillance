using System;
using Domain.Equity.Interfaces;

namespace Domain.Equity
{
    public struct SecurityIdentifiers : ISecurityIdentifiers
    {
        public SecurityIdentifiers(
            string id,
            string reddeerId,
            string clientIdentifier,
            string sedol,
            string isin,
            string figi,
            string cusip,
            string exchangeSymbol,
            string lei,
            string bloombergTicker)
        {
            Id = id ?? string.Empty;
            ReddeerId = reddeerId ?? string.Empty;
            ClientIdentifier = clientIdentifier ?? string.Empty;
            Sedol = sedol ?? string.Empty;
            Isin = isin ?? string.Empty;
            Figi = figi ?? string.Empty;
            Cusip = cusip ?? string.Empty;
            ExchangeSymbol = exchangeSymbol ?? string.Empty;
            Lei = lei ?? string.Empty;
            BloombergTicker = bloombergTicker ?? string.Empty;
        }

        /// <summary>
        /// Primary key id
        /// </summary>
        public string Id { get; }
        public string ReddeerId { get; }
        public string ClientIdentifier { get; }
        public string Sedol { get; }
        public string Isin { get; }
        public string Figi { get; }
        public string Cusip { get; }
        public string ExchangeSymbol { get; }
        public string Lei { get; }
        public string BloombergTicker { get; }

        public override int GetHashCode()
        {
            if (string.IsNullOrWhiteSpace(ReddeerId))
            {
                return 0;
            }

            return ReddeerId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SecurityIdentifiers))
            {
                return false;
            }

            var otherId = (SecurityIdentifiers)obj;

            if (!string.IsNullOrWhiteSpace(ReddeerId)
                && string.Equals(ReddeerId, otherId.ReddeerId, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(ClientIdentifier)
                && string.Equals(ClientIdentifier, otherId.ClientIdentifier, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(Sedol)
                && string.Equals(Sedol, otherId.Sedol, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(Isin)
                && string.Equals(Isin, otherId.Isin, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(Figi)
                && string.Equals(Figi, otherId.Figi, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(Cusip)
                && string.Equals(Cusip, otherId.Cusip, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(BloombergTicker)
                && string.Equals(BloombergTicker, otherId.BloombergTicker, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Don't show reddeer id otherwise it'll be exposed to clients
        /// </summary>
        public override string ToString()
        {
            return $"Client Id: {ClientIdentifier} | Sedol {Sedol} | Isin {Isin} | Figi {Figi} | Cusip {Cusip} | Exchange Symbol {ExchangeSymbol} | Lei {Lei} | Bloomberg Ticker {BloombergTicker}";
        }
    }
}