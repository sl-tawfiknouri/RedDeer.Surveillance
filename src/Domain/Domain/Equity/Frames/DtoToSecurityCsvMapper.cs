using Domain.Equity.Frames.Interfaces;
using Microsoft.Extensions.Logging;

namespace Domain.Equity.Frames
{
    public class DtoToSecurityCsvMapper : IDtoToSecurityCsvMapper
    {
        private readonly ILogger<SecurityCsvToDtoMapper> _logger;

        // ReSharper disable once UnusedMember.Global
        public DtoToSecurityCsvMapper()
        { }

        public DtoToSecurityCsvMapper(ILogger<SecurityCsvToDtoMapper> logger)
        {
            _logger = logger;
        }

        public int FailedMapTotal { get; set; }

        public SecurityTickCsv Map(SecurityTick securityTick)
        {
            if (securityTick == null)
            {
                FailedMapTotal += 1;
                _logger?.LogError("Failed to map security tick to security tick csv due to being passed a null value");
                return null;
            }

            var securityTickCsv = new SecurityTickCsv
            {
                Volume = securityTick.Volume.Traded.ToString(),
                DailyVolume = securityTick.DailyVolume.Traded.ToString(),
                Timestamp = securityTick.TimeStamp.ToString(),
                MarketCap = securityTick.MarketCap?.ToString(),
                ListedSecurities = securityTick.ListedSecurities?.ToString(),

                Currency = securityTick.Spread.Price.Currency, // TODO: check currency field. Ask || Bid || Price? 
                
                // Spread
                Ask = securityTick.Spread.Ask.Value.ToString(),
                Bid = securityTick.Spread.Bid.Value.ToString(),
                Price = securityTick.Spread.Price.Value.ToString(),
            };

            // Market
            if(securityTick.Market != null)
            {
                securityTickCsv.MarketIdentifierCode = securityTick.Market.Id.Id;
                securityTickCsv.MarketName = securityTick.Market.Name;
            }

            // Intraday Prices
            if (securityTick.IntradayPrices != null)
            {
                securityTickCsv.Open = securityTick.IntradayPrices.Open?.Value.ToString();
                securityTickCsv.Close = securityTick.IntradayPrices.Close?.Value.ToString();
                securityTickCsv.Low = securityTick.IntradayPrices.Low?.Value.ToString();
                securityTickCsv.High = securityTick.IntradayPrices.High?.Value.ToString();
            }

            // Security Identifiers
            if (securityTick.Security != null)
            {
                // Security
                securityTickCsv.SecurityName = securityTick.Security.Name;
                securityTickCsv.Cfi = securityTick.Security.Cfi;
                securityTickCsv.IssuerIdentifier = securityTick.Security.IssuerIdentifier;

                // Security Identifiers
                securityTickCsv.SecurityClientIdentifier = securityTick.Security.Identifiers.ClientIdentifier;
                securityTickCsv.Sedol = securityTick.Security.Identifiers.Sedol;
                securityTickCsv.Isin = securityTick.Security.Identifiers.Isin;
                securityTickCsv.Figi = securityTick.Security.Identifiers.Figi;
                securityTickCsv.Cusip = securityTick.Security.Identifiers.Cusip;
                securityTickCsv.ExchangeSymbol = securityTick.Security.Identifiers.ExchangeSymbol;
                securityTickCsv.Lei = securityTick.Security.Identifiers.Lei;
                securityTickCsv.BloombergTicker = securityTick.Security.Identifiers.BloombergTicker;
            }

            return securityTickCsv;
        }
    }
}