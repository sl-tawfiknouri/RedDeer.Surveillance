using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RedDeer.Etl.SqlSriptExecutor.Services.Interfaces;
using RedDeer.Etl.SqlSriptExecutor.Services.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RedDeer.Etl.SqlSriptExecutor.Services
{
    public class FilePreProcessorService
        : IFilePreProcessorService
    {
        private readonly IS3ClientService _s3ClientService;
        private readonly ICSVService _csvService;
        private readonly ILogger<FilePreProcessorService> _logger;

        public FilePreProcessorService(
            IS3ClientService s3ClientService,
            ICSVService csvService,
            ILogger<FilePreProcessorService> logger)
        {
            _s3ClientService = s3ClientService ?? throw new ArgumentNullException(nameof(s3ClientService));
            _csvService = csvService ?? throw new ArgumentNullException(nameof(csvService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> PreProcessAsync(FilePreProcessorData data)
        {
            var requestJson = data != null ? JsonConvert.SerializeObject(data) : "";
            _logger.LogDebug($"Request: '{requestJson}'");

            if (data == null)
            {
                return true;
            }

            if (data.Minutes <= 0)
            {
                return true;
            }

            if ((data.S3Locations?.Any() ?? false) == false)
            {
                return true;
            }

            var lastModified = DateTime.UtcNow.AddMinutes(data.Minutes * -1);

            foreach (var location in data.S3Locations)
            {
                var s3Objects = await _s3ClientService.ListObjectsAsync(location);
                
                var s3ObjectFiles = s3Objects
                    .Where(w => !w.Key.EndsWith("/") && w.LastModified >= lastModified)
                    .ToList();

                _logger.LogDebug($"PreProcessing '{s3ObjectFiles.Count}' files in location '{location}' newer than '{lastModified}'");

                foreach (var s3ObjectFile in s3ObjectFiles)
                {
                    _logger.LogDebug($"PreProcessing file: '{s3ObjectFile.BucketName}/{s3ObjectFile.Key}' (LastModified:'{s3ObjectFile.LastModified}', Size:'{s3ObjectFile.Size}')");

                    var s3ObjectStream = await _s3ClientService.GetObjectStream(s3ObjectFile.BucketName, s3ObjectFile.Key);

                    var memoryStream = new MemoryStream();
                    var newLinesReplaced = _csvService.ReplaceNewLines($"{s3ObjectFile.BucketName}/{s3ObjectFile.Key}", s3ObjectStream, memoryStream);
                    
                    var memoryStreamLength = memoryStream.Length;

                    _logger.LogDebug($"PreProcessing file: '{s3ObjectFile.BucketName}/{s3ObjectFile.Key}'. New Lines: '{newLinesReplaced}' replaced. (LastModified:'{s3ObjectFile.LastModified}', Size:'{s3ObjectFile.Size}')");

                    if (newLinesReplaced <= 0)
                    {
                        continue;
                    }

                    var result = await _s3ClientService.PutObjectStream(s3ObjectFile.BucketName, s3ObjectFile.Key, memoryStream);

                    _logger.LogDebug($"PreProcessing file: '{s3ObjectFile.BucketName}/{s3ObjectFile.Key}'. New Lines: '{newLinesReplaced}' replaced, New Size: '{memoryStreamLength}'. (LastModified:'{s3ObjectFile.LastModified}', Size:'{s3ObjectFile.Size}')");
                }
            }

            return await Task.FromResult(true);
        }
    }
}
