namespace DataImport.Disk_IO.AllocationFile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CsvHelper;

    using DataImport.Disk_IO.AllocationFile.Interfaces;

    using Domain.Core.Trading.Orders;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Files.Allocations;
    using SharedKernel.Files.Allocations.Interfaces;

    public class AllocationFileProcessor : BaseUploadFileProcessor<AllocationFileContract, OrderAllocation>,
                                           IAllocationFileProcessor
    {
        private readonly IAllocationFileValidator _allocationFileValidator;

        private readonly IAllocationFileCsvToOrderAllocationSerialiser _allocationMapper;

        private readonly ILogger<AllocationFileProcessor> _logger;

        public AllocationFileProcessor(
            IAllocationFileCsvToOrderAllocationSerialiser allocationMapper,
            IAllocationFileValidator allocationFileValidator,
            ILogger<AllocationFileProcessor> logger)
            : base(logger, "Allocation File Processor")
        {
            this._allocationMapper = allocationMapper ?? throw new ArgumentNullException(nameof(allocationMapper));
            this._allocationFileValidator = allocationFileValidator
                                            ?? throw new ArgumentNullException(nameof(allocationFileValidator));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void CheckAndLogFailedParsesFromDtoMapper(string path)
        {
            if (this._allocationMapper.FailedParseTotal > 0)
                this._logger.LogError(
                    $"{this.UploadFileProcessorName} had {this._allocationMapper.FailedParseTotal} rows with errors when parsing the input CSV file {path}");

            this._allocationMapper.FailedParseTotal = 0;
        }

        protected override void MapRecord(
            AllocationFileContract record,
            List<OrderAllocation> marketUpdates,
            List<AllocationFileContract> failedMarketUpdateReads)
        {
            this.Logger.LogInformation($"{this.UploadFileProcessorName} about to validate record {record?.RowId}");
            var validationResult = this._allocationFileValidator.Validate(record);

            if (!validationResult.IsValid)
            {
                this.Logger.LogInformation($"{this.UploadFileProcessorName} was unable to validate {record?.RowId}");
                this._allocationMapper.FailedParseTotal += 1;
                failedMarketUpdateReads.Add(record);

                if (validationResult.Errors.Any())
                {
                    var consolidatedErrorMessage = validationResult.Errors.Aggregate(
                        string.Empty,
                        (a, b) => a + " " + b.ErrorMessage);
                    this.Logger.LogWarning(consolidatedErrorMessage);
                }

                return;
            }

            var mappedRecord = this._allocationMapper.Map(record);
            if (mappedRecord != null)
            {
                this.Logger.LogInformation(
                    $"{this.UploadFileProcessorName} successfully validated and mapped record {record?.RowId}");
                marketUpdates.Add(mappedRecord);
            }

            this.Logger.LogInformation(
                $"{this.UploadFileProcessorName} did not successfully map record {record?.RowId}");
        }

        protected override AllocationFileContract MapToCsvDto(CsvReader rawRecord, int rowId)
        {
            if (rawRecord == null)
            {
                this.Logger.LogInformation(
                    $"{this.UploadFileProcessorName} received a null record to map with row id {rowId}");
                return null;
            }

            this.Logger.LogInformation($"{this.UploadFileProcessorName} about to map raw record to csv dto");
            var tradeCsv = new AllocationFileContract
                               {
                                   OrderId = this.PreProcess(rawRecord["OrderId"]),
                                   Fund = this.PreProcess(rawRecord["Fund"]),
                                   Strategy = this.PreProcess(rawRecord["Strategy"]),
                                   ClientAccountId = this.PreProcess(rawRecord["ClientAccountId"]),
                                   OrderFilledVolume = this.PreProcess(rawRecord["OrderFilledVolume"]),
                                   RowId = rowId
                               };

            this.Logger.LogInformation($"{this.UploadFileProcessorName} has mapped raw record to csv dto");

            return tradeCsv;
        }
    }
}