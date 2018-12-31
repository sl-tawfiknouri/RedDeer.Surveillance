﻿using System;

namespace DomainV2.Financial
{
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
            string bloombergTicker) : this(
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
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty)
        { }

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
            string underlyingSedol,
            string underlyingIsin,
            string underlyingFigi,
            string underlyingCusip,
            string underlyingLei,
            string underlyingExchangeSymbol,
            string underlyingBloombergTicker,
            string underlyingClientIdentifier)
        {
            Id = id ?? string.Empty;
            ReddeerId = reddeerId ?? string.Empty;
            ReddeerEnrichmentId = reddeerEnrichmentId ?? string.Empty;
            ClientIdentifier = clientIdentifier ?? string.Empty;
            Sedol = sedol ?? string.Empty;
            Isin = isin ?? string.Empty;
            Figi = figi ?? string.Empty;
            Cusip = cusip ?? string.Empty;
            ExchangeSymbol = exchangeSymbol ?? string.Empty;
            Lei = lei ?? string.Empty;
            BloombergTicker = bloombergTicker ?? string.Empty;

            UnderlyingSedol = underlyingSedol ?? string.Empty;
            UnderlyingIsin = underlyingIsin ?? string.Empty;
            UnderlyingFigi = underlyingFigi ?? string.Empty;
            UnderlyingCusip = underlyingCusip ?? string.Empty;
            UnderlyingLei = underlyingLei ?? string.Empty;
            UnderlyingExchangeSymbol = underlyingExchangeSymbol ?? string.Empty;
            UnderlyingBloombergTicker = underlyingBloombergTicker ?? string.Empty;
            UnderlyingClientIdentifier = underlyingClientIdentifier ?? string.Empty;
        }

        /// <summary>
        /// Primary key id
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


        public string UnderlyingSedol { get; set; }
        public string UnderlyingIsin { get; set; }
        public string UnderlyingFigi { get; set; }
        public string UnderlyingCusip { get; set; }
        public string UnderlyingLei { get; set; }
        public string UnderlyingExchangeSymbol { get; set; }
        public string UnderlyingBloombergTicker { get; set; }
        public string UnderlyingClientIdentifier { get; set; }

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
            if (!(obj is InstrumentIdentifiers))
            {
                return false;
            }

            var otherId = (InstrumentIdentifiers)obj;

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