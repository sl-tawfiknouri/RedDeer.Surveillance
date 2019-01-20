﻿using System;
using System.Globalization;
using DomainV2.Files.Interfaces;
using DomainV2.Financial;
using FluentValidation;
using FluentValidation.Validators;

namespace DomainV2.Files
{
    public class TradeFileCsvValidator : AbstractValidator<TradeFileCsv>, ITradeFileCsvValidator
    {
        public TradeFileCsvValidator()
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
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentUnderlyingSedol));

            RuleFor(x => x.InstrumentUnderlyingIsin)
                .Length(12)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentUnderlyingIsin));

            RuleFor(x => x.InstrumentUnderlyingCusip)
                .MinimumLength(6)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentUnderlyingCusip));

            RuleFor(x => x.InstrumentUnderlyingCusip)
                .MaximumLength(9)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentUnderlyingCusip));

            RuleFor(x => x.InstrumentUnderlyingLei)
                .Length(20)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentUnderlyingLei));

            RuleFor(x => x.InstrumentUnderlyingFigi)
                .Length(12)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentUnderlyingFigi));
        }

        private void RulesForOrderProperties()
        {
            RuleFor(x => x.OrderId).NotEmpty().MinimumLength(1).MaximumLength(255);
            RuleFor(x => x.OrderTraderId).MaximumLength(255);
            RuleFor(x => x.OrderVersion).MaximumLength(255);
            RuleFor(x => x.OrderVersionLinkId).MaximumLength(255);
            RuleFor(x => x.OrderGroupId).MaximumLength(255);

            RuleFor(x => x.OrderCurrency).NotEmpty().MaximumLength(3).MinimumLength(3);
            RuleFor(x => x.OrderSettlementCurrency).MaximumLength(3).MinimumLength(3)
                .When(x => !string.IsNullOrWhiteSpace(x.OrderSettlementCurrency));

            RuleFor(x => x.OrderType).NotEmpty().SetValidator(new EnumParseableValidator<OrderTypes>("OrderType"));
            RuleFor(x => x.OrderDirection).NotEmpty().SetValidator(new EnumParseableValidator<OrderDirections>("OrderPosition"));
            RuleFor(x => x.OrderLimitPrice)
                .NotEmpty()
                .When(x => string.Equals(x.OrderType, "LIMIT", StringComparison.InvariantCultureIgnoreCase));
            RuleFor(x => x.OrderCleanDirty).SetValidator(new EnumParseableValidator<OrderCleanDirty>("OrderCleanDirty"))
                .When(x => !string.IsNullOrWhiteSpace(x.OrderCleanDirty));

            RuleFor(x => x.OrderTraderName).MaximumLength(255).WithMessage("Order Trader Name should not exceed 255 characters");
            RuleFor(x => x.OrderClearingAgent).MaximumLength(255);
            RuleFor(x => x.OrderDealingInstructions).MaximumLength(4095);

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
            RuleFor(x => x.DealerOrderId).NotEmpty().When(HasDealerOrderData).MaximumLength(255).When(HasDealerOrderData);
            RuleFor(x => x.DealerOrderVersion).MaximumLength(255).When(HasDealerOrderData);
            RuleFor(x => x.DealerOrderVersionLinkId).MaximumLength(255).When(HasDealerOrderData);
            RuleFor(x => x.DealerOrderGroupId).MaximumLength(255).When(HasDealerOrderData);
            RuleFor(x => x.DealerOrderDealerId).NotEmpty().When(HasDealerOrderData).MaximumLength(255).When(HasDealerOrderData);
            RuleFor(x => x.DealerOrderNotes).MaximumLength(4095).When(HasDealerOrderData);
            RuleFor(x => x.DealerOrderCounterParty).MaximumLength(255).When(HasDealerOrderData);
            RuleFor(x => x.DealerOrderCurrency).NotEmpty().When(HasDealerOrderData).Length(3).When(HasDealerOrderData);
            RuleFor(x => x.DealerOrderSettlementCurrency).MinimumLength(3).MaximumLength(3).When(x => !string.IsNullOrWhiteSpace(x.DealerOrderSettlementCurrency));
            RuleFor(x => x.DealerOrderDealerName).MaximumLength(255).When(HasDealerOrderData);

            RuleFor(x => x.DealerOrderCleanDirty)
                .SetValidator(new EnumParseableValidator<OrderCleanDirty>("DealerOrderCleanDirty"))
                .When(x => !string.IsNullOrWhiteSpace(x.DealerOrderCleanDirty));

            RuleFor(x => x.DealerOrderType).NotEmpty().SetValidator(new EnumParseableValidator<OrderTypes>("TradeType")).When(HasDealerOrderData);
            RuleFor(x => x.DealerOrderDirection).NotEmpty().SetValidator(new EnumParseableValidator<OrderDirections>("TradePosition")).When(HasDealerOrderData);

            RuleFor(x => x.DealerOrderLimitPrice)
                .NotEmpty()
                .When(x => string.Equals(x.DealerOrderType, "LIMIT", StringComparison.InvariantCultureIgnoreCase));

            RuleFor(x => x.DealerOrderLimitPrice).SetValidator(new DecimalParseableValidator("TradeLimitPrice")).When(HasDealerOrderData);
            RuleFor(x => x.DealerOrderAverageFillPrice).SetValidator(new DecimalParseableValidator("TradeAveragePrice")).When(HasDealerOrderData);
            RuleFor(x => x.DealerOrderOrderedVolume).SetValidator(new NonZeroLongParseableValidator("TradeOrderedVolume")).When(HasDealerOrderData);
            RuleFor(x => x.DealerOrderFilledVolume).SetValidator(new LongParseableValidator("TradeFilledVolume")).When(HasDealerOrderData);
            RuleFor(x => x.DealerOrderAccumulatedInterest)
                .SetValidator(new DecimalParseableValidator("AccumulatedInterest"))
                .When(x => !string.IsNullOrWhiteSpace(x.DealerOrderAccumulatedInterest));

            RuleFor(x => x.DealerOrderDealerName).MaximumLength(255).When(HasDealerOrderData);
            RuleFor(x => x.DealerOrderDirection)
                .Must(x => !string.Equals(x, "none", StringComparison.InvariantCultureIgnoreCase))
                .WithMessage("Trade position had an illegal value of 'none'");

            RuleFor(x => x.DealerOrderDirection)
                .Must(x => !string.Equals(x, "0", StringComparison.InvariantCultureIgnoreCase))
                .WithMessage("TradePosition had an illegal value of '0'");

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

        private bool HasDealerOrderData(TradeFileCsv csv)
        {
            if (HasOptionFieldSet(csv))
            {
                return true;
            }

            return !string.IsNullOrWhiteSpace(csv.DealerOrderDealerId)
                   || !string.IsNullOrWhiteSpace(csv.DealerOrderDealerName)
                   || !string.IsNullOrWhiteSpace(csv.DealerOrderPlacedDate)
                   || !string.IsNullOrWhiteSpace(csv.DealerOrderBookedDate)
                   || !string.IsNullOrWhiteSpace(csv.DealerOrderAmendedDate)
                   || !string.IsNullOrWhiteSpace(csv.DealerOrderRejectedDate)
                   || !string.IsNullOrWhiteSpace(csv.DealerOrderCancelledDate)
                   || !string.IsNullOrWhiteSpace(csv.DealerOrderFilledDate)
                   || !string.IsNullOrWhiteSpace(csv.DealerOrderCounterParty)
                   || !string.IsNullOrWhiteSpace(csv.DealerOrderType)
                   || !string.IsNullOrWhiteSpace(csv.DealerOrderDirection)
                   || !string.IsNullOrWhiteSpace(csv.DealerOrderCurrency)
                   || !string.IsNullOrWhiteSpace(csv.DealerOrderLimitPrice)
                   || !string.IsNullOrWhiteSpace(csv.DealerOrderAverageFillPrice)
                   || !string.IsNullOrWhiteSpace(csv.DealerOrderOrderedVolume)
                   || !string.IsNullOrWhiteSpace(csv.DealerOrderFilledVolume);
        }

        private void RulesForTradeOptionsProperties()
        {
            RuleFor(x => x.DealerOrderOptionExpirationDate).NotEmpty().When(HasOptionFieldSet);
            RuleFor(x => x.DealerOrderOptionEuropeanAmerican).NotEmpty().When(HasOptionFieldSet);
            RuleFor(x => x.DealerOrderOptionStrikePrice).NotEmpty().When(HasOptionFieldSet);
        }

        private bool HasOptionFieldSet(TradeFileCsv csv)
        {
            if (csv == null)
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(csv.DealerOrderOptionExpirationDate)
                   || !string.IsNullOrWhiteSpace(csv.DealerOrderOptionEuropeanAmerican)
                   || !string.IsNullOrWhiteSpace(csv.DealerOrderOptionStrikePrice);
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
}