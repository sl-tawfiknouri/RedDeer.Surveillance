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
                SecurityClientIdentifier = rawRecord[_csvConfig.SecurityTickClientIdentifierFieldName],
                SecuritySedol = rawRecord[_csvConfig.SecurityTickSedolFieldName],
                SecurityIsin =  rawRecord[_csvConfig.SecurityTickIsinFieldName],
                SecurityFigi = rawRecord[_csvConfig.SecurityTickFigiFieldName],
                SecurityCfi = rawRecord[_csvConfig.SecurityTickCfiFieldName],

                TickerSymbol = rawRecord[_csvConfig.SecurityTickTickerSymbolFieldName],
                SecurityName = rawRecord[_csvConfig.SecurityTickSecurityNameFieldName],
                SpreadAsk = rawRecord[_csvConfig.SecurityTickSpreadAskFieldName],
                SpreadBid = rawRecord[_csvConfig.SecurityTickSpreadBidFieldName],
                SpreadPrice = rawRecord[_csvConfig.SecurityTickSpreadPriceFieldName],

                SpreadCurrency = rawRecord[_csvConfig.SecurityTickSpreadCurrencyFieldName],
                VolumeTraded = rawRecord[_csvConfig.SecurityTickVolumeTradedFieldName],
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
                csv.WriteField(_csvConfig.SecurityTickCfiFieldName);
                csv.WriteField(_csvConfig.SecurityTickTickerSymbolFieldName);
                csv.WriteField(_csvConfig.SecurityTickSecurityNameFieldName);
                csv.WriteField(_csvConfig.SecurityTickSpreadAskFieldName);
                csv.WriteField(_csvConfig.SecurityTickSpreadBidFieldName);
                csv.WriteField(_csvConfig.SecurityTickSpreadPriceFieldName);
                csv.WriteField(_csvConfig.SecurityTickSpreadCurrencyFieldName);
                csv.WriteField(_csvConfig.SecurityTickVolumeTradedFieldName);
                csv.WriteField(_csvConfig.SecurityTickMarketCapFieldName);

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