using RedDeer.Surveillance.Api.Client.Infrastructure;

namespace RedDeer.Surveillance.Api.Client.Nodes
{
    public class ProcessOperationNode : Node<ProcessOperationNode>
    {
        public ProcessOperationNode(Parent parent) : base(parent) { }

        public ProcessOperationNode FieldId() => AddField("id");
        public ProcessOperationNode FieldOperationStart() => AddField("operationStart");
        public ProcessOperationNode FieldOperationEnd() => AddField("operationEnd");
        public ProcessOperationNode FieldOperationState() => AddField("operationState");
    }
}
