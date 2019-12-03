namespace Surveillance.DataLayer.Aurora.Orders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Dapper;

    using Domain.Core.Trading.Orders;
    using Domain.Core.Trading.Orders.Interfaces;

    using Microsoft.Extensions.Logging;

    using RedDeer.Contracts.SurveillanceService.Api.BrokerEnrichment;

    using Surveillance.DataLayer.Aurora.Interfaces;
    using Surveillance.DataLayer.Aurora.Orders.Interfaces;

    public class OrderBrokerRepository : IOrderBrokerRepository
    {
        private const string GetBrokerUnEnrichedSql =
            @"SELECT Id, ExternalId, Name, CreatedOn, Live FROM Brokers WHERE Live = 0;";

        // INSERT OR CREATE
        private const string InsertBrokerSql =
            @"INSERT IGNORE INTO Brokers (ExternalId, Name, CreatedOn, Live, Updated) VALUES(@ExternalId, @Name, UTC_TIMESTAMP(), @Live, UTC_TIMESTAMP());
                SELECT Id FROM Brokers WHERE Name = @Name;";

        // INSERT OR CREATE
        private const string InsertEnrichedBrokerSql =
            @"UPDATE Brokers SET ExternalId = @ExternalId, Live = 1, Updated = UTC_TIMESTAMP() WHERE Name = @Name;";

        private readonly IConnectionStringFactory _dbConnectionFactory;

        private readonly ILogger<OrderBrokerRepository> _logger;

        public OrderBrokerRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<OrderBrokerRepository> logger)
        {
            this._dbConnectionFactory =
                dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IReadOnlyCollection<IOrderBroker>> GetUnEnrichedBrokers()
        {
            this._logger?.LogInformation("Fetching un enriched brokers");

            try
            {
                using (var dbConn = this._dbConnectionFactory.BuildConn())
                using (var conn = dbConn.QueryAsync<BrokerDto>(GetBrokerUnEnrichedSql))
                {
                    var brokerDtos = await conn;

                    return brokerDtos.ToList()
                        .Select(_ => new OrderBroker(_.Id, _.ExternalId, _.Name, _.CreatedOn, _.Live)).ToList();
                }
            }
            catch (Exception e)
            {
                this._logger?.LogError($"Error in broker insert or update {e.Message} {e?.InnerException?.Message}");
                return new IOrderBroker[0];
            }
        }

        public async Task<string> InsertOrUpdateBrokerAsync(IOrderBroker broker)
        {
            this._logger?.LogInformation($"{broker?.Name} about to insert or update broker");

            if (broker == null) return string.Empty;

            try
            {
                using (var dbConn = this._dbConnectionFactory.BuildConn())
                {
                    var id = await dbConn.ExecuteScalarAsync<string>(InsertBrokerSql, broker);
                    return id;
                }
            }
            catch (Exception e)
            {
                this._logger?.LogError($"Error in broker insert or update {e.Message} {e?.InnerException?.Message}");
                return string.Empty;
            }
        }

        public async Task UpdateEnrichedBroker(IReadOnlyCollection<BrokerEnrichmentDto> brokers)
        {
            this._logger?.LogInformation($"{brokers?.Count ?? 0} about to insert or update enriched brokers");

            if (brokers == null || !brokers.Any())
            {
                this._logger?.LogInformation("could not insert or update empty or null broker response");
                return;
            }

            try
            {
                var brokerDtos = brokers?.Select(
                    _ => new BrokerDto
                             {
                                 Name = _.Name,
                                 Id = _.Id,
                                 CreatedOn = _.CreatedOn,
                                 ExternalId = _.ExternalId,
                                 Live = _.Live
                             }).ToList();

                using (var dbConn = this._dbConnectionFactory.BuildConn())
                using (var conn = dbConn.ExecuteAsync(InsertEnrichedBrokerSql, brokerDtos))
                {
                    await conn;
                }
            }
            catch (Exception e)
            {
                this._logger?.LogError($"Error in broker insert or update {e.Message} {e?.InnerException?.Message}");
            }
        }

        private class BrokerDto
        {
            public BrokerDto()
            {
            }

            public BrokerDto(IOrderBroker orderBroker)
            {
                if (orderBroker == null) return;

                this.Id = string.IsNullOrWhiteSpace(orderBroker.Id) ? null : orderBroker.Id;
                this.ExternalId = string.IsNullOrWhiteSpace(orderBroker.ReddeerId) ? null : orderBroker.ReddeerId;
                this.Name = string.IsNullOrWhiteSpace(orderBroker.Name) ? null : orderBroker.Name?.ToLower();
                this.Live = orderBroker.Live;
            }

            public DateTime? CreatedOn { get; set; }

            /// <summary>
            ///     AKA external id
            /// </summary>
            public string ExternalId { get; set; }

            /// <summary>
            ///     Primary key
            /// </summary>
            public string Id { get; set; }

            public bool Live { get; set; }

            public string Name { get; set; }
        }
    }
}