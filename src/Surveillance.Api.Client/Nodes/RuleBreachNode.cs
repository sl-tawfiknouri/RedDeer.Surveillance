using RedDeer.Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedDeer.Surveillance.Api.Client.Nodes
{
    public class RuleBreachNode : Node<RuleBreachNode>
    {
        public RuleBreachNode(Parent parent) : base(parent) {}

        public RuleBreachNode FieldId() => AddField("id");
        public RuleBreachNode FieldStartOfPeriodUnderInvestigation() => AddField("startOfRuleBreachPeriod");
        public RuleBreachNode FieldEndOfPeriodUnderInvestigation() => AddField("endOfRuleBreachPeriod");
        public RuleBreachNode FieldReddeerEnrichmentId() => AddField("reddeerEnrichmentId");
        public RuleBreachNode FieldTitle() => AddField("title");
        public RuleBreachNode FieldDescription() => AddField("description");
        public RuleBreachNode FieldVenue() => AddField("venue");
        public RuleBreachNode FieldAssetCfi() => AddField("assetCfi");
        public RuleBreachNode FieldCreatedOn() => AddField("created");
        public RuleBreachNode FieldRuleId() => AddField("ruleId");
        public RuleBreachNode FieldCorrelationId() => AddField("correlationId");
        public RuleBreachNode FieldIsBackTest() => AddField("isBackTest");
        public RuleBreachNode FieldSystemOperationId() => AddField("systemOperationId");
        public OrderNode FieldOrders() => AddChild("orders", new OrderNode(this));
    }
}
