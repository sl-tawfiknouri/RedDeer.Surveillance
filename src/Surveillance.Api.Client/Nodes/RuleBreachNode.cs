using Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.Client.Nodes
{
    public class RuleBreachNode : Node
    {
        public RuleBreachNode(NodeParent parent) : base(parent) {}

        public RuleBreachNode ArgumentId(int id) => AddArgument("id", id, this);

        public RuleBreachNode FieldId() => AddField("id", this);
        public RuleBreachNode FieldStartOfPeriodUnderInvestigation() => AddField("startOfRuleBreachPeriod", this);
        public RuleBreachNode FieldEndOfPeriodUnderInvestigation() => AddField("endOfRuleBreachPeriod", this);
        public RuleBreachNode FieldReddeerEnrichmentId() => AddField("reddeerEnrichmentId", this);
        public RuleBreachNode FieldTitle() => AddField("title", this);
        public RuleBreachNode FieldDescription() => AddField("description", this);
        public RuleBreachNode FieldVenue() => AddField("venue", this);
        public RuleBreachNode FieldAssetCfi() => AddField("assetCfi", this);
        public RuleBreachNode FieldCreatedOn() => AddField("created", this);
        public RuleBreachNode FieldRuleId() => AddField("ruleId", this);
        public RuleBreachNode FieldCorrelationId() => AddField("correlationId", this);
        public RuleBreachNode FieldIsBackTest() => AddField("isBackTest", this);
        public RuleBreachNode FieldSystemOperationId() => AddField("systemOperationId", this);
        public OrderNode FieldOrders() => AddChild("orders", new OrderNode(this));
    }
}
