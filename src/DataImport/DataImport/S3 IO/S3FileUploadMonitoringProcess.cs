using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Relay.Configuration.Interfaces;
using Relay.Disk_IO.EquityFile.Interfaces;
using Relay.Disk_IO.Interfaces;
using Relay.S3_IO.Interfaces;
using Utilities.Aws_IO;
using Utilities.Aws_IO.Interfaces;

namespace Relay.S3_IO
{
    public class S3FileUploadMonitoringProcess : IS3FileUploadMonitoringProcess
    {
        private CancellationTokenSource _cts;
        private AwsResusableCancellationToken _token;

        private readonly object _lock = new object();

        private IUploadEquityFileMonitor _uploadEquityFileMonitor;
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
            IUploadTradeFileMonitor uploadTradeFileMonitor,
            IUploadEquityFileMonitor uploadEquityFileMonitor)
        {
            _logger.LogInformation("Initialising file upload monitoring process");

            _uploadTradeFileMonitor = uploadTradeFileMonitor;
            _uploadEquityFileMonitor = uploadEquityFileMonitor;

            _cts = new CancellationTokenSource();
            _token = new AwsResusableCancellationToken();

            _queueClient.SubscribeToQueueAsync(_configuration.RelayS3UploadQueueName , ReadMessage, _cts.Token, _token);
        }

        private async Task ReadMessage(string messageId, string messageBody)
        {
            lock (_lock)
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
                            var ptf = ProcessTradeFile(
                                dto,
                                _configuration.RelayTradeFileFtpDirectoryPath,
                                _configuration.RelayTradeFileUploadDirectoryPath);
                            ptf.Wait();
                            break;
                        case "surveillance-market":
                            var pef = ProcessEquityFile(
                                dto,
                                _configuration.RelayEquityFileFtpDirectoryPath,
                                _configuration.RelayEquityFileUploadDirectoryPath);
                            pef.Wait();
                            break;
                        default:
                            _logger.LogInformation($"S3 File Upload Monitoring Process did not recognise the directory of a file. Ignoring file. {dto.FileName}");
                            return;
                    }
                }
                catch (Exception e)
                {
                    _logger.Log(LogLevel.Critical, e.Message);
                }
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
                    var result = _uploadTradeFileMonitor.ProcessFile(file);

                    if (result == false)
                    {
                        _token.Cancel = true;
                    }

                    if (File.Exists(file))
                    {
                        File.Delete(file);
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

        private async Task ProcessEquityFile(FileUploadMessageDto dto, string ftpDirectoryPath, string uploadDirectoryPath)
        {
            await ProcessFile(dto, 3, ftpDirectoryPath);

            var files = Directory.EnumerateFiles(ftpDirectoryPath).ToList();
            var fileCount = files.Count;
            _logger.LogInformation($"Found {fileCount} files in the local ftp folder. Moving to the processing folder.");

            foreach (var file in files)
            {
                try
                {
                    var result = _uploadEquityFileMonitor.ProcessFile(file);

                    if (result == false)
                    {
                        _token.Cancel = true;
                    }

                    if (File.Exists(file))
                    {
                        File.Delete(file);
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
