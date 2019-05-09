using Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.Client.Nodes
{
    public class TraderNode : Node
    {
        public TraderNode(NodeParent parent) : base(parent) { }

        public TraderNode FieldId() => AddField("id", this);
        public TraderNode FieldName() => AddField("name", this);
    }
}
