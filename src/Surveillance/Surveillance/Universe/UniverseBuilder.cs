using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Equity.Frames;
using Domain.Scheduling;
using Domain.Trades.Orders;
using Surveillance.DataLayer.Aurora.Market.Interfaces;
using Surveillance.DataLayer.Aurora.Trade.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.MarketEvents.Interfaces;

namespace Surveillance.Universe
{
    /// <summary>
    /// Generates a universe from the existing data set
    /// </summary>
    public class UniverseBuilder : IUniverseBuilder
    {
        private readonly IReddeerTradeRepository _auroraTradeRepository;
        private readonly IReddeerMarketRepository _auroraMarketRepository;
        private readonly IMarketOpenCloseEventManager _marketManager;

        public UniverseBuilder(
            IReddeerTradeRepository auroraTradeRepository,
            IReddeerMarketRepository auroraMarketRepository,
            IMarketOpenCloseEventManager marketManager)
        {
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

            var projectedTrades = await TradeDataFetchAurora(execution);
            var exchangeFrames = await MarketEquityDataFetchAurora(execution);
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

        private async Task<IReadOnlyCollection<ExchangeFrame>> MarketEquityDataFetchAurora(ScheduledExecution execution)
        {
            var equities =
                await _auroraMarketRepository.Get(
                    execution.TimeSeriesInitiation.Date,
                    execution.TimeSeriesTermination.Date);

            return equities ?? new List<ExchangeFrame>();
        }

        private async Task<IReadOnlyCollection<IUniverseEvent>> UniverseEvents(
            ScheduledExecution execution,
            IReadOnlyCollection<TradeOrderFrame> trades,
            IReadOnlyCollection<ExchangeFrame> exchangeFrames)
        {
            var tradeSubmittedEvents =
                trades
                    .Where(tr => tr != null)
                    .Select(tr => new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, tr.TradeSubmittedOn, tr))
                    .ToArray();

            var tradeStatusChangedOnEvents =
                trades
                    .Where(tr => tr != null)
                    .Where(tr => ! (tr.OrderStatus == OrderStatus.Booked && tr.TradeSubmittedOn == tr.StatusChangedOn))
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
            intraUniversalHistoryEvents.AddRange(tradeSubmittedEvents);
            intraUniversalHistoryEvents.AddRange(tradeStatusChangedOnEvents);
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