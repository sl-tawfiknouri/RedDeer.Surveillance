namespace RedDeer.Surveillance.Api.Client.Nodes
{
    using RedDeer.Surveillance.Api.Client.Infrastructure;

    public class RuleBreachNode : Node<RuleBreachNode>
    {
        public RuleBreachNode(Parent parent)
            : base(parent)
        {
        }

        public RuleBreachNode FieldAssetCfi()
        {
            return this.AddField("assetCfi");
        }

        public RuleBreachNode FieldCorrelationId()
        {
            return this.AddField("correlationId");
        }

        public RuleBreachNode FieldCreatedOn()
        {
            return this.AddField("createdOn");
        }

        public RuleBreachNode FieldDescription()
        {
            return this.AddField("description");
        }

        public RuleBreachNode FieldEndOfPeriodUnderInvestigation()
        {
            return this.AddField("endOfPeriodUnderInvestigation");
        }

        public RuleBreachNode FieldId()
        {
            return this.AddField("id");
        }

        public RuleBreachNode FieldIsBackTest()
        {
            return this.AddField("isBackTest");
        }

        public OrderNode FieldOrders()
        {
            return this.AddChild("orders", new OrderNode(this));
        }

        public RuleBreachNode FieldReddeerEnrichmentId()
        {
            return this.AddField("reddeerEnrichmentId");
        }

        public RuleBreachNode FieldRuleId()
        {
            return this.AddField("ruleId");
        }

        public RuleBreachNode FieldStartOfPeriodUnderInvestigation()
        {
            return this.AddField("startOfPeriodUnderInvestigation");
        }

        public RuleBreachNode FieldSystemOperationId()
        {
            return this.AddField("systemOperationId");
        }

        public RuleBreachNode FieldTitle()
        {
            return this.AddField("title");
        }

        public RuleBreachNode FieldVenue()
        {
            return this.AddField("venue");
        }
    }
}