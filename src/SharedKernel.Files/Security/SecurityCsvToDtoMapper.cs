using System;
using Domain.Core.Financial.Assets;
using Domain.Core.Financial.Money;
using Domain.Core.Markets;
using Domain.Core.Markets.Timebars;
using Microsoft.Extensions.Logging;
using SharedKernel.Files.Security.Interfaces;

namespace SharedKernel.Files.Security
{
    public class SecurityCsvToDtoMapper : ISecurityCsvToDtoMapper
    {
        private readonly ILogger<SecurityCsvToDtoMapper> _logger;

        // ReSharper disable once UnusedMember.Global
        public SecurityCsvToDtoMapper()
        { }

        public SecurityCsvToDtoMapper(ILogger<SecurityCsvToDtoMapper> logger)
        {
            _logger = logger;
        }

        public int FailedParseTotal { get; set; }

        public EquityInstrumentIntraDayTimeBar Map(FinancialInstrumentTimeBarCsv csv)
        {
            if (csv == null)
            {
                FailedParseTotal += 1;
                _logger?.LogError("Failed to parse security tick csv due to being passed a null value");
                return null;
            }

            var failedRead = false;
            if (!int.TryParse(csv.Volume, out var volume))
            {
                _logger?.LogError($"Failed to parse security tick csv due to being passed an unparseable volume {csv.Volume} for row {csv.RowId}");
                failedRead = true;
            }

            if (!int.TryParse(csv.DailyVolume, out var dailyVolume))
            {
                _logger?.LogError($"Failed to parse security tick csv due to being passed an unparseable daily volume {csv.DailyVolume} for row {csv.RowId}");
                failedRead = true;
            }

            if (!DateTime.TryParse(csv.Timestamp, out var timeStamp))
            {
                _logger?.LogError($"Failed to parse security tick csv due to being passed an unparseable timestamp {csv.Timestamp} for row {csv.RowId}");
                failedRead = true;
            }

            decimal marketCap = 0;
            if (!string.IsNullOrWhiteSpace(csv.MarketCap)
                && !decimal.TryParse(csv.MarketCap, out marketCap))
            {
                _logger?.LogError($"Failed to parse security tick csv due to being passed an unparseable market cap {csv.MarketCap} for row {csv.RowId}");
                failedRead = true;
            }

            decimal spreadAsk = 0;
            if (!string.IsNullOrWhiteSpace(csv.Ask)
                && !decimal.TryParse(csv.Ask, out spreadAsk))
            {
                _logger?.LogError($"Failed to parse security tick csv due to being passed an unparseable spread ask price {csv.Ask} for row {csv.RowId}");
                failedRead = true;
            }

            decimal spreadBid = 0;
            if (!string.IsNullOrWhiteSpace(csv.Bid)
                && !decimal.TryParse(csv.Bid, out spreadBid))
            {
                _logger?.LogError($"Failed to parse security tick csv due to being passed an unparseable spread bid price {csv.Bid} for row {csv.RowId}");
                failedRead = true;
            }

            decimal spreadPrice = 0;
            if (!string.IsNullOrWhiteSpace(csv.Price)
                && !decimal.TryParse(csv.Price, out spreadPrice))
            {
                _logger?.LogError($"Failed to parse security tick csv due to being passed an unparseable spread price {csv.Price} for row {csv.RowId}");
                failedRead = true;
            }

            var listedSecurities = 0;
            if (!string.IsNullOrWhiteSpace(csv.ListedSecurities)
                && !int.TryParse(csv.ListedSecurities, out listedSecurities))
            {
                _logger?.LogError($"Failed to parse listed securities due to being passed an unparseable listed security {csv.ListedSecurities} for row {csv.RowId}");
                failedRead = true;
            }

            decimal open = 0;
            if (!string.IsNullOrWhiteSpace(csv.Open)
                && !decimal.TryParse(csv.Open, out open))
            {
                _logger?.LogError($"Failed to parse open price due to being passed an unparseable price {csv.Open} for row {csv.RowId}");
                failedRead = true;
            }

            decimal close = 0;
            if (!string.IsNullOrWhiteSpace(csv.Close)
                && !decimal.TryParse(csv.Close, out close))
            {
                _logger?.LogError($"Failed to parse close price due to being passed an unparseable price {csv.Close} for row {csv.RowId}");
                failedRead = true;
            }

            decimal high = 0;
            if (!string.IsNullOrWhiteSpace(csv.High)
                && !decimal.TryParse(csv.High, out high))
            {
                _logger?.LogError($"Failed to parse high price due to being passed an unparseable price {csv.High} for row {csv.RowId}");
                failedRead = true;
            }

            decimal low = 0;
            if (!string.IsNullOrWhiteSpace(csv.Low)
                && !decimal.TryParse(csv.Low, out low))
            {
                _logger?.LogError($"Failed to parse low price due to being passed an unparseable price {csv.Low} for row {csv.RowId}");
                failedRead = true;
            }

            if (failedRead)
            {
                FailedParseTotal += 1;
                return null;
            }

            var security = BuildSecurity(csv);
            var spread = BuildSpread(csv, spreadAsk, spreadBid, spreadPrice, new Volume(volume));
            var intradayPrices = BuildIntradayPrices(csv, open, close, high, low);
            var dailySummary = new DailySummaryTimeBar(marketCap, csv.Currency, intradayPrices, listedSecurities, new Volume(dailyVolume), timeStamp);
            var market = new Market(string.Empty, csv.MarketIdentifierCode, csv.MarketName, MarketTypes.STOCKEXCHANGE);

            return new EquityInstrumentIntraDayTimeBar(
                security,
                spread,
                dailySummary,
                timeStamp,
                market);
        }

        private FinancialInstrument BuildSecurity(FinancialInstrumentTimeBarCsv csv)
        {
            return new FinancialInstrument(
                InstrumentTypes.Equity,
                new InstrumentIdentifiers(
                    string.Empty,
                    string.Empty,
                    null,
                    csv.SecurityClientIdentifier,
                    csv.Sedol,
                    csv.Isin,
                    csv.Figi,
                    csv.Cusip,
                    csv.ExchangeSymbol,
                    csv.Lei,
                    csv.BloombergTicker),
                csv.SecurityName,
                csv.Cfi,
                csv.Currency,
                csv.IssuerIdentifier);
        }

        private SpreadTimeBar BuildSpread(
            FinancialInstrumentTimeBarCsv csv,
            decimal spreadAsk,
            decimal spreadBid,
            decimal spreadPrice,
            Volume spreadVolume)
        {
            return new SpreadTimeBar(
                new Money(spreadAsk, csv.Currency),
                new Money(spreadBid, csv.Currency),
                new Money(spreadPrice, csv.Currency),
                spreadVolume);
        }

        private IntradayPrices BuildIntradayPrices(
            FinancialInstrumentTimeBarCsv csv,
            decimal open,
            decimal close,
            decimal high,
            decimal low)
        {
            var openPrice =
                open != 0
                ? new Money(open, csv.Currency)
                : (Money?)null;

            var closePrice =
                close != 0
                ? new Money(close, csv.Currency)
                : (Money?)null;

            var highPrice =
                high != 0
                ? new Money(high, csv.Currency)
                : (Money?)null;

            var lowPrice =
                low != 0
                ? new Money(low, csv.Currency)
                : (Money?)null;

            var intradayPrices = new IntradayPrices(
                openPrice,
                closePrice,
                highPrice,
                lowPrice);

            return intradayPrices;
        }
    }
}