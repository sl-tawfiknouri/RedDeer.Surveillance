using Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.Client.Nodes
{
    public class OrderNode : Node
    {
        public OrderNode(NodeParent parent) : base(parent) { }

        public OrderNode ArgumentIds(List<int> ids) => AddArgument("ids", ids, this);
        public OrderNode ArgumentTraderIds(List<string> traderIds) => AddArgument("traderIds", traderIds, this);
        public OrderNode ArgumentReddeerIds(List<string> reddeerIds) => AddArgument("reddeerIds", reddeerIds, this);
        public OrderNode ArgumentTake(int count) => AddArgument("take", count, this);

        public OrderNode FieldId() => AddField("id", this);
        public OrderNode FieldClientOrderId() => AddField("clientOrderId", this);
        public OrderNode FieldOrderVersion() => AddField("orderVersion", this);
        public OrderNode FieldOrderVersionLinkId() => AddField("orderVersionLinkId", this);
        public OrderNode FieldOrderGroupId() => AddField("orderGroupId", this);
        public OrderNode FieldOrderType() => AddField("orderType", this);
        public OrderNode FieldDirection() => AddField("direction", this);
        public OrderNode FieldCurrency() => AddField("currency", this);
        public OrderNode FieldSettlementCurrency() => AddField("settlementCurrency", this);
        public OrderNode FieldCleanDirty() => AddField("cleanDirty", this);
        public OrderNode FieldAccumulatedInterest() => AddField("accumulatedInterest", this);
        public OrderNode FieldLimitPrice() => AddField("limitPrice", this);
        public OrderNode FieldAverageFillPrice() => AddField("averageFillPrice", this);
        public OrderNode FieldOrderedVolume() => AddField("orderedVolume", this);
        public OrderNode FieldFilledVolume() => AddField("filledVolume", this);
        public OrderNode FieldClearingAgent() => AddField("clearingAgent", this);
        public OrderNode FieldDealingInstructions() => AddField("dealingInstructions", this);
        public OrderNode FieldOptionStrikePrice() => AddField("optionStrikePrice", this);
        public OrderNode FieldOptionExpirationDate() => AddField("optionExpiration", this);
        public OrderNode FieldOptionEuropeanAmerican() => AddField("optionEuropeanAmerican", this);

        public OrderDatesNode FieldOrderDates() => AddChild("orderDates", new OrderDatesNode(this));
        public TraderNode FieldTrader() => AddChild("trader", new TraderNode(this));
        public MarketNode FieldMarket() => AddChild("market", new MarketNode(this));
        public FinancialInstrumentNode FieldFinancialInstrument() => AddChild("financialInstrument", new FinancialInstrumentNode(this));
    }
}
