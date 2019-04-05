using System;
using System.Collections.Generic;
using System.Linq;
using DataImport.Disk_IO.Shared;
using DataImport.Disk_IO.TradeFile.Interfaces;
using Domain.Core.Trading.Orders;
using Microsoft.Extensions.Logging;
using SharedKernel.Files.Orders;
using SharedKernel.Files.Orders.Interfaces;

namespace DataImport.Disk_IO.TradeFile
{
    public class UploadTradeFileProcessor : BaseUploadOrderFileProcessor, IUploadTradeFileProcessor
    {
        private readonly IOrderFileToOrderSerialiser _orderFileSerialiser;
        private readonly IOrderFileValidator _orderFileValidator;

        public UploadTradeFileProcessor(
            IOrderFileToOrderSerialiser csvToDtoMapper,
            IOrderFileValidator tradeFileCsvValidator,
            ILogger<UploadTradeFileProcessor> logger)
            : base(logger)
        {
            _orderFileSerialiser = csvToDtoMapper ?? throw new ArgumentNullException(nameof(csvToDtoMapper));
            _orderFileValidator = tradeFileCsvValidator ?? throw new ArgumentNullException(nameof(tradeFileCsvValidator));
        }

        protected override void MapRecord(
            OrderFileContract record,
            List<Order> marketUpdates,
            List<OrderFileContract> failedMarketUpdateReads)
        {
            Logger.LogInformation($"Upload Trade File Processor about to validate record {record?.RowId}");
            var validationResult = _orderFileValidator.Validate(record);
            
            if (!validationResult.IsValid)
            {
                Logger.LogInformation($"Upload Trade File Processor was unable to validate {record?.RowId}");
                _orderFileSerialiser.FailedParseTotal += 1;
                failedMarketUpdateReads.Add(record);

                if (validationResult.Errors.Any())
                {
                    var consolidatedErrorMessage = validationResult.Errors.Aggregate($"order {record?.OrderId}", (a, b) => a + " " + b.ErrorMessage);
                    Logger.LogWarning(consolidatedErrorMessage);
                }

                return;
            }

            var mappedRecord = _orderFileSerialiser.Map(record);
            if (mappedRecord != null)
            {
                Logger.LogInformation($"Upload Trade File Processor successfully validated and mapped record {record?.RowId}");
                marketUpdates.Add(mappedRecord);
            }
            Logger.LogInformation($"Upload Trade File Processor did not successfully map record {record?.RowId}");
        }

        protected override void CheckAndLogFailedParsesFromDtoMapper(string path)
        {
            if (_orderFileSerialiser.FailedParseTotal > 0)
            {
                Logger.LogError($"{UploadFileProcessorName} had {_orderFileSerialiser.FailedParseTotal} rows with errors when parsing the input CSV file ({path})");
            }

            _orderFileSerialiser.FailedParseTotal = 0;
        }
    }
}