using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Equity.Frames;
using Domain.Scheduling;
using Domain.Trades.Orders;
using Surveillance.DataLayer.ElasticSearch.Market.Interfaces;
using Surveillance.DataLayer.ElasticSearch.Trade.Interfaces;
using Surveillance.DataLayer.Projectors.Interfaces;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Universe
{
    /// <summary>
    /// Generates a universe from the existing data set
    /// </summary>
    public class UniverseBuilder : IUniverseBuilder
    {
        private readonly IRedDeerTradeFormatRepository _tradeRepository;
        private readonly IReddeerTradeFormatToReddeerTradeFrameProjector _documentProjector;

        private readonly IRedDeerMarketExchangeFormatRepository _equityMarketRepository;
        private readonly IReddeerMarketExchangeFormatToReddeerExchangeFrameProjector _equityMarketProjector;

        public UniverseBuilder(
            IRedDeerTradeFormatRepository tradeRepository,
            IReddeerTradeFormatToReddeerTradeFrameProjector documentProjector,
            IRedDeerMarketExchangeFormatRepository equityMarketRepository,
            IReddeerMarketExchangeFormatToReddeerExchangeFrameProjector equityMarketProjector)
        {
            _tradeRepository = tradeRepository ?? throw new ArgumentNullException(nameof(tradeRepository));
            _documentProjector = documentProjector ?? throw new ArgumentNullException(nameof(documentProjector));
            _equityMarketRepository = equityMarketRepository ?? throw new ArgumentNullException(nameof(equityMarketRepository));
            _equityMarketProjector = equityMarketProjector ?? throw new ArgumentNullException(nameof(equityMarketProjector));
        }

        /// <summary>
        /// Crack the cosmic egg and unscramble your reality
        /// </summary>
        public async Task<IUniverse> Summon(ScheduledExecution execution)
        {
            if (execution == null)
            {
                return new Universe(null, null, null);
            }

            var projectedTrades = await TradeDataFetch(execution);
            var exchangeFrames = await MarketEquityDataFetch(execution);
            var universe = await UniverseEvents(execution);

            return new Universe(projectedTrades, exchangeFrames, universe);
        }

        private async Task<IReadOnlyCollection<TradeOrderFrame>> TradeDataFetch(ScheduledExecution execution)
        {
            var trades = await _tradeRepository.Get(
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime);

            var projectedTrades = _documentProjector.Project(trades);

            return projectedTrades;
        }

        private async Task<IReadOnlyCollection<ExchangeFrame>> MarketEquityDataFetch(ScheduledExecution execution)
        {
            var equities = await _equityMarketRepository.Get(
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime);

            var projectedEquityData = _equityMarketProjector.Project(equities);

            return projectedEquityData;
        }

        private async Task<IReadOnlyCollection<IUniverseEvent>> UniverseEvents(ScheduledExecution execution)
        {
            //todo implement me
            return new IUniverseEvent[0];
        }
    }
}