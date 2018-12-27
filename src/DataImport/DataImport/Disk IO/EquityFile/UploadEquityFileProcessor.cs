using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using DataImport.Disk_IO.EquityFile.Interfaces;
using DomainV2.Equity.Frames;
using DomainV2.Equity.Frames.Interfaces;
using DomainV2.Financial;
using Microsoft.Extensions.Logging;

namespace DataImport.Disk_IO.EquityFile
{
    public class UploadEquityFileProcessor : BaseUploadFileProcessor<SecurityTickCsv, ExchangeFrame>, IUploadEquityFileProcessor
    {
        private readonly ISecurityCsvToDtoMapper _csvToDtoMapper;

        public UploadEquityFileProcessor(
            ISecurityCsvToDtoMapper csvToDtoMapper,
            ILogger<UploadEquityFileProcessor> logger)
            : base(logger, "Upload Equity File Processor")
        {
            _csvToDtoMapper = csvToDtoMapper ?? throw new ArgumentNullException(nameof(csvToDtoMapper));
        }
        
        protected override SecurityTickCsv MapToCsvDto(CsvReader rawRecord, int rowId)
        {
            Logger.LogInformation($"UploadEquityFileProcessor about to map a raw record with row id {rowId}");
            if (rawRecord == null)
            {
                Logger.LogInformation($"UploadEquityFileProcessor received a null record. Returning");
                return null;
            }

            Logger.LogInformation($"UploadEquityFileProcessor mapping raw record from csv reader to dto");
            var tickCsv = new SecurityTickCsv
            {
                Timestamp = PreProcess(rawRecord["Timestamp"]),
                MarketIdentifierCode = PreProcess(rawRecord["MarketIdentifierCode"]),
                MarketName = PreProcess(rawRecord["MarketName"]),

                SecurityName = PreProcess(rawRecord["SecurityClientIdentifier"]),
                Currency = PreProcess(rawRecord["Currency"]),

                SecurityClientIdentifier = PreProcess(rawRecord["SecurityClientIdentifier"]),
                Sedol = PreProcess(rawRecord["Sedol"]),
                Isin = PreProcess(rawRecord["Isin"]),
                Figi = PreProcess(rawRecord["Figi"]),
                ExchangeSymbol = PreProcess(rawRecord["ExchangeSymbol"]),
                Cusip = PreProcess(rawRecord["Cusip"]),

                Cfi = PreProcess(rawRecord["Cfi"]),

                Ask = PreProcess(rawRecord["Ask"]),
                Bid = PreProcess(rawRecord["Bid"]),
                Price = PreProcess(rawRecord["Price"]),

                Open = PreProcess(rawRecord["Open"]),
                Close = PreProcess(rawRecord["Close"]),
                High = PreProcess(rawRecord["High"]),
                Low = PreProcess(rawRecord["Low"]),

                Volume = PreProcess(rawRecord["Volume"]),
                ListedSecurities = PreProcess(rawRecord["ListedSecurities"]),
                MarketCap = PreProcess(rawRecord["MarketCap"]),

                IssuerIdentifier = PreProcess(rawRecord["IssuerIdentifier"]),
                Lei = PreProcess(rawRecord["Lei"]),
                BloombergTicker = PreProcess(rawRecord["BloombergTicker"]),
                DailyVolume = PreProcess(rawRecord["DailyVolume"]),

                RowId = rowId
            };
            Logger.LogInformation($"UploadEquityFileProcessor completed mapping raw record {rowId}");

            return tickCsv;
        }

        protected override void MapRecord(
            SecurityTickCsv record,
            List<ExchangeFrame> marketUpdates,
            List<SecurityTickCsv> failedMarketUpdateReads)
        {
            var mappedRecord = _csvToDtoMapper.Map(record);

            if (mappedRecord == null)
            {
                Logger.LogInformation("UploadEquityFileProcessor read a null record, adding to failed reads");
                failedMarketUpdateReads.Add(record);
                return;
            }

            var matchingExchange = marketUpdates
                .Where(to => to.Exchange?.MarketIdentifierCode == mappedRecord.Market?.MarketIdentifierCode)
                .Where(to => to.TimeStamp == mappedRecord.TimeStamp)
                .ToList();

            if (matchingExchange.Any())
            {
                foreach (var match in matchingExchange)
                {
                    var newSecurities = new List<SecurityTick>(match.Securities) {mappedRecord};
                    var newExch = new ExchangeFrame(match.Exchange, match.TimeStamp, newSecurities);
                    marketUpdates.Remove(match);
                    marketUpdates.Add(newExch);
                }
            }
            else
            {
                var exchange = new Market(string.Empty, record.MarketIdentifierCode, record.MarketName, MarketTypes.STOCKEXCHANGE);
                var exchangeFrame = new ExchangeFrame(exchange, mappedRecord.TimeStamp, new List<SecurityTick> { mappedRecord });
                marketUpdates.Add(exchangeFrame);
            }
        }

        protected override void CheckAndLogFailedParsesFromDtoMapper(string path)
        {
            if (_csvToDtoMapper.FailedParseTotal > 0)
            {
                Logger.LogError($"{UploadFileProcessorName} had {_csvToDtoMapper.FailedParseTotal} rows with errors when parsing the input CSV file ({path})");
            }

            _csvToDtoMapper.FailedParseTotal = 0;
        }
    }
}