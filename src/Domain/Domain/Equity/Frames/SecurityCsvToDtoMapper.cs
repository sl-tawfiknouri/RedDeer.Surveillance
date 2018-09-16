using System;
using Domain.Equity.Frames.Interfaces;
using Microsoft.Extensions.Logging;

namespace Domain.Equity.Frames
{
    public class SecurityCsvToDtoMapper : ISecurityCsvToDtoMapper
    {
        private readonly ILogger<SecurityCsvToDtoMapper> _logger;

        public SecurityCsvToDtoMapper()
        { }

        public SecurityCsvToDtoMapper(ILogger<SecurityCsvToDtoMapper> logger)
        {
            _logger = logger;
        }

        public int FailedParseTotal { get; set; }

        public SecurityTick Map(SecurityTickCsv csv)
        {
            if (csv == null)
            {
                FailedParseTotal += 1;
                _logger?.LogError("Failed to parse security tick csv due to being passed a null value");
                return null;
            }

            if (!int.TryParse(csv.Volume, out var volume))
            {
                FailedParseTotal += 1;
                _logger?.LogError("Failed to parse security tick csv due to being passed an unparseable volume");

                return null;
            }

            if (!DateTime.TryParse(csv.Timestamp, out var timeStamp))
            {
                FailedParseTotal += 1;
                _logger?.LogError("Failed to parse security tick csv due to being passed an unparseable timestamp");

                return null;
            }

            decimal marketCap = 0;
            if (!string.IsNullOrWhiteSpace(csv.MarketCap)
                && !decimal.TryParse(csv.MarketCap, out marketCap))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse security tick csv due to being passed an unparseable market cap {csv.MarketCap}");

                return null;
            }

            decimal spreadAsk = 0;
            if (!string.IsNullOrWhiteSpace(csv.Ask)
                && !decimal.TryParse(csv.Ask, out spreadAsk))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse security tick csv due to being passed an unparseable spread ask price {csv.Ask}");

                return null;
            }

            decimal spreadBid = 0;
            if (!string.IsNullOrWhiteSpace(csv.Bid)
                && !decimal.TryParse(csv.Bid, out spreadBid))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse security tick csv due to being passed an unparseable spread bid price {csv.Bid}");

                return null;
            }

            decimal spreadPrice = 0;
            if (!string.IsNullOrWhiteSpace(csv.Price)
                && !decimal.TryParse(csv.Price, out spreadPrice))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse security tick csv due to being passed an unparseable spread price {csv.Price}");

                return null;
            }

            int listedSecurities = 0;
            if (!string.IsNullOrWhiteSpace(csv.ListedSecurities)
                && !int.TryParse(csv.ListedSecurities, out listedSecurities))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse listed securities due to being passed an unparseable listed security {csv.ListedSecurities}");

                return null;
            }

            decimal open = 0;
            if (!string.IsNullOrWhiteSpace(csv.Open)
                && !decimal.TryParse(csv.Open, out open))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse open price due to being passed an unparseable price {csv.Open}");

                return null;
            }

            decimal close = 0;
            if (!string.IsNullOrWhiteSpace(csv.Close)
                && !decimal.TryParse(csv.Close, out close))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse close price due to being passed an unparseable price {csv.Close}");

                return null;
            }

            decimal high = 0;
            if (!string.IsNullOrWhiteSpace(csv.High)
                && !decimal.TryParse(csv.High, out high))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse high price due to being passed an unparseable price {csv.High}");

                return null;
            }

            decimal low = 0;
            if (!string.IsNullOrWhiteSpace(csv.Low)
                && !decimal.TryParse(csv.Low, out low))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse low price due to being passed an unparseable price {csv.Low}");

                return null;
            }

            var security = BuildSecurity(csv);
            var spread = BuildSpread(csv, spreadAsk, spreadBid, spreadPrice);
            var intradayPrices = BuildIntradayPrices(csv, open, close, high, low);

            return new SecurityTick(security, spread, new Volume(volume), timeStamp, marketCap, intradayPrices, listedSecurities);
        }

        private Security BuildSecurity(SecurityTickCsv csv)
        {
            return new Security(
                new SecurityIdentifiers(
                    csv.SecurityClientIdentifier,
                    csv.Sedol,
                    csv.Isin,
                    csv.Figi,
                    csv.Cusip,
                    csv.ExchangeSymbol),
                csv.SecurityName,
                csv.Cfi);
        }

        private Spread BuildSpread(
            SecurityTickCsv csv,
            decimal spreadAsk,
            decimal spreadBid,
            decimal spreadPrice)
        {
            return new Spread(
                new Price(spreadAsk, csv.Currency),
                new Price(spreadBid, csv.Currency),
                new Price(spreadPrice, csv.Currency));
        }

        private IntradayPrices BuildIntradayPrices(
            SecurityTickCsv csv,
            decimal open,
            decimal close,
            decimal high,
            decimal low)
        {
            var openPrice =
                open != 0
                ? (Price?)new Price(open, csv.Currency)
                : null;

            var closePrice =
                close != 0
                ? (Price?)new Price(close, csv.Currency)
                : null;

            var highPrice =
                high != 0
                ? (Price?)new Price(high, csv.Currency)
                : null;

            var lowPrice =
                low != 0
                ? (Price?)new Price(low, csv.Currency)
                : null;

            var intradayPrices = new IntradayPrices(
                openPrice,
                closePrice,
                highPrice,
                lowPrice);

            return intradayPrices;
        }
    }
}