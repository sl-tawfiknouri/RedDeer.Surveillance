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

namespace Surveillance.DataLayer.Aurora.Orders
{
    public class OrderBrokerRepository : IOrderBrokerRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<OrderBrokerRepository> _logger;

        // INSERT OR CREATE
        private const string InsertBrokerSql =
            @"INSERT IGNORE INTO Brokers (ExternalId, Name, CreatedOn, Live, Updated) VALUES(@ExternalId, @Name, now(), @Live, now());
                SELECT Id FROM Brokers WHERE Name = @Name;";

        // INSERT OR CREATE
        private const string InsertEnrichedBrokerSql =
            @"UPDATE Brokers SET ExternalId = @ExternalId, Live = 1, Updated = now() WHERE Name = @Name;";

        private const string GetBrokerUnEnrichedSql = @"SELECT Id, ExternalId, Name, CreatedOn, Live FROM Brokers WHERE Live = 0;";
        
        public OrderBrokerRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<OrderBrokerRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> InsertOrUpdateBroker(IOrderBroker broker)
        {
            _logger?.LogInformation($"{broker?.Name} about to insert or update broker");

            if (broker == null)
            {
                return string.Empty;
            }

            try
            {
                using (var dbConn = _dbConnectionFactory.BuildConn())
                using (var conn = dbConn.ExecuteScalarAsync<string>(InsertBrokerSql, broker))
                {
                    var id = await conn;
                    return id;
                }
            }
            catch (Exception e)
            {
                _logger?.LogError($"Error in broker insert or update {e.Message} {e?.InnerException?.Message}");
                return string.Empty;
            }
        }

        public async Task UpdateEnrichedBroker(IReadOnlyCollection<BrokerEnrichmentDto> brokers)
        {
            _logger?.LogInformation($"{brokers?.Count ?? 0} about to insert or update enriched brokers");

            if (brokers == null
                || !brokers.Any())
            {
                _logger?.LogInformation($"could not insert or update empty or null broker response");
                return;
            }

            try
            {
                var brokerDtos = brokers
                    ?.Select(_ =>
                        new BrokerDto
                        {
                            Name = _.Name,
                            Id = _.Id,
                            CreatedOn = _.CreatedOn,
                            ExternalId = _.ExternalId,
                            Live = _.Live
                        })
                    .ToList();

                using (var dbConn = _dbConnectionFactory.BuildConn())
                using (var conn = dbConn.ExecuteAsync(InsertEnrichedBrokerSql, brokerDtos))
                {
                    await conn;
                }
            }
            catch (Exception e)
            {
                _logger?.LogError($"Error in broker insert or update {e.Message} {e?.InnerException?.Message}");
                return;
            }
        }

        public async Task<IReadOnlyCollection<IOrderBroker>> GetUnEnrichedBrokers()
        {
            _logger?.LogInformation($"Fetching un enriched brokers");

            try
            {
                using (var dbConn = _dbConnectionFactory.BuildConn())
                using (var conn = dbConn.QueryAsync<BrokerDto>(GetBrokerUnEnrichedSql))
                {
                    var brokerDtos = await conn;

                    return 
                        brokerDtos
                            .ToList()
                            .Select(_ =>
                                new OrderBroker(
                                    _.Id,
                                    _.ExternalId,
                                    _.Name,
                                    _.CreatedOn,
                                    _.Live))
                            .ToList();
                }
            }
            catch (Exception e)
            {
                _logger?.LogError($"Error in broker insert or update {e.Message} {e?.InnerException?.Message}");
                return new IOrderBroker[0];
            }
        }

        private class BrokerDto
        {
            public BrokerDto()
            { }

            public BrokerDto(IOrderBroker orderBroker)
            {
                if (orderBroker == null)
                {
                    return;
                }

                Id = string.IsNullOrWhiteSpace(orderBroker.Id) ? null : orderBroker.Id;
                ExternalId = string.IsNullOrWhiteSpace(orderBroker.ReddeerId) ? null : orderBroker.ReddeerId;
                Name = string.IsNullOrWhiteSpace(orderBroker.Name) ? null : orderBroker.Name?.ToLower();
                Live = orderBroker.Live;
            }

            /// <summary>
            /// Primary key
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// AKA external id
            /// </summary>
            public string ExternalId { get; set; }

            public string Name { get; set; }

            public DateTime? CreatedOn { get; set; }

            public bool Live { get; set; }
        }
    }
}
