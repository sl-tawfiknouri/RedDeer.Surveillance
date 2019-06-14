using RedDeer.Surveillance.Api.Client.Infrastructure;

namespace RedDeer.Surveillance.Api.Client.Nodes
{
    public class ClientAccountNode : Node<ClientAccountNode>
    {
        public ClientAccountNode(Parent parent) : base(parent) { }

        public ClientAccountNode FieldId() => AddField("id");
    }
}
