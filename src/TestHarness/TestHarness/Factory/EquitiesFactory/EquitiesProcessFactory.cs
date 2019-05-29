using System;
using Microsoft.Extensions.Logging;
using TestHarness.Engine.EquitiesGenerator;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Engine.EquitiesGenerator.Strategies;
using TestHarness.Engine.Heartbeat;
using TestHarness.Engine.Heartbeat.Interfaces;
using TestHarness.Factory.EquitiesFactory.Interfaces;

namespace TestHarness.Factory.EquitiesFactory
{
    public class EquitiesProcessFactory : IEquitiesProcessFactory, ICompleteSelector, IHeartbeatSelector
    {
        private readonly ILogger _logger;
        private IHeartbeat _heartbeat;

        public EquitiesProcessFactory(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IHeartbeatSelector Create()
        {
            return this;
        }

        public ICompleteSelector Regular(TimeSpan frequency)
        {
            _heartbeat = new Heartbeat(frequency);
            return this;
        }

        public ICompleteSelector Irregular(TimeSpan frequency, int sd)
        {
            _heartbeat = new IrregularHeartbeat(frequency, sd);
            return this;
        }

        public IEquityDataGenerator Finish()
        {
            var equityDataStrategy = new MarkovEquityStrategy();
            var nasdaqInitialiser = new NasdaqInitialiser();
            var equityDataGenerator = 
                new EquitiesMarkovProcess(
                    nasdaqInitialiser,
                    equityDataStrategy,
                    _heartbeat,
                    _logger);

            return equityDataGenerator;
        }
    }
}
