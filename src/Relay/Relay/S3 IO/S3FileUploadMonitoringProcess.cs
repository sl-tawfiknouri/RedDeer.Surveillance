using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Relay.Configuration.Interfaces;
using Relay.S3_IO.Interfaces;
using Utilities.Aws_IO.Interfaces;

namespace Relay.S3_IO
{
    public class S3FileUploadMonitoringProcess : IS3FileUploadMonitoringProcess
    {
        private CancellationTokenSource _cts;

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

        public void Initialise()
        {
            _logger.LogInformation("Initialising file upload monitoring process");

            _cts = new CancellationTokenSource();

            _queueClient.SubscribeToQueueAsync(_configuration.RelayS3UploadQueueName , ReadMessage, _cts.Token);
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

                switch (splitPath)
                {
                    case "surveillance-trade":
                        await ProcessTradeFile(
                            dto,
                            _configuration.RelayTradeFileFtpDirectoryPath,
                            _configuration.RelayTradeFileUploadDirectoryPath);
                        break;
                    case "surveillance-market":
                        await ProcessTradeFile(
                            dto,
                            _configuration.RelayEquityFileFtpDirectoryPath,
                            _configuration.RelayEquityFileUploadDirectoryPath);
                        break;
                    default:
                        _logger.LogInformation($"S3 File Upload Monitoring Process did not recognise the directory of a file. Ignoring file. {dto.FileName}");
                        return;
                }

                // so now what?
                // we need a way of blocking on this ?
                // yup...how can this be done better? maybe you can tell it as well as monitoring for it?


            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Critical, e.Message);
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
                    MoveFile(file, uploadDirectoryPath);
                }
                catch (Exception e)
                {
                    _logger.LogError($"S3 File Upload Monitoring Process moving process trade file {file} to {uploadDirectoryPath} {e.Message}");
                    continue;
                }
            }

            _logger.LogInformation($"Moved all {fileCount}.");
        }

        private void MoveFile(string file, string uploadDirectoryPath)
        {
            var fileName = Path.GetFileName(file) ?? string.Empty;

            if (string.IsNullOrWhiteSpace(fileName))
            {
                _logger.LogInformation($"{file} had an empty file name");

                return;
            }

            _logger.LogInformation($"Moving {fileName} to processing folder.");
            var fullPathToNewFile = Path.Combine(uploadDirectoryPath, fileName);

            if (File.Exists(fullPathToNewFile))
            {
                _logger.LogInformation($"Deleting {fullPathToNewFile} as it already exists");
                File.Delete(fullPathToNewFile);
            }

            Directory.Move(file, fullPathToNewFile);
        }

        private async Task ProcessFile(FileUploadMessageDto dto, int retries, string ftpDirectoryPath)
        {
            var filePath = Path.GetFileName(dto.FileName) ?? string.Empty;

            if (string.IsNullOrWhiteSpace(filePath))
            {
                _logger.LogInformation($"S3 File Upload Monitoring Process had a null or empty file path when reading from S3 message");
                return;
            }

            var newPath = Path.Combine(ftpDirectoryPath, filePath);
            var result = await _s3Client.RetrieveFile(dto.Bucket, dto.FileName, newPath);

            if (result || retries <= 0)
            {
                return;
            }

            await ProcessFile(dto, retries - 1, ftpDirectoryPath);
        }

        public void Terminate()
        {
            _cts?.Cancel();
        }
    }
}
