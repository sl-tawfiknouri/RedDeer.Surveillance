﻿using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using TestHarness.Engine.Heartbeat;
using TestHarness.Engine.Heartbeat.Interfaces;
using TestHarness.Engine.OrderGenerator;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Engine.OrderGenerator.Strategies;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;
using TestHarness.Factory.TradingFactory.Interfaces;

namespace TestHarness.Factory.TradingFactory
{
    /// <summary>
    /// Keep the complexity of building these objects
    /// inside of factories or it will spread throughout the application
    /// </summary>
    public class TradingFactory 
        : ITradingFactory, 
        ITradingFactoryHeartbeatOrMarketUpdateSelector,
        ITradingFactoryHeartbeatSelector,
        ITradingFactoryVolumeStrategySelector,
         ITradingFactoryFilterStrategySelector,
        ICompleteSelector
    {
        private readonly ILogger _logger;

        // trade process selection
        private bool _heartbeatSelected;
        private bool _marketUpdateSelected;

        // heartbeat selection
        private IHeartbeat _heartbeat;

        // volume strategy
        private ITradeVolumeStrategy _volumeStrategy;

        private IReadOnlyCollection<string> _sedolFilter;

        public TradingFactory(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ITradingFactoryHeartbeatOrMarketUpdateSelector Create()
        {
            return this;
        }

        // Trading update process selector
        public ITradingFactoryVolumeStrategySelector MarketUpdate()
        {
            _marketUpdateSelected = true;
            return this;
        }

        public ITradingFactoryHeartbeatSelector Heartbeat()
        {
            _heartbeatSelected = true;
            return this;
        }

        // Heartbeat details
        public ITradingFactoryVolumeStrategySelector Regular(TimeSpan frequency)
        {
            _heartbeat = new Heartbeat(frequency);
            return this;
        }

        public ITradingFactoryVolumeStrategySelector Irregular(TimeSpan frequency, int sd)
        {
            _heartbeat = new IrregularHeartbeat(frequency, sd);
            return this;
        }
        
        // Trading volume picker
        public ITradingFactoryFilterStrategySelector TradingFixedVolume(int fixedVolume)
        {
            _volumeStrategy = new TradeVolumeFixedStrategy(fixedVolume);
            return this;
        }

        public ITradingFactoryFilterStrategySelector TradingNormalDistributionVolume(int sd)
        {
            _volumeStrategy = new TradeVolumeNormalDistributionStrategy(sd);
            return this;
        }

        // Sedol filter selector
        public ICompleteSelector FilterSedol(IReadOnlyCollection<string> sedols)
        {
            _sedolFilter = sedols ?? new List<string>();
            return this;
        }

        public ICompleteSelector FilterNone()
        {
            return this;
        }

        // Assemble choices
        public IOrderDataGenerator Finish()
        {
            if (_heartbeatSelected)
            {
                var strategy = new MarkovTradeStrategy(_logger, _volumeStrategy);
                IOrderDataGenerator process = new TradingHeartBeatDrivenProcess(_logger, strategy, _heartbeat);

                if (_sedolFilter?.Any() ?? false)
                {
                    process = new OrderDataGeneratorSedolFilteringDecorator(process, _sedolFilter);
                }

                return process;
            }

            if (_marketUpdateSelected)
            {
                var strategy = new MarkovTradeStrategy(_logger, _volumeStrategy);
                IOrderDataGenerator process = new TradingMarketUpdateDrivenProcess(_logger, strategy);

                if (_sedolFilter?.Any() ?? false)
                {
                    process = new OrderDataGeneratorSedolFilteringDecorator(process, _sedolFilter);
                }

                return process;
            }

            throw new ArgumentOutOfRangeException(nameof(_marketUpdateSelected));
        }
    }
}