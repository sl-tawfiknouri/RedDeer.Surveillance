namespace TestHarness.Factory.TradingFactory
{
    using System;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Files.Orders.Interfaces;

    using TestHarness.Display.Interfaces;
    using TestHarness.Engine.OrderStorage;
    using TestHarness.Engine.OrderStorage.Interfaces;
    using TestHarness.Factory.TradingFactory.Interfaces;

    public class OrderFileStorageProcessFactory : IOrderFileStorageProcessFactory
    {
        private readonly IConsole _console;

        private readonly ILogger _logger;

        private readonly IOrderFileToOrderSerialiser _orderSerialiser;

        public OrderFileStorageProcessFactory(
            IConsole console,
            IOrderFileToOrderSerialiser orderSerialiser,
            ILogger logger)
        {
            this._console = console ?? throw new ArgumentNullException(nameof(console));
            this._orderSerialiser = orderSerialiser ?? throw new ArgumentNullException(nameof(orderSerialiser));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IOrderFileStorageProcess Build(string directory)
        {
            return new OrderFileStorageProcess(directory, this._console, this._orderSerialiser, this._logger);
        }
    }
}