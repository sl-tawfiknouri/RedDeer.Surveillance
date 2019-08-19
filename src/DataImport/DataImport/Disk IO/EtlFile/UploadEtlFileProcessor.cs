namespace DataImport.Disk_IO.EtlFile
{
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

    public class UploadEtlFileProcessor : BaseUploadOrderFileProcessor, IUploadEtlFileProcessor
    {
        private readonly IUploadConfiguration _configuration;

        private readonly IEtlFileValidator _etlFileValidator;

        private readonly IEtlUploadErrorStore _etlUploadErrorStore;

        private readonly IEmailNotificationMessageSender _messageSender;

        private readonly IOrderFileToOrderSerialiser _orderFileSerialiser;

        public UploadEtlFileProcessor(
            IOrderFileToOrderSerialiser orderFileSerialiser,
            IEtlFileValidator etlFileValidator,
            IEtlUploadErrorStore etlUploadErrorStore,
            IUploadConfiguration configuration,
            IEmailNotificationMessageSender messageSender,
            ILogger<UploadEtlFileProcessor> logger)
            : base(logger)
        {
            this._orderFileSerialiser =
                orderFileSerialiser ?? throw new ArgumentNullException(nameof(orderFileSerialiser));
            this._etlFileValidator = etlFileValidator ?? throw new ArgumentNullException(nameof(etlFileValidator));
            this._etlUploadErrorStore =
                etlUploadErrorStore ?? throw new ArgumentNullException(nameof(etlUploadErrorStore));
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this._messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        }

        protected override void CheckAndLogFailedParsesFromDtoMapper(string path)
        {
            if (this._orderFileSerialiser.FailedParseTotal > 0)
            {
                this.Logger.LogError(
                    $"{this.UploadFileProcessorName} had {this._orderFileSerialiser.FailedParseTotal} rows with errors when parsing the input CSV file ({path})");

                if (!string.IsNullOrWhiteSpace(this._configuration.DataImportEtlFailureNotifications))
                    this.DispatchEmailNotifications();
                else
                    this.Logger.LogInformation(
                        $"No ETL notification targets set {this._configuration.DataImportEtlFailureNotifications}");
            }

            this._orderFileSerialiser.FailedParseTotal = 0;
            this._etlUploadErrorStore.Clear();
        }

        protected override void MapRecord(
            OrderFileContract record,
            List<Order> marketUpdates,
            List<OrderFileContract> failedMarketUpdateReads)
        {
            this.Logger.LogInformation($"About to validate record {record?.RowId}");
            var validationResult = this._etlFileValidator.Validate(record);

            if (!validationResult.IsValid)
            {
                this.Logger.LogInformation($"Processor was unable to validate {record?.RowId}");
                this._orderFileSerialiser.FailedParseTotal += 1;
                failedMarketUpdateReads.Add(record);

                if (validationResult.Errors.Any())
                {
                    var consolidatedErrorMessage = validationResult.Errors.Aggregate(
                        string.Empty,
                        (a, b) => a + " " + b.ErrorMessage);
                    this.Logger.LogWarning(consolidatedErrorMessage);
                }

                this._etlUploadErrorStore.Add(record, validationResult.Errors);

                return;
            }

            var mappedRecord = this._orderFileSerialiser.Map(record);
            if (mappedRecord != null)
            {
                this.Logger.LogInformation($"Processor successfully validated and mapped record {record?.RowId}");
                marketUpdates.Add(mappedRecord);
                return;
            }

            this.Logger.LogInformation($"Processor did not successfully map record {record?.RowId}");
        }

        private void DispatchEmail(string errorBody)
        {
            var preamble =
                $"Uploaded file for surveillance received at {DateTime.UtcNow} (UTC) had validation errors. The file will not be processed until these errors are addressed. {Environment.NewLine} {Environment.NewLine}";
            var htmlTaggedPreamble = this.HtmlTagEnvironmentNewLines(preamble);

            var errorMessage = $"{htmlTaggedPreamble} {errorBody}";
            this.Logger.LogError(errorMessage);

            var trimmedTargets = this._configuration.DataImportEtlFailureNotifications.Split(',')
                .Where(i => !string.IsNullOrWhiteSpace(i)).Select(y => y.Trim()).ToList();

            if (!trimmedTargets.Any())
            {
                this.Logger.LogInformation(
                    $"No ETL notification targets set {this._configuration.DataImportEtlFailureNotifications}");
                return;
            }

            var message = new SendSimpleEmailToRecipient
                              {
                                  Subject = "Surveillance File Validation",
                                  Message = errorMessage,
                                  OverrideDefaultFromAddress = false,
                                  ToAddresses = trimmedTargets,
                                  IsHtml = true
                              };

            this._messageSender.Send(message);
        }

        private void DispatchEmailNotifications()
        {
            // each error record is roughly around 1kb, limit of 256k for message. Under utilise sent rows to allow for very bad error states.
            var errorBody = this._etlUploadErrorStore.SerialisedErrors(100).ToList().Take(3);

            foreach (var error in errorBody)
            {
                var htmlTaggedBody = this.HtmlTagEnvironmentNewLines(error);
                this.DispatchEmail(htmlTaggedBody);
            }
        }

        /// <summary>
        ///     This function is order dependant - if there is html in situ for the text body its transformation is potentially
        ///     dangerous
        /// </summary>
        private string HtmlTagEnvironmentNewLines(string body)
        {
            if (string.IsNullOrWhiteSpace(body)) return string.Empty;

            const string htmlNewLineTag = " <br/> ";
            body = body.Replace(Environment.NewLine, htmlNewLineTag);

            return body;
        }
    }
}