using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using DataImport.Disk_IO.EquityFile.Interfaces;
using DomainV2.Equity.TimeBars;
using DomainV2.Equity.TimeBars.Interfaces;
using DomainV2.Financial;
using Microsoft.Extensions.Logging;

namespace DataImport.Disk_IO.EquityFile
{
    public class UploadEquityFileProcessor : BaseUploadFileProcessor<FinancialInstrumentTimeBarCsv, MarketTimeBarCollection>, IUploadEquityFileProcessor
    {
        private readonly ISecurityCsvToDtoMapper _csvToDtoMapper;

        public UploadEquityFileProcessor(
            ISecurityCsvToDtoMapper csvToDtoMapper,
            ILogger<UploadEquityFileProcessor> logger)
            : base(logger, "Upload Equity File Processor")
        {
            _csvToDtoMapper = csvToDtoMapper ?? throw new ArgumentNullException(nameof(csvToDtoMapper));
        }
        
        protected override FinancialInstrumentTimeBarCsv MapToCsvDto(CsvReader rawRecord, int rowId)
        {
            Logger.LogInformation($"UploadEquityFileProcessor about to map a raw record with row id {rowId}");
            if (rawRecord == null)
            {
                Logger.LogInformation($"UploadEquityFileProcessor received a null record. Returning");
                return null;
            }

            Logger.LogInformation($"UploadEquityFileProcessor mapping raw record from csv reader to dto");
            var tickCsv = new FinancialInstrumentTimeBarCsv
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
            FinancialInstrumentTimeBarCsv record,
            List<MarketTimeBarCollection> marketUpdates,
            List<FinancialInstrumentTimeBarCsv> failedMarketUpdateReads)
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
                .Where(to => to.Epoch == mappedRecord.TimeStamp)
                .ToList();

            if (matchingExchange.Any())
            {
                foreach (var match in matchingExchange)
                {
                    var newSecurities = new List<FinancialInstrumentTimeBar>(match.Securities) {mappedRecord};
                    var newExch = new MarketTimeBarCollection(match.Exchange, match.Epoch, newSecurities);
                    marketUpdates.Remove(match);
                    marketUpdates.Add(newExch);
                }
            }
            else
            {
                var exchange = new Market(string.Empty, record.MarketIdentifierCode, record.MarketName, MarketTypes.STOCKEXCHANGE);
                var exchangeFrame = new MarketTimeBarCollection(exchange, mappedRecord.TimeStamp, new List<FinancialInstrumentTimeBar> { mappedRecord });
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