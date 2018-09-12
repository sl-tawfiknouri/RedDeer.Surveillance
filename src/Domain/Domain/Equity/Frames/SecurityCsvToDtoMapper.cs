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

            var cfiCode = csv.SecurityCfi;
            var tickerSymbol = csv.TickerSymbol;

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

            if (!decimal.TryParse(csv.SpreadAsk, out var spreadAsk))
            {
                FailedParseTotal += 1;
                _logger?.LogError("Failed to parse security tick csv due to being passed an unparseable spread ask price");

                return null;
            }

            if (!decimal.TryParse(csv.SpreadBid, out var spreadBid))
            {
                FailedParseTotal += 1;
                _logger?.LogError("Failed to parse security tick csv due to being passed an unparseable spread bid price");

                return null;
            }

            if (!decimal.TryParse(csv.SpreadPrice, out var spreadPrice))
            {
                FailedParseTotal += 1;
                _logger?.LogError("Failed to parse security tick csv due to being passed an unparseable spread price");

                return null;
            }

            var spread = new Spread(
                new Price(spreadAsk, csv.SpreadCurrency),
                new Price(spreadBid, csv.SpreadCurrency),
                new Price(spreadPrice, csv.SpreadCurrency));

            var security = new Security(
                new SecurityIdentifiers(
                    csv.SecurityClientIdentifier,
                    csv.SecuritySedol,
                    csv.SecurityIsin,
                    csv.SecurityFigi), 
                csv.SecurityName);

            return new SecurityTick(security, cfiCode, tickerSymbol, spread, new Volume(volume), timeStamp);
        }
    }
}