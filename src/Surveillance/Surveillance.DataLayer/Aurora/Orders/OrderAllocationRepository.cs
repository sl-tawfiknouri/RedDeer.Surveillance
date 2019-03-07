using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Domain.Core.Trading.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Interfaces;
using Surveillance.DataLayer.Aurora.Orders.Interfaces;

namespace Surveillance.DataLayer.Aurora.Orders
{
    public class OrderAllocationRepository : IOrderAllocationRepository
    {
        private readonly IConnectionStringFactory _connectionFactory;
        private readonly ILogger<OrderAllocationRepository> _logger;

        private const string InsertAttributionSql = @"
            INSERT INTO 
                OrdersAllocation (OrderId, Fund, Strategy, ClientAccountId, OrderFilledVolume, CreatedDate)
                VALUES(@OrderId, @Fund, @Strategy, @ClientAccountId, @OrderFilledVolume, now())
            ON DUPLICATE KEY UPDATE OrderFilledVolume = @OrderFilledVolume, Id = LAST_INSERT_ID(Id), CreatedDate = now(), Live = 0, Autoscheduled = 0;
            SELECT LAST_INSERT_ID();";

        private const string GetAllocationSql = @"
            SELECT
                Id as Id,
                OrderId as OrderId,
                Fund as Fund,
                Strategy as Strategy,
                ClientAccountId as ClientAccountId,
                OrderFilledVolume as OrderFilledVolume,
                CreatedDate as CreatedDate
            FROM OrdersAllocation
            WHERE OrderId IN @OrderIds
            AND Live = 1;";

        private const string GetStaleAllocationSql = @"
            SELECT
                Id as Id,
                OrderId as OrderId,
                Fund as Fund,
                Strategy as Strategy,
                ClientAccountId as ClientAccountId,
                OrderFilledVolume as OrderFilledVolume,
                CreatedDate as CreatedDate
            FROM OrdersAllocation
            WHERE 
                Live = 0
            AND 
                CreatedDate < @StalenessDate;";

        public OrderAllocationRepository(
            IConnectionStringFactory connectionFactory,
            ILogger<OrderAllocationRepository> logger)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<string>> Create(IReadOnlyCollection<OrderAllocation> entities)
        {
            _logger.LogInformation($"OrderAllocationRepository Create bulk method called");

            var filteredEntities = entities?.Where(i => i != null && i.IsValid())?.ToList();

            if (filteredEntities == null
                || !filteredEntities.Any())
            {
                _logger.LogInformation($"OrderAllocationRepository Create bulk method called with null or invalid order allocation, returning without saving");
                return new List<string>();
            }

            var projectedFilteredEntities =
                filteredEntities
                    .Where(i => i != null)
                    .Select(i => new OrderAllocationDto(i))
                    .ToList();

            var insertedIds = new List<string>();

            try
            {
                _logger.LogInformation($"OrderAllocationRepository Create bulk method opened db connection and about to write {projectedFilteredEntities.Count} records");

                using (var dbConn = _connectionFactory.BuildConn())
                {
                    foreach (var dto in projectedFilteredEntities)
                    {
                        using (var conn = dbConn.ExecuteScalarAsync<string>(InsertAttributionSql, dto))
                        {
                            var id = await conn;
                           insertedIds.Add(id);
                            _logger.LogInformation($"OrderAllocationRepository Create bulk method completed writing record");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"OrderAllocationRepository Create bulk method had an exception. ", e);
            }

            _logger.LogInformation($"OrderAllocationRepository Create bulk method completed");

            return insertedIds;
        }

        public async Task Create(OrderAllocation entity)
        {
            _logger.LogInformation($"OrderAllocationRepository Create method called");

            if (entity == null
                || !entity.IsValid())
            {
                _logger.LogInformation($"OrderAllocationRepository Create method called with null or invalid order allocation, returning without saving");
                return;
            }

            try
            {
                var dto = new OrderAllocationDto(entity);
                _logger.LogInformation($"OrderAllocationRepository Create method opened db connection and about to write record"); 

                using (var dbConn = _connectionFactory.BuildConn())
                using (var conn = dbConn.ExecuteAsync(InsertAttributionSql, dto))
                {
                    await conn;
                    _logger.LogInformation($"OrderAllocationRepository Create method completed writing record");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"OrderAllocationRepository Create method had an exception. ", e);
            }

            _logger.LogInformation($"OrderAllocationRepository Create method completed");
        }

        public async Task<IReadOnlyCollection<OrderAllocation>> Get(IReadOnlyCollection<string> orderIds)
        {
            _logger.LogInformation($"OrderAllocationRepository Get method called");

            orderIds = orderIds?.Where(o => !string.IsNullOrWhiteSpace(o))?.ToList();

            if (orderIds == null
                || !orderIds.Any())
            {
                return new OrderAllocation[0];
            }

            try
            {
                _logger.LogInformation($"OrderAllocationRepository Create method opened db connection and about to query for {orderIds?.Count} order ids");

                using (var dbConnection = _connectionFactory.BuildConn())
                using (var conn = dbConnection.QueryAsync<OrderAllocationDto>(GetAllocationSql, new { @OrderIds = orderIds }))
                {
                    var result = await conn;

                    var allocations = result.Select(Project).ToList();

                    return allocations;
                }
            }
            catch (Exception e)
            {
                _logger?.LogError($"OrderAllocationRepository Get encountered an error", e);
            }

            _logger.LogInformation($"OrderAllocationRepository Get method completed");

            return new OrderAllocation[0];
        }

        public async Task<IReadOnlyCollection<OrderAllocation>> GetStaleOrderAllocations(DateTime stalenessIndicator)
        {
            _logger.LogInformation($"OrderAllocationRepository GetStaleOrderAllocations method called");

            try
            {
                _logger.LogInformation($"OrderAllocationRepository GetStaleOrderAllocations opening connections");

                using (var dbConnection = _connectionFactory.BuildConn())
                using (var conn = dbConnection.QueryAsync<OrderAllocationDto>(GetStaleAllocationSql, new { @StalenessDate = stalenessIndicator }))
                {
                    var result = await conn;
                    var allocations = result.Select(Project).ToList();

                    return allocations;
                }
            }
            catch (Exception e)
            {
                _logger?.LogError($"OrderAllocationRepository GetStaleOrderAllocations encountered an error", e);
            }

            _logger.LogInformation($"OrderAllocationRepository GetStaleOrderAllocations method completed");

            return new OrderAllocation[0];
        }

        public OrderAllocation Project(OrderAllocationDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            return new OrderAllocation(
                dto.Id,
                dto.OrderId,
                dto.Fund,
                dto.Strategy,
                dto.ClientAccountId,
                dto.OrderFilledVolume,
                dto.CreatedDate);
        }

        public class OrderAllocationDto
        {
            public OrderAllocationDto()
            { 
                // leave for dapper
            }

            public OrderAllocationDto(OrderAllocation oa)
            {
                Id = oa.Id;
                OrderId = oa.OrderId;
                Fund = oa.Fund;
                Strategy = oa.Strategy;
                ClientAccountId = oa.ClientAccountId;
                OrderFilledVolume = oa.OrderFilledVolume;
                CreatedDate = oa.CreatedDate;
            }

            public string Id { get; set; }
            public string OrderId { get; set; }
            public string Fund { get; set; }
            public string Strategy { get; set; }
            public string ClientAccountId { get; set; }
            public long OrderFilledVolume { get; set; }
            public DateTime? CreatedDate { get; set; }
        }
    }
}
