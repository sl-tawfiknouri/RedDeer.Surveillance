﻿using System;
using Domain.Trades.Orders.Interfaces;
using NLog;
using TestHarness.Display.Interfaces;
using TestHarness.Engine.OrderStorage;
using TestHarness.Engine.OrderStorage.Interfaces;
using TestHarness.Factory.TradingFactory.Interfaces;

namespace TestHarness.Factory.TradingFactory
{
    public class OrderFileStorageProcessFactory : IOrderFileStorageProcessFactory
    {
        private readonly IConsole _console;
        private readonly ITradeOrderCsvToDtoMapper _csvMapper;
        private readonly ILogger _logger;
        
        public OrderFileStorageProcessFactory(IConsole console, ITradeOrderCsvToDtoMapper csvMapper, ILogger logger)
        {
            _console = console ?? throw new ArgumentNullException(nameof(console));
            _csvMapper = csvMapper ?? throw new ArgumentNullException(nameof(csvMapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IOrderFileStorageProcess Build(string directory)
        {
            return new OrderFileStorageProcess(directory, _console, _csvMapper, _logger);
        }
    }
}