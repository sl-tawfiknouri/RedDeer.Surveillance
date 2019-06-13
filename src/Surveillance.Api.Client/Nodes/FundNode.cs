using RedDeer.Surveillance.Api.Client.Infrastructure;

namespace RedDeer.Surveillance.Api.Client.Nodes
{
    public class FundNode : Node<FundNode>
    {
        public FundNode(Parent parent) : base(parent) { }

        public FundNode FieldName() => AddField("name");
    }
}
