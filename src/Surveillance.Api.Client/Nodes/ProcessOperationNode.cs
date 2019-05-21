using RedDeer.Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedDeer.Surveillance.Api.Client.Nodes
{
    public class ProcessOperationNode : Node<ProcessOperationNode>
    {
        public ProcessOperationNode(Parent parent) : base(parent) { }

        public ProcessOperationNode FieldId() => AddField("id");
        public ProcessOperationNode FieldOperationStart() => AddField("start");
        public ProcessOperationNode FieldOperationEnd() => AddField("end");
        public ProcessOperationNode FieldOperationState() => AddField("operationState");
    }
}
