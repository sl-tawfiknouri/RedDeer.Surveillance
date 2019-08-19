namespace RedDeer.Surveillance.Api.Client.Nodes
{
    using RedDeer.Surveillance.Api.Client.Infrastructure;

    public class FundNode : Node<FundNode>
    {
        public FundNode(Parent parent)
            : base(parent)
        {
        }

        public FundNode FieldName()
        {
            return this.AddField("name");
        }
    }
}