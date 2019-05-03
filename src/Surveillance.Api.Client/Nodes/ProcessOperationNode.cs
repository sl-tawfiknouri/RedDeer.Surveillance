using Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.Client.Nodes
{
    public class ProcessOperationNode : Node
    {
        public ProcessOperationNode(NodeParent parent) : base(parent) { }

        public ProcessOperationNode FieldId() => AddField("id", this);
        public ProcessOperationNode FieldOperationStart() => AddField("start", this);
        public ProcessOperationNode FieldOperationEnd() => AddField("end", this);
        public ProcessOperationNode FieldOperationState() => AddField("operationState", this);
    }
}
