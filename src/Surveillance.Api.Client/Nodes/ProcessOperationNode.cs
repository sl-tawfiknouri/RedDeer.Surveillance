namespace RedDeer.Surveillance.Api.Client.Nodes
{
    using RedDeer.Surveillance.Api.Client.Infrastructure;

    public class ProcessOperationNode : Node<ProcessOperationNode>
    {
        public ProcessOperationNode(Parent parent)
            : base(parent)
        {
        }

        public ProcessOperationNode FieldId()
        {
            return this.AddField("id");
        }

        public ProcessOperationNode FieldOperationEnd()
        {
            return this.AddField("operationEnd");
        }

        public ProcessOperationNode FieldOperationStart()
        {
            return this.AddField("operationStart");
        }

        public ProcessOperationNode FieldOperationState()
        {
            return this.AddField("operationState");
        }
    }
}