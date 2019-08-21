namespace RedDeer.Surveillance.Api.Client.Nodes
{
    using RedDeer.Surveillance.Api.Client.Infrastructure;

    public class OrderNode : Node<OrderNode>
    {
        public OrderNode(Parent parent)
            : base(parent)
        {
        }

        public OrderNode FieldAccumulatedInterest()
        {
            return this.AddField("accumulatedInterest");
        }

        public OrderNode FieldAverageFillPrice()
        {
            return this.AddField("averageFillPrice");
        }

        public BrokerNode FieldBroker()
        {
            return this.AddChild("broker", new BrokerNode(this));
        }

        public OrderNode FieldCleanDirty()
        {
            return this.AddField("cleanDirty");
        }

        public OrderNode FieldClearingAgent()
        {
            return this.AddField("clearingAgent");
        }

        public OrderNode FieldClientOrderId()
        {
            return this.AddField("clientOrderId");
        }

        public OrderNode FieldCurrency()
        {
            return this.AddField("currency");
        }

        public OrderNode FieldDealingInstructions()
        {
            return this.AddField("dealingInstructions");
        }

        public OrderNode FieldDirection()
        {
            return this.AddField("direction");
        }

        public OrderNode FieldFilledVolume()
        {
            return this.AddField("filledVolume");
        }

        public FinancialInstrumentNode FieldFinancialInstrument()
        {
            return this.AddChild("financialInstrument", new FinancialInstrumentNode(this));
        }

        public OrderNode FieldId()
        {
            return this.AddField("id");
        }

        public OrderNode FieldLimitPrice()
        {
            return this.AddField("limitPrice");
        }

        public MarketNode FieldMarket()
        {
            return this.AddChild("market", new MarketNode(this));
        }

        public OrderNode FieldOptionEuropeanAmerican()
        {
            return this.AddField("optionEuropeanAmerican");
        }

        public OrderNode FieldOptionExpirationDate()
        {
            return this.AddField("optionExpirationDate");
        }

        public OrderNode FieldOptionStrikePrice()
        {
            return this.AddField("optionStrikePrice");
        }

        public OrderAllocationNode FieldOrderAllocations()
        {
            return this.AddChild("orderAllocations", new OrderAllocationNode(this));
        }

        public OrderDatesNode FieldOrderDates()
        {
            return this.AddChild("orderDates", new OrderDatesNode(this));
        }

        public OrderNode FieldOrderedVolume()
        {
            return this.AddField("orderedVolume");
        }

        public OrderNode FieldOrderGroupId()
        {
            return this.AddField("orderGroupId");
        }

        public OrderNode FieldOrderType()
        {
            return this.AddField("orderType");
        }

        public OrderNode FieldOrderVersion()
        {
            return this.AddField("orderVersion");
        }

        public OrderNode FieldOrderVersionLinkId()
        {
            return this.AddField("orderVersionLinkId");
        }

        public OrderNode FieldSettlementCurrency()
        {
            return this.AddField("settlementCurrency");
        }

        public TraderNode FieldTrader()
        {
            return this.AddChild("trader", new TraderNode(this));
        }
    }
}