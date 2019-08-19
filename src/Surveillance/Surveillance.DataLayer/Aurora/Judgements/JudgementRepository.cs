namespace Surveillance.DataLayer.Aurora.Judgements
{
    using System;
    using System.Threading.Tasks;

    using Dapper;

    using Domain.Surveillance.Judgement.Equity.Interfaces;

    using Microsoft.Extensions.Logging;

    using Surveillance.DataLayer.Aurora.Interfaces;
    using Surveillance.DataLayer.Aurora.Judgements.Interfaces;

    public class JudgementRepository : IJudgementRepository
    {
        private const string SaveHighProfit =
            @"INSERT INTO JudgementEquityHighProfitRule(RuleRunId, RuleRunCorrelationId, OrderId, ClientOrderId, Parameter, AbsoluteHighProfit, AbsoluteHighProfitCurrency, PercentageHighProfit, Analysis) VALUES(@RuleRunId, @RuleRunCorrelationId, @OrderId, @ClientOrderId, @Parameter, @AbsoluteHighProfit, @AbsoluteHighProfitCurrency, @PercentageHighProfit, @Analysis);";

        private readonly IConnectionStringFactory _dbConnectionFactory;

        private readonly ILogger<JudgementRepository> _logger;

        public JudgementRepository(IConnectionStringFactory dbConnectionFactory, ILogger<JudgementRepository> logger)
        {
            this._dbConnectionFactory =
                dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Save(IHighProfitJudgement highProfit)
        {
            this._logger?.LogInformation($"High profit judgement saving for rule run {highProfit.RuleRunId}");

            if (highProfit == null)
            {
                this._logger?.LogError("High profit judgement was null");
                return;
            }

            var dto = new HighProfitJudgementDto(highProfit);

            try
            {
                using (var dbConn = this._dbConnectionFactory.BuildConn())
                using (var conn = dbConn.ExecuteAsync(SaveHighProfit, dto))
                {
                    await conn;
                }
            }
            catch (Exception e)
            {
                this._logger?.LogError(
                    e,
                    $"Error in save insert for high profit {e.Message} {e?.InnerException?.Message}");
            }
        }

        public void Save(ICancelledOrderJudgement cancelledOrder)
        {
            if (cancelledOrder == null) this._logger?.LogError("Cancelled Order Judgement was null");
        }

        public void Save(IHighVolumeJudgement highVolume)
        {
            if (highVolume == null) this._logger?.LogError("High Volume Judgement was null");
        }

        public void Save(ILayeringJudgement layering)
        {
            if (layering == null) this._logger?.LogError("Layering Judgement was null");
        }

        public void Save(IMarkingTheCloseJudgement markingTheClose)
        {
            if (markingTheClose == null) this._logger?.LogError("Marking The Close Judgement was null");
        }

        public void Save(IPlacingOrdersWithNoIntentToExecuteJudgement placingOrders)
        {
            if (placingOrders == null) this._logger?.LogError("Placing Orders Judgement was null");
        }

        public void Save(IRampingJudgement ramping)
        {
            if (ramping == null) this._logger?.LogError("Ramping Judgement was null");
        }

        public void Save(ISpoofingJudgement spoofing)
        {
            if (spoofing == null)
            {
                this._logger?.LogError("Spoofing Judgement was null");
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
                if (judgement == null) return;

                this.RuleRunId = judgement.RuleRunId;
                this.RuleRunCorrelationId = judgement.RuleRunCorrelationId;
                this.OrderId = judgement.OrderId;
                this.ClientOrderId = judgement.ClientOrderId;
                this.AbsoluteHighProfit = judgement.AbsoluteHighProfit;
                this.AbsoluteHighProfitCurrency = judgement.AbsoluteHighProfitCurrency;
                this.PercentageHighProfit = judgement.PercentageHighProfit;
                this.Parameter = judgement.Parameters;
                this.Analysis = !judgement.NoAnalysis;
            }

            public decimal? AbsoluteHighProfit { get; }

            public string AbsoluteHighProfitCurrency { get; }

            public bool Analysis { get; }

            public string ClientOrderId { get; }

            public string OrderId { get; }

            public string Parameter { get; }

            public decimal? PercentageHighProfit { get; }

            public string RuleRunCorrelationId { get; }

            public string RuleRunId { get; }
        }
    }
}