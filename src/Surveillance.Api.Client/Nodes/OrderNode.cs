using Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.Client.Nodes
{
    public class OrderNode : Node
    {
        public OrderNode(NodeParent parent) : base(parent) { }

        public OrderNode FieldId() => AddField("id", this);
        public OrderNode FieldLimitPrice() => AddField("limitPrice", this);
    }
}
