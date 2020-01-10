namespace DataImport.S3_IO
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using DataImport.Configuration.Interfaces;
    using DataImport.Disk_IO.AllocationFile.Interfaces;
    using DataImport.Disk_IO.EtlFile.Interfaces;
    using DataImport.Disk_IO.Interfaces;
    using DataImport.S3_IO.Interfaces;

    using Infrastructure.Network.Aws;
    using Infrastructure.Network.Aws.Interfaces;

    using Microsoft.Extensions.Logging;

    public class S3FileUploadMonitoringProcess : IS3FileUploadMonitoringProcess
    {
        private readonly IUploadConfiguration _configuration;

        private readonly ILogger<S3FileUploadMonitoringProcess> _logger;

        private readonly IFileUploadMessageMapper _mapper;

        private readonly IAwsQueueClient _queueClient;

        private readonly IAwsS3Client _s3Client;

        private CancellationTokenSource _cts;

        private AwsResusableCancellationToken _token;

        private IUploadAllocationFileMonitor _uploadAllocationFileMonitor;

        private IUploadEtlFileMonitor _uploadEtlFileMonitor;

        private IUploadTradeFileMonitor _uploadTradeFileMonitor;

        public S3FileUploadMonitoringProcess(
            IFileUploadMessageMapper mapper,
            IUploadConfiguration configuration,
            IAwsQueueClient queueClient,
            IAwsS3Client s3Client,
            ILogger<S3FileUploadMonitoringProcess> logger)
        {
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this._queueClient = queueClient ?? throw new ArgumentNullException(nameof(queueClient));
            this._s3Client = s3Client ?? throw new ArgumentNullException(nameof(s3Client));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initialise(
            IUploadAllocationFileMonitor uploadAllocationFileMonitor,
            IUploadTradeFileMonitor uploadTradeFileMonitor,
            IUploadEtlFileMonitor uploadEtlFileMonitor)
        {
            try
            {
                this._logger.LogInformation("Initialising file upload monitoring process");

                this._uploadAllocationFileMonitor = uploadAllocationFileMonitor;
                this._uploadTradeFileMonitor = uploadTradeFileMonitor;
                this._uploadEtlFileMonitor = uploadEtlFileMonitor;
                this._cts = new CancellationTokenSource();
                this._token = new AwsResusableCancellationToken();

                this._queueClient.SubscribeToQueueAsync(
                    this._configuration.DataImportS3UploadQueueName,
                    this.ReadMessage,
                    this._cts.Token,
                    this._token);
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"S3FileUploadMonitoringProcess");
            }
        }

        public void Terminate()
        {
            this._logger.LogInformation("S3 Processor terminating");
            this._cts?.Cancel();
        }

        private async Task ProcessAllocationFile(
            FileUploadMessageDto dto,
            string ftpDirectoryPath,
            string uploadDirectoryPath)
        {
            bool ProcessFile(string x)
            {
                return this._uploadAllocationFileMonitor.ProcessFile(x);
            }

            await this.ProcessS3File(dto, ftpDirectoryPath, uploadDirectoryPath, ProcessFile);
        }

        private async Task ProcessEtlFile(FileUploadMessageDto dto, string ftpDirectoryPath, string uploadDirectoryPath)
        {
            bool ProcessFile(string x)
            {
                return this._uploadEtlFileMonitor.ProcessFile(x);
            }

            await this.ProcessS3File(dto, ftpDirectoryPath, uploadDirectoryPath, ProcessFile);
        }

        private async Task ProcessFile(FileUploadMessageDto dto, int retries, string ftpDirectoryPath)
        {
            if (dto == null)
            {
                this._logger.LogError("File upload dto was null, exiting process file");
                return;
            }

            var versionId = string.IsNullOrWhiteSpace(dto?.VersionId) ? string.Empty : $"{dto?.VersionId}-";
            var fileName = Path.GetFileName(dto.FileName) ?? string.Empty;

            if (string.IsNullOrWhiteSpace(fileName))
            {
                this._logger.LogInformation(
                    $"S3 File Upload Monitoring Process had a null or empty file path when reading from S3 message.  File ({dto}).");
                return;
            }

            this._logger.LogInformation($"S3 Processor about to process file ({dto}).");

            fileName = $"{versionId}{fileName}";
            var destinationFileName = Path.Combine(ftpDirectoryPath, fileName);
            var result = await this._s3Client.RetrieveFile(
                             dto.Bucket,
                             dto.FileName,
                             dto.VersionId,
                             destinationFileName);

            if (retries <= 0)
                this._logger.LogInformation($"S3 Process File ran out of retries for processing file {dto.FileName}");

            if (result)
            {
                this._logger.LogInformation(
                    $"S3 Processor successfully retrieved file ({dto}) to {destinationFileName}");
                return;
            }

            if (retries <= 0)
            {
                this._logger.LogError($"S3 Processor ran out of retries trying to fetch file ({dto}) to {destinationFileName}");
                return;
            }

            await this.ProcessFile(dto, retries - 1, ftpDirectoryPath);
        }

        private async Task ProcessS3File(
            FileUploadMessageDto dto,
            string ftpDirectoryPath,
            string uploadDirectoryPath,
            Func<string, bool> processFileDelegate)
        {
            await this.ProcessFile(dto, 3, ftpDirectoryPath);

            var files = Directory.EnumerateFiles(ftpDirectoryPath).ToList();
            var fileCount = files.Count;
            this._logger.LogInformation(
                $"Found {fileCount} files in the local ftp folder. Moving to the processing folder.");

            foreach (var file in files)
                try
                {
                    this._logger.LogInformation($"S3 processing file {file}");
                    var result = processFileDelegate(file);

                    if (!result) this._logger.LogError($"S3 processing file {file} was unsuccessful");

                    if (File.Exists(file))
                    {
                        this._logger.LogInformation($"S3 completed processing {file}. Now deleting {file}.");
                        File.Delete(file);
                    }
                    else
                    {
                        this._logger.LogInformation($"S3 Processor could not find file {file}");
                    }
                }
                catch (Exception e)
                {
                    this._logger.LogError(e, $"S3 File Upload Monitoring Process moving process trade file {file} to {uploadDirectoryPath}");
                }

            this._logger.LogInformation($"Moved all {fileCount}.");
        }

        private async Task ProcessTradeFile(
            FileUploadMessageDto dto,
            string ftpDirectoryPath,
            string uploadDirectoryPath)
        {
            bool ProcessFile(string x)
            {
                return this._uploadTradeFileMonitor.ProcessFile(x);
            }

            await this.ProcessS3File(dto, ftpDirectoryPath, uploadDirectoryPath, ProcessFile);
        }

        private async Task ReadMessage(string messageId, string messageBody)
        {
            try
            {
                this._logger.LogDebug($"S3 upload picked up a message with id of {messageId} from the queue (MessageBody: '{messageBody}')");

                var dto = this._mapper.Map(messageBody);

                if (dto == null)
                {
                    this._logger.LogError($"Tried to process a message {messageId} but when deserialising the message '{messageBody}' it had a null result");
                    return;
                }

                if (dto.FileSize == 0)
                {
                    this._logger.LogDebug($"Deserialised message {messageId} but found the file size to be 0. Assuming this is the preceding message to the actual file uploaded message. File ({dto}).");
                    return;
                }

                var directoryName = Path.GetDirectoryName(dto.FileName)?.ToLower() ?? string.Empty;
                var splitPath = directoryName.Split(Path.DirectorySeparatorChar).Last();

                this._logger.LogInformation($"S3Processor received message for '{directoryName}' directory.");

                switch (splitPath)
                {
                    case "surveillance-trade":
                        await this.ProcessTradeFile(
                            dto,
                            this._configuration.DataImportTradeFileFtpDirectoryPath,
                            this._configuration.DataImportTradeFileUploadDirectoryPath);
                        break;
                    case "surveillance-allocation":
                        await this.ProcessAllocationFile(
                            dto,
                            this._configuration.DataImportAllocationFileFtpDirectoryPath,
                            this._configuration.DataImportAllocationFileUploadDirectoryPath);
                        break;
                    case "surveillance-etl-order":
                        await this.ProcessEtlFile(
                            dto,
                            this._configuration.DataImportEtlFileFtpDirectoryPath,
                            this._configuration.DataImportEtlFileUploadDirectoryPath);
                        break;
                    default:
                        this._logger.LogInformation($"S3 File Upload Monitoring Process did not recognise the directory of a file. Ignoring file '{dto.FileName}' for MessageId '{messageId}'.");
                        return;
                }
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"Exception while processing messageid '{messageId}' with body '{messageBody}'.");
                throw e;
            }
        }
    }
}