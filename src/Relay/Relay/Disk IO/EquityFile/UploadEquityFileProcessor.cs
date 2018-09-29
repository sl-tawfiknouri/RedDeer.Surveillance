using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using Domain.Equity.Frames;
using Domain.Equity.Frames.Interfaces;
using Domain.Market;
using Microsoft.Extensions.Logging;
using Relay.Disk_IO.EquityFile.Interfaces;

namespace Relay.Disk_IO.EquityFile
{
    public class UploadEquityFileProcessor : BaseUploadFileProcessor<SecurityTickCsv, ExchangeFrame>, IUploadEquityFileProcessor
    {
        private readonly ISecurityCsvToDtoMapper _csvToDtoMapper;
        private readonly ISecurityTickCsvConfig _csvConfig;

        public UploadEquityFileProcessor(
            ISecurityCsvToDtoMapper csvToDtoMapper,
            ISecurityTickCsvConfig csvConfig,
            ILogger<UploadEquityFileProcessor> logger)
            : base(logger, "Upload Equity File Processor")
        {
            _csvToDtoMapper = csvToDtoMapper ?? throw new ArgumentNullException(nameof(csvToDtoMapper));
            _csvConfig = csvConfig ?? throw new ArgumentNullException(nameof(csvConfig));
        }
        
        protected override SecurityTickCsv MapToCsvDto(CsvReader rawRecord)
        {
            if (rawRecord == null)
            {
                return null;
            }

            return new SecurityTickCsv
            {
                Timestamp = rawRecord[_csvConfig.SecurityTickTimestampFieldName],
                MarketIdentifierCode = rawRecord[_csvConfig.SecurityTickMarketIdentifierCodeFieldName],
                MarketName = rawRecord[_csvConfig.SecurityTickMarketNameFieldName],

                SecurityName = rawRecord[_csvConfig.SecurityTickSecurityNameFieldName],
                Currency = rawRecord[_csvConfig.SecurityTickCurrencyFieldName],

                SecurityClientIdentifier = rawRecord[_csvConfig.SecurityTickClientIdentifierFieldName],
                Sedol = rawRecord[_csvConfig.SecurityTickSedolFieldName],
                Isin =  rawRecord[_csvConfig.SecurityTickIsinFieldName],
                Figi = rawRecord[_csvConfig.SecurityTickFigiFieldName],
                ExchangeSymbol = rawRecord[_csvConfig.SecurityTickExchangeSymbolFieldName],
                Cusip = rawRecord[_csvConfig.SecurityTickCusipFieldName],

                Cfi = rawRecord[_csvConfig.SecurityTickCfiFieldName],

                Ask = rawRecord[_csvConfig.SecurityTickSpreadAskFieldName],
                Bid = rawRecord[_csvConfig.SecurityTickSpreadBidFieldName],
                Price = rawRecord[_csvConfig.SecurityTickSpreadPriceFieldName],

                Open = rawRecord[_csvConfig.SecurityTickOpenPriceFieldName],
                Close = rawRecord[_csvConfig.SecurityTickClosePriceFieldName],
                High = rawRecord[_csvConfig.SecurityTickHighPriceFieldName],
                Low = rawRecord[_csvConfig.SecurityTickLowPriceFieldName],

                Volume = rawRecord[_csvConfig.SecurityTickVolumeTradedFieldName],
                ListedSecurities = rawRecord[_csvConfig.SecurityTickListedSecuritiesFieldName],
                MarketCap = rawRecord[_csvConfig.SecurityTickMarketCapFieldName],

                IssuerIdentifier = rawRecord[_csvConfig.SecurityIssuerIdentifierFieldName],
                Lei = rawRecord[_csvConfig.SecurityLeiFieldName],
                BloombergTicker = rawRecord[_csvConfig.SecurityBloombergTicker],
                DailyVolume = rawRecord[_csvConfig.SecurityDailyVolumeFieldName]
            };
        }

        protected override void MapRecord(
            SecurityTickCsv record,
            List<ExchangeFrame> tradeOrders,
            List<SecurityTickCsv> failedTradeOrderReads)
        {
            var mappedRecord = _csvToDtoMapper.Map(record);

            if (mappedRecord == null)
            {
                failedTradeOrderReads.Add(record);
                return;
            }

            var matchingExchange = tradeOrders
                .Where(to => to.Exchange?.Id?.Id == mappedRecord.Market?.Id.Id)
                .Where(to => to.TimeStamp == mappedRecord.TimeStamp)
                .ToList();

            if (matchingExchange.Any())
            {
                foreach (var match in matchingExchange)
                {
                    var newSecurities = new List<SecurityTick>(match.Securities) {mappedRecord};
                    var newExch = new ExchangeFrame(match.Exchange, match.TimeStamp, newSecurities);
                    tradeOrders.Remove(match);
                    tradeOrders.Add(newExch);
                }
            }
            else
            {
                var exchange = new StockExchange(new Market.MarketId(record.MarketIdentifierCode), record.MarketName);
                var exchangeFrame = new ExchangeFrame(exchange, mappedRecord.TimeStamp, new List<SecurityTick> { mappedRecord });
                tradeOrders.Add(exchangeFrame);
            }
        }

        protected override void CheckAndLogFailedParsesFromDtoMapper()
        {
            if (_csvToDtoMapper.FailedParseTotal > 0)
            {
                Logger.LogError($"{UploadFileProcessorName} had {_csvToDtoMapper.FailedParseTotal} errors parsing the input CSV file");
            }
        }

        public void WriteFailedReadsToDisk(
            string path,
            string originalFileName,
            IReadOnlyCollection<SecurityTickCsv> failedReads)
        {
            if (failedReads == null
                || !failedReads.Any())
            {
                return;
            }

            var failedFileName = $"{originalFileName}-failed-read-{Guid.NewGuid()}.csv";
            var target = Path.Combine(path, failedFileName);

            using (TextWriter twriter = new StreamWriter(target))
            {
                var csv = new CsvWriter(twriter);

                // write out headers
                csv.WriteField(_csvConfig.SecurityTickTimestampFieldName);
                csv.WriteField(_csvConfig.SecurityTickMarketIdentifierCodeFieldName);
                csv.WriteField(_csvConfig.SecurityTickMarketNameFieldName);

                csv.WriteField(_csvConfig.SecurityTickClientIdentifierFieldName);
                csv.WriteField(_csvConfig.SecurityTickSedolFieldName);
                csv.WriteField(_csvConfig.SecurityTickIsinFieldName);
                csv.WriteField(_csvConfig.SecurityTickFigiFieldName);
                csv.WriteField(_csvConfig.SecurityTickCusipFieldName);
                csv.WriteField(_csvConfig.SecurityTickExchangeSymbolFieldName);

                csv.WriteField(_csvConfig.SecurityTickCfiFieldName);
                csv.WriteField(_csvConfig.SecurityTickSecurityNameFieldName);
                csv.WriteField(_csvConfig.SecurityTickSpreadAskFieldName);
                csv.WriteField(_csvConfig.SecurityTickSpreadBidFieldName);
                csv.WriteField(_csvConfig.SecurityTickSpreadPriceFieldName);
                csv.WriteField(_csvConfig.SecurityTickCurrencyFieldName);
                csv.WriteField(_csvConfig.SecurityTickVolumeTradedFieldName);
                csv.WriteField(_csvConfig.SecurityTickMarketCapFieldName);

                csv.WriteField(_csvConfig.SecurityTickListedSecuritiesFieldName);
                csv.WriteField(_csvConfig.SecurityTickOpenPriceFieldName);
                csv.WriteField(_csvConfig.SecurityTickClosePriceFieldName);
                csv.WriteField(_csvConfig.SecurityTickHighPriceFieldName);
                csv.WriteField(_csvConfig.SecurityTickLowPriceFieldName);

                csv.WriteField(_csvConfig.SecurityIssuerIdentifierFieldName);
                csv.WriteField(_csvConfig.SecurityLeiFieldName);
                csv.WriteField(_csvConfig.SecurityBloombergTicker);
                csv.WriteField(_csvConfig.SecurityDailyVolumeFieldName);

                csv.NextRecord();

                foreach (var rec in failedReads)
                {
                    if (rec == null)
                    {
                        continue;
                    }

                    csv.WriteRecord(rec);
                    csv.NextRecord();
                }
            }
        }
    }
}