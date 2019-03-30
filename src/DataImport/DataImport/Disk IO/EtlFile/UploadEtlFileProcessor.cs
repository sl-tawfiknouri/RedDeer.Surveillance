using System;
using System.Collections.Generic;
using System.Linq;
using Contracts.Email;
using DataImport.Configuration.Interfaces;
using DataImport.Disk_IO.EtlFile.Interfaces;
using DataImport.Disk_IO.Shared;
using DataImport.MessageBusIO.Interfaces;
using Domain.Core.Trading.Orders;
using Microsoft.Extensions.Logging;
using SharedKernel.Files.Orders;
using SharedKernel.Files.Orders.Interfaces;

namespace DataImport.Disk_IO.EtlFile
{
    public class UploadEtlFileProcessor : BaseUploadOrderFileProcessor, IUploadEtlFileProcessor
    {
        private readonly IOrderFileToOrderSerialiser _orderFileSerialiser;
        private readonly IEtlFileValidator _etlFileValidator;
        private readonly IEtlUploadErrorStore _etlUploadErrorStore;
        private readonly IUploadConfiguration _configuration;
        private readonly IEmailNotificationMessageSender _messageSender;

        public UploadEtlFileProcessor(
            IOrderFileToOrderSerialiser orderFileSerialiser,
            IEtlFileValidator etlFileValidator,
            IEtlUploadErrorStore etlUploadErrorStore,
            IUploadConfiguration configuration,
            IEmailNotificationMessageSender messageSender,
            ILogger<UploadEtlFileProcessor> logger)
            : base(logger)
        {
            _orderFileSerialiser = orderFileSerialiser ?? throw new ArgumentNullException(nameof(orderFileSerialiser));
            _etlFileValidator = etlFileValidator ?? throw new ArgumentNullException(nameof(etlFileValidator));
            _etlUploadErrorStore = etlUploadErrorStore ?? throw new ArgumentNullException(nameof(etlUploadErrorStore));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        }

        protected override void MapRecord(
            OrderFileContract record,
            List<Order> marketUpdates,
            List<OrderFileContract> failedMarketUpdateReads)
        {
            Logger.LogInformation($"About to validate record {record?.RowId}");
            var validationResult = _etlFileValidator.Validate(record);

            if (!validationResult.IsValid)
            {
                Logger.LogInformation($"Processor was unable to validate {record?.RowId}");
                _orderFileSerialiser.FailedParseTotal += 1;
                failedMarketUpdateReads.Add(record);

                if (validationResult.Errors.Any())
                {
                    var consolidatedErrorMessage = validationResult.Errors.Aggregate(string.Empty, (a, b) => a + " " + b.ErrorMessage);
                    Logger.LogWarning(consolidatedErrorMessage);
                }

                _etlUploadErrorStore.Add(record, validationResult.Errors);

                return;
            }

            var mappedRecord = _orderFileSerialiser.Map(record);
            if (mappedRecord != null)
            {
                Logger.LogInformation($"Processor successfully validated and mapped record {record?.RowId}");
                marketUpdates.Add(mappedRecord);
            }

            Logger.LogInformation($"Processor did not successfully map record {record?.RowId}");
        }

        protected override void CheckAndLogFailedParsesFromDtoMapper(string path)
        {
            if (_orderFileSerialiser.FailedParseTotal > 0)
            {
                Logger.LogError($"{UploadFileProcessorName} had {_orderFileSerialiser.FailedParseTotal} rows with errors when parsing the input CSV file ({path})");

                if (!string.IsNullOrWhiteSpace(_configuration.DataImportEtlFailureNotifications))
                {
                    DispatchEmailNotifications();
                }
                else
                {
                    Logger.LogInformation($"No ETL notification targets set {_configuration.DataImportEtlFailureNotifications}");
                }
            }

            _orderFileSerialiser.FailedParseTotal = 0;
            _etlUploadErrorStore.Clear();
        }

        private void DispatchEmailNotifications()
        {
            var errorMessage = _etlUploadErrorStore.SerialisedErrors();
            Logger.LogError(errorMessage);

            var trimmedTargets =
                _configuration
                    .DataImportEtlFailureNotifications
                    .Split(',')
                    .Where(i => !string.IsNullOrWhiteSpace(i))
                    .Select(y => y.Trim());

            if (!trimmedTargets.Any())
            {
                Logger.LogInformation($"No ETL notification targets set {_configuration.DataImportEtlFailureNotifications}");
                return;
            }

            _messageSender.Send(new SendEmailToRecipient());
        }
    }
}
