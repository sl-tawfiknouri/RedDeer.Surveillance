using DomainV2.Equity.TimeBars.Interfaces;
using Microsoft.Extensions.Logging;

namespace DomainV2.Equity.TimeBars
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

        public SecurityTickCsv Map(FinancialInstrumentTimeBar financialInstrumentTimeBar)
        {
            if (financialInstrumentTimeBar == null)
            {
                FailedMapTotal += 1;
                _logger?.LogError("Failed to map security tick to security tick csv due to being passed a null value");
                return null;
            }

            var securityTickCsv = new SecurityTickCsv
            {
                Volume = financialInstrumentTimeBar.Volume.Traded.ToString(),
                DailyVolume = financialInstrumentTimeBar.DailyVolume.Traded.ToString(),
                Timestamp = financialInstrumentTimeBar.TimeStamp.ToString(),
                MarketCap = financialInstrumentTimeBar.MarketCap?.ToString(),
                ListedSecurities = financialInstrumentTimeBar.ListedSecurities?.ToString(),

                Currency = financialInstrumentTimeBar.Spread.Price.Currency.Value,
                
                // Spread
                Ask = financialInstrumentTimeBar.Spread.Ask.Value.ToString(),
                Bid = financialInstrumentTimeBar.Spread.Bid.Value.ToString(),
                Price = financialInstrumentTimeBar.Spread.Price.Value.ToString(),
            };

            // Market
            if(financialInstrumentTimeBar.Market != null)
            {
                securityTickCsv.MarketIdentifierCode = financialInstrumentTimeBar.Market.MarketIdentifierCode;
                securityTickCsv.MarketName = financialInstrumentTimeBar.Market.Name;
            }

            // Intraday Prices
            if (financialInstrumentTimeBar.IntradayPrices != null)
            {
                securityTickCsv.Open = financialInstrumentTimeBar.IntradayPrices.Open?.Value.ToString();
                securityTickCsv.Close = financialInstrumentTimeBar.IntradayPrices.Close?.Value.ToString();
                securityTickCsv.Low = financialInstrumentTimeBar.IntradayPrices.Low?.Value.ToString();
                securityTickCsv.High = financialInstrumentTimeBar.IntradayPrices.High?.Value.ToString();
            }

            // Security Identifiers
            if (financialInstrumentTimeBar.Security != null)
            {
                // Security
                securityTickCsv.SecurityName = financialInstrumentTimeBar.Security.Name;
                securityTickCsv.Cfi = financialInstrumentTimeBar.Security.Cfi;
                securityTickCsv.IssuerIdentifier = financialInstrumentTimeBar.Security.IssuerIdentifier;

                // Security Identifiers
                securityTickCsv.SecurityClientIdentifier = financialInstrumentTimeBar.Security.Identifiers.ClientIdentifier;
                securityTickCsv.Sedol = financialInstrumentTimeBar.Security.Identifiers.Sedol;
                securityTickCsv.Isin = financialInstrumentTimeBar.Security.Identifiers.Isin;
                securityTickCsv.Figi = financialInstrumentTimeBar.Security.Identifiers.Figi;
                securityTickCsv.Cusip = financialInstrumentTimeBar.Security.Identifiers.Cusip;
                securityTickCsv.ExchangeSymbol = financialInstrumentTimeBar.Security.Identifiers.ExchangeSymbol;
                securityTickCsv.Lei = financialInstrumentTimeBar.Security.Identifiers.Lei;
                securityTickCsv.BloombergTicker = financialInstrumentTimeBar.Security.Identifiers.BloombergTicker;
            }

            return securityTickCsv;
        }
    }
}