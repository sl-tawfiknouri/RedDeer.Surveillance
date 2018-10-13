﻿using System;
using Dapper;
using MySql.Data.MySqlClient;
using TestHarness.Display.Interfaces;
using TestHarness.Repository.Interfaces;
using Utilities.Aws_IO.Interfaces;

namespace TestHarness.Repository
{
    public class AuroraRepository : IAuroraRepository
    {
        private readonly IAwsConfiguration _configuration;
        private readonly IConsole _console;

        private const string DeleteSql = @"
            DELETE FROM MarketStockExchangePrices WHERE ID > -1;
            DELETE FROM MarketStockExchangeSecurities WHERE ID > -1;
            DELETE FROM MarketStockExchange WHERE ID > -1;
            DELETE FROM TradeReddeer WHERE ID > -1;";

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
    }
}
