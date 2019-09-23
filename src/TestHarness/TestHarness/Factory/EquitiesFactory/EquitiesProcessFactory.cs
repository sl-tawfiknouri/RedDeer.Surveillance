namespace TestHarness.Factory.EquitiesFactory
{
    using System;

    using Microsoft.Extensions.Logging;

    using TestHarness.Engine.EquitiesGenerator;
    using TestHarness.Engine.EquitiesGenerator.Interfaces;
    using TestHarness.Engine.EquitiesGenerator.Strategies;
    using TestHarness.Engine.Heartbeat;
    using TestHarness.Engine.Heartbeat.Interfaces;
    using TestHarness.Factory.EquitiesFactory.Interfaces;

    public class EquitiesProcessFactory : IEquitiesProcessFactory, ICompleteSelector, IHeartbeatSelector
    {
        private readonly ILogger _logger;

        private IHeartbeat _heartbeat;

        public EquitiesProcessFactory(ILogger logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IHeartbeatSelector Create()
        {
            return this;
        }

        public IEquityDataGenerator Finish()
        {
            var equityDataStrategy = new MarkovEquityStrategy();
            var nasdaqInitialiser = new NasdaqInitialiser();
            var equityDataGenerator = new EquitiesMarkovProcess(
                nasdaqInitialiser,
                equityDataStrategy,
                this._heartbeat,
                this._logger);

            return equityDataGenerator;
        }

        public ICompleteSelector Irregular(TimeSpan frequency, int sd)
        {
            this._heartbeat = new IrregularHeartbeat(frequency, sd);
            return this;
        }

        public ICompleteSelector Regular(TimeSpan frequency)
        {
            this._heartbeat = new Heartbeat(frequency);
            return this;
        }
    }
}