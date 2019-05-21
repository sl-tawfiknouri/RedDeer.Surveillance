using RedDeer.Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedDeer.Surveillance.Api.Client.Nodes
{
    public class AggregationNode : Node<AggregationNode>
    {
        public AggregationNode(Parent parent) : base(parent) { }

        public AggregationNode FieldKey() => AddField("key");
        public AggregationNode FieldCount() => AddField("count");
    }
}
