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
    public class FileUploadOrdersRepository : IFileUploadOrdersRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<FileUploadOrdersRepository> _logger;

        private const string InsertFileUploadOrdersSql = @"
            INSERT IGNORE INTO FileUploadOrders(FileUploadId, OrderId) VALUES(@FileUploadId, @OrderId);";

        public FileUploadOrdersRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<FileUploadOrdersRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Create(IReadOnlyCollection<string> orderIds, int uploadId)
        {
            if (orderIds == null
                || !orderIds.Any())
            {
                _logger.LogInformation($"FileUploadOrdersRepository passed null or empty order ids collection. Exiting");
                return;
            }

            try
            {
                _logger?.LogInformation($"FileUploadOrdersRepository about to save {orderIds.Count} orders for file upload {uploadId}.");

                var dtos = orderIds.Select(ord => new FileUploadOrderDto(ord, uploadId)).ToList();

                using (var dbConn = _dbConnectionFactory.BuildConn())
                using (var conn = dbConn.ExecuteAsync(InsertFileUploadOrdersSql, dtos))
                {
                    await conn;
                }

                _logger?.LogInformation($"FileUploadOrdersRepository completed the save of {orderIds.Count} orders for file upload {uploadId}.");
            }
            catch (Exception e)
            {
                _logger.LogError($"FileUploadOrdersRepository upload of {orderIds.Count} orders for file upload {uploadId} failed to insert because of {e.Message}");
            }
        }

        private class FileUploadOrderDto
        {
            public FileUploadOrderDto()
            {
                // leave for dapper
            }

            public FileUploadOrderDto(string orderId, int fileUploadId)
            {
                OrderId = orderId;
                FileUploadId = fileUploadId;
            }

            public long Id { get; set; }
            public int FileUploadId { get; set; }
            public string OrderId { get; set; }
        }
    }
}
