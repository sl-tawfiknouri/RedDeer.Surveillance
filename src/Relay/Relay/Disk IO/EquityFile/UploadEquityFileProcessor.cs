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
                MarketCap = rawRecord[_csvConfig.SecurityTickMarketCapFieldName]
            };
        }

        protected override void MapRecord(
            SecurityTickCsv record,
            List<ExchangeFrame> tradeOrders,
            List<SecurityTickCsv> failedTradeOrderReads)
        {
            var mappedRecord = _csvToDtoMapper.Map(record);

            var exchange = tradeOrders.FirstOrDefault();

            if (exchange == null)
            {
                exchange = 
                    new ExchangeFrame(
                        new StockExchange(new Market.MarketId(record.MarketIdentifierCode), record.MarketName),
                        new List<SecurityTick>());
            }

            if (mappedRecord != null)
            {
                var newSecurities = new List<SecurityTick>(exchange.Securities) {mappedRecord};
                // for market items our top level item itself is the aggregation so it can hold metadata
                tradeOrders.RemoveAll(i => true);
                tradeOrders.Add(new ExchangeFrame(exchange.Exchange, newSecurities));
            }
            else
            {
                failedTradeOrderReads.Add(record);
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