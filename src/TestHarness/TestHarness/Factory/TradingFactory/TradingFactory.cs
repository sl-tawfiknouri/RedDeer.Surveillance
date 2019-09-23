namespace TestHarness.Factory.TradingFactory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Extensions.Logging;

    using TestHarness.Engine.Heartbeat;
    using TestHarness.Engine.Heartbeat.Interfaces;
    using TestHarness.Engine.OrderGenerator;
    using TestHarness.Engine.OrderGenerator.Interfaces;
    using TestHarness.Engine.OrderGenerator.Strategies;
    using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;
    using TestHarness.Factory.EquitiesFactory.Interfaces;
    using TestHarness.Factory.TradingFactory.Interfaces;

    using ICompleteSelector = TestHarness.Factory.TradingFactory.Interfaces.ICompleteSelector;

    /// <summary>
    ///     Keep the complexity of building these objects
    ///     inside of factories or it will spread throughout the application
    /// </summary>
    public class TradingFactory : ITradingFactory,
                                  ITradingFactoryHeartbeatOrMarketUpdateSelector,
                                  ITradingFactoryHeartbeatSelector,
                                  ITradingFactoryVolumeStrategySelector,
                                  ITradingFactoryFilterStrategySelector,
                                  ICompleteSelector
    {
        private readonly ILogger _logger;

        private readonly IStockExchangeStreamFactory _streamFactory;

        // heartbeat selection
        private IHeartbeat _heartbeat;

        // trade process selection
        private bool _heartbeatSelected;

        private bool _inclusive;

        private bool _marketUpdateSelected;

        private IReadOnlyCollection<string> _sedolFilter;

        // volume strategy
        private ITradeVolumeStrategy _volumeStrategy;

        public TradingFactory(IStockExchangeStreamFactory streamFactory, ILogger logger)
        {
            this._streamFactory = streamFactory ?? throw new ArgumentNullException(nameof(streamFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ITradingFactoryHeartbeatOrMarketUpdateSelector Create()
        {
            return this;
        }

        // Assemble choices
        public IOrderDataGenerator Finish()
        {
            if (this._heartbeatSelected)
            {
                var strategy = new MarkovTradeStrategy(this._logger, this._volumeStrategy);
                IOrderDataGenerator process = new TradingHeartBeatDrivenProcess(
                    this._logger,
                    strategy,
                    this._heartbeat);

                if (this._sedolFilter != null && this._sedolFilter.Any())
                    process = new OrderDataGeneratorSedolFilteringDecorator(
                        this._streamFactory,
                        process,
                        this._sedolFilter,
                        this._inclusive);

                return process;
            }

            if (this._marketUpdateSelected)
            {
                var strategy = new MarkovTradeStrategy(this._logger, this._volumeStrategy);
                IOrderDataGenerator process = new TradingMarketUpdateDrivenProcess(this._logger, strategy);

                if (this._sedolFilter != null && this._sedolFilter.Any())
                    process = new OrderDataGeneratorSedolFilteringDecorator(
                        this._streamFactory,
                        process,
                        this._sedolFilter,
                        this._inclusive);

                return process;
            }

            throw new ArgumentOutOfRangeException(nameof(this._marketUpdateSelected));
        }

        public ITradingFactoryHeartbeatSelector Heartbeat()
        {
            this._heartbeatSelected = true;
            return this;
        }

        public ITradingFactoryVolumeStrategySelector Irregular(TimeSpan frequency, int sd)
        {
            this._heartbeat = new IrregularHeartbeat(frequency, sd);
            return this;
        }

        // Trading update process selector
        public ITradingFactoryVolumeStrategySelector MarketUpdate()
        {
            this._marketUpdateSelected = true;
            return this;
        }

        // Heartbeat details
        public ITradingFactoryVolumeStrategySelector Regular(TimeSpan frequency)
        {
            this._heartbeat = new Heartbeat(frequency);
            return this;
        }

        public ICompleteSelector SetFilterNone()
        {
            this._sedolFilter = new List<string>();
            return this;
        }

        // Sedol filter selector
        public ICompleteSelector SetFilterSedol(IReadOnlyCollection<string> sedols, bool inclusive)
        {
            this._sedolFilter = sedols ?? new List<string>();
            this._inclusive = inclusive;
            return this;
        }

        // Trading volume picker
        public ITradingFactoryFilterStrategySelector TradingFixedVolume(int fixedVolume)
        {
            this._volumeStrategy = new TradeVolumeFixedStrategy(fixedVolume);
            return this;
        }

        public ITradingFactoryFilterStrategySelector TradingNormalDistributionVolume(int sd)
        {
            this._volumeStrategy = new TradeVolumeNormalDistributionStrategy(sd);
            return this;
        }
    }
}