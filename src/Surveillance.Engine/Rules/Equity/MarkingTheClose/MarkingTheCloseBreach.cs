namespace Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose
{
    using System;
    using System.Collections.Generic;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Trading;
    using Domain.Core.Trading.Interfaces;
    using Domain.Core.Trading.Orders;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Trades.Interfaces;
    using Surveillance.Engine.Rules.Universe.MarketEvents;

    public class MarkingTheCloseBreach : IMarkingTheCloseBreach
    {
        public MarkingTheCloseBreach(
            IFactorValue factorValue,
            ISystemProcessOperationContext operationContext,
            string correlationId,
            TimeSpan window,
            FinancialInstrument security,
            MarketOpenClose marketClose,
            ITradePosition tradingPosition,
            IMarkingTheCloseEquitiesParameters equitiesParameters,
            VolumeBreach dailyBreach,
            VolumeBreach windowBreach,
            string description,
            string caseTitle,
            DateTime universeDateTime)
        {
            this.FactorValue = factorValue;

            this.Window = window;
            this.Security = security ?? throw new ArgumentNullException(nameof(security));

            this.MarketClose = marketClose ?? throw new ArgumentNullException(nameof(marketClose));
            this.Trades = tradingPosition ?? new TradePosition(new List<Order>());
            this.EquitiesParameters = equitiesParameters ?? throw new ArgumentNullException(nameof(equitiesParameters));

            this.DailyBreach = dailyBreach;
            this.WindowBreach = windowBreach;

            this.RuleParameterId = equitiesParameters?.Id ?? string.Empty;
            this.SystemOperationId = operationContext.Id.ToString();
            this.CorrelationId = correlationId;
            this.RuleParameters = equitiesParameters;
            this.Description = description ?? string.Empty;
            this.CaseTitle = caseTitle ?? string.Empty;
            this.UniverseDateTime = universeDateTime;
        }

        public string CaseTitle { get; set; }

        public string CorrelationId { get; set; }

        public VolumeBreach DailyBreach { get; }

        public string Description { get; set; }

        public IMarkingTheCloseEquitiesParameters EquitiesParameters { get; }

        public IFactorValue FactorValue { get; set; }

        public bool IsBackTestRun { get; set; }

        public MarketOpenClose MarketClose { get; }

        public string RuleParameterId { get; set; }

        public IRuleParameter RuleParameters { get; set; }

        public FinancialInstrument Security { get; }

        public string SystemOperationId { get; set; }

        public ITradePosition Trades { get; }

        public DateTime UniverseDateTime { get; set; }

        public TimeSpan Window { get; }

        public VolumeBreach WindowBreach { get; }
    }
}