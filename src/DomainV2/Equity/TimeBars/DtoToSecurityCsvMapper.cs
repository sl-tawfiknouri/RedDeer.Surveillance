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

        public FinancialInstrumentTimeBarCsv Map(FinancialInstrumentTimeBar financialInstrumentTimeBar)
        {
            if (financialInstrumentTimeBar == null)
            {
                FailedMapTotal += 1;
                _logger?.LogError("Failed to map security tick to financial instrument time bar csv due to being passed a null value");
                return null;
            }

            var financialInstrumentTimeBarCsv = new FinancialInstrumentTimeBarCsv
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
                financialInstrumentTimeBarCsv.MarketIdentifierCode = financialInstrumentTimeBar.Market.MarketIdentifierCode;
                financialInstrumentTimeBarCsv.MarketName = financialInstrumentTimeBar.Market.Name;
            }

            // Intraday Prices
            if (financialInstrumentTimeBar.IntradayPrices != null)
            {
                financialInstrumentTimeBarCsv.Open = financialInstrumentTimeBar.IntradayPrices.Open?.Value.ToString();
                financialInstrumentTimeBarCsv.Close = financialInstrumentTimeBar.IntradayPrices.Close?.Value.ToString();
                financialInstrumentTimeBarCsv.Low = financialInstrumentTimeBar.IntradayPrices.Low?.Value.ToString();
                financialInstrumentTimeBarCsv.High = financialInstrumentTimeBar.IntradayPrices.High?.Value.ToString();
            }

            // Security Identifiers
            if (financialInstrumentTimeBar.Security != null)
            {
                // Security
                financialInstrumentTimeBarCsv.SecurityName = financialInstrumentTimeBar.Security.Name;
                financialInstrumentTimeBarCsv.Cfi = financialInstrumentTimeBar.Security.Cfi;
                financialInstrumentTimeBarCsv.IssuerIdentifier = financialInstrumentTimeBar.Security.IssuerIdentifier;

                // Security Identifiers
                financialInstrumentTimeBarCsv.SecurityClientIdentifier = financialInstrumentTimeBar.Security.Identifiers.ClientIdentifier;
                financialInstrumentTimeBarCsv.Sedol = financialInstrumentTimeBar.Security.Identifiers.Sedol;
                financialInstrumentTimeBarCsv.Isin = financialInstrumentTimeBar.Security.Identifiers.Isin;
                financialInstrumentTimeBarCsv.Figi = financialInstrumentTimeBar.Security.Identifiers.Figi;
                financialInstrumentTimeBarCsv.Cusip = financialInstrumentTimeBar.Security.Identifiers.Cusip;
                financialInstrumentTimeBarCsv.ExchangeSymbol = financialInstrumentTimeBar.Security.Identifiers.ExchangeSymbol;
                financialInstrumentTimeBarCsv.Lei = financialInstrumentTimeBar.Security.Identifiers.Lei;
                financialInstrumentTimeBarCsv.BloombergTicker = financialInstrumentTimeBar.Security.Identifiers.BloombergTicker;
            }

            return financialInstrumentTimeBarCsv;
        }
    }
}