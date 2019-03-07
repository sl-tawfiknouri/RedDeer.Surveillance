using System;
using Dapper;
using Infrastructure.Network.Aws_IO.Interfaces;
using MySql.Data.MySqlClient;
using TestHarness.Display.Interfaces;
using TestHarness.Repository.Interfaces;

namespace TestHarness.Repository
{
    public class AuroraRepository : IAuroraRepository
    {
        private readonly IAwsConfiguration _configuration;
        private readonly IConsole _console;

        private const string DeleteSql = @"
            DELETE FROM DealerOrders WHERE ID > -1;
            DELETE FROM Orders WHERE ID > -1;
            DELETE FROM InstrumentEquityDailySummary WHERE ID > -1;
            DELETE FROM InstrumentEquityTimeBars WHERE ID > -1;
            DELETE FROM FinancialInstruments WHERE ID > -1;
            DELETE FROM Market WHERE ID > -1;";

        private const string DeleteTradeSql = @"
            DELETE FROM DealerOrders
            WHERE PlacedDate >= @FromDate
            AND PlacedDate <@ToDate;
            DELETE FROM Orders
            WHERE PlacedDate >= @FromDate
            AND PlacedDate < @ToDate;";

        private const string DeleteSecurityPriceSql = @"
            DELETE msep FROM InstrumentEquityDailySummary msep
            LEFT OUTER JOIN FinancialInstruments mses
            ON msep.SecurityId = mses.Id
            LEFT OUTER JOIN Market mse
            ON mse.Id = mses.MarketId
            WHERE mse.MarketId = @MarketId
            AND Epoch >= @FromDate
            AND Epoch < @ToDate;

            DELETE msep FROM InstrumentEquityTimeBars msep
            LEFT OUTER JOIN FinancialInstruments mses
            ON msep.SecurityId = mses.Id
            LEFT OUTER JOIN Market mse
            ON mse.Id = mses.MarketId
            WHERE mse.MarketId = @MarketId
            AND Epoch >= @FromDate
            AND Epoch < @ToDate;";

        public AuroraRepository(
            IAwsConfiguration configuration,
            IConsole console)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }

        public void DeleteTradingAndMarketData()
        {
            if (string.IsNullOrWhiteSpace(_configuration.AuroraConnectionString))
            {
               _console.WriteToUserFeedbackLine("Attempted to delete trading and market data but the aurora connection string was empty");
                return;
            }

            var connection = new MySqlConnection(_configuration.AuroraConnectionString);
            connection.Open();

            using (var conn = connection.ExecuteAsync(DeleteSql))
            {
                conn.Wait();
            }
        }

        public void DeleteTradingAndMarketDataForMarketOnDate(string market, DateTime date)
        {
            var delete = new DeleteTradeDto
            {
                MarketId = market,
                FromDate = date.Date,
                ToDate = date.Date.AddDays(1)
            };

            var connection = new MySqlConnection(_configuration.AuroraConnectionString);
            connection.Open();

            using (var conn = connection.ExecuteAsync(DeleteTradeSql, delete))
            {
                conn.Wait();
            }

            using (var conn = connection.ExecuteAsync(DeleteSecurityPriceSql, delete))
            {
                conn.Wait();
            }
        }

        public class DeleteTradeDto
        {
            public string MarketId { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
        }
    }
}
