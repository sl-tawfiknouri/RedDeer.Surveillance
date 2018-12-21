using System;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using DataImport.Disk_IO.Interfaces;
using Microsoft.Extensions.Logging;

namespace DataImport.Disk_IO
{
    public abstract class BaseUploadFileProcessor<TCsv, TFrame> : IBaseUploadFileProcessor<TCsv, TFrame>
    {
        protected readonly ILogger Logger;
        protected readonly string UploadFileProcessorName;

        protected BaseUploadFileProcessor(
            ILogger logger,
            string uploadFileProcessorName)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            UploadFileProcessorName = uploadFileProcessorName ?? string.Empty;
        }

        public UploadFileProcessorResult<TCsv, TFrame> Process(string path)
        {
            if (!File.Exists(path))
            {
                Logger.LogError($"{UploadFileProcessorName} did not find file {path}");

                return
                    new UploadFileProcessorResult<TCsv, TFrame>(
                        new List<TFrame>(),
                        new List<TCsv>());
            }

            Logger.LogInformation($"BaseUploadFileProcessor processing {path}");

            var tradeOrders = new List<TFrame>();
            var failedTradeOrderReads = new List<TCsv>();

            try
            {
                using (var reader = File.OpenText(path))
                {
                    var csv = new CsvReader(reader);
                    csv.Configuration.HasHeaderRecord = true;
                    csv.Configuration.MissingFieldFound = null;

                    var csvRecords = new List<TCsv>();

                    csv.Read();
                    csv.ReadHeader();

                    var row = 0;
                    while (csv.Read())
                    {
                        row += 1;
                        var record = MapToCsvDto(csv, row);

                        if (record == null)
                        {
                            continue;
                        }

                        csvRecords.Add(record);
                    }

                    foreach (var record in csvRecords)
                    {
                        MapRecord(record, tradeOrders, failedTradeOrderReads);
                    }

                    csv.Dispose();
                    reader.Dispose();
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }

            Logger.LogInformation($"BaseUploadFileProcessor processed {path}. Data in memory.");
            CheckAndLogFailedParsesFromDtoMapper(path);

            return new UploadFileProcessorResult<TCsv, TFrame>(tradeOrders, failedTradeOrderReads);
        }

        protected abstract TCsv MapToCsvDto(CsvReader rawRecord, int rowId);

        protected abstract void MapRecord(TCsv record, List<TFrame> marketUpdates, List<TCsv> failedMarketUpdateReads);

        protected abstract void CheckAndLogFailedParsesFromDtoMapper(string path);
    }
}