using System;
using System.Threading.Tasks;
using Dapper;
using Domain.Surveillance.Judgement.Equity;
using Domain.Surveillance.Judgement.Equity.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Interfaces;
using Surveillance.DataLayer.Aurora.Judgements.Interfaces;

namespace Surveillance.DataLayer.Aurora.Judgements
{
    public class JudgementRepository : IJudgementRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<JudgementRepository> _logger;

        private const string SaveHighProfit =
            @"INSERT INTO JudgementEquityHighProfitRule(RuleRunId, RuleRunCorrelationId, OrderId, ClientOrderId, Parameter, AbsoluteHighProfit, AbsoluteHighProfitCurrency, PercentageHighProfit, Analysis) VALUES(@RuleRunId, @RuleRunCorrelationId, @OrderId, @ClientOrderId, @Parameter, @AbsoluteHighProfit, @AbsoluteHighProfitCurrency, @PercentageHighProfit, @Analysis);";

        public JudgementRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<JudgementRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Save(IHighProfitJudgement highProfit)
        {
            _logger?.LogInformation($"High profit judgement saving for rule run {highProfit.RuleRunId}");

            if (highProfit == null)
            {
                _logger?.LogError($"High profit judgement was null");
                return;
            }

            var dto = new HighProfitJudgementDto(highProfit);

            try
            {
                using (var dbConn = _dbConnectionFactory.BuildConn())
                using (var conn = dbConn.ExecuteAsync(SaveHighProfit, dto))
                {
                    await conn;
                }
            }
            catch (Exception e)
            {
                _logger?.LogError(e, $"Error in save insert for high profit {e.Message} {e?.InnerException?.Message}");
            }
        }
        
        public void Save(ICancelledOrderJudgement cancelledOrder)
        {
            if (cancelledOrder == null)
            {
                _logger?.LogError($"Cancelled Order Judgement was null");
                return;
            }
        }

        public void Save(IHighVolumeJudgement highVolume)
        {
            if (highVolume == null)
            {
                _logger?.LogError($"High Volume Judgement was null");
                return;
            }
        }

        public void Save(ILayeringJudgement layering)
        {
            if (layering == null)
            {
                _logger?.LogError($"Layering Judgement was null");
                return;
            }
        }
        
        public void Save(IMarkingTheCloseJudgement markingTheClose)
        {
            if (markingTheClose == null)
            {
                _logger?.LogError($"Marking The Close Judgement was null");
                return;
            }
        }

        public void Save(IPlacingOrdersWithNoIntentToExecuteJudgement placingOrders)
        {
            if (placingOrders == null)
            {
                _logger?.LogError($"Placing Orders Judgement was null");
                return;
            }
        }

        public void Save(IRampingJudgement ramping)
        {
            if (ramping == null)
            {
                _logger?.LogError($"Ramping Judgement was null");
                return;
            }
        }

        public void Save(ISpoofingJudgement spoofing)
        {
            if (spoofing == null)
            {
                _logger?.LogError($"Spoofing Judgement was null");
                return;
            }
        }

        private class HighProfitJudgementDto
        {
            public HighProfitJudgementDto()
            {
                // leave for dapper
            }

            public HighProfitJudgementDto(IHighProfitJudgement judgement)
            {
                if (judgement == null)
                {
                    return;
                }

                RuleRunId = judgement.RuleRunId;
                RuleRunCorrelationId = judgement.RuleRunCorrelationId;
                OrderId = judgement.OrderId;
                ClientOrderId = judgement.ClientOrderId;
                AbsoluteHighProfit = judgement.AbsoluteHighProfit;
                AbsoluteHighProfitCurrency = judgement.AbsoluteHighProfitCurrency;
                PercentageHighProfit = judgement.PercentageHighProfit;
                Parameter = judgement.Parameters;
                Analysis = !judgement.NoAnalysis;
            }

            public string RuleRunId { get; set; }
            public string RuleRunCorrelationId { get; set; }
            public string OrderId { get; set; }
            public string ClientOrderId { get; set; }

            public decimal? AbsoluteHighProfit { get; set; }
            public string AbsoluteHighProfitCurrency { get; set; }
            public decimal? PercentageHighProfit { get; set; }
            public string Parameter { get; set; }
            public bool Analysis { get; set; }
        }
    }
}
