using System;
using System.Globalization;
using Domain.Core.Markets;
using Domain.Core.Trading.Orders;
using FluentValidation;
using FluentValidation.Validators;
using SharedKernel.Files.Orders.Interfaces;

namespace SharedKernel.Files.Orders
{
    public class OrderFileValidator : AbstractValidator<OrderFileContract>, IOrderFileValidator
    {
        public OrderFileValidator()
        {
            // Market
            RuleFor(x => x.MarketIdentifierCode)
                .NotEmpty()
                .WithMessage("Market Identifier Code must not be empty");

            RuleFor(x => x.MarketType)
                .NotEmpty()
                .SetValidator(new EnumParseableValidator<MarketTypes>("MarketType"))
                .WithMessage("Market Type must be valid");

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
            RuleFor(x => x).Must(x =>
                !string.IsNullOrWhiteSpace(x.InstrumentIsin)
                || !string.IsNullOrWhiteSpace(x.InstrumentSedol)
                || !string.IsNullOrWhiteSpace(x.InstrumentCusip)
                || !string.IsNullOrWhiteSpace(x.InstrumentBloombergTicker));
        }

        private void RulesForIdentificationCodes()
        {
            RuleFor(x => x.InstrumentSedol)
                .Length(7)
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

            RuleFor(x => x.OrderType).NotEmpty().SetValidator(new EnumParseableValidator<OrderTypes>("OrderType"));
            RuleFor(x => x.OrderDirection).NotEmpty().SetValidator(new EnumParseableValidator<OrderDirections>("OrderPosition"));

            RuleFor(x => x.OrderLimitPrice)
                .NotEmpty()
                .When(x => string.Equals(x.OrderType, "LIMIT", StringComparison.InvariantCultureIgnoreCase))
                .WithMessage("OrderLimitPrice must have a value when the order is of type LIMIT");

            RuleFor(x => x.OrderCleanDirty).SetValidator(new EnumParseableValidator<OrderCleanDirty>("OrderCleanDirty"))
                .When(x => !string.IsNullOrWhiteSpace(x.OrderCleanDirty));

            RuleFor(x => x.OrderTraderName).MaximumLength(255).WithMessage("Order Trader Name should not exceed 255 characters in length");
            RuleFor(x => x.OrderClearingAgent).MaximumLength(255).WithMessage("OrderClearingAgent must have a maximum length of 255 characters"); ;
            RuleFor(x => x.OrderDealingInstructions).MaximumLength(4095).WithMessage("OrderDealingInstructions must have a maximum length of 4095 characters"); ;

            RuleFor(x => x.OrderLimitPrice).SetValidator(new DecimalParseableValidator("OrderLimitPrice"));
            RuleFor(x => x.OrderAverageFillPrice).SetValidator(new DecimalParseableValidator("OrderAveragePrice"));
            RuleFor(x => x.OrderFilledVolume).SetValidator(new LongParseableValidator("OrderFilledVolume"));
            RuleFor(x => x.OrderFilledVolume)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .When(y => !string.IsNullOrWhiteSpace(y.OrderFilledDate))
                .WithMessage("Must have an order filled volume when there is an order filled date");
            RuleFor(x => x.OrderOrderedVolume)
                .Must(x => !string.IsNullOrWhiteSpace(x))
                .SetValidator(new LongParseableValidator("OrderOrderedVolume"));

            RuleFor(x => x.OrderAccumulatedInterest)
                .SetValidator(new DecimalParseableValidator("OrderAccumulatedInterest"))
                .When(x => !string.IsNullOrWhiteSpace(x.OrderAccumulatedInterest));

            RuleFor(x => x.OrderPlacedDate).NotEmpty();
            RuleFor(x => x.OrderPlacedDate).SetValidator(new DateParseableValidator("OrderPlacedDate"));
            RuleFor(x => x.OrderBookedDate).SetValidator(new DateParseableValidator("OrderBookedDate"));
            RuleFor(x => x.OrderAmendedDate).SetValidator(new DateParseableValidator("OrderAmendedDate"));
            RuleFor(x => x.OrderRejectedDate).SetValidator(new DateParseableValidator("OrderRejectedDate"));
            RuleFor(x => x.OrderCancelledDate).SetValidator(new DateParseableValidator("OrderCancelledDate"));
            RuleFor(x => x.OrderFilledDate).SetValidator(new DateParseableValidator("OrderFilledDate"));

            RuleFor(x => x).Custom((i, o) =>
            {
                if (string.IsNullOrWhiteSpace(i.OrderFilledDate))
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(i.OrderFilledVolume))
                {
                    o.AddFailure("OrderFilledVolume", "Order Filled Volume must have a value when there is an order filled date.");
                    return;
                }

                var parsedVol = long.TryParse(i.OrderFilledVolume, out var ofv);

                if (!parsedVol)
                {
                    o.AddFailure("OrderFilledVolume", "Order Filled Volume could not be parsed to a long.");
                    return;
                }

                if (ofv < 1)
                {
                    o.AddFailure("OrderFilledVolume", "Order Filled Volume was below 1 which is invalid when we have an order fill date.");
                }
            });

            RuleFor(x => x).Custom((i, o) =>
            {
                if (string.IsNullOrWhiteSpace(i.OrderFilledDate))
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(i.OrderAverageFillPrice))
                {
                    o.AddFailure("OrderAverageFillPrice", "Order Average Fill Price must have a value when there is an order filled date.");
                    return;
                }

                var parsedPrice = decimal.TryParse(i.OrderAverageFillPrice, out var ofv);
                if (!parsedPrice)
                {
                    o.AddFailure("OrderAverageFillPrice", "Order Average Fill Price could not be parsed to a decimal.");
                    return;
                }

                if (ofv == 0)
                {
                    o.AddFailure("OrderAverageFillPrice", "Order Average Fill Price was 0 which is invalid when we have an order fill date.");
                }
            });

            RuleFor(x => x.OrderOrderedVolume).SetValidator(new NonZeroLongParseableValidator("OrderOrderedVolume"));

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
            RuleFor(x => x.DealerOrderCurrency).NotEmpty().When(HasDealerOrderData).Length(3).When(HasDealerOrderData).WithMessage("DealerOrderCurrency must have a length of 3 characters");
            RuleFor(x => x.DealerOrderSettlementCurrency).Length(3).When(x => !string.IsNullOrWhiteSpace(x.DealerOrderSettlementCurrency)).WithMessage("DealerOrderSettlementCurrency must have a length of 3 characters when it is provided"); ;
            RuleFor(x => x.DealerOrderDealerName).MaximumLength(255).When(HasDealerOrderData).WithMessage("DealerOrderDealerName must have a maximum length of 255 characters");

            RuleFor(x => x.DealerOrderCleanDirty)
                .SetValidator(new EnumParseableValidator<OrderCleanDirty>("DealerOrderCleanDirty"))
                .When(x => !string.IsNullOrWhiteSpace(x.DealerOrderCleanDirty));

            RuleFor(x => x.DealerOrderType).NotEmpty().SetValidator(new EnumParseableValidator<OrderTypes>("TradeType")).When(HasDealerOrderData);
            RuleFor(x => x.DealerOrderDirection).NotEmpty().SetValidator(new EnumParseableValidator<OrderDirections>("TradePosition")).When(HasDealerOrderData);

            RuleFor(x => x.DealerOrderLimitPrice)
                .NotEmpty()
                .When(x => string.Equals(x.DealerOrderType, "LIMIT", StringComparison.InvariantCultureIgnoreCase))
                .WithMessage("DealerOrderLimitPrice must be provided when there is a dealer order type of LIMIT"); ;

            RuleFor(x => x.DealerOrderLimitPrice).SetValidator(new DecimalParseableValidator("TradeLimitPrice")).When(HasDealerOrderData);
            RuleFor(x => x.DealerOrderAverageFillPrice).SetValidator(new DecimalParseableValidator("TradeAveragePrice")).When(HasDealerOrderData);
            RuleFor(x => x.DealerOrderOrderedVolume).SetValidator(new NonZeroLongParseableValidator("TradeOrderedVolume")).When(HasDealerOrderData);
            RuleFor(x => x.DealerOrderFilledVolume).SetValidator(new LongParseableValidator("TradeFilledVolume")).When(HasDealerOrderData);
            RuleFor(x => x.DealerOrderAccumulatedInterest)
                .SetValidator(new DecimalParseableValidator("AccumulatedInterest"))
                .When(x => !string.IsNullOrWhiteSpace(x.DealerOrderAccumulatedInterest));

            RuleFor(x => x.DealerOrderDealerName).MaximumLength(255).When(HasDealerOrderData).WithMessage("DealerOrderDealerName must have a maximum length of 255 characters");
            RuleFor(x => x.DealerOrderDirection)
                .Must(x => !string.Equals(x, "none", StringComparison.InvariantCultureIgnoreCase))
                .WithMessage("Trade position had an illegal value of 'none'");

            RuleFor(x => x.DealerOrderDirection)
                .Must(x => !string.Equals(x, "0", StringComparison.InvariantCultureIgnoreCase))
                .WithMessage("TradePosition had an illegal value of '0'");

            RuleFor(x => x.DealerOrderPlacedDate).NotEmpty().When(HasDealerOrderData);
            RuleFor(x => x.DealerOrderPlacedDate).SetValidator(new DateParseableValidator("DealerOrderPlacedDate")).When(HasDealerOrderData);
            RuleFor(x => x.DealerOrderBookedDate).SetValidator(new DateParseableValidator("DealerOrderBookedDate")).When(HasDealerOrderData);
            RuleFor(x => x.DealerOrderAmendedDate).SetValidator(new DateParseableValidator("DealerOrderAmendedDate")).When(HasDealerOrderData);
            RuleFor(x => x.DealerOrderCancelledDate).SetValidator(new DateParseableValidator("DealerOrderCancelledDate")).When(HasDealerOrderData);
            RuleFor(x => x.DealerOrderRejectedDate).SetValidator(new DateParseableValidator("DealerOrderRejectedDate")).When(HasDealerOrderData);
            RuleFor(x => x.DealerOrderFilledDate).SetValidator(new DateParseableValidator("DealerOrderFilledDate")).When(HasDealerOrderData);

            RuleFor(x => x).Custom((i, o) =>
            {
                if (string.IsNullOrWhiteSpace(i.DealerOrderFilledDate))
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(i.DealerOrderFilledVolume))
                {
                    o.AddFailure("DealerOrderFilledVolume", "Dealer Order Filled Volume must have a value when there is an order filled date.");
                    return;
                }

                var parsedVol = long.TryParse(i.DealerOrderFilledVolume, out var ofv);

                if (!parsedVol)
                {
                    o.AddFailure("DealerOrderFilledVolume", "Dealer Order Filled Volume could not be parsed to a long.");
                    return;
                }

                if (ofv < 1)
                {
                    o.AddFailure("DealerOrderFilledVolume", "Dealer Order Filled Volume was below 1 which is invalid when we have an dealer order fill date.");
                }
            });

            RuleFor(x => x).Custom((i, o) =>
            {
                if (string.IsNullOrWhiteSpace(i.DealerOrderFilledDate))
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(i.DealerOrderAverageFillPrice))
                {
                    o.AddFailure("DealerOrderAverageFillPrice", "Dealer Order Average Fill Price must have a value when there is an order filled date.");
                    return;
                }

                var parsedPrice = decimal.TryParse(i.DealerOrderAverageFillPrice, out var ofv);
                if (!parsedPrice)
                {
                    o.AddFailure("DealerOrderAverageFillPrice", "Dealer Order Average Fill Price could not be parsed to a decimal.");
                    return;
                }

                if (ofv == 0)
                {
                    o.AddFailure("DealerOrderAverageFillPrice", "Dealer Order Average Fill Price was 0 which is invalid when we have an order fill date.");
                }
            });

            RulesForTradeOptionsProperties();
        }

        private bool HasDealerOrderData(OrderFileContract contract)
        {
            if (HasOptionFieldSet(contract))
            {
                return true;
            }

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

        private void RulesForTradeOptionsProperties()
        {
            RuleFor(x => x.DealerOrderOptionExpirationDate).NotEmpty().When(HasOptionFieldSet);
            RuleFor(x => x.DealerOrderOptionEuropeanAmerican).NotEmpty().When(HasOptionFieldSet);
            RuleFor(x => x.DealerOrderOptionStrikePrice).NotEmpty().When(HasOptionFieldSet);
        }

        private bool HasOptionFieldSet(OrderFileContract contract)
        {
            if (contract == null)
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(contract.DealerOrderOptionExpirationDate)
                   || !string.IsNullOrWhiteSpace(contract.DealerOrderOptionEuropeanAmerican)
                   || !string.IsNullOrWhiteSpace(contract.DealerOrderOptionStrikePrice);
        }
    }

    public class LongParseableValidator : PropertyValidator
    {
        public LongParseableValidator(string longPropertyName) : base($"Property had a value but could not be parsed to long {longPropertyName}")
        { }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var prop = context.PropertyValue as string;

            if (string.IsNullOrWhiteSpace(prop))
            {
                return true;
            }

            return long.TryParse(prop, out var result);
        }
    }

    public class NonZeroLongParseableValidator : PropertyValidator
    {
        public NonZeroLongParseableValidator(string longPropertyName) : base($"Property had a value but could not be parsed to long {longPropertyName}")
        { }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var prop = context.PropertyValue as string;

            if (string.IsNullOrWhiteSpace(prop))
            {
                return true;
            }

            var parseResult = long.TryParse(prop, out var result);

            if (!parseResult)
            {
                return false;
            }

            return result != 0;
        }
    }

    public class DecimalParseableValidator : PropertyValidator
    {
        public DecimalParseableValidator(string decimalPropertyName) : base($"Property had a value but could not be parsed to decimal {decimalPropertyName}")
        { }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var prop = context.PropertyValue as string;

            if (string.IsNullOrWhiteSpace(prop))
            {
                return true;
            }

            return decimal.TryParse(prop, out var result);
        }
    }

    public class EnumParseableValidator<T> : PropertyValidator where T: struct, IConvertible
    {
        public EnumParseableValidator(string enumPropertyName) : base($"Property out of enum range {enumPropertyName}")
        { }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var prop = context.PropertyValue as string;

            if (!Enum.TryParse(prop, out T result))
            {
                context.MessageFormatter.AppendArgument("MarketType", typeof(T));

                return false;
            }

            return true;
        }
    }

    public class DateParseableValidator : PropertyValidator
    {
        public DateParseableValidator(string errorMessage) : base(errorMessage)
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var prop = context.PropertyValue as string;

            if (string.IsNullOrWhiteSpace(prop))
            {
                return true;
            }
            
            var hasDateOnlyFormat = DateTime.TryParseExact(prop, "yyyy-MM-dd", null, DateTimeStyles.None, out var dateOnlyResult);

            if (hasDateOnlyFormat && dateOnlyResult.Year >= 2010)
            {
                return true;
            }

            if (hasDateOnlyFormat && dateOnlyResult.Year < 2010)
            {
                context.MessageFormatter.AppendArgument(context.PropertyName, prop);
                return false;
            }

            var hasDateAndTimeFormat = DateTime.TryParseExact(prop, "yyyy-MM-ddTHH:mm:ss", null, DateTimeStyles.None, out var dateAndTimeResult);

            if (hasDateAndTimeFormat && dateAndTimeResult.Year >= 2010)
            {
                return true;
            }

            if (hasDateAndTimeFormat && dateAndTimeResult.Year < 2010)
            {
                context.MessageFormatter.AppendArgument(context.PropertyName, prop);
                return false;
            }

            context.MessageFormatter.AppendArgument(context.PropertyName, prop);
            return false;
        }
    }
}