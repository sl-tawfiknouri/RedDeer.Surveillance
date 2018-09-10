using System;
using System.Threading.Tasks;
using Surveillance.DataLayer.ElasticSearch.Trade.Interfaces;
using Surveillance.DataLayer.Projectors.Interfaces;
using Surveillance.Scheduler;
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

        public UniverseBuilder(
            IRedDeerTradeFormatRepository tradeRepository,
            IReddeerTradeFormatToReddeerTradeFrameProjector documentProjector)
        {
            _tradeRepository = tradeRepository ?? throw new ArgumentNullException(nameof(tradeRepository));
            _documentProjector = documentProjector ?? throw new ArgumentNullException(nameof(documentProjector));
        }

        /// <summary>
        /// Crack the cosmic egg and unscramble a reality
        /// </summary>
        public async Task<IUniverse> Summon(ScheduledExecution execution)
        {
            if (execution == null)
            {
                return new Universe(null);
            }

            var trades = await _tradeRepository.Get(
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime);

            var projectedTrades = _documentProjector.Project(trades);

            return new Universe(projectedTrades);
        }
    }
}