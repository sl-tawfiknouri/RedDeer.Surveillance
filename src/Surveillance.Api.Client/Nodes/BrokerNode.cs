using RedDeer.Surveillance.Api.Client.Infrastructure;

namespace RedDeer.Surveillance.Api.Client.Nodes
{
    public class BrokerNode : Node<BrokerNode>
    {
        public BrokerNode(Parent parent) : base(parent) { }

        public BrokerNode FieldId() => AddField("id");
        public BrokerNode FieldExternalId() => AddField("externalId");
        public BrokerNode FieldName() => AddField("name");
        public BrokerNode FieldCreatedOn() => AddField("createdOn");
    }
}
