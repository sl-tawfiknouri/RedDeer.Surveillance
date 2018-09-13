using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using Relay.Disk_IO.Interfaces;

namespace Relay.Disk_IO
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

            var tradeOrders = new List<TFrame>();
            var failedTradeOrderReads = new List<TCsv>();

            using (var reader = File.OpenText(path))
            {
                var csv = new CsvReader(reader);
                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.MissingFieldFound = null;

                var csvRecords = new List<TCsv>();

                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    var record = MapToCsvDto(csv);

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

            CheckAndLogFailedParsesFromDtoMapper();

            return new UploadFileProcessorResult<TCsv, TFrame>(tradeOrders, failedTradeOrderReads);
        }

        protected abstract TCsv MapToCsvDto(CsvReader rawRecord);

        protected abstract void MapRecord(TCsv record, List<TFrame> tradeOrders, List<TCsv> failedTradeOrderReads);

        protected abstract void CheckAndLogFailedParsesFromDtoMapper();
    }
}