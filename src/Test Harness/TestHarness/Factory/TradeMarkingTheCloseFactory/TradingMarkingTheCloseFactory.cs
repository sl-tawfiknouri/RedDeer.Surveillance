﻿using System;
using System.Collections.Generic;
using NLog;
using RedDeer.Contracts.SurveillanceService.Api.Markets;
using TestHarness.Engine.OrderGenerator;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Engine.OrderGenerator.Strategies;
using TestHarness.Factory.TradeMarkingTheCloseFactory.Interfaces;

namespace TestHarness.Factory.TradeMarkingTheCloseFactory
{
    public class TradingMarkingTheCloseFactory : ITradingMarkingTheCloseFactory
    {
        private readonly ILogger _logger;

        public TradingMarkingTheCloseFactory(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IOrderDataGenerator Build(IReadOnlyCollection<string> sedols, ExchangeDto market)
        {
            sedols = sedols ?? new List<string>();

            return new TradingMarkingTheCloseProcess(sedols, new StubTradeStrategy(), market, _logger);
        }
    }
}
