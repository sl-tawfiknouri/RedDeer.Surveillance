using System;
using Domain.Core.Trading.Orders;
using FluentValidation;
using SharedKernel.Files.Orders.Interfaces;
using SharedKernel.Files.PropertyValidators;

namespace SharedKernel.Files.Orders
{
    public class EtlFileValidator : AbstractValidator<OrderFileContract>, IEtlFileValidator
    {
        public EtlFileValidator()
        {
            // Market
            RuleFor(x => x.MarketIdentifierCode)
                .NotEmpty()
                .WithMessage("Market Identifier Code must not be empty");

            // Instrument
            RulesForSufficientInstrumentIdentificationCodes();
            RulesForIdentificationCodes();
            RulesForDerivativeIdentificationCodes();

            RuleFor(x => x.InstrumentCfi).NotEmpty().MinimumLength(1).MaximumLength(6).WithMessage("Instrument CFI must be between 1 and 6 characters");
            RuleFor(x => x.InstrumentCfi).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Instrument CFI must not be empty");

            // Order
            RulesForOrderProperties();

            // Trade
            RulesForDealerOrderProperties();
        }
        
        private void RulesForSufficientInstrumentIdentificationCodes()
        {
            RuleFor(x => x)
                .Must(x =>
                    !string.IsNullOrWhiteSpace(x.InstrumentIsin)
                    || !string.IsNullOrWhiteSpace(x.InstrumentSedol)
                    || !string.IsNullOrWhiteSpace(x.InstrumentFigi))
                .WithMessage("Instrument must have either a sedol, isin or figi");
        }

        private void RulesForIdentificationCodes()
        {
            RuleFor(x => x.InstrumentSedol)
                .MinimumLength(6)
                .MaximumLength(7)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentSedol))
                .WithMessage("Instrument Sedol must have a length of 7 characters when it is provided");

            RuleFor(x => x.InstrumentIsin)
                .Length(12)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentIsin))
                .WithMessage("Instrument ISIN must have a length of 12 characters when it is provided");

            RuleFor(x => x.InstrumentCusip)
                .MinimumLength(6)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentCusip))
                .WithMessage("Instrument CUSIP must have a minimum length of 6 when it is provided");

            RuleFor(x => x.InstrumentCusip)
                .MaximumLength(9)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentCusip))
                .WithMessage("Instrument CUSIP must have a maximum length of 9 characters when it is provided");

            RuleFor(x => x.InstrumentLei)
                .Length(20)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentLei))
                .WithMessage("Instrument LEI must have a length of 20 characters if it is provided");

            RuleFor(x => x.InstrumentFigi)
                .Length(12)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentFigi))
                .WithMessage("Instrument FIGI must have a length of 12 characters if it is provided");
        }

        private void RulesForDerivativeIdentificationCodes()
        {
            RuleFor(x => x.InstrumentUnderlyingSedol)
                .Length(7)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentUnderlyingSedol))
                .WithMessage("Instrument Underlying Sedol must have a length of 7 characters when it is provided");

            RuleFor(x => x.InstrumentUnderlyingIsin)
                .Length(12)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentUnderlyingIsin))
                .WithMessage("Instrument Underlying ISIN must have a length of 12 characters when it is provided");

            RuleFor(x => x.InstrumentUnderlyingCusip)
                .MinimumLength(6)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentUnderlyingCusip))
                .WithMessage("Instrument Underlying CUSIP must have a minimum length of 6 characters when it is provided");

            RuleFor(x => x.InstrumentUnderlyingCusip)
                .MaximumLength(9)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentUnderlyingCusip))
                .WithMessage("Instrument Underlying CUSIP must have a maximum length of 9 characters when it is provided");

            RuleFor(x => x.InstrumentUnderlyingLei)
                .Length(20)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentUnderlyingLei))
                .WithMessage("Instrument Underlying LEI must have a length of 20 characters when it is provided");

            RuleFor(x => x.InstrumentUnderlyingFigi)
                .Length(12)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentUnderlyingFigi))
                .WithMessage("Instrument Underlying FIGI must have a length of 12 characters when it is provided");
        }

        private void RulesForOrderProperties()
        {
            RuleFor(x => x.OrderId).NotEmpty().MinimumLength(1).MaximumLength(255).WithMessage("OrderId must have a length between 1 and 255 characters");
            RuleFor(x => x.OrderTraderId).MaximumLength(255).WithMessage("OrderTraderId must have a maximum length of 255 characters");
            RuleFor(x => x.OrderVersion).MaximumLength(255).WithMessage("OrderVersion must have a maximum length of 255 characters");
            RuleFor(x => x.OrderVersionLinkId).MaximumLength(255).WithMessage("OrderVersionLinkId must have a maximum length of 255 characters");
            RuleFor(x => x.OrderGroupId).MaximumLength(255).WithMessage("OrderGroupId must have a maximum length of 255 characters");

            RuleFor(x => x.OrderCurrency).NotEmpty().Length(3).WithMessage("OrderCurrency must have a length of 3 characters"); ;
            RuleFor(x => x.OrderSettlementCurrency).Length(3)
                .When(x => !string.IsNullOrWhiteSpace(x.OrderSettlementCurrency))
                .WithMessage("OrderSettlementCurrency must have a length of 3 characters when it is provided"); ;

            RuleFor(x => x.OrderType).NotEmpty().SetValidator(new EnumParseableValidator<OrderTypes>("OrderType")).WithMessage("Order type was not in a valid range. Order types are a closed set");
            RuleFor(x => x.OrderDirection).NotEmpty().SetValidator(new EnumParseableValidator<OrderDirections>("OrderPosition")).WithMessage("Order position was not in a valid range. Order position are a closed set"); ;

            RuleFor(x => x.OrderLimitPrice)
                .NotEmpty()
                .When(x => string.Equals(x.OrderType, "LIMIT", StringComparison.OrdinalIgnoreCase))
                .WithMessage("OrderLimitPrice must have a value when the order is of type LIMIT");

            RuleFor(x => x.OrderCleanDirty).SetValidator(new EnumParseableValidator<OrderCleanDirty>("OrderCleanDirty"))
                .When(x => !string.IsNullOrWhiteSpace(x.OrderCleanDirty)).WithMessage("Order clean dirty was not in a valid range. Order clean dirty are a closed set"); ;

            RuleFor(x => x.OrderTraderName).MaximumLength(255).WithMessage("Order Trader Name should not exceed 255 characters in length");
            RuleFor(x => x.OrderClearingAgent).MaximumLength(255).WithMessage("OrderClearingAgent must have a maximum length of 255 characters"); ;
            RuleFor(x => x.OrderDealingInstructions).MaximumLength(4095).WithMessage("OrderDealingInstructions must have a maximum length of 4095 characters"); ;

            RuleFor(x => x.OrderLimitPrice).SetValidator(new DecimalParseableValidator("OrderLimitPrice")).WithMessage("Order limit price was not a valid value");
            RuleFor(x => x.OrderAverageFillPrice).SetValidator(new DecimalParseableValidator("OrderAveragePrice")).WithMessage("Order average price was not a valid value");
            RuleFor(x => x.OrderFilledVolume).SetValidator(new LongParseableValidator("OrderFilledVolume")).WithMessage("Order filled volume was not a valid long value (64 bit integer)");
            RuleFor(x => x.OrderFilledVolume)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .When(y => !string.IsNullOrWhiteSpace(y.OrderFilledDate))
                .WithMessage("Must have an order filled volume when there is an order filled date");
            RuleFor(x => x.OrderOrderedVolume)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .SetValidator(new LongParseableValidator("OrderOrderedVolume")).WithMessage("Order filled volume was not a valid long value (64 bit integer)");

            RuleFor(x => x.OrderAccumulatedInterest)
                .SetValidator(new DecimalParseableValidator("OrderAccumulatedInterest"))
                .When(x => !string.IsNullOrWhiteSpace(x.OrderAccumulatedInterest))
                .WithMessage("Order accumulated interest was not recognised as a parseable decimal value");

            RuleFor(x => x.OrderPlacedDate).NotEmpty().WithMessage($"OrderPlacedDate must not be empty");
            RuleFor(x => x.OrderPlacedDate).SetValidator(new DateParseableValidator("OrderPlacedDate")).WithMessage($"OrderPlacedDate was not recognised as a valid date");
            RuleFor(x => x.OrderBookedDate).SetValidator(new DateParseableValidator("OrderBookedDate")).WithMessage($"OrderBookedDate was not recognised as a valid date");
            RuleFor(x => x.OrderAmendedDate).SetValidator(new DateParseableValidator("OrderAmendedDate")).WithMessage($"OrderAmendedDate was not recognised as a valid date");
            RuleFor(x => x.OrderRejectedDate).SetValidator(new DateParseableValidator("OrderRejectedDate")).WithMessage($"OrderRejectedDate was not recognised as a valid date");
            RuleFor(x => x.OrderCancelledDate).SetValidator(new DateParseableValidator("OrderCancelledDate")).WithMessage($"OrderCancelledDate  was not recognised as a valid date");
            RuleFor(x => x.OrderFilledDate).SetValidator(new DateParseableValidator("OrderFilledDate")).WithMessage($"OrderFilledDate was not recognised as a valid date");
            RuleFor(x => x.OrderOrderedVolume).SetValidator(new NonZeroLongParseableValidator("OrderOrderedVolume")).WithMessage($"OrderOrderedVolume was not recognised as a non zero long (64 bit integer)");

            RuleFor(x => x.OrderDirection)
                .Must(x => !string.Equals(x, "none", StringComparison.InvariantCultureIgnoreCase))
                .WithMessage("Order position had an illegal value of 'none'");

            RuleFor(x => x.OrderDirection)
                .Must(x => !string.Equals(x, "0", StringComparison.InvariantCultureIgnoreCase))
                .WithMessage("Order position had an illegal value of '0'");
        }

        private void RulesForDealerOrderProperties()
        {
            RuleFor(x => x.DealerOrderId).NotEmpty().When(HasDealerOrderData).MaximumLength(255).When(HasDealerOrderData)
                .WithMessage("DealerOrderId must have a maximum length of 255 characters");
            RuleFor(x => x.DealerOrderVersion).MaximumLength(255).When(HasDealerOrderData).WithMessage("DealerOrderVersion must have a maximum length of 255 characters");
            RuleFor(x => x.DealerOrderVersionLinkId).MaximumLength(255).When(HasDealerOrderData).WithMessage("DealerOrderVersionLinkId must have a maximum length of 255 characters");
            RuleFor(x => x.DealerOrderGroupId).MaximumLength(255).When(HasDealerOrderData).WithMessage("DealerOrderGroupId must have a maximum length of 255 characters"); ;
            RuleFor(x => x.DealerOrderDealerId).NotEmpty().When(HasDealerOrderData).MaximumLength(255).When(HasDealerOrderData).WithMessage("DealerOrderDealerId must have a maximum length of 255 characters"); ;
            RuleFor(x => x.DealerOrderNotes).MaximumLength(4095).When(HasDealerOrderData).WithMessage("DealerOrderNotes must have a maximum length of 4095  characters"); ;
            RuleFor(x => x.DealerOrderCounterParty).MaximumLength(255).When(HasDealerOrderData).WithMessage("DealerOrderCounterParty must have a maximum length of 255 characters");
            RuleFor(x => x.DealerOrderCurrency).Length(3).When(HasDealerOrderData).WithMessage("DealerOrderCurrency must have a length of 3 characters");
            RuleFor(x => x.DealerOrderSettlementCurrency).Length(3).When(x => !string.IsNullOrWhiteSpace(x.DealerOrderSettlementCurrency)).WithMessage("DealerOrderSettlementCurrency must have a length of 3 characters when it is provided"); ;
            RuleFor(x => x.DealerOrderDealerName).MaximumLength(255).When(HasDealerOrderData).WithMessage("DealerOrderDealerName must have a maximum length of 255 characters");

            RuleFor(x => x.DealerOrderCleanDirty)
                .SetValidator(new EnumParseableValidator<OrderCleanDirty>("DealerOrderCleanDirty"))
                .When(x => !string.IsNullOrWhiteSpace(x.DealerOrderCleanDirty))
                .WithMessage("DealerOrderCleanDirty was not recognised as a valid value. It is a closed set");

            RuleFor(x => x.DealerOrderType).NotEmpty().SetValidator(new EnumParseableValidator<OrderTypes>("TradeType")).When(HasDealerOrderData).WithMessage("DealerOrderType was not recognised as a valid value. It is a closed set");
            RuleFor(x => x.DealerOrderDirection).NotEmpty().SetValidator(new EnumParseableValidator<OrderDirections>("TradePosition")).When(HasDealerOrderData).WithMessage("DealerOrderDirection was not recognised as a valid value. It is a closed set");

            RuleFor(x => x.DealerOrderLimitPrice)
                .NotEmpty()
                .When(x => string.Equals(x.DealerOrderType, "LIMIT", StringComparison.InvariantCultureIgnoreCase))
                .WithMessage("DealerOrderLimitPrice must be provided when there is a dealer order type of LIMIT");

            RuleFor(x => x.DealerOrderLimitPrice).SetValidator(new DecimalParseableValidator("TradeLimitPrice")).When(HasDealerOrderData).WithMessage("DealerOrderLimitPrice must be provided when there is a dealer order type of LIMIT");
            RuleFor(x => x.DealerOrderAverageFillPrice).SetValidator(new DecimalParseableValidator("TradeAveragePrice")).When(HasDealerOrderData).WithMessage("DealerOrderAveragePrice was not recognised as a valid decimal");
            RuleFor(x => x.DealerOrderOrderedVolume).SetValidator(new NonZeroLongParseableValidator("TradeOrderedVolume")).When(HasDealerOrderData).WithMessage("DealerOrderOrderedVolume was not recognised as a valid non zero long (64 bit integer)");
            RuleFor(x => x.DealerOrderFilledVolume).SetValidator(new LongParseableValidator("TradeFilledVolume")).When(HasDealerOrderData).WithMessage("DealerOrderFilledVolume was not recognised as a valid nullable long (64 bit integer)");
            RuleFor(x => x.DealerOrderAccumulatedInterest)
                .SetValidator(new DecimalParseableValidator("AccumulatedInterest"))
                .When(x => !string.IsNullOrWhiteSpace(x.DealerOrderAccumulatedInterest))
                .WithMessage("DealerOrderAccumulatedInterest was not recognised as a valid decimal");

            RuleFor(x => x.DealerOrderDealerName).MaximumLength(255).When(HasDealerOrderData).WithMessage("DealerOrderDealerName must have a maximum length of 255 characters");

            RuleFor(x => x.DealerOrderDirection)
                .Must(x => !string.Equals(x, "none", StringComparison.InvariantCultureIgnoreCase))
                .WithMessage("Trade position had an illegal value of 'none'");

            RuleFor(x => x.DealerOrderDirection)
                .Must(x => !string.Equals(x, "0", StringComparison.InvariantCultureIgnoreCase))
                .WithMessage("TradePosition had an illegal value of '0'");

            RuleFor(x => x.DealerOrderPlacedDate).NotEmpty().When(HasDealerOrderData).WithMessage($"DealerOrderPlacedDate must not be empty");
            RuleFor(x => x.DealerOrderPlacedDate).SetValidator(new DateParseableValidator("DealerOrderPlacedDate")).When(HasDealerOrderData).WithMessage("DealerOrderPlacedDate was not recognised as a valid date");
            RuleFor(x => x.DealerOrderBookedDate).SetValidator(new DateParseableValidator("DealerOrderBookedDate")).When(HasDealerOrderData).WithMessage("DealerOrderBookedDate was not recognised as a valid date");
            RuleFor(x => x.DealerOrderAmendedDate).SetValidator(new DateParseableValidator("DealerOrderAmendedDate")).When(HasDealerOrderData).WithMessage("DealerOrderAmendedDate was not recognised as a valid date");
            RuleFor(x => x.DealerOrderCancelledDate).SetValidator(new DateParseableValidator("DealerOrderCancelledDate")).When(HasDealerOrderData).WithMessage("DealerOrderCancelledDate was not recognised as a valid date");
            RuleFor(x => x.DealerOrderRejectedDate).SetValidator(new DateParseableValidator("DealerOrderRejectedDate")).When(HasDealerOrderData).WithMessage("DealerOrderRejectedDate was not recognised as a valid date");
            RuleFor(x => x.DealerOrderFilledDate).SetValidator(new DateParseableValidator("DealerOrderFilledDate")).When(HasDealerOrderData).WithMessage("DealerOrderFilledDate was not recognised as a valid date");
        }

        private bool HasDealerOrderData(OrderFileContract contract)
        {
            return !string.IsNullOrWhiteSpace(contract.DealerOrderDealerId)
                   || !string.IsNullOrWhiteSpace(contract.DealerOrderDealerName)
                   || !string.IsNullOrWhiteSpace(contract.DealerOrderPlacedDate)
                   || !string.IsNullOrWhiteSpace(contract.DealerOrderBookedDate)
                   || !string.IsNullOrWhiteSpace(contract.DealerOrderAmendedDate)
                   || !string.IsNullOrWhiteSpace(contract.DealerOrderRejectedDate)
                   || !string.IsNullOrWhiteSpace(contract.DealerOrderCancelledDate)
                   || !string.IsNullOrWhiteSpace(contract.DealerOrderFilledDate)
                   || !string.IsNullOrWhiteSpace(contract.DealerOrderCounterParty)
                   || !string.IsNullOrWhiteSpace(contract.DealerOrderType)
                   || !string.IsNullOrWhiteSpace(contract.DealerOrderDirection)
                   || !string.IsNullOrWhiteSpace(contract.DealerOrderCurrency)
                   || !string.IsNullOrWhiteSpace(contract.DealerOrderLimitPrice)
                   || !string.IsNullOrWhiteSpace(contract.DealerOrderAverageFillPrice)
                   || !string.IsNullOrWhiteSpace(contract.DealerOrderOrderedVolume)
                   || !string.IsNullOrWhiteSpace(contract.DealerOrderFilledVolume);
        }
    }
}
