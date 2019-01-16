using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using DomainV2.Files;
using DomainV2.Files.AllocationFile.Interfaces;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;

namespace DataImport.Disk_IO.AllocationFile
{
    public class AllocationFileProcessor : BaseUploadFileProcessor<AllocationFileCsv, OrderAllocation>
    {
        private readonly IAllocationFileCsvToOrderAllocationMapper _allocationMapper;
        private readonly IAllocationFileCsvValidator _allocationFileValidator;
        private readonly ILogger<AllocationFileProcessor> _logger;

        public AllocationFileProcessor(
            IAllocationFileCsvToOrderAllocationMapper allocationMapper,
            IAllocationFileCsvValidator allocationFileValidator,
            ILogger<AllocationFileProcessor> logger
            )
            : base(logger, "Allocation File Processor")
        {
            _allocationMapper = allocationMapper ?? throw new ArgumentNullException(nameof(allocationMapper));
            _allocationFileValidator = allocationFileValidator ?? throw new ArgumentNullException(nameof(allocationFileValidator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void MapRecord(
            AllocationFileCsv record, 
            List<OrderAllocation> marketUpdates,
            List<AllocationFileCsv> failedMarketUpdateReads)
        {
            Logger.LogInformation($"{UploadFileProcessorName} about to validate record {record?.RowId}");
            var validationResult = _allocationFileValidator.Validate(record);

            if (!validationResult.IsValid)
            {
                Logger.LogInformation($"{UploadFileProcessorName} was unable to validate {record?.RowId}");
                _allocationMapper.FailedParseTotal += 1;
                failedMarketUpdateReads.Add(record);

                if (validationResult.Errors.Any())
                {
                    var consolidatedErrorMessage = validationResult.Errors.Aggregate(string.Empty, (a, b) => a + " " + b.ErrorMessage);
                    Logger.LogWarning(consolidatedErrorMessage);
                }

                return;
            }

            var mappedRecord = _allocationMapper.Map(record);
            if (mappedRecord != null)
            {
                Logger.LogInformation($"{UploadFileProcessorName} successfully validated and mapped record {record?.RowId}");
                marketUpdates.Add(mappedRecord);
            }
            Logger.LogInformation($"{UploadFileProcessorName} did not successfully map record {record?.RowId}");
        }

        protected override void CheckAndLogFailedParsesFromDtoMapper(string path)
        {
            if (_allocationMapper.FailedParseTotal > 0)
            {
                _logger.LogError($"{UploadFileProcessorName} had {_allocationMapper.FailedParseTotal} rows with errors when parsing the input CSV file {path}");
            }

            _allocationMapper.FailedParseTotal = 0;
        }

        protected override AllocationFileCsv MapToCsvDto(CsvReader rawRecord, int rowId)
        {
            if (rawRecord == null)
            {
                Logger.LogInformation($"{UploadFileProcessorName} received a null record to map with row id {rowId}");
                return null;
            }

            Logger.LogInformation($"{UploadFileProcessorName} about to map raw record to csv dto");
            var tradeCsv = new AllocationFileCsv
            {
                OrderId = PreProcess(rawRecord["OrderId"]),
                Fund = PreProcess(rawRecord["Fund"]),
                Strategy = PreProcess(rawRecord["Strategy"]),
                ClientAccountId = PreProcess(rawRecord["ClientAccountId"]),
                OrderFilledVolume = PreProcess(rawRecord["OrderFilledVolume"]),
                RowId = rowId
            };

            Logger.LogInformation($"{UploadFileProcessorName} has mapped raw record to csv dto");

            return tradeCsv;
        }
    }
}
