namespace TestHarness.Factory.TradeMarkingTheCloseFactory
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Extensions.Logging;

    using RedDeer.Contracts.SurveillanceService.Api.Markets;

    using TestHarness.Engine.OrderGenerator;
    using TestHarness.Engine.OrderGenerator.Interfaces;
    using TestHarness.Engine.OrderGenerator.Strategies;
    using TestHarness.Factory.TradeMarkingTheCloseFactory.Interfaces;

    public class TradingMarkingTheCloseFactory : ITradingMarkingTheCloseFactory
    {
        private readonly ILogger _logger;

        public TradingMarkingTheCloseFactory(ILogger logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IOrderDataGenerator Build(IReadOnlyCollection<string> sedols, ExchangeDto market)
        {
            sedols = sedols ?? new List<string>();

            return new TradingMarkingTheCloseProcess(sedols, new StubTradeStrategy(), market, this._logger);
        }
    }
}