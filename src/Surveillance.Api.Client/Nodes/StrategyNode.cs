using RedDeer.Surveillance.Api.Client.Infrastructure;

namespace RedDeer.Surveillance.Api.Client.Nodes
{
    public class StrategyNode : Node<StrategyNode>
    {
        public StrategyNode(Parent parent) : base(parent) { }

        public StrategyNode FieldName() => AddField("name");
    }
}
