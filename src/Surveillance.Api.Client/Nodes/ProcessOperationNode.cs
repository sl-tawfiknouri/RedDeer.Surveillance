using Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.Client.Nodes
{
    public class ProcessOperationNode : Node<ProcessOperationNode>
    {
        public ProcessOperationNode(NodeParent parent) : base(parent) { }

        public ProcessOperationNode FieldId() => AddField("id");
        public ProcessOperationNode FieldOperationStart() => AddField("start");
        public ProcessOperationNode FieldOperationEnd() => AddField("end");
        public ProcessOperationNode FieldOperationState() => AddField("operationState");
    }
}
