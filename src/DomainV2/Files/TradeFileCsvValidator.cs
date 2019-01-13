﻿using System;
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
            RuleFor(x => x.MarketIdentifierCode).NotEmpty();
            RuleFor(x => x.MarketType).NotEmpty().SetValidator(new EnumParseableValidator<MarketTypes>("MarketType"));

            // Instrument
            RulesForSufficientInstrumentIdentificationCodes();
            RulesForIdentificationCodes();
            RulesForDerivativeIdentificationCodes();
            RuleFor(x => x.InstrumentCfi).NotEmpty().MinimumLength(1).MaximumLength(6);
            RuleFor(x => x.InstrumentCfi).Must(x => !string.IsNullOrWhiteSpace(x));

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
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentSedol));

            RuleFor(x => x.InstrumentIsin)
                .Length(12)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentIsin));

            RuleFor(x => x.InstrumentCusip)
                .MinimumLength(6)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentCusip));

            RuleFor(x => x.InstrumentCusip)
                .MaximumLength(9)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentCusip));

            RuleFor(x => x.InstrumentLei)
                .Length(20)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentLei));

            RuleFor(x => x.InstrumentFigi)
                .Length(12)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentFigi));
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
            RuleFor(x => x.OrderId).NotEmpty().MinimumLength(1);
            RuleFor(x => x.OrderCurrency).NotEmpty().MaximumLength(3).MinimumLength(3);
            RuleFor(x => x.OrderType).NotEmpty().SetValidator(new EnumParseableValidator<OrderTypes>("OrderType"));
            RuleFor(x => x.OrderDirection).NotEmpty().SetValidator(new EnumParseableValidator<OrderDirections>("OrderPosition"));
            RuleFor(x => x.OrderLimitPrice)
                .NotEmpty()
                .When(x => string.Equals(x.OrderType, "LIMIT", StringComparison.InvariantCultureIgnoreCase));

            RuleFor(x => x.OrderLimitPrice).SetValidator(new DecimalParseableValidator("OrderLimitPrice"));
            RuleFor(x => x.OrderAverageFillPrice).SetValidator(new DecimalParseableValidator("OrderAveragePrice"));
            RuleFor(x => x.OrderFilledVolume).SetValidator(new LongParseableValidator("OrderFilledVolume"));
            RuleFor(x => x.OrderOrderedVolume).SetValidator(new LongParseableValidator("OrderOrderedVolume"));
            RuleFor(x => x.OrderDirection)
                .Must(x => !string.Equals(x, "none", StringComparison.InvariantCultureIgnoreCase))
                .WithMessage("Order position had an illegal value of 'none'");

            RuleFor(x => x.OrderDirection)
                .Must(x => !string.Equals(x, "0", StringComparison.InvariantCultureIgnoreCase))
                .WithMessage("Order position had an illegal value of '0'");
        }

        private void RulesForDealerOrderProperties()
        {
            RuleFor(x => x.DealerOrderId).NotEmpty().When(HasTradeOrTransactionData);
            RuleFor(x => x.DealerOrderDealerId).NotEmpty().When(HasTradeOrTransactionData);
            RuleFor(x => x.DealerOrderCurrency).NotEmpty().When(HasTradeOrTransactionData).Length(3).When(HasTradeOrTransactionData);
            RuleFor(x => x.DealerOrderType).NotEmpty().SetValidator(new EnumParseableValidator<OrderTypes>("TradeType")).When(HasTradeOrTransactionData);
            RuleFor(x => x.DealerOrderDirection).NotEmpty().SetValidator(new EnumParseableValidator<OrderDirections>("TradePosition")).When(HasTradeOrTransactionData);

            RuleFor(x => x.DealerOrderLimitPrice)
                .NotEmpty()
                .When(x => string.Equals(x.DealerOrderType, "LIMIT", StringComparison.InvariantCultureIgnoreCase));

            RuleFor(x => x.DealerOrderLimitPrice).SetValidator(new DecimalParseableValidator("TradeLimitPrice")).When(HasTradeOrTransactionData);
            RuleFor(x => x.DealerOrderAverageFillPrice).SetValidator(new DecimalParseableValidator("TradeAveragePrice")).When(HasTradeOrTransactionData);
            RuleFor(x => x.DealerOrderOrderedVolume).SetValidator(new LongParseableValidator("TradeOrderedVolume")).When(HasTradeOrTransactionData);
            RuleFor(x => x.DealerOrderFilledVolume).SetValidator(new LongParseableValidator("TradeFilledVolume")).When(HasTradeOrTransactionData);

            RuleFor(x => x.DealerOrderDirection)
                .Must(x => !string.Equals(x, "none", StringComparison.InvariantCultureIgnoreCase))
                .WithMessage("Trade position had an illegal value of 'none'");
            
            RuleFor(x => x.DealerOrderDirection)
                .Must(x => !string.Equals(x, "0", StringComparison.InvariantCultureIgnoreCase))
                .WithMessage("TradePosition had an illegal value of '0'");

            RulesForTradeOptionsProperties();
        }

        private bool HasTradeOrTransactionData(TradeFileCsv csv)
        {
            if (HasOptionFieldSet(csv))
            {
                return true;
            }

            return !string.IsNullOrWhiteSpace(csv.DealerOrderDealerId)
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