using Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.Client.Nodes
{
    public class AggregationNode : Node<AggregationNode>
    {
        public AggregationNode(NodeParent parent) : base(parent) { }

        public AggregationNode FieldKey() => AddField("key");
        public AggregationNode FieldCount() => AddField("count");
    }
}
