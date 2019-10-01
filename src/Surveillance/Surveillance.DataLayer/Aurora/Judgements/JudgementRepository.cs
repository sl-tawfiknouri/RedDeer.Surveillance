namespace Surveillance.DataLayer.Aurora.Judgements
{
    // ReSharper disable MemberCanBePrivate.Local
    // ReSharper disable MemberCanBeProtected.Local
    using System;
    using System.Threading.Tasks;

    using Dapper;

    using Domain.Surveillance.Judgement.Equity.Interfaces;
    using Domain.Surveillance.Judgement.FixedIncome.Interfaces;

    using Microsoft.Extensions.Logging;

    using Surveillance.DataLayer.Aurora.Interfaces;
    using Surveillance.DataLayer.Aurora.Judgements.Interfaces;

    /// <summary>
    /// The judgement repository.
    /// </summary>
    public class JudgementRepository : IJudgementRepository
    {
        /// <summary>
        /// The save high profit.
        /// </summary>
        private const string SaveHighProfit =
            @"INSERT INTO JudgementEquityHighProfitRule(RuleRunId, RuleRunCorrelationId, OrderId, ClientOrderId, Parameter, AbsoluteHighProfit, AbsoluteHighProfitCurrency, PercentageHighProfit, Analysis) VALUES(@RuleRunId, @RuleRunCorrelationId, @OrderId, @ClientOrderId, @Parameter, @AbsoluteHighProfit, @AbsoluteHighProfitCurrency, @PercentageHighProfit, @Analysis);";

        /// <summary>
        /// The save high volume.
        /// </summary>
        private const string SaveHighVolume =
            @"INSERT INTO JudgementEquityHighVolumeRule(RuleRunId, RuleRunCorrelationId, OrderId, ClientOrderId, Parameter, Analysis, WindowVolumeThresholdAmount, WindowVolumeThresholdPercentage, WindowVolumeTradedAmount, WindowVolumeTradedPercentage, WindowVolumeBreach, DailyVolumeThresholdAmount, DailyVolumeThresholdPercentage, DailyVolumeTradedAmount, DailyVolumeTradedPercentage, DailyVolumeBreach)
                VALUES(@RuleRunId, @RuleRunCorrelationId, @OrderId, @ClientOrderId, @Parameter, @Analysis, @WindowVolumeThresholdAmount, @WindowVolumeThresholdPercentage, @WindowVolumeTradedAmount, @WindowVolumeTradedPercentage, @WindowVolumeBreach, @DailyVolumeThresholdAmount, @DailyVolumeThresholdPercentage, @DailyVolumeTradedAmount, @DailyVolumeTradedPercentage, @DailyVolumeBreach);";

        /// <summary>
        /// The database connection factory.
        /// </summary>
        private readonly IConnectionStringFactory databaseConnectionFactory;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<JudgementRepository> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="JudgementRepository"/> class.
        /// </summary>
        /// <param name="databaseConnectionFactory">
        /// The database connection factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public JudgementRepository(
            IConnectionStringFactory databaseConnectionFactory,
            ILogger<JudgementRepository> logger)
        {
            this.databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="highProfit">
        /// The high profit.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Save(IHighProfitJudgement highProfit)
        {
            this.logger?.LogInformation($"High profit judgement saving for rule run {highProfit.RuleRunId}");

            if (highProfit == null)
            {
                this.logger?.LogError("High profit judgement was null");
                return;
            }

            var dto = new HighProfitJudgementDto(highProfit);

            try
            {
                using (var databaseConnection = this.databaseConnectionFactory.BuildConn())
                using (var connection = databaseConnection.ExecuteAsync(SaveHighProfit, dto))
                {
                    await connection;
                }
            }
            catch (Exception e)
            {
                this.logger?.LogError(
                    e,
                    $"Error in save insert for high profit {e.Message} {e?.InnerException?.Message}");
            }
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="cancelledOrder">
        /// The cancelled order.
        /// </param>
        public void Save(ICancelledOrderJudgement cancelledOrder)
        {
            if (cancelledOrder == null)
            {
                this.logger?.LogError("Cancelled Order Judgement was null");
            }
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="highVolume">
        /// The high volume.
        /// </param>
        public void Save(IHighVolumeJudgement highVolume)
        {
            if (highVolume == null)
            {
                this.logger?.LogError("High Volume Judgement was null");
            }
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="layering">
        /// The layering.
        /// </param>
        public void Save(ILayeringJudgement layering)
        {
            if (layering == null)
            {
                this.logger?.LogError("Layering Judgement was null");
            }
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="markingTheClose">
        /// The marking the close.
        /// </param>
        public void Save(IMarkingTheCloseJudgement markingTheClose)
        {
            if (markingTheClose == null)
            {
                this.logger?.LogError("Marking The Close Judgement was null");
            }
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="placingOrders">
        /// The placing orders.
        /// </param>
        public void Save(IPlacingOrdersWithNoIntentToExecuteJudgement placingOrders)
        {
            if (placingOrders == null)
            {
                this.logger?.LogError("Placing Orders Judgement was null");
            }
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="ramping">
        /// The ramping.
        /// </param>
        public void Save(IRampingJudgement ramping)
        {
            if (ramping == null)
            {
                this.logger?.LogError("Ramping Judgement was null");
            }
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="spoofing">
        /// The spoofing.
        /// </param>
        public void Save(ISpoofingJudgement spoofing)
        {
            if (spoofing == null)
            {
                this.logger?.LogError("Spoofing Judgement was null");
            }
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="highProfit">
        /// The high profit.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Save(IFixedIncomeHighProfitJudgement highProfit)
        {
            this.logger?.LogInformation($"Fixed Income High profit judgement saving for rule run {highProfit.RuleRunId}");

            if (highProfit == null)
            {
                this.logger?.LogError("Fixed Income High profit judgement was null");
                return;
            }

            var dto = new HighProfitJudgementDto(highProfit);

            try
            {
                using (var databaseConnection = this.databaseConnectionFactory.BuildConn())
                using (var connection = databaseConnection.ExecuteAsync(SaveHighProfit, dto))
                {
                    await connection;
                }
            }
            catch (Exception e)
            {
                this.logger?.LogError(e, $"Error in save insert for high profit {e.Message} {e?.InnerException?.Message}");
            }
        }

        /// <summary>
        /// The save fixed income high volume judgement.
        /// </summary>
        /// <param name="highVolume">
        /// The high volume.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Save(IFixedIncomeHighVolumeJudgement highVolume)
        {
            this.logger?.LogInformation($"Fixed Income High Volume judgement saving for rule run {highVolume.RuleRunId}");

            if (highVolume == null)
            {
                this.logger?.LogError("Fixed Income High Volume judgement was null");

                return;
            }

            var dto = new HighVolumeJudgementDto(highVolume);

            try
            {
                using (var databaseConnection = this.databaseConnectionFactory.BuildConn())
                using (var conn = databaseConnection.ExecuteAsync(SaveHighVolume, dto))
                {
                    await conn;
                }
            }
            catch (Exception e)
            {
                this.logger?.LogError(e, $"Error in save insert for high volume {e.Message} {e?.InnerException?.Message}");
            }
        }

        /// <summary>
        /// The judgement.
        /// </summary>
        private abstract class JudgementDto
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="JudgementDto"/> class.
            /// </summary>
            /// <param name="analysis">
            /// The analysis.
            /// </param>
            /// <param name="clientOrderId">
            /// The client order id.
            /// </param>
            /// <param name="orderId">
            /// The order id.
            /// </param>
            /// <param name="parameter">
            /// The parameter.
            /// </param>
            /// <param name="ruleRunCorrelationId">
            /// The rule run correlation id.
            /// </param>
            /// <param name="ruleRunId">
            /// The rule run id.
            /// </param>
            protected JudgementDto(
                bool analysis,
                string clientOrderId,
                string orderId,
                string parameter,
                string ruleRunCorrelationId,
                string ruleRunId)
            {
                this.Analysis = analysis;
                this.ClientOrderId = clientOrderId;
                this.OrderId = orderId;
                this.Parameter = parameter;
                this.RuleRunCorrelationId = ruleRunCorrelationId;
                this.RuleRunId = ruleRunId;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="JudgementDto"/> class.
            /// </summary>
            protected JudgementDto()
            {
                // leave for dapper
            }

            /// <summary>
            /// Gets a value indicating whether analysis.
            /// </summary>
            public bool Analysis { get; }

            /// <summary>
            /// Gets the client order id.
            /// </summary>
            public string ClientOrderId { get; }

            /// <summary>
            /// Gets the order id.
            /// </summary>
            public string OrderId { get; }

            /// <summary>
            /// Gets the parameter.
            /// </summary>
            public string Parameter { get; }

            /// <summary>
            /// Gets the rule run correlation id.
            /// </summary>
            public string RuleRunCorrelationId { get; }

            /// <summary>
            /// Gets the rule run id.
            /// </summary>
            public string RuleRunId { get; }
        }

        /// <summary>
        /// The high volume judgement.
        /// </summary>
        private class HighVolumeJudgementDto : JudgementDto
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="HighVolumeJudgementDto"/> class.
            /// </summary>
            public HighVolumeJudgementDto()
            {
                // leave for dapper
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="HighVolumeJudgementDto"/> class.
            /// </summary>
            /// <param name="judgement">
            /// The judgement.
            /// </param>
            public HighVolumeJudgementDto(IFixedIncomeHighVolumeJudgement judgement)
            : base(
                !judgement?.NoAnalysis ?? false,
                judgement?.ClientOrderId,
                judgement?.OrderId,
                judgement?.Parameters,
                judgement?.RuleRunCorrelationId,
                judgement?.RuleRunId)
            {
                this.WindowVolumeThresholdAmount = judgement.WindowAnalysisAnalysis?.VolumeThresholdAmount;
                this.WindowVolumeThresholdPercentage = judgement.WindowAnalysisAnalysis?.VolumeThresholdPercentage;
                this.WindowVolumeTradedAmount = judgement.WindowAnalysisAnalysis?.VolumeTradedAmount;
                this.WindowVolumeTradedPercentage = judgement.WindowAnalysisAnalysis?.VolumeTradedPercentage;
                this.WindowVolumeBreach = judgement?.WindowAnalysisAnalysis?.VolumeBreach ?? false;
                this.DailyVolumeThresholdAmount = judgement.DailyAnalysisAnalysis?.VolumeThresholdAmount;
                this.DailyVolumeThresholdPercentage = judgement.DailyAnalysisAnalysis?.VolumeThresholdPercentage;
                this.DailyVolumeTradedAmount = judgement.DailyAnalysisAnalysis?.VolumeTradedAmount;
                this.DailyVolumeTradedPercentage = judgement.DailyAnalysisAnalysis?.VolumeTradedPercentage;
                this.DailyVolumeBreach = judgement?.DailyAnalysisAnalysis?.VolumeBreach ?? false;
            }

            /// <summary>
            /// Gets or sets the window volume threshold amount.
            /// </summary>
            public decimal? WindowVolumeThresholdAmount { get; set; }

            /// <summary>
            /// Gets or sets the window volume threshold percentage.
            /// </summary>
            public decimal? WindowVolumeThresholdPercentage { get; set; }

            /// <summary>
            /// Gets or sets the window volume traded amount.
            /// </summary>
            public decimal? WindowVolumeTradedAmount { get; set; }

            /// <summary>
            /// Gets or sets the window volume traded percentage.
            /// </summary>
            public decimal? WindowVolumeTradedPercentage { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether window volume breach.
            /// </summary>
            public bool WindowVolumeBreach { get; set; }

            /// <summary>
            /// Gets or sets the daily volume threshold amount.
            /// </summary>
            public decimal? DailyVolumeThresholdAmount { get; set; }

            /// <summary>
            /// Gets or sets the daily volume threshold percentage.
            /// </summary>
            public decimal? DailyVolumeThresholdPercentage { get; set; }

            /// <summary>
            /// Gets or sets the daily volume traded amount.
            /// </summary>
            public decimal? DailyVolumeTradedAmount { get; set; }

            /// <summary>
            /// Gets or sets the daily volume traded percentage.
            /// </summary>
            public decimal? DailyVolumeTradedPercentage { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether daily volume breach.
            /// </summary>
            public bool DailyVolumeBreach { get; set; }
        }

        /// <summary>
        /// The high profit judgement.
        /// </summary>
        private class HighProfitJudgementDto : JudgementDto
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="HighProfitJudgementDto"/> class.
            /// </summary>
            public HighProfitJudgementDto()
            {
                // leave for dapper
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="HighProfitJudgementDto"/> class.
            /// </summary>
            /// <param name="judgement">
            /// The judgement.
            /// </param>
            public HighProfitJudgementDto(IHighProfitJudgement judgement)
                : base(
                    !judgement?.NoAnalysis ?? false,
                    judgement?.ClientOrderId,
                    judgement?.OrderId,
                    judgement?.Parameters,
                    judgement?.RuleRunCorrelationId, 
                    judgement?.RuleRunId)
            {
                if (judgement == null)
                {
                    return;
                }

                this.AbsoluteHighProfit = judgement.AbsoluteHighProfit;
                this.AbsoluteHighProfitCurrency = judgement.AbsoluteHighProfitCurrency;
                this.PercentageHighProfit = judgement.PercentageHighProfit;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="HighProfitJudgementDto"/> class.
            /// </summary>
            /// <param name="judgement">
            /// The judgement.
            /// </param>
            public HighProfitJudgementDto(IFixedIncomeHighProfitJudgement judgement)
                : base(
                    !judgement?.NoAnalysis ?? false,
                    judgement?.ClientOrderId,
                    judgement?.OrderId,
                    judgement?.SerialisedParameters,
                    judgement?.RuleRunCorrelationId,
                    judgement?.RuleRunId)
            {
                if (judgement == null)
                {
                    return;
                }

                this.AbsoluteHighProfit = judgement.AbsoluteHighProfit;
                this.AbsoluteHighProfitCurrency = judgement.AbsoluteHighProfitCurrency;
                this.PercentageHighProfit = judgement.PercentageHighProfit;
            }

            /// <summary>
            /// Gets the absolute high profit.
            /// </summary>
            public decimal? AbsoluteHighProfit { get; }

            /// <summary>
            /// Gets the absolute high profit currency.
            /// </summary>
            public string AbsoluteHighProfitCurrency { get; }

            /// <summary>
            /// Gets the percentage high profit.
            /// </summary>
            public decimal? PercentageHighProfit { get; }
        }
    }
}