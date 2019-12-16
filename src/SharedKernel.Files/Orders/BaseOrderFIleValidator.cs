using Domain.Core.Markets;
using Domain.Core.Trading.Orders;
using FluentValidation;
using SharedKernel.Files.ExtendedValidators;
using System;

namespace SharedKernel.Files.Orders
{
    public class BaseOrderFIleValidator
        : AbstractValidator<OrderFileContract>
    {
        private const string whenFixedIncomeMessage = "When fixed income.";

        public BaseOrderFIleValidator()
            : base()
        {
            // Market
            this.RulesForMarket();
            
            // Instrument
            this.RulesForSufficientInstrumentIdentificationCodes();
            this.RulesForIdentificationCodes();
            this.RulesForDerivativeIdentificationCodes();

            // Order
            this.RulesForOrderProperties();

            // Trade
            this.RulesForDealerOrderProperties();
        }

        protected virtual void RulesForMarket()
        {
            this.RuleFor(x => x.MarketIdentifierCode)
                .IsNotEmpty()
                .When(x => !IsFixedIncome(x));

            this.RuleFor(x => x.MarketIdentifierCode)
                .Equal("RDFI", StringComparer.OrdinalIgnoreCase)
                .When(IsFixedIncome)
                .WithAdditionalMessage(whenFixedIncomeMessage);

            this.RuleFor(x => x.MarketName)
                .Equal("RDFI", StringComparer.OrdinalIgnoreCase)
                .When(IsFixedIncome)
                .WithAdditionalMessage(whenFixedIncomeMessage);

            this.RuleFor(x => x.MarketType)
                .SetValidator(new EnumParseableValidator<MarketTypes>())
                .WhenIsNotNullOrWhitespace();
        }

        protected virtual void RulesForOrderProperties()
        {
            this.RuleFor(x => x.OrderLimitPrice)
                .IsDecimalWhenIsNotNullOrWhitespace();

            this.RuleFor(x => x.OrderOrderedVolume)
                .IsNotEmpty()
                .StringDecimalGreaterThan(0);

            this.RuleFor(x => x.OrderId)
                .IsNotEmpty()
                .MaximumLength(255);

            this.RuleFor(x => x.OrderTraderId)
                .MaximumLength(255);

            this.RuleFor(x => x.OrderVersion)
                .MaximumLength(255);

            this.RuleFor(x => x.OrderVersionLinkId)
                .MaximumLength(255);

            this.RuleFor(x => x.OrderGroupId)
                .MaximumLength(255);

            this.RuleFor(x => x.OrderBroker)
                .MaximumLength(1023);

            this.RuleFor(x => x.OrderCurrency)
                .IsNotEmpty()
                .Length(3);

            this.RuleFor(x => x.OrderSettlementCurrency)
                .Length(3)
                .When(x => !string.IsNullOrWhiteSpace(x.OrderSettlementCurrency));

            this.RuleFor(x => x.OrderType)
                .SetValidator(new EnumParseableValidator<OrderTypes>())
                .WhenIsNotNullOrWhitespace();

            this.RuleFor(x => x.OrderDirection)
                .IsNotEmpty()
                .SetValidator(new EnumParseableValidator<OrderDirections>());

            this.RuleFor(x => x.OrderLimitPrice)
                 .IsDecimalWhenIsNotNullOrWhitespace()
                 .IsNotEmpty()
                 .When(x => string.Equals(x.OrderType, OrderTypes.LIMIT.ToString(), StringComparison.OrdinalIgnoreCase))
                 .WithAdditionalMessage($"When '{nameof(OrderFileContract.OrderType)}' value is '{OrderTypes.LIMIT.ToString()}'");

            this.RuleFor(x => x.OrderCleanDirty)
                .SetValidator(new EnumParseableValidator<OrderCleanDirty>())
                .When(x => !IsFixedIncome(x) && !string.IsNullOrWhiteSpace(x.OrderCleanDirty), ApplyConditionTo.CurrentValidator);

            this.RuleFor(x => x.OrderCleanDirty)
                .Equal(OrderCleanDirty.CLEAN.ToString(), StringComparer.OrdinalIgnoreCase)
                .When(IsFixedIncome, ApplyConditionTo.CurrentValidator)
                .WithAdditionalMessage(whenFixedIncomeMessage);

            this.RuleFor(x => x.OrderTraderName)
                .MaximumLength(255);

            this.RuleFor(x => x.OrderClearingAgent)
                .MaximumLength(255);

            this.RuleFor(x => x.OrderDealingInstructions)
                .MaximumLength(4095);

            this.RuleFor(x => x.OrderAverageFillPrice)
                .IsDecimalWhenIsNotNullOrWhitespace()
                .When(o => !string.IsNullOrWhiteSpace(o.OrderFilledDate))
                .WithAdditionalMessageWhenPropertyIsValueNotNullOrEmptySpace(o => o.OrderFilledDate)
                .StringDecimalGreaterThan(0)
                .When(o => !string.IsNullOrWhiteSpace(o.OrderFilledDate))
                .WithAdditionalMessageWhenPropertyIsValueNotNullOrEmptySpace(o => o.OrderFilledDate);

            this.RuleFor(x => x.OrderFilledVolume)
                .IsDecimal()
                .When(y => !string.IsNullOrWhiteSpace(y.OrderFilledDate))
                .WithAdditionalMessageWhenPropertyIsValueNotNullOrEmptySpace(o => o.OrderFilledDate)
                .StringDecimalGreaterThan(0)
                .When(o => !string.IsNullOrWhiteSpace(o.OrderFilledDate))
                .WithAdditionalMessageWhenPropertyIsValueNotNullOrEmptySpace(o => o.OrderFilledDate);

            this.RuleFor(x => x.OrderAccumulatedInterest)
                .IsDecimalWhenIsNotNullOrWhitespace();

            this.RuleFor(x => x.OrderPlacedDate)
                .IsNotEmpty()
                .IsParseableDate();

            this.RuleFor(x => x.OrderBookedDate)
                .IsParseableDate();

            this.RuleFor(x => x.OrderAmendedDate)
                .IsParseableDate();

            this.RuleFor(x => x.OrderRejectedDate)
                .IsParseableDate();

            this.RuleFor(x => x.OrderCancelledDate)
                .IsParseableDate();

            this.RuleFor(x => x.OrderFilledDate)
                .IsParseableDate();

            this.RuleFor(x => x.OrderDirection)
                .NotEqual(OrderDirections.NONE.ToString(), StringComparer.InvariantCultureIgnoreCase);

            this.RuleFor(x => x.OrderDirection)
                .NotEqual(((int)OrderDirections.NONE).ToString());

            this.RuleFor(x => x.InstrumentExchangeSymbol)
                .MaximumLength(100);

            this.RuleFor(x => x.InstrumentUnderlyingExchangeSymbol)
                .MaximumLength(100);
        }

        protected bool IsFixedIncome(OrderFileContract orderFileContract)
            => orderFileContract.InstrumentCfi?.StartsWith("D", StringComparison.OrdinalIgnoreCase) ?? false;

        protected virtual void RulesForDealerOrderProperties()
        {
            this.RuleFor(x => x.DealerOrderId).IsEmpty();
            this.RuleFor(x => x.DealerOrderCurrency).IsEmpty();
            this.RuleFor(x => x.DealerOrderPlacedDate).IsEmpty();
            this.RuleFor(x => x.DealerOrderBookedDate).IsEmpty();
            this.RuleFor(x => x.DealerOrderVersion).IsEmpty();
            this.RuleFor(x => x.DealerOrderVersionLinkId).IsEmpty();
            this.RuleFor(x => x.DealerOrderGroupId).IsEmpty();
            this.RuleFor(x => x.DealerOrderDealerId).IsEmpty();
            this.RuleFor(x => x.DealerOrderNotes).IsEmpty();
            this.RuleFor(x => x.DealerOrderCounterParty).IsEmpty();
            this.RuleFor(x => x.DealerOrderSettlementCurrency).IsEmpty();
            this.RuleFor(x => x.DealerOrderDealerName).IsEmpty();
            this.RuleFor(x => x.DealerOrderCleanDirty).IsEmpty();
            this.RuleFor(x => x.DealerOrderType).IsEmpty();
            this.RuleFor(x => x.DealerOrderDirection).IsEmpty();
            this.RuleFor(x => x.DealerOrderLimitPrice).IsEmpty();
            this.RuleFor(x => x.DealerOrderAverageFillPrice).IsEmpty();
            this.RuleFor(x => x.DealerOrderOrderedVolume).IsEmpty();
            this.RuleFor(x => x.DealerOrderFilledVolume).IsEmpty();
            this.RuleFor(x => x.DealerOrderAccumulatedInterest).IsEmpty();
            this.RuleFor(x => x.DealerOrderAmendedDate).IsEmpty();
            this.RuleFor(x => x.DealerOrderCancelledDate).IsEmpty();
            this.RuleFor(x => x.DealerOrderRejectedDate).IsEmpty();
            this.RuleFor(x => x.DealerOrderFilledDate).IsEmpty();
            this.RuleFor(x => x.DealerOrderOptionExpirationDate).IsEmpty();
            this.RuleFor(x => x.DealerOrderOptionEuropeanAmerican).IsEmpty();
            this.RuleFor(x => x.DealerOrderOptionStrikePrice).IsEmpty();
        }

        protected virtual void RulesForIdentificationCodes()
        {
            this.RuleFor(x => x.InstrumentCfi)
                .IsNotEmpty()
                .MaximumLength(6);

            this.RuleFor(x => x.InstrumentSedol)
                .Length(1, 7)
                .WhenIsNotNullOrWhitespace();

            this.RuleFor(x => x.InstrumentIsin)
                .Length(12)
                .WhenIsNotNullOrWhitespace();

            this.RuleFor(x => x.InstrumentCusip)
                .Length(6, 9)
                .WhenIsNotNullOrWhitespace();

            this.RuleFor(x => x.InstrumentLei)
                .Length(20)
                .WhenIsNotNullOrWhitespace();

            this.RuleFor(x => x.InstrumentFigi)
                .Length(12)
                .WhenIsNotNullOrWhitespace();

            this.RuleFor(x => x.InstrumentRic)
                .Length(0, 30)
                .WhenIsNotNullOrWhitespace();
        }

        protected void RulesForDerivativeIdentificationCodes()
        {
            this.RuleFor(x => x.InstrumentUnderlyingSedol)
                .Length(1, 7)
                .WhenIsNotNullOrWhitespace();

            this.RuleFor(x => x.InstrumentUnderlyingIsin)
                .Length(12)
                .WhenIsNotNullOrWhitespace();

            this.RuleFor(x => x.InstrumentUnderlyingCusip)
                .Length(6, 9)
                .WhenIsNotNullOrWhitespace();

            this.RuleFor(x => x.InstrumentUnderlyingLei)
                .Length(20)
                .WhenIsNotNullOrWhitespace();

            this.RuleFor(x => x.InstrumentUnderlyingFigi)
                .Length(12)
                .WhenIsNotNullOrWhitespace();

            this.RuleFor(x => x.InstrumentUnderlyingRic)
                .Length(1, 30)
                .WhenIsNotNullOrWhitespace();
        }

        protected void RulesForSufficientInstrumentIdentificationCodes()
        {
            this.RuleFor(x => x)
                .Must(x => ((string.IsNullOrWhiteSpace(x.InstrumentSedol) && (string.IsNullOrWhiteSpace(x.InstrumentIsin) || string.IsNullOrWhiteSpace(x.MarketIdentifierCode)))) == false)
                .When(o => !IsFixedIncome(o), ApplyConditionTo.CurrentValidator)
                .WithMessage($"At least '{nameof(OrderFileContract.InstrumentSedol)}' or '{nameof(OrderFileContract.InstrumentIsin)}' with '{nameof(OrderFileContract.MarketIdentifierCode)}' must be definded for instrument when not fixed income.");

            this.RuleFor(x => x)
                .Must(x => ((string.IsNullOrWhiteSpace(x.InstrumentRic) && (string.IsNullOrWhiteSpace(x.InstrumentIsin) || string.IsNullOrWhiteSpace(x.MarketIdentifierCode)))) == false)
                .When(o => IsFixedIncome(o), ApplyConditionTo.CurrentValidator)
                .WithMessage($"At least '{nameof(OrderFileContract.InstrumentRic)}' or '{nameof(OrderFileContract.InstrumentIsin)}' with '{nameof(OrderFileContract.MarketIdentifierCode)}' must be definded for instrument when fixed income.");
        }
    }
}
