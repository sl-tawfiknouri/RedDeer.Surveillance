using System;
using System.Collections.Generic;
using System.Linq;
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
            var universe = UniverseEvents(execution, projectedTrades, exchangeFrames);

            return new Universe(projectedTrades, exchangeFrames, universe);
        }

        private async Task<IReadOnlyCollection<TradeOrderFrame>> TradeDataFetch(ScheduledExecution execution)
        {
            var trades = await _tradeRepository.Get(
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime);

            var projectedTrades = _documentProjector.Project(trades);

            return projectedTrades ?? new List<TradeOrderFrame>();
        }

        private async Task<IReadOnlyCollection<ExchangeFrame>> MarketEquityDataFetch(ScheduledExecution execution)
        {
            var equities = await _equityMarketRepository.Get(
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime);

            var projectedEquityData = _equityMarketProjector.Project(equities);

            return projectedEquityData ?? new List<ExchangeFrame>();
        }

        private IReadOnlyCollection<IUniverseEvent> UniverseEvents(
            ScheduledExecution execution,
            IReadOnlyCollection<TradeOrderFrame> trades,
            IReadOnlyCollection<ExchangeFrame> exchangeFrames)
        {
            var tradeEvents = 
                trades
                    .Select(tr => new UniverseEvent(UniverseStateEvent.TradeReddeer, tr.StatusChangedOn, (object) tr))
                    .ToArray();

            var exchangeEvents =
                exchangeFrames
                    .Select(exch => new UniverseEvent(UniverseStateEvent.StockTickReddeer, exch.TimeStamp, (object) exch))
                    .ToArray();

            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, execution.TimeSeriesInitiation.DateTime, execution);
            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, execution.TimeSeriesTermination.DateTime, execution);

            var intraHistoryEvents = new List<IUniverseEvent>();
            intraHistoryEvents.AddRange(tradeEvents);
            intraHistoryEvents.AddRange(exchangeEvents);
            var orderedIntraHistory = intraHistoryEvents.OrderBy(ihe => ihe.EventTime).ToList();

            var universeEvents = new List<IUniverseEvent>();
            universeEvents.Add(genesis);
            universeEvents.AddRange(orderedIntraHistory);
            universeEvents.Add(eschaton);

            return universeEvents;
        }
    }
}