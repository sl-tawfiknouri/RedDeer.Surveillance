namespace Surveillance.DataLayer.Aurora.Files
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Dapper;

    using Microsoft.Extensions.Logging;

    using Surveillance.DataLayer.Aurora.Files.Interfaces;
    using Surveillance.DataLayer.Aurora.Interfaces;

    public class FileUploadOrderAllocationRepository : IFileUploadOrderAllocationRepository
    {
        private const string InsertFileUploadOrderAllocationsSql = @"
            INSERT IGNORE INTO FileUploadAllocations(FileUploadId, OrderAllocationId) VALUES(@FileUploadId, @OrderAllocationId);";

        private readonly IConnectionStringFactory _dbConnectionFactory;

        private readonly ILogger<FileUploadOrderAllocationRepository> _logger;

        public FileUploadOrderAllocationRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<FileUploadOrderAllocationRepository> logger)
        {
            this._dbConnectionFactory =
                dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Create(IReadOnlyCollection<string> orderAllocationIds, int uploadId)
        {
            if (orderAllocationIds == null || !orderAllocationIds.Any())
            {
                this._logger.LogInformation(
                    "FileUploadOrderAllocationRepository passed null or empty order ids collection. Exiting");
                return;
            }

            try
            {
                this._logger?.LogInformation(
                    $"FileUploadOrderAllocationRepository about to save {orderAllocationIds.Count} orders for file upload {uploadId}.");

                var dtos = orderAllocationIds.Select(ord => new FileUploadOrderAllocationDto(ord, uploadId)).ToList();

                using (var dbConn = this._dbConnectionFactory.BuildConn())
                using (var conn = dbConn.ExecuteAsync(InsertFileUploadOrderAllocationsSql, dtos))
                {
                    await conn;
                }

                this._logger?.LogInformation(
                    $"FileUploadOrderAllocationRepository completed the save of {orderAllocationIds.Count} orders for file upload {uploadId}.");
            }
            catch (Exception e)
            {
                this._logger.LogError(
                    $"FileUploadOrderAllocationRepository upload of {orderAllocationIds.Count} orders for file upload {uploadId} failed to insert because of {e.Message}");
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
                this.OrderAllocationId = orderAllocationId;
                this.FileUploadId = fileUploadId;
            }

            public int FileUploadId { get; }

            public int Id { get; set; }

            public string OrderAllocationId { get; }
        }
    }
}