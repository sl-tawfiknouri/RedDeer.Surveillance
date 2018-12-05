﻿using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using DataImport.Disk_IO.EquityFile.Interfaces;
using DomainV2.Equity.Frames;
using DomainV2.Equity.Frames.Interfaces;
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
            if (rawRecord == null)
            {
                return null;
            }

            return new SecurityTickCsv
            {
                Timestamp = rawRecord["Timestamp"],
                MarketIdentifierCode = rawRecord["MarketIdentifierCode"],
                MarketName = rawRecord["MarketName"],

                SecurityName = rawRecord["SecurityClientIdentifier"],
                Currency = rawRecord["Currency"],

                SecurityClientIdentifier = rawRecord["SecurityClientIdentifier"],
                Sedol = rawRecord["Sedol"],
                Isin =  rawRecord["Isin"],
                Figi = rawRecord["Figi"],
                ExchangeSymbol = rawRecord["ExchangeSymbol"],
                Cusip = rawRecord["Cusip"],

                Cfi = rawRecord["Cfi"],

                Ask = rawRecord["Ask"],
                Bid = rawRecord["Bid"],
                Price = rawRecord["Price"],

                Open = rawRecord["Open"],
                Close = rawRecord["Close"],
                High = rawRecord["High"],
                Low = rawRecord["Low"],

                Volume = rawRecord["Volume"],
                ListedSecurities = rawRecord["ListedSecurities"],
                MarketCap = rawRecord["MarketCap"],

                IssuerIdentifier = rawRecord["IssuerIdentifier"],
                Lei = rawRecord["Lei"],
                BloombergTicker = rawRecord["BloombergTicker"],
                DailyVolume = rawRecord["DailyVolume"],

                RowId = rowId
            };
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
                .Where(to => to.Exchange?.Id?.Id == mappedRecord.Market?.Id.Id)
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
                var exchange = new Market(new Market.MarketId(record.MarketIdentifierCode), record.MarketName);
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