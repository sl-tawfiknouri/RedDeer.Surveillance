namespace Surveillance.DataLayer.Aurora.Orders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Dapper;

    using Domain.Core.Trading.Orders;

    using Microsoft.Extensions.Logging;

    using Surveillance.DataLayer.Aurora.Interfaces;
    using Surveillance.DataLayer.Aurora.Orders.Interfaces;

    public class OrderAllocationRepository : IOrderAllocationRepository
    {
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

        private const string InsertAttributionSql = @"
            INSERT INTO 
                OrdersAllocation (OrderId, Fund, Strategy, ClientAccountId, OrderFilledVolume, CreatedDate)
                VALUES(@OrderId, @Fund, @Strategy, @ClientAccountId, @OrderFilledVolume, now())
            ON DUPLICATE KEY UPDATE OrderFilledVolume = @OrderFilledVolume, Id = LAST_INSERT_ID(Id), CreatedDate = now(), Live = 0, Autoscheduled = 0;
            SELECT LAST_INSERT_ID();";

        private readonly IConnectionStringFactory _connectionFactory;

        private readonly object _lock = new object();

        private readonly ILogger<OrderAllocationRepository> _logger;

        public OrderAllocationRepository(
            IConnectionStringFactory connectionFactory,
            ILogger<OrderAllocationRepository> logger)
        {
            this._connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<string>> Create(IReadOnlyCollection<OrderAllocation> entities)
        {
            this._logger.LogInformation("OrderAllocationRepository Create bulk method called");

            lock (this._lock)
            {
                var filteredEntities = entities?.Where(i => i != null && i.IsValid())?.ToList();

                if (filteredEntities == null || !filteredEntities.Any())
                {
                    this._logger.LogInformation(
                        "OrderAllocationRepository Create bulk method called with null or invalid order allocation, returning without saving");
                    return new List<string>();
                }

                var projectedFilteredEntities = filteredEntities.Where(i => i != null)
                    .Select(i => new OrderAllocationDto(i)).ToList();

                var insertedIds = new List<string>();

                try
                {
                    this._logger.LogInformation(
                        $"OrderAllocationRepository Create bulk method opened db connection and about to write {projectedFilteredEntities.Count} records");

                    using (var dbConn = this._connectionFactory.BuildConn())
                    {
                        foreach (var dto in projectedFilteredEntities)
                            using (var conn = dbConn.ExecuteScalarAsync<string>(InsertAttributionSql, dto))
                            {
                                var idTask = conn;
                                idTask.Wait();
                                var id = idTask.Result;

                                insertedIds.Add(id);
                                this._logger.LogInformation(
                                    "OrderAllocationRepository Create bulk method completed writing record");
                            }
                    }
                }
                catch (Exception e)
                {
                    this._logger.LogError(e, "OrderAllocationRepository Create bulk method had an exception.");
                }

                this._logger.LogInformation("OrderAllocationRepository Create bulk method completed");

                return insertedIds;
            }
        }

        public async Task Create(OrderAllocation entity)
        {
            this._logger.LogInformation("OrderAllocationRepository Create method called");

            lock (this._lock)
            {
                if (entity == null || !entity.IsValid())
                {
                    this._logger.LogInformation(
                        "OrderAllocationRepository Create method called with null or invalid order allocation, returning without saving");
                    return;
                }

                try
                {
                    var dto = new OrderAllocationDto(entity);
                    this._logger.LogInformation(
                        "OrderAllocationRepository Create method opened db connection and about to write record");

                    using (var dbConn = this._connectionFactory.BuildConn())
                    {
                        var conn = dbConn.ExecuteAsync(InsertAttributionSql, dto).ConfigureAwait(false).GetAwaiter().GetResult();
                        this._logger.LogInformation("OrderAllocationRepository Create method completed writing record");
                    }
                }
                catch (Exception e)
                {
                    this._logger.LogError(e, "OrderAllocationRepository Create method had an exception. ");
                }

                this._logger.LogInformation("OrderAllocationRepository Create method completed");
            }
        }

        public async Task<IReadOnlyCollection<OrderAllocation>> Get(IReadOnlyCollection<string> orderIds)
        {
            this._logger.LogInformation("OrderAllocationRepository Get method called");

            orderIds = orderIds?.Where(o => !string.IsNullOrWhiteSpace(o))?.ToList();

            if (orderIds == null || !orderIds.Any()) return new OrderAllocation[0];

            try
            {
                this._logger.LogInformation(
                    $"OrderAllocationRepository Create method opened db connection and about to query for {orderIds?.Count} order ids");

                using (var dbConnection = this._connectionFactory.BuildConn())
                using (var conn = dbConnection.QueryAsync<OrderAllocationDto>(
                    GetAllocationSql,
                    new { OrderIds = orderIds }))
                {
                    var result = await conn;

                    var allocations = result.Select(this.Project).ToList();

                    return allocations;
                }
            }
            catch (Exception e)
            {
                this._logger?.LogError(e, "OrderAllocationRepository Get encountered an error");
            }

            this._logger.LogInformation("OrderAllocationRepository Get method completed");

            return new OrderAllocation[0];
        }

        public async Task<IReadOnlyCollection<OrderAllocation>> GetStaleOrderAllocations(DateTime stalenessIndicator)
        {
            this._logger.LogInformation("OrderAllocationRepository GetStaleOrderAllocations method called");

            try
            {
                this._logger.LogInformation("OrderAllocationRepository GetStaleOrderAllocations opening connections");

                using (var dbConnection = this._connectionFactory.BuildConn())
                using (var conn = dbConnection.QueryAsync<OrderAllocationDto>(
                    GetStaleAllocationSql,
                    new { StalenessDate = stalenessIndicator }))
                {
                    var result = await conn;
                    var allocations = result.Select(this.Project).ToList();

                    return allocations;
                }
            }
            catch (Exception e)
            {
                this._logger?.LogError(e, "OrderAllocationRepository GetStaleOrderAllocations encountered an error");
            }

            this._logger.LogInformation("OrderAllocationRepository GetStaleOrderAllocations method completed");

            return new OrderAllocation[0];
        }

        public OrderAllocation Project(OrderAllocationDto dto)
        {
            if (dto == null) return null;

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
                this.Id = oa.Id;
                this.OrderId = oa.OrderId;
                this.Fund = oa.Fund;
                this.Strategy = oa.Strategy;
                this.ClientAccountId = oa.ClientAccountId;
                this.OrderFilledVolume = oa.OrderFilledVolume;
                this.CreatedDate = oa.CreatedDate;
            }

            public string ClientAccountId { get; set; }

            public DateTime? CreatedDate { get; set; }

            public string Fund { get; set; }

            public string Id { get; set; }

            public decimal OrderFilledVolume { get; set; }

            public string OrderId { get; set; }

            public string Strategy { get; set; }
        }
    }
}