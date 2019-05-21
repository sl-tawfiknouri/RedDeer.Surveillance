using Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.Client.Nodes
{
    public class TraderNode : Node<TraderNode>
    {
        public TraderNode(Parent parent) : base(parent) { }

        public TraderNode FieldId() => AddField("id");
        public TraderNode FieldName() => AddField("name");
    }
}
