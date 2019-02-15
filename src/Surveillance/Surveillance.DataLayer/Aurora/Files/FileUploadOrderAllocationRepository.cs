using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Files.Interfaces;
using Surveillance.DataLayer.Aurora.Interfaces;

namespace Surveillance.DataLayer.Aurora.Files
{
    public class FileUploadOrderAllocationRepository : IFileUploadOrderAllocationRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<FileUploadOrderAllocationRepository> _logger;

        private const string InsertFileUploadOrderAllocationsSql = @"
            INSERT IGNORE INTO FileUploadAllocations(FileUploadId, OrderAllocationId) VALUES(@FileUploadId, @OrderAllocationId);";

        public FileUploadOrderAllocationRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<FileUploadOrderAllocationRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Create(IReadOnlyCollection<string> orderAllocationIds, int uploadId)
        {
            if (orderAllocationIds == null
                || !orderAllocationIds.Any())
            {
                _logger.LogInformation($"FileUploadOrderAllocationRepository passed null or empty order ids collection. Exiting");
                return;
            }

            try
            {
                _logger?.LogInformation($"FileUploadOrderAllocationRepository about to save {orderAllocationIds.Count} orders for file upload {uploadId}.");

                var dtos = orderAllocationIds.Select(ord => new FileUploadOrderAllocationDto(ord, uploadId)).ToList();

                using (var dbConn = _dbConnectionFactory.BuildConn())
                using (var conn = dbConn.ExecuteAsync(InsertFileUploadOrderAllocationsSql, dtos))
                {
                    await conn;
                }

                _logger?.LogInformation($"FileUploadOrderAllocationRepository completed the save of {orderAllocationIds.Count} orders for file upload {uploadId}.");
            }
            catch (Exception e)
            {
                _logger.LogError($"FileUploadOrderAllocationRepository upload of {orderAllocationIds.Count} orders for file upload {uploadId} failed to insert because of {e.Message}");
            }
        }

        private class FileUploadOrderAllocationDto
        {
            public FileUploadOrderAllocationDto()
            {
                // leave for dapper
            }

            public FileUploadOrderAllocationDto(string orderAllocationId, int fileUploadId)
            {
                OrderAllocationId = orderAllocationId;
                FileUploadId = fileUploadId;
            }

            public int Id { get; set; }
            public int FileUploadId { get; set; }
            public string OrderAllocationId { get; set; }
        }
    }
}
