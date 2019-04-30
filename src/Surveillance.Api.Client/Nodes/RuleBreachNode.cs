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
        public RuleBreachNode FieldRuleId() => AddField("ruleId", this);
        public OrderNode FieldOrders()
        {
            var orderGraph = new OrderNode(this);
            _actions.Add(ruleBreaches => ruleBreaches
                .Field("orders", orders =>
                 {
                     orderGraph._actions.ForEach(x => x(orders));
                 }));
            return orderGraph;
        }
    }
}
