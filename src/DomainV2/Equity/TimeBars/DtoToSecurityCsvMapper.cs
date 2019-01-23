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

        public FinancialInstrumentTimeBarCsv Map(EquityInstrumentIntraDayTimeBar equityInstrumentIntraDayTimeBar)
        {
            if (equityInstrumentIntraDayTimeBar == null)
            {
                FailedMapTotal += 1;
                _logger?.LogError("Failed to map security tick to financial instrument time bar csv due to being passed a null value");
                return null;
            }

            var financialInstrumentTimeBarCsv = new FinancialInstrumentTimeBarCsv
            {
                Volume = equityInstrumentIntraDayTimeBar.SpreadTimeBar.Volume.Traded.ToString(),
                DailyVolume = equityInstrumentIntraDayTimeBar.DailySummaryTimeBar.DailyVolume.Traded.ToString(),
                Timestamp = equityInstrumentIntraDayTimeBar.TimeStamp.ToString("yyyy/MM/dd hh:mm:ss"),
                MarketCap = equityInstrumentIntraDayTimeBar.DailySummaryTimeBar.MarketCap?.ToString(),
                ListedSecurities = equityInstrumentIntraDayTimeBar.DailySummaryTimeBar.ListedSecurities?.ToString(),

                Currency = equityInstrumentIntraDayTimeBar.SpreadTimeBar.Price.Currency.Value,
                
                // Spread
                Ask = equityInstrumentIntraDayTimeBar.SpreadTimeBar.Ask.Value.ToString(),
                Bid = equityInstrumentIntraDayTimeBar.SpreadTimeBar.Bid.Value.ToString(),
                Price = equityInstrumentIntraDayTimeBar.SpreadTimeBar.Price.Value.ToString(),
            };

            // Market
            if(equityInstrumentIntraDayTimeBar.Market != null)
            {
                financialInstrumentTimeBarCsv.MarketIdentifierCode = equityInstrumentIntraDayTimeBar.Market.MarketIdentifierCode;
                financialInstrumentTimeBarCsv.MarketName = equityInstrumentIntraDayTimeBar.Market.Name;
            }

            // Intraday Prices
            if (equityInstrumentIntraDayTimeBar.DailySummaryTimeBar.IntradayPrices != null)
            {
                financialInstrumentTimeBarCsv.Open = equityInstrumentIntraDayTimeBar.DailySummaryTimeBar.IntradayPrices.Open?.Value.ToString();
                financialInstrumentTimeBarCsv.Close = equityInstrumentIntraDayTimeBar.DailySummaryTimeBar.IntradayPrices.Close?.Value.ToString();
                financialInstrumentTimeBarCsv.Low = equityInstrumentIntraDayTimeBar.DailySummaryTimeBar.IntradayPrices.Low?.Value.ToString();
                financialInstrumentTimeBarCsv.High = equityInstrumentIntraDayTimeBar.DailySummaryTimeBar.IntradayPrices.High?.Value.ToString();
            }

            // Security Identifiers
            if (equityInstrumentIntraDayTimeBar.Security != null)
            {
                // Security
                financialInstrumentTimeBarCsv.SecurityName = equityInstrumentIntraDayTimeBar.Security.Name;
                financialInstrumentTimeBarCsv.Cfi = equityInstrumentIntraDayTimeBar.Security.Cfi;
                financialInstrumentTimeBarCsv.IssuerIdentifier = equityInstrumentIntraDayTimeBar.Security.IssuerIdentifier;

                // Security Identifiers
                financialInstrumentTimeBarCsv.SecurityClientIdentifier = equityInstrumentIntraDayTimeBar.Security.Identifiers.ClientIdentifier;
                financialInstrumentTimeBarCsv.Sedol = equityInstrumentIntraDayTimeBar.Security.Identifiers.Sedol;
                financialInstrumentTimeBarCsv.Isin = equityInstrumentIntraDayTimeBar.Security.Identifiers.Isin;
                financialInstrumentTimeBarCsv.Figi = equityInstrumentIntraDayTimeBar.Security.Identifiers.Figi;
                financialInstrumentTimeBarCsv.Cusip = equityInstrumentIntraDayTimeBar.Security.Identifiers.Cusip;
                financialInstrumentTimeBarCsv.ExchangeSymbol = equityInstrumentIntraDayTimeBar.Security.Identifiers.ExchangeSymbol;
                financialInstrumentTimeBarCsv.Lei = equityInstrumentIntraDayTimeBar.Security.Identifiers.Lei;
                financialInstrumentTimeBarCsv.BloombergTicker = equityInstrumentIntraDayTimeBar.Security.Identifiers.BloombergTicker;
            }

            return financialInstrumentTimeBarCsv;
        }
    }
}