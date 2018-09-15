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

            if (!int.TryParse(csv.VolumeTraded, out var volume))
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
            if (!string.IsNullOrWhiteSpace(csv.SpreadAsk)
                && !decimal.TryParse(csv.SpreadAsk, out spreadAsk))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse security tick csv due to being passed an unparseable spread ask price {csv.SpreadAsk}");

                return null;
            }

            decimal spreadBid = 0;
            if (!string.IsNullOrWhiteSpace(csv.SpreadBid)
                && !decimal.TryParse(csv.SpreadBid, out spreadBid))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse security tick csv due to being passed an unparseable spread bid price {csv.SpreadBid}");

                return null;
            }

            decimal spreadPrice = 0;
            if (!string.IsNullOrWhiteSpace(csv.SpreadPrice)
                && !decimal.TryParse(csv.SpreadPrice, out spreadPrice))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse security tick csv due to being passed an unparseable spread price {csv.SpreadPrice}");

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
            if (!string.IsNullOrWhiteSpace(csv.OpenPrice)
                && !decimal.TryParse(csv.OpenPrice, out open))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse open price due to being passed an unparseable price {csv.OpenPrice}");

                return null;
            }

            decimal close = 0;
            if (!string.IsNullOrWhiteSpace(csv.ClosePrice)
                && !decimal.TryParse(csv.ClosePrice, out close))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse close price due to being passed an unparseable price {csv.ClosePrice}");

                return null;
            }

            decimal high = 0;
            if (!string.IsNullOrWhiteSpace(csv.HighPrice)
                && !decimal.TryParse(csv.HighPrice, out high))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse high price due to being passed an unparseable price {csv.HighPrice}");

                return null;
            }

            decimal low = 0;
            if (!string.IsNullOrWhiteSpace(csv.LowPrice)
                && !decimal.TryParse(csv.LowPrice, out low))
            {
                FailedParseTotal += 1;
                _logger?.LogError($"Failed to parse low price due to being passed an unparseable price {csv.LowPrice}");

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
                    csv.SecuritySedol,
                    csv.SecurityIsin,
                    csv.SecurityFigi,
                    csv.SecurityCusip,
                    csv.SecurityExchangeSymbol),
                csv.SecurityName,
                csv.SecurityCfi);
        }

        private Spread BuildSpread(
            SecurityTickCsv csv,
            decimal spreadAsk,
            decimal spreadBid,
            decimal spreadPrice)
        {
            return new Spread(
                new Price(spreadAsk, csv.SecurityCurrency),
                new Price(spreadBid, csv.SecurityCurrency),
                new Price(spreadPrice, csv.SecurityCurrency));
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
                ? (Price?)new Price(open, csv.SecurityCurrency)
                : null;

            var closePrice =
                close != 0
                ? (Price?)new Price(close, csv.SecurityCurrency)
                : null;

            var highPrice =
                high != 0
                ? (Price?)new Price(high, csv.SecurityCurrency)
                : null;

            var lowPrice =
                low != 0
                ? (Price?)new Price(low, csv.SecurityCurrency)
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