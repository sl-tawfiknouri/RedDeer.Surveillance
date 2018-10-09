using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Equity.Frames;
using Domain.Scheduling;
using Domain.Trades.Orders;
using Surveillance.DataLayer.Aurora.Market.Interfaces;
using Surveillance.DataLayer.Aurora.Trade.Interfaces;
using Surveillance.DataLayer.ElasticSearch.Market.Interfaces;
using Surveillance.DataLayer.ElasticSearch.Trade.Interfaces;
using Surveillance.DataLayer.Projectors.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.MarketEvents.Interfaces;

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
        private readonly IReddeerTradeRepository _auroraTradeRepository;
        private readonly IReddeerMarketRepository _auroraMarketRepository;
        private readonly IMarketOpenCloseEventManager _marketManager;

        public UniverseBuilder(
            IRedDeerTradeFormatRepository tradeRepository,
            IReddeerTradeFormatToReddeerTradeFrameProjector documentProjector,
            IRedDeerMarketExchangeFormatRepository equityMarketRepository,
            IReddeerMarketExchangeFormatToReddeerExchangeFrameProjector equityMarketProjector,
            IReddeerTradeRepository auroraTradeRepository,
            IReddeerMarketRepository auroraMarketRepository,
            IMarketOpenCloseEventManager marketManager)
        {
            _tradeRepository = tradeRepository ?? throw new ArgumentNullException(nameof(tradeRepository));
            _documentProjector = documentProjector ?? throw new ArgumentNullException(nameof(documentProjector));
            _equityMarketRepository = equityMarketRepository ?? throw new ArgumentNullException(nameof(equityMarketRepository));
            _equityMarketProjector = equityMarketProjector ?? throw new ArgumentNullException(nameof(equityMarketProjector));
            _auroraTradeRepository = auroraTradeRepository ?? throw new ArgumentNullException(nameof(auroraTradeRepository));
            _auroraMarketRepository = auroraMarketRepository ?? throw new ArgumentNullException(nameof(auroraMarketRepository));
            _marketManager = marketManager ?? throw new ArgumentNullException(nameof(marketManager));
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

            execution.TimeSeriesInitiation = execution.TimeSeriesInitiation.ToUniversalTime().Date;
            execution.TimeSeriesTermination = execution.TimeSeriesTermination.ToUniversalTime().Date;

            var projectedTrades = await TradeDataFetchAurora(execution); //TradeDataFetch(execution);
            var exchangeFrames = await MarketEquityDataFetchAurora(execution); // MarketEquityDataFetch(execution);
            var universe = await UniverseEvents(execution, projectedTrades, exchangeFrames);

            return new Universe(projectedTrades, exchangeFrames, universe);
        }

        private async Task<IReadOnlyCollection<TradeOrderFrame>> TradeDataFetchAurora(ScheduledExecution execution)
        {
            var trades =
                await _auroraTradeRepository.Get(
                    execution.TimeSeriesInitiation.Date,
                    execution.TimeSeriesTermination.Date);

            return trades ?? new List<TradeOrderFrame>();
        }

        private async Task<IReadOnlyCollection<TradeOrderFrame>> TradeDataFetch(ScheduledExecution execution)
        {
            var trades = await _tradeRepository.Get(
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime);

            var projectedTrades = _documentProjector.Project(trades);

            return projectedTrades ?? new List<TradeOrderFrame>();
        }

        private async Task<IReadOnlyCollection<ExchangeFrame>> MarketEquityDataFetchAurora(ScheduledExecution execution)
        {
            var equities =
                await _auroraMarketRepository.Get(
                    execution.TimeSeriesInitiation.Date,
                    execution.TimeSeriesTermination.Date);

            return equities ?? new List<ExchangeFrame>();
        }

        private async Task<IReadOnlyCollection<ExchangeFrame>> MarketEquityDataFetch(ScheduledExecution execution)
        {
            var equities = await _equityMarketRepository.Get(
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime);

            var projectedEquityData = _equityMarketProjector.Project(equities);

            return projectedEquityData ?? new List<ExchangeFrame>();
        }

        private async Task<IReadOnlyCollection<IUniverseEvent>> UniverseEvents(
            ScheduledExecution execution,
            IReadOnlyCollection<TradeOrderFrame> trades,
            IReadOnlyCollection<ExchangeFrame> exchangeFrames)
        {
            var tradeEvents = 
                trades
                    .Where(tr => tr != null)
                    .Select(tr => new UniverseEvent(UniverseStateEvent.TradeReddeer, tr.StatusChangedOn, tr))
                    .ToArray();

            var exchangeEvents =
                exchangeFrames
                    .Select(exch => new UniverseEvent(UniverseStateEvent.StockTickReddeer, exch.TimeStamp, exch))
                    .ToArray();

            var marketEvents =
                await _marketManager
                    .AllOpenCloseEvents(
                        execution.TimeSeriesInitiation.DateTime,
                        execution.TimeSeriesTermination.DateTime);

            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, execution.TimeSeriesInitiation.DateTime, execution);
            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, execution.TimeSeriesTermination.DateTime, execution);

            var intraUniversalHistoryEvents = new List<IUniverseEvent>();
            intraUniversalHistoryEvents.AddRange(tradeEvents);
            intraUniversalHistoryEvents.AddRange(exchangeEvents);
            intraUniversalHistoryEvents.AddRange(marketEvents);
            var orderedIntraUniversalHistory = intraUniversalHistoryEvents.OrderBy(ihe => ihe.EventTime).ToList();

            var universeEvents = new List<IUniverseEvent> {genesis};
            universeEvents.AddRange(orderedIntraUniversalHistory);
            universeEvents.Add(eschaton);

            return universeEvents;
        }
    }
}