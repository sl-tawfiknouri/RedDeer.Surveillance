﻿using System;
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

namespace DataImport.S3_IO
{
    public class S3FileUploadMonitoringProcess : IS3FileUploadMonitoringProcess
    {
        private CancellationTokenSource _cts;
        private AwsResusableCancellationToken _token;

        private IUploadAllocationFileMonitor _uploadAllocationFileMonitor;
        private IUploadTradeFileMonitor _uploadTradeFileMonitor;
        private IUploadEtlFileMonitor _uploadEtlFileMonitor;
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
            IUploadTradeFileMonitor uploadTradeFileMonitor,
            IUploadEtlFileMonitor uploadEtlFileMonitor)
        {
            try
            {
                 _logger.LogInformation("Initialising file upload monitoring process");

                _uploadAllocationFileMonitor = uploadAllocationFileMonitor;
                _uploadTradeFileMonitor = uploadTradeFileMonitor;
                _uploadEtlFileMonitor = uploadEtlFileMonitor;
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
                    _logger.LogInformation($"S3FileUploadMonitoringProcess deserialised message {messageId} but found the file size to be 0. Assuming this is the preceding message to the actual file uploaded message. File ({dto?.ToString()}).");

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
                    case "surveillance-etl-order":
                        await ProcessEtlFile(
                            dto,
                            _configuration.DataImportEtlFileFtpDirectoryPath,
                            _configuration.DataImportEtlFileUploadDirectoryPath);
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

        private async Task ProcessEtlFile(
            FileUploadMessageDto dto,
            string ftpDirectoryPath,
            string uploadDirectoryPath)
        {
            bool ProcessFile(string x) => _uploadEtlFileMonitor.ProcessFile(x);
            await ProcessS3File(dto, ftpDirectoryPath, uploadDirectoryPath, ProcessFile);
        }

        private async Task ProcessTradeFile(
            FileUploadMessageDto dto,
            string ftpDirectoryPath,
            string uploadDirectoryPath)
        {
            bool ProcessFile(string x) => _uploadTradeFileMonitor.ProcessFile(x);
            await ProcessS3File(dto, ftpDirectoryPath, uploadDirectoryPath, ProcessFile);
        }

        private async Task ProcessAllocationFile(
            FileUploadMessageDto dto,
            string ftpDirectoryPath,
            string uploadDirectoryPath)
        {
            bool ProcessFile(string x) => _uploadAllocationFileMonitor.ProcessFile(x);
            await ProcessS3File(dto, ftpDirectoryPath, uploadDirectoryPath, ProcessFile);
        }

        private async Task ProcessS3File(
            FileUploadMessageDto dto,
            string ftpDirectoryPath,
            string uploadDirectoryPath,
            Func<string, bool> processFileDelegate)
        {
            await ProcessFile(dto, 3, ftpDirectoryPath);

            var files = Directory.EnumerateFiles(ftpDirectoryPath).ToList();
            var fileCount = files.Count;
            _logger.LogInformation($"Found {fileCount} files in the local ftp folder. Moving to the processing folder.");

            foreach (var file in files)
            {
                try
                {
                    _logger.LogInformation($"S3 processing file {file}");
                    var result = processFileDelegate(file);

                    if (!result)
                    {
                        _logger.LogError($"S3 processing file {file} was unsuccessful");
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

        private async Task ProcessFile(FileUploadMessageDto dto, int retries, string ftpDirectoryPath)
        {
            if (dto == null)
            {
                this._logger.LogError($"File upload dto was null, exiting process file");
                return;
            }

            var versionId = string.IsNullOrWhiteSpace(dto?.VersionId) ? "" : $"{dto?.VersionId}-";
            var fileName = Path.GetFileName(dto.FileName) ?? string.Empty;

            if (string.IsNullOrWhiteSpace(fileName))
            {
                _logger.LogInformation($"S3 File Upload Monitoring Process had a null or empty file path when reading from S3 message.  File ({dto?.ToString()}).");
                return;
            }
            else
            {
                _logger.LogInformation($"S3 Processor about to process file ({dto}).");
            }

            fileName = $"{versionId}{fileName}";
            var destinationFileName = Path.Combine(ftpDirectoryPath, fileName);
            var result = await _s3Client.RetrieveFile(dto.Bucket, dto.FileName, dto.VersionId, destinationFileName);

            if (retries <= 0)
                _logger.LogInformation($"S3 Process File ran out of retries for processing file {dto.FileName}");

            if (result)
            {
                _logger.LogInformation($"S3 Processor successfully retrieved file ({dto}) to {destinationFileName}");
                return;
            }

            if (retries <= 0)
            {
                _logger.LogError($"S3 Processor ran out of retries trying to fetch file ({dto}) to {destinationFileName}");
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
