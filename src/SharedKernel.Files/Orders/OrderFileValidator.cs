namespace SharedKernel.Files.Orders
{
    using System;

    using Domain.Core.Markets;
    using Domain.Core.Trading.Orders;

    using FluentValidation;

    using SharedKernel.Files.Orders.Interfaces;
    using SharedKernel.Files.PropertyValidators;

    public class OrderFileValidator : AbstractValidator<OrderFileContract>, IOrderFileValidator
    {
        public OrderFileValidator()
        {
            // Market
            this.RuleFor(x => x.MarketIdentifierCode).NotEmpty()
                .WithMessage("Market Identifier Code must not be empty");

            this.RuleFor(x => x.MarketType).NotEmpty()
                .SetValidator(new EnumParseableValidator<MarketTypes>("MarketType"))
                .WithMessage("Market Type must be valid");

            // Instrument
            this.RulesForSufficientInstrumentIdentificationCodes();
            this.RulesForIdentificationCodes();
            this.RulesForDerivativeIdentificationCodes();

            this.RuleFor(x => x.InstrumentCfi).NotEmpty().MinimumLength(1).MaximumLength(6)
                .WithMessage("Instrument CFI must be between 1 and 6 characters");
            this.RuleFor(x => x.InstrumentCfi).Must(x => !string.IsNullOrWhiteSpace(x))
                .WithMessage("Instrument CFI must not be empty");

            // Order
            this.RulesForOrderProperties();

            // Trade
            this.RulesForDealerOrderProperties();
        }

        private bool HasDealerOrderData(OrderFileContract contract)
        {
            if (this.HasOptionFieldSet(contract)) return true;

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

        private bool HasOptionFieldSet(OrderFileContract contract)
        {
            if (contract == null) return false;

            return !string.IsNullOrWhiteSpace(contract.DealerOrderOptionExpirationDate)
                   || !string.IsNullOrWhiteSpace(contract.DealerOrderOptionEuropeanAmerican)
                   || !string.IsNullOrWhiteSpace(contract.DealerOrderOptionStrikePrice);
        }

        private void RulesForDealerOrderProperties()
        {
            this.RuleFor(x => x.DealerOrderId).NotEmpty().When(this.HasDealerOrderData).MaximumLength(255)
                .When(this.HasDealerOrderData)
                .WithMessage("DealerOrderId must have a maximum length of 255 characters");
            this.RuleFor(x => x.DealerOrderVersion).MaximumLength(255).When(this.HasDealerOrderData)
                .WithMessage("DealerOrderVersion must have a maximum length of 255 characters");
            this.RuleFor(x => x.DealerOrderVersionLinkId).MaximumLength(255).When(this.HasDealerOrderData)
                .WithMessage("DealerOrderVersionLinkId must have a maximum length of 255 characters");
            this.RuleFor(x => x.DealerOrderGroupId).MaximumLength(255).When(this.HasDealerOrderData)
                .WithMessage("DealerOrderGroupId must have a maximum length of 255 characters");

            this.RuleFor(x => x.DealerOrderDealerId).NotEmpty().When(this.HasDealerOrderData).MaximumLength(255)
                .When(this.HasDealerOrderData)
                .WithMessage("DealerOrderDealerId must have a maximum length of 255 characters");

            this.RuleFor(x => x.DealerOrderNotes).MaximumLength(4095).When(this.HasDealerOrderData)
                .WithMessage("DealerOrderNotes must have a maximum length of 4095  characters");

            this.RuleFor(x => x.DealerOrderCounterParty).MaximumLength(255).When(this.HasDealerOrderData)
                .WithMessage("DealerOrderCounterParty must have a maximum length of 255 characters");
            this.RuleFor(x => x.DealerOrderCurrency).NotEmpty().When(this.HasDealerOrderData).Length(3)
                .When(this.HasDealerOrderData).WithMessage("DealerOrderCurrency must have a length of 3 characters");
            this.RuleFor(x => x.DealerOrderSettlementCurrency).Length(3)
                .When(x => !string.IsNullOrWhiteSpace(x.DealerOrderSettlementCurrency)).WithMessage(
                    "DealerOrderSettlementCurrency must have a length of 3 characters when it is provided");

            this.RuleFor(x => x.DealerOrderDealerName).MaximumLength(255).When(this.HasDealerOrderData)
                .WithMessage("DealerOrderDealerName must have a maximum length of 255 characters");

            this.RuleFor(x => x.DealerOrderCleanDirty)
                .SetValidator(new EnumParseableValidator<OrderCleanDirty>("DealerOrderCleanDirty"))
                .When(x => !string.IsNullOrWhiteSpace(x.DealerOrderCleanDirty));

            this.RuleFor(x => x.DealerOrderType).NotEmpty()
                .SetValidator(new EnumParseableValidator<OrderTypes>("TradeType")).When(this.HasDealerOrderData);
            this.RuleFor(x => x.DealerOrderDirection).NotEmpty()
                .SetValidator(new EnumParseableValidator<OrderDirections>("TradePosition"))
                .When(this.HasDealerOrderData);

            this.RuleFor(x => x.DealerOrderLimitPrice).NotEmpty()
                .When(x => string.Equals(x.DealerOrderType, "LIMIT", StringComparison.InvariantCultureIgnoreCase))
                .WithMessage("DealerOrderLimitPrice must be provided when there is a dealer order type of LIMIT");

            this.RuleFor(x => x.DealerOrderLimitPrice).SetValidator(new DecimalParseableValidator("TradeLimitPrice"))
                .When(this.HasDealerOrderData);
            this.RuleFor(x => x.DealerOrderAverageFillPrice)
                .SetValidator(new DecimalParseableValidator("TradeAveragePrice")).When(this.HasDealerOrderData);
            this.RuleFor(x => x.DealerOrderOrderedVolume)
                .SetValidator(new NonZeroDecimalParseableValidator("TradeOrderedVolume")).When(this.HasDealerOrderData);
            this.RuleFor(x => x.DealerOrderFilledVolume)
                .SetValidator(new DecimalParseableValidator("TradeFilledVolume")).When(this.HasDealerOrderData);
            this.RuleFor(x => x.DealerOrderAccumulatedInterest)
                .SetValidator(new DecimalParseableValidator("AccumulatedInterest")).When(
                    x => !string.IsNullOrWhiteSpace(x.DealerOrderAccumulatedInterest));

            this.RuleFor(x => x.DealerOrderDealerName).MaximumLength(255).When(this.HasDealerOrderData)
                .WithMessage("DealerOrderDealerName must have a maximum length of 255 characters");
            this.RuleFor(x => x.DealerOrderDirection)
                .Must(x => !string.Equals(x, "none", StringComparison.InvariantCultureIgnoreCase))
                .WithMessage("Trade position had an illegal value of 'none'");

            this.RuleFor(x => x.DealerOrderDirection)
                .Must(x => !string.Equals(x, "0", StringComparison.InvariantCultureIgnoreCase))
                .WithMessage("TradePosition had an illegal value of '0'");

            this.RuleFor(x => x.DealerOrderPlacedDate).NotEmpty().When(this.HasDealerOrderData);
            this.RuleFor(x => x.DealerOrderPlacedDate).SetValidator(new DateParseableValidator("DealerOrderPlacedDate"))
                .When(this.HasDealerOrderData);
            this.RuleFor(x => x.DealerOrderBookedDate).SetValidator(new DateParseableValidator("DealerOrderBookedDate"))
                .When(this.HasDealerOrderData);
            this.RuleFor(x => x.DealerOrderAmendedDate)
                .SetValidator(new DateParseableValidator("DealerOrderAmendedDate")).When(this.HasDealerOrderData);
            this.RuleFor(x => x.DealerOrderCancelledDate)
                .SetValidator(new DateParseableValidator("DealerOrderCancelledDate")).When(this.HasDealerOrderData);
            this.RuleFor(x => x.DealerOrderRejectedDate)
                .SetValidator(new DateParseableValidator("DealerOrderRejectedDate")).When(this.HasDealerOrderData);
            this.RuleFor(x => x.DealerOrderFilledDate).SetValidator(new DateParseableValidator("DealerOrderFilledDate"))
                .When(this.HasDealerOrderData);

            this.RuleFor(x => x).Custom(
                (i, o) =>
                    {
                        if (string.IsNullOrWhiteSpace(i.DealerOrderFilledDate)) return;

                        if (string.IsNullOrWhiteSpace(i.DealerOrderFilledVolume))
                        {
                            o.AddFailure(
                                "DealerOrderFilledVolume",
                                "Dealer Order Filled Volume must have a value when there is an order filled date.");
                            return;
                        }

                        var parsedVol = long.TryParse(i.DealerOrderFilledVolume, out var ofv);

                        if (!parsedVol)
                        {
                            o.AddFailure(
                                "DealerOrderFilledVolume",
                                "Dealer Order Filled Volume could not be parsed to a long.");
                            return;
                        }

                        if (ofv < 1)
                            o.AddFailure(
                                "DealerOrderFilledVolume",
                                "Dealer Order Filled Volume was below 1 which is invalid when we have an dealer order fill date.");
                    });

            this.RuleFor(x => x).Custom(
                (i, o) =>
                    {
                        if (string.IsNullOrWhiteSpace(i.DealerOrderFilledDate)) return;

                        if (string.IsNullOrWhiteSpace(i.DealerOrderAverageFillPrice))
                        {
                            o.AddFailure(
                                "DealerOrderAverageFillPrice",
                                "Dealer Order Average Fill Price must have a value when there is an order filled date.");
                            return;
                        }

                        var parsedPrice = decimal.TryParse(i.DealerOrderAverageFillPrice, out var ofv);
                        if (!parsedPrice)
                        {
                            o.AddFailure(
                                "DealerOrderAverageFillPrice",
                                "Dealer Order Average Fill Price could not be parsed to a decimal.");
                            return;
                        }

                        if (ofv == 0)
                            o.AddFailure(
                                "DealerOrderAverageFillPrice",
                                "Dealer Order Average Fill Price was 0 which is invalid when we have an order fill date.");
                    });

            this.RulesForTradeOptionsProperties();
        }

        private void RulesForDerivativeIdentificationCodes()
        {
            this.RuleFor(x => x.InstrumentUnderlyingSedol).MinimumLength(1).MaximumLength(7)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentUnderlyingSedol)).WithMessage(
                    "Instrument Underlying Sedol must have a length of 7 characters when it is provided");

            this.RuleFor(x => x.InstrumentUnderlyingIsin).Length(12)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentUnderlyingIsin)).WithMessage(
                    "Instrument Underlying ISIN must have a length of 12 characters when it is provided");

            this.RuleFor(x => x.InstrumentUnderlyingCusip).MinimumLength(6)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentUnderlyingCusip)).WithMessage(
                    "Instrument Underlying CUSIP must have a minimum length of 6 characters when it is provided");

            this.RuleFor(x => x.InstrumentUnderlyingCusip).MaximumLength(9)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentUnderlyingCusip)).WithMessage(
                    "Instrument Underlying CUSIP must have a maximum length of 9 characters when it is provided");

            this.RuleFor(x => x.InstrumentUnderlyingLei).Length(20)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentUnderlyingLei)).WithMessage(
                    "Instrument Underlying LEI must have a length of 20 characters when it is provided");

            this.RuleFor(x => x.InstrumentUnderlyingFigi).Length(12)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentUnderlyingFigi)).WithMessage(
                    "Instrument Underlying FIGI must have a length of 12 characters when it is provided");
        }

        private void RulesForIdentificationCodes()
        {
            this.RuleFor(x => x.InstrumentSedol).MinimumLength(1).MaximumLength(7)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentSedol)).WithMessage(
                    "Instrument Sedol must have a length of 7 characters when it is provided");

            this.RuleFor(x => x.InstrumentIsin).Length(12).When(x => !string.IsNullOrWhiteSpace(x.InstrumentIsin))
                .WithMessage("Instrument ISIN must have a length of 12 characters when it is provided");

            this.RuleFor(x => x.InstrumentCusip).MinimumLength(6)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentCusip)).WithMessage(
                    "Instrument CUSIP must have a minimum length of 6 when it is provided");

            this.RuleFor(x => x.InstrumentCusip).MaximumLength(9)
                .When(x => !string.IsNullOrWhiteSpace(x.InstrumentCusip)).WithMessage(
                    "Instrument CUSIP must have a maximum length of 9 characters when it is provided");

            this.RuleFor(x => x.InstrumentLei).Length(20).When(x => !string.IsNullOrWhiteSpace(x.InstrumentLei))
                .WithMessage("Instrument LEI must have a length of 20 characters if it is provided");

            this.RuleFor(x => x.InstrumentFigi).Length(12).When(x => !string.IsNullOrWhiteSpace(x.InstrumentFigi))
                .WithMessage("Instrument FIGI must have a length of 12 characters if it is provided");
        }

        private void RulesForOrderProperties()
        {
            this.RuleFor(x => x.OrderId).NotEmpty().MinimumLength(1).MaximumLength(255)
                .WithMessage("OrderId must have a length between 1 and 255 characters");
            this.RuleFor(x => x.OrderTraderId).MaximumLength(255)
                .WithMessage("OrderTraderId must have a maximum length of 255 characters");
            this.RuleFor(x => x.OrderVersion).MaximumLength(255)
                .WithMessage("OrderVersion must have a maximum length of 255 characters");
            this.RuleFor(x => x.OrderVersionLinkId).MaximumLength(255)
                .WithMessage("OrderVersionLinkId must have a maximum length of 255 characters");
            this.RuleFor(x => x.OrderGroupId).MaximumLength(255)
                .WithMessage("OrderGroupId must have a maximum length of 255 characters");
            this.RuleFor(x => x.OrderBroker).MaximumLength(1023)
                .WithMessage("OrderBroker must have a maximum length of 1023 characters");

            this.RuleFor(x => x.OrderCurrency).NotEmpty().Length(3)
                .WithMessage("OrderCurrency must have a length of 3 characters");

            this.RuleFor(x => x.OrderSettlementCurrency).Length(3)
                .When(x => !string.IsNullOrWhiteSpace(x.OrderSettlementCurrency)).WithMessage(
                    "OrderSettlementCurrency must have a length of 3 characters when it is provided");

            this.RuleFor(x => x.OrderType).NotEmpty().SetValidator(new EnumParseableValidator<OrderTypes>("OrderType"));
            this.RuleFor(x => x.OrderDirection).NotEmpty()
                .SetValidator(new EnumParseableValidator<OrderDirections>("OrderPosition"));

            this.RuleFor(x => x.OrderLimitPrice).NotEmpty()
                .When(x => string.Equals(x.OrderType, "LIMIT", StringComparison.InvariantCultureIgnoreCase))
                .WithMessage("OrderLimitPrice must have a value when the order is of type LIMIT");

            this.RuleFor(x => x.OrderCleanDirty)
                .SetValidator(new EnumParseableValidator<OrderCleanDirty>("OrderCleanDirty"))
                .When(x => !string.IsNullOrWhiteSpace(x.OrderCleanDirty));

            this.RuleFor(x => x.OrderTraderName).MaximumLength(255)
                .WithMessage("Order Trader Name should not exceed 255 characters in length");
            this.RuleFor(x => x.OrderClearingAgent).MaximumLength(255)
                .WithMessage("OrderClearingAgent must have a maximum length of 255 characters");

            this.RuleFor(x => x.OrderDealingInstructions).MaximumLength(4095).WithMessage(
                "OrderDealingInstructions must have a maximum length of 4095 characters");

            this.RuleFor(x => x.OrderLimitPrice).SetValidator(new DecimalParseableValidator("OrderLimitPrice"));
            this.RuleFor(x => x.OrderAverageFillPrice).SetValidator(new DecimalParseableValidator("OrderAveragePrice"));
            this.RuleFor(x => x.OrderFilledVolume).SetValidator(new DecimalParseableValidator("OrderFilledVolume"));
            this.RuleFor(x => x.OrderFilledVolume).Must(x => !string.IsNullOrWhiteSpace(x))
                .When(y => !string.IsNullOrWhiteSpace(y.OrderFilledDate)).WithMessage(
                    "Must have an order filled volume when there is an order filled date");
            this.RuleFor(x => x.OrderOrderedVolume).Must(x => !string.IsNullOrWhiteSpace(x))
                .SetValidator(new NonZeroDecimalParseableValidator("OrderOrderedVolume"));

            this.RuleFor(x => x.OrderAccumulatedInterest)
                .SetValidator(new DecimalParseableValidator("OrderAccumulatedInterest"))
                .When(x => !string.IsNullOrWhiteSpace(x.OrderAccumulatedInterest));

            this.RuleFor(x => x.OrderPlacedDate).NotEmpty();
            this.RuleFor(x => x.OrderPlacedDate).SetValidator(new DateParseableValidator("OrderPlacedDate"));
            this.RuleFor(x => x.OrderBookedDate).SetValidator(new DateParseableValidator("OrderBookedDate"));
            this.RuleFor(x => x.OrderAmendedDate).SetValidator(new DateParseableValidator("OrderAmendedDate"));
            this.RuleFor(x => x.OrderRejectedDate).SetValidator(new DateParseableValidator("OrderRejectedDate"));
            this.RuleFor(x => x.OrderCancelledDate).SetValidator(new DateParseableValidator("OrderCancelledDate"));
            this.RuleFor(x => x.OrderFilledDate).SetValidator(new DateParseableValidator("OrderFilledDate"));

            this.RuleFor(x => x).Custom(
                (i, o) =>
                    {
                        if (string.IsNullOrWhiteSpace(i.OrderFilledDate)) return;

                        if (string.IsNullOrWhiteSpace(i.OrderFilledVolume))
                        {
                            o.AddFailure(
                                "OrderFilledVolume",
                                "Order Filled Volume must have a value when there is an order filled date.");
                            return;
                        }

                        var parsedVol = long.TryParse(i.OrderFilledVolume, out var ofv);

                        if (!parsedVol)
                        {
                            o.AddFailure("OrderFilledVolume", "Order Filled Volume could not be parsed to a long.");
                            return;
                        }

                        if (ofv < 1)
                            o.AddFailure(
                                "OrderFilledVolume",
                                "Order Filled Volume was below 1 which is invalid when we have an order fill date.");
                    });

            this.RuleFor(x => x).Custom(
                (i, o) =>
                    {
                        if (string.IsNullOrWhiteSpace(i.OrderFilledDate)) return;

                        if (string.IsNullOrWhiteSpace(i.OrderAverageFillPrice))
                        {
                            o.AddFailure(
                                "OrderAverageFillPrice",
                                "Order Average Fill Price must have a value when there is an order filled date.");
                            return;
                        }

                        var parsedPrice = decimal.TryParse(i.OrderAverageFillPrice, out var ofv);
                        if (!parsedPrice)
                        {
                            o.AddFailure(
                                "OrderAverageFillPrice",
                                "Order Average Fill Price could not be parsed to a decimal.");
                            return;
                        }

                        if (ofv == 0)
                            o.AddFailure(
                                "OrderAverageFillPrice",
                                "Order Average Fill Price was 0 which is invalid when we have an order fill date.");
                    });

            this.RuleFor(x => x.OrderDirection)
                .Must(x => !string.Equals(x, "none", StringComparison.InvariantCultureIgnoreCase))
                .WithMessage("Order position had an illegal value of 'none'");

            this.RuleFor(x => x.OrderDirection)
                .Must(x => !string.Equals(x, "0", StringComparison.InvariantCultureIgnoreCase))
                .WithMessage("Order position had an illegal value of '0'");
        }

        private void RulesForSufficientInstrumentIdentificationCodes()
        {
            this.RuleFor(x => x).Must(
                x => !string.IsNullOrWhiteSpace(x.InstrumentIsin) || !string.IsNullOrWhiteSpace(x.InstrumentSedol)
                                                                  || !string.IsNullOrWhiteSpace(x.InstrumentCusip)
                                                                  || !string.IsNullOrWhiteSpace(
                                                                      x.InstrumentBloombergTicker));
        }

        private void RulesForTradeOptionsProperties()
        {
            this.RuleFor(x => x.DealerOrderOptionExpirationDate).NotEmpty().When(this.HasOptionFieldSet);
            this.RuleFor(x => x.DealerOrderOptionEuropeanAmerican).NotEmpty().When(this.HasOptionFieldSet);
            this.RuleFor(x => x.DealerOrderOptionStrikePrice).NotEmpty().When(this.HasOptionFieldSet);
        }
    }
}