namespace RedDeer.Surveillance.Api.Client.Nodes
{
    using RedDeer.Surveillance.Api.Client.Infrastructure;

    public class StrategyNode : Node<StrategyNode>
    {
        public StrategyNode(Parent parent)
            : base(parent)
        {
        }

        public StrategyNode FieldName()
        {
            return this.AddField("name");
        }
    }
}