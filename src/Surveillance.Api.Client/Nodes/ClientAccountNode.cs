namespace RedDeer.Surveillance.Api.Client.Nodes
{
    using RedDeer.Surveillance.Api.Client.Infrastructure;

    public class ClientAccountNode : Node<ClientAccountNode>
    {
        public ClientAccountNode(Parent parent)
            : base(parent)
        {
        }

        public ClientAccountNode FieldId()
        {
            return this.AddField("id");
        }
    }
}