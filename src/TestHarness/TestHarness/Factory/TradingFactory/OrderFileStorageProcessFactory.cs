using System;
using Microsoft.Extensions.Logging;
using SharedKernel.Files.Orders.Interfaces;
using TestHarness.Display.Interfaces;
using TestHarness.Engine.OrderStorage;
using TestHarness.Engine.OrderStorage.Interfaces;
using TestHarness.Factory.TradingFactory.Interfaces;

namespace TestHarness.Factory.TradingFactory
{
    public class OrderFileStorageProcessFactory : IOrderFileStorageProcessFactory
    {
        private readonly IConsole _console;
        private readonly IOrderFileToOrderSerialiser _orderSerialiser;
        private readonly ILogger _logger;
        
        public OrderFileStorageProcessFactory(IConsole console, IOrderFileToOrderSerialiser orderSerialiser, ILogger logger)
        {
            _console = console ?? throw new ArgumentNullException(nameof(console));
            _orderSerialiser = orderSerialiser ?? throw new ArgumentNullException(nameof(orderSerialiser));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IOrderFileStorageProcess Build(string directory)
        {
            return new OrderFileStorageProcess(directory, _console, _orderSerialiser, _logger);
        }
    }
}
