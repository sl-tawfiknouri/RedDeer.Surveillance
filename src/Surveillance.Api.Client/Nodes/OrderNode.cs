using Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.Client.Nodes
{
    public class OrderNode : Node<OrderNode>
    {
        public OrderNode(Parent parent) : base(parent) { }

        public OrderNode FieldId() => AddField("id");
        public OrderNode FieldClientOrderId() => AddField("clientOrderId");
        public OrderNode FieldOrderVersion() => AddField("orderVersion");
        public OrderNode FieldOrderVersionLinkId() => AddField("orderVersionLinkId");
        public OrderNode FieldOrderGroupId() => AddField("orderGroupId");
        public OrderNode FieldOrderType() => AddField("orderType");
        public OrderNode FieldDirection() => AddField("direction");
        public OrderNode FieldCurrency() => AddField("currency");
        public OrderNode FieldSettlementCurrency() => AddField("settlementCurrency");
        public OrderNode FieldCleanDirty() => AddField("cleanDirty");
        public OrderNode FieldAccumulatedInterest() => AddField("accumulatedInterest");
        public OrderNode FieldLimitPrice() => AddField("limitPrice");
        public OrderNode FieldAverageFillPrice() => AddField("averageFillPrice");
        public OrderNode FieldOrderedVolume() => AddField("orderedVolume");
        public OrderNode FieldFilledVolume() => AddField("filledVolume");
        public OrderNode FieldClearingAgent() => AddField("clearingAgent");
        public OrderNode FieldDealingInstructions() => AddField("dealingInstructions");
        public OrderNode FieldOptionStrikePrice() => AddField("optionStrikePrice");
        public OrderNode FieldOptionExpirationDate() => AddField("optionExpiration");
        public OrderNode FieldOptionEuropeanAmerican() => AddField("optionEuropeanAmerican");

        public OrderDatesNode FieldOrderDates() => AddChild("orderDates", new OrderDatesNode(this));
        public TraderNode FieldTrader() => AddChild("trader", new TraderNode(this));
        public MarketNode FieldMarket() => AddChild("market", new MarketNode(this));
        public FinancialInstrumentNode FieldFinancialInstrument() => AddChild("financialInstrument", new FinancialInstrumentNode(this));
    }
}
