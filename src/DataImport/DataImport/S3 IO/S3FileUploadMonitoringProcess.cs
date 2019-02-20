using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataImport.Configuration.Interfaces;
using DataImport.Disk_IO.AllocationFile.Interfaces;
using DataImport.Disk_IO.Interfaces;
using DataImport.S3_IO.Interfaces;
using Microsoft.Extensions.Logging;
using Utilities.Aws_IO;
using Utilities.Aws_IO.Interfaces;

namespace DataImport.S3_IO
{
    public class S3FileUploadMonitoringProcess : IS3FileUploadMonitoringProcess
    {
        private CancellationTokenSource _cts;
        private AwsResusableCancellationToken _token;

        private readonly object _lock = new object();

        private IUploadAllocationFileMonitor _uploadAllocationFileMonitor;
        private IUploadTradeFileMonitor _uploadTradeFileMonitor;
        private readonly IFileUploadMessageMapper _mapper;
        private readonly IUploadConfiguration _configuration;
        private readonly IAwsQueueClient _queueClient;
        private readonly IAwsS3Client _s3Client;
        private readonly ILogger<S3FileUploadMonitoringProcess> _logger;

        public S3FileUploadMonitoringProcess(
            IFileUploadMessageMapper mapper,
            IUploadConfiguration configuration,
            IAwsQueueClient queueClient,
            IAwsS3Client s3Client,
            ILogger<S3FileUploadMonitoringProcess> logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _queueClient = queueClient ?? throw new ArgumentNullException(nameof(queueClient));
            _s3Client = s3Client ?? throw new ArgumentNullException(nameof(s3Client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initialise(
            IUploadAllocationFileMonitor uploadAllocationFileMonitor,
            IUploadTradeFileMonitor uploadTradeFileMonitor)
        {
            try
            {
                 _logger.LogInformation("Initialising file upload monitoring process");

                _uploadAllocationFileMonitor = uploadAllocationFileMonitor;
                _uploadTradeFileMonitor = uploadTradeFileMonitor;
                _cts = new CancellationTokenSource();
                _token = new AwsResusableCancellationToken();

                _queueClient.SubscribeToQueueAsync(_configuration.DataImportS3UploadQueueName, ReadMessage, _cts.Token, _token);
            }
            catch (Exception e)
            {
                _logger.LogError($"S3FileUploadMonitoringProcess - {e.Message} - {e.InnerException?.Message}");
            }
        }

        private async Task ReadMessage(string messageId, string messageBody)
        {
            try
            {
                _logger.LogInformation($"S3 upload picked up a message with id of {messageId} from the queue");

                var dto = _mapper.Map(messageBody);

                if (dto == null)
                {
                    _logger.LogError($"S3 File Upload Monitoring Processor tried to process a message {messageId} but when deserialising the message it had a null result");

                    return;
                }

                if (dto.FileSize == 0)
                {
                    _logger.LogInformation($"S3FileUploadMonitoringProcess deserialised message {messageId} but found the file size to be 0. Assuming this is the preceding message to the actual file uploaded message.");

                    return;
                }

                var directoryName = Path.GetDirectoryName(dto.FileName)?.ToLower() ?? string.Empty;
                var splitPath = directoryName.Split(Path.DirectorySeparatorChar).Last();

                _logger.LogInformation($"S3Processor received message for {directoryName}");

                switch (splitPath)
                {
                    case "surveillance-trade":
                        await ProcessTradeFile(
                            dto,
                            _configuration.DataImportTradeFileFtpDirectoryPath,
                            _configuration.DataImportTradeFileUploadDirectoryPath);
                        break;
                    case "surveillance-allocation":
                        var paf = ProcessAllocationFile(
                            dto,
                            _configuration.DataImportAllocationFileFtpDirectoryPath,
                            _configuration.DataImportAllocationFileUploadDirectoryPath);
                        paf.Wait();
                        break;
                    default:
                        _logger.LogInformation($"S3 File Upload Monitoring Process did not recognise the directory of a file. Ignoring file. {dto.FileName}");
                        return;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("S3FileUploadMonitoringProcess: " + e.Message);
            }
        }

        private async Task ProcessTradeFile(FileUploadMessageDto dto, string ftpDirectoryPath, string uploadDirectoryPath)
        {
            await ProcessFile(dto, 3, ftpDirectoryPath);

            var files = Directory.EnumerateFiles(ftpDirectoryPath).ToList();
            var fileCount = files.Count;
            _logger.LogInformation($"Found {fileCount} files in the local ftp folder. Moving to the processing folder.");

            foreach (var file in files)
            {
                try
                {
                    _logger.LogInformation($"S3 processing trade file {file}");
                    var result = _uploadTradeFileMonitor.ProcessFile(file);

                    if (result == false)
                    {
                        _logger.LogInformation($"S3 Processor cancellation token initiated for {file}");
                        _token.Cancel = true;
                    }

                    if (File.Exists(file))
                    {
                        _logger.LogInformation($"S3 completed processing {file}. Now deleting {file}.");
                        File.Delete(file);
                    }
                    else
                    {
                        _logger.LogInformation($"S3 Processor could not find file {file}");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"S3 File Upload Monitoring Process moving process trade file {file} to {uploadDirectoryPath} {e.Message}");
                    continue;
                }
            }

            _logger.LogInformation($"Moved all {fileCount}.");
        }

        private async Task ProcessAllocationFile(FileUploadMessageDto dto, string ftpDirectoryPath, string uploadDirectoryPath)
        {
            await ProcessFile(dto, 3, ftpDirectoryPath);

            var files = Directory.EnumerateFiles(ftpDirectoryPath).ToList();
            var fileCount = files.Count;
            _logger.LogInformation($"Found {fileCount} files in the local ftp folder. Moving to the processing folder.");

            foreach (var file in files)
            {
                try
                {
                    _logger.LogInformation($"S3 processing trade file {file}");
                    var result = _uploadAllocationFileMonitor.ProcessFile(file);

                    if (result == false)
                    {
                        _logger.LogInformation($"S3 Processor cancellation token initiated for {file}");
                        _token.Cancel = true;
                    }

                    if (File.Exists(file))
                    {
                        _logger.LogInformation($"S3 completed processing {file}. Now deleting {file}.");
                        File.Delete(file);
                    }
                    else
                    {
                        _logger.LogInformation($"S3 Processor could not find file {file}");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"S3 File Upload Monitoring Process moving process allocation file {file} to {uploadDirectoryPath} {e.Message}");
                    continue;
                }
            }

            _logger.LogInformation($"S3 File Upload Moved all {fileCount} files.");
        }

        private async Task ProcessFile(FileUploadMessageDto dto, int retries, string ftpDirectoryPath)
        {
            var filePath = Path.GetFileName(dto.FileName) ?? string.Empty;

            if (string.IsNullOrWhiteSpace(filePath))
            {
                _logger.LogInformation($"S3 File Upload Monitoring Process had a null or empty file path when reading from S3 message");
                return;
            }
            else
            {
                _logger.LogInformation($"S3 Processor about to process {filePath}");
            }

            var newPath = Path.Combine(ftpDirectoryPath, filePath);
            var result = await _s3Client.RetrieveFile(dto.Bucket, dto.FileName, dto.VersionId, newPath);

            if (retries <= 0)
                _logger.LogInformation($"S3 Process File ran out of retries for processing file {dto.FileName}");

            if (result)
            {
                _logger.LogInformation($"S3 Processor successfully retrieved file from {dto.Bucket} {dto.FileName} {dto.VersionId} to {newPath}");
                return;
            }

            if (retries <= 0)
            {
                _logger.LogError($"S3 Processor ran out of retries trying to fetch from {dto.Bucket} {dto.FileName} {dto.VersionId} to {newPath}");
                return;
            }

            await ProcessFile(dto, retries - 1, ftpDirectoryPath);
        }

        public void Terminate()
        {
            _logger.LogInformation($"S3 Processor terminating");
            _cts?.Cancel();
        }
    }
}
