using System;
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
            RuleFor(x => x.MarketType).NotEmpty().SetValidator(new EnumParseableValidator<MarketTypes>());

            // Instrument
            RulesForSufficientInstrumentIdentificationCodes();
            RulesForIdentificationCodes();
            RulesForDerivativeIdentificationCodes();
            RuleFor(x => x.InstrumentCfi).NotEmpty().MinimumLength(1).MaximumLength(6);

            // Order
            RulesForOrderProperties();

            // Trade
            RulesForTradeProperties();

            // Transaction
            RuleForTransactionProperties();
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
            RuleFor(x => x.OrderType).NotEmpty().SetValidator(new EnumParseableValidator<OrderTypes>());
            RuleFor(x => x.OrderPosition).NotEmpty().SetValidator(new EnumParseableValidator<OrderPositions>());
            RuleFor(x => x.OrderLimitPrice)
                .NotEmpty()
                .When(x => string.Equals(x.OrderType, "LIMIT", StringComparison.InvariantCultureIgnoreCase));

            RuleFor(x => x.OrderLimitPrice).SetValidator(new DecimalParseableValidator());
            RuleFor(x => x.OrderAveragePrice).SetValidator(new DecimalParseableValidator());
            RuleFor(x => x.OrderFilledVolume).SetValidator(new LongParseableValidator());
            RuleFor(x => x.OrderOrderedVolume).SetValidator(new LongParseableValidator());
        }

        private void RulesForTradeProperties()
        {
            RuleFor(x => x.TradeId).NotEmpty().When(HasTradeOrTransactionData);
            RuleFor(x => x.TraderId).NotEmpty().When(HasTradeOrTransactionData);
            RuleFor(x => x.TradeCurrency).NotEmpty().When(HasTradeOrTransactionData).Length(3).When(HasTradeOrTransactionData);
            RuleFor(x => x.TradeType).NotEmpty().SetValidator(new EnumParseableValidator<OrderTypes>()).When(HasTradeOrTransactionData);
            RuleFor(x => x.TradePosition).NotEmpty().SetValidator(new EnumParseableValidator<OrderPositions>()).When(HasTradeOrTransactionData);

            RuleFor(x => x.TradeLimitPrice)
                .NotEmpty()
                .When(x => 
                    string.Equals(x.TradeType, "LIMIT", StringComparison.InvariantCultureIgnoreCase)
                    && HasTransactionData(x));

            RuleFor(x => x.TradeLimitPrice).SetValidator(new DecimalParseableValidator()).When(HasTradeOrTransactionData);
            RuleFor(x => x.TradeAveragePrice).SetValidator(new DecimalParseableValidator()).When(HasTradeOrTransactionData);
            RuleFor(x => x.TradeOrderedVolume).SetValidator(new LongParseableValidator()).When(HasTradeOrTransactionData);
            RuleFor(x => x.TradeFilledVolume).SetValidator(new LongParseableValidator()).When(HasTradeOrTransactionData);

            RulesForTradeOptionsProperties();
        }

        private bool HasTradeOrTransactionData(TradeFileCsv csv)
        {
            if (HasTransactionData(csv))
            {
                return true;
            }

            if (HasOptionFieldSet(csv))
            {
                return true;
            }

            return !string.IsNullOrWhiteSpace(csv.TraderId)
                   || !string.IsNullOrWhiteSpace(csv.TradePlacedDate)
                   || !string.IsNullOrWhiteSpace(csv.TradeBookedDate)
                   || !string.IsNullOrWhiteSpace(csv.TradeAmendedDate)
                   || !string.IsNullOrWhiteSpace(csv.TradeRejectedDate)
                   || !string.IsNullOrWhiteSpace(csv.TradeCancelledDate)
                   || !string.IsNullOrWhiteSpace(csv.TradeFilledDate)
                   || !string.IsNullOrWhiteSpace(csv.TradeCounterParty)
                   || !string.IsNullOrWhiteSpace(csv.TradeType)
                   || !string.IsNullOrWhiteSpace(csv.TradePosition)
                   || !string.IsNullOrWhiteSpace(csv.TradeCurrency)
                   || !string.IsNullOrWhiteSpace(csv.TradeLimitPrice)
                   || !string.IsNullOrWhiteSpace(csv.TradeAveragePrice)
                   || !string.IsNullOrWhiteSpace(csv.TradeOrderedVolume)
                   || !string.IsNullOrWhiteSpace(csv.TradeFilledVolume);
        }

        private void RulesForTradeOptionsProperties()
        {
            RuleFor(x => x.TradeOptionExpirationDate).NotEmpty().When(HasOptionFieldSet);
            RuleFor(x => x.TradeOptionEuropeanAmerican).NotEmpty().When(HasOptionFieldSet);
            RuleFor(x => x.TradeOptionStrikePrice).NotEmpty().When(HasOptionFieldSet);
        }

        private bool HasOptionFieldSet(TradeFileCsv csv)
        {
            if (csv == null)
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(csv.TradeOptionExpirationDate)
                   || !string.IsNullOrWhiteSpace(csv.TradeOptionEuropeanAmerican)
                   || !string.IsNullOrWhiteSpace(csv.TradeOptionStrikePrice);
        }

        private void RuleForTransactionProperties()
        {
            RuleFor(x => x.TransactionId).NotEmpty().When(HasTransactionData);
            RuleFor(x => x.TransactionPlacedDate).NotEmpty().When(HasTransactionData);
            RuleFor(x => x.TransactionTraderId).NotEmpty().When(HasTransactionData);
            RuleFor(x => x.TransactionCurrency).NotEmpty().When(HasTransactionData);
            RuleFor(x => x.TransactionCurrency).Length(3).When(HasTransactionData);

            RuleFor(x => x.TransactionType).NotEmpty().When(HasTransactionData);
            RuleFor(x => x.TransactionType).SetValidator(new EnumParseableValidator<MarketTypes>()).When(HasTransactionData);
            RuleFor(x => x.TransactionPosition).NotEmpty().When(HasTransactionData);
            RuleFor(x => x.TransactionPosition).SetValidator(new EnumParseableValidator<OrderPositions>()).When(HasTransactionData);

            RuleFor(x => x.TransactionLimitPrice).SetValidator(new DecimalParseableValidator()).When(HasTransactionData);
            RuleFor(x => x.TransactionAveragePrice).SetValidator(new DecimalParseableValidator()).When(HasTransactionData);

            RuleFor(x => x.TransactionFilledVolume).SetValidator(new LongParseableValidator()).When(HasTransactionData);
            RuleFor(x => x.TransactionOrderedVolume).SetValidator(new LongParseableValidator()).When(HasTransactionData);
        }

        private bool HasTransactionData(TradeFileCsv csv)
        {
            if (csv == null)
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(csv.TransactionId)
                   || !string.IsNullOrWhiteSpace(csv.TransactionPlacedDate)
                   || !string.IsNullOrWhiteSpace(csv.TransactionBookedDate)
                   || !string.IsNullOrWhiteSpace(csv.TransactionAmendedDate)
                   || !string.IsNullOrWhiteSpace(csv.TransactionRejectedDate)
                   || !string.IsNullOrWhiteSpace(csv.TransactionCancelledDate)
                   || !string.IsNullOrWhiteSpace(csv.TransactionFilledDate)
                   || !string.IsNullOrWhiteSpace(csv.TransactionTraderId)
                   || !string.IsNullOrWhiteSpace(csv.TransactionCounterParty)
                   || !string.IsNullOrWhiteSpace(csv.TransactionType)
                   || !string.IsNullOrWhiteSpace(csv.TransactionPosition)
                   || !string.IsNullOrWhiteSpace(csv.TransactionCurrency)
                   || !string.IsNullOrWhiteSpace(csv.TransactionLimitPrice)
                   || !string.IsNullOrWhiteSpace(csv.TransactionAveragePrice)
                   || !string.IsNullOrWhiteSpace(csv.TransactionOrderedVolume)
                   || !string.IsNullOrWhiteSpace(csv.TransactionFilledVolume);
        }
    }

    public class LongParseableValidator : PropertyValidator
    {
        public LongParseableValidator() : base("Property had a value but could not be parsed to long")
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
        public DecimalParseableValidator() : base("Property had a value but could not be parsed to decimal")
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
        public EnumParseableValidator() : base("Property out of enum range")
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