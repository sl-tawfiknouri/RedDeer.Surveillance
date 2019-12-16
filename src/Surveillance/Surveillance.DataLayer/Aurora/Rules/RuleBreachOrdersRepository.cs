namespace Surveillance.DataLayer.Aurora.Rules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Dapper;

    using Domain.Surveillance.Rules;

    using Microsoft.Extensions.Logging;

    using Surveillance.DataLayer.Aurora.Interfaces;
    using Surveillance.DataLayer.Aurora.Rules.Interfaces;

    public class RuleBreachOrdersRepository : IRuleBreachOrdersRepository
    {
        private const string GetRuleBreachSql = @"SELECT * FROM RuleBreachOrders WHERE RuleBreachId = @RuleBreachId";

        private const string SaveRuleBreachSql =
            @"INSERT IGNORE INTO RuleBreachOrders (RuleBreachId, OrderId) VALUES(@RuleBreachId, @OrderId);";

        private readonly IConnectionStringFactory _dbConnectionFactory;

        private readonly ILogger<RuleBreachOrdersRepository> _logger;

        public RuleBreachOrdersRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<RuleBreachOrdersRepository> logger)
        {
            this._dbConnectionFactory =
                dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Create(IReadOnlyCollection<RuleBreachOrder> message)
        {
            if (message == null || !message.Any())
            {
                this._logger.LogWarning("RuleBreachOrdersRepository saving rule was passed a null message. Returning.");
                return;
            }

            var dbConnection = this._dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                this._logger.LogInformation("RuleBreachOrdersRepository saving rule breach to repository");

                var dtos = message.Select(m => new RuleBreachOrderDto(m)).ToList();
                var ruleBreachList = new List<List<RuleBreachOrderDto>>();

                // send over in blocks of 1k - sql parameter limit of around 1k seems silly to solve a 1k
                // trades issue then break on saving the 1k trades to the db =)
                var count = dtos.Count;
                var iter = 0;
                while (count > 0)
                {
                    count = count - 1000;
                    var skippedResults = dtos.Skip(iter * 1000).Take(1000).ToList();
                    ruleBreachList.Add(skippedResults);
                    iter++;
                }

                foreach (var sublist in ruleBreachList)
                    using (var conn = dbConnection.ExecuteAsync(SaveRuleBreachSql, sublist))
                    {
                        await conn;
                        this._logger.LogInformation(
                            "RuleBreachOrdersRepository completed saving rule breach to repository");
                    }
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"RuleBreachOrdersRepository error for Create");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        public async Task<IReadOnlyCollection<RuleBreachOrder>> Get(string ruleBreachId)
        {
            if (string.IsNullOrWhiteSpace(ruleBreachId))
            {
                this._logger.LogWarning(
                    "RuleBreachOrdersRepository get rule breach was passed a null message. Returning.");
                return null;
            }

            try
            {
                this._logger.LogInformation("RuleBreachOrdersRepository fetching rule breaches");
                using (var dbConnection = this._dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.QueryAsync<RuleBreachOrderDto>(
                    GetRuleBreachSql,
                    new { RuleBreachId = ruleBreachId }))
                {
                    var result = await conn;

                    this._logger.LogInformation("RuleBreachOrdersRepository completed fetching rule breach");

                    var mappedResult = result.Select(this.Project).ToList();

                    return mappedResult;
                }
            }
            catch (Exception e)
            {
                this._logger.LogError(
                    $"RuleBreachOrdersRepository error for Create {e.Message} - {e?.InnerException?.Message}");
            }

            return null;
        }

        private RuleBreachOrder Project(RuleBreachOrderDto dto)
        {
            if (dto == null) return null;

            return new RuleBreachOrder(dto.RuleBreachId, dto.OrderId);
        }

        /// <summary>
        ///     Database dto for rule breaches
        /// </summary>
        public class RuleBreachOrderDto
        {
            public RuleBreachOrderDto()
            {
                // leave blank ctor  
            }

            public RuleBreachOrderDto(RuleBreachOrder message)
            {
                if (message == null) return;

                this.RuleBreachId = message.RuleBreachId;
                this.OrderId = message.OrderId;
            }

            public string OrderId { get; set; }

            public string RuleBreachId { get; set; }
        }
    }
}