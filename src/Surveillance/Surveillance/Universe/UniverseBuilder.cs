using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainV2.Equity.Frames;
using DomainV2.Financial;
using DomainV2.Scheduling;
using DomainV2.Trading;
using Surveillance.DataLayer.Aurora.Market.Interfaces;
using Surveillance.DataLayer.Aurora.Trade.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
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
        private readonly IUniverseSortComparer _universeSorter;

        public UniverseBuilder(
            IReddeerTradeRepository auroraTradeRepository,
            IReddeerMarketRepository auroraMarketRepository,
            IMarketOpenCloseEventManager marketManager,
            IUniverseSortComparer universeSorter)
        {
            _auroraTradeRepository = auroraTradeRepository ?? throw new ArgumentNullException(nameof(auroraTradeRepository));
            _auroraMarketRepository = auroraMarketRepository ?? throw new ArgumentNullException(nameof(auroraMarketRepository));
            _marketManager = marketManager ?? throw new ArgumentNullException(nameof(marketManager));
            _universeSorter = universeSorter ?? throw new ArgumentNullException(nameof(universeSorter));
        }

        /// <summary>
        /// Crack the cosmic egg and unscramble your reality
        /// </summary>
        public async Task<IUniverse> Summon(ScheduledExecution execution, ISystemProcessOperationContext opCtx)
        {
            if (execution == null)
            {
                return new Universe(null, null, null);
            }

            var projectedTrades = await TradeDataFetchAurora(execution, opCtx);
            var exchangeFrames = await MarketEquityDataFetchAurora(execution, opCtx);
            var universe = await UniverseEvents(execution, projectedTrades, exchangeFrames);

            return new Universe(projectedTrades, exchangeFrames, universe);
        }

        private async Task<IReadOnlyCollection<Order>> TradeDataFetchAurora(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx)
        {
            var trades =
                await _auroraTradeRepository.Get(
                    execution.TimeSeriesInitiation.Date,
                    execution.TimeSeriesTermination.Date,
                    opCtx);

            return trades ?? new List<Order>();
        }

        private async Task<IReadOnlyCollection<ExchangeFrame>> MarketEquityDataFetchAurora(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx)
        {
            var equities =
                await _auroraMarketRepository.Get(
                    execution.TimeSeriesInitiation.Date,
                    execution.TimeSeriesTermination.Date,
                    opCtx);

            return equities ?? new List<ExchangeFrame>();
        }

        private async Task<IReadOnlyCollection<IUniverseEvent>> UniverseEvents(
            ScheduledExecution execution,
            IReadOnlyCollection<Order> trades,
            IReadOnlyCollection<ExchangeFrame> exchangeFrames)
        {
            var tradeSubmittedEvents =
                trades
                    .Where(tr => tr != null)
                    .Select(tr => new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, tr.OrderPlacedDate.GetValueOrDefault(), tr))
                    .ToArray();

            var tradeStatusChangedOnEvents =
                trades
                    .Where(tr => tr != null)
                    .Where(tr => ! (tr.OrderStatus() == OrderStatus.Booked && tr.OrderPlacedDate == tr.MostRecentDateEvent()))
                    .Select(tr => new UniverseEvent(UniverseStateEvent.TradeReddeer, tr.MostRecentDateEvent(), tr))
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

            var intraUniversalHistoryEvents = new List<IUniverseEvent>();
            intraUniversalHistoryEvents.AddRange(tradeSubmittedEvents);
            intraUniversalHistoryEvents.AddRange(tradeStatusChangedOnEvents);
            intraUniversalHistoryEvents.AddRange(exchangeEvents);
            intraUniversalHistoryEvents.AddRange(marketEvents);
            var orderedIntraUniversalHistory = intraUniversalHistoryEvents.OrderBy(ihe => ihe, _universeSorter).ToList();

            var universeEvents = new List<IUniverseEvent> {genesis};
            universeEvents.AddRange(orderedIntraUniversalHistory);

            var youngestEventInUniverse = orderedIntraUniversalHistory.Any()
                ? orderedIntraUniversalHistory.Max(i => i.EventTime)
                : execution.TimeSeriesTermination.DateTime;
            var eschatonDate = youngestEventInUniverse > execution.TimeSeriesTermination.DateTime
                ? youngestEventInUniverse
                : execution.TimeSeriesTermination.DateTime;
            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, eschatonDate, execution);
            universeEvents.Add(eschaton);

            return universeEvents;
        }
    }
}