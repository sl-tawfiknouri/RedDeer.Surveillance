namespace RedDeer.Surveillance.Api.Client.Nodes
{
    using RedDeer.Surveillance.Api.Client.Infrastructure;

    public class BrokerNode : Node<BrokerNode>
    {
        public BrokerNode(Parent parent)
            : base(parent)
        {
        }

        public BrokerNode FieldCreatedOn()
        {
            return this.AddField("createdOn");
        }

        public BrokerNode FieldExternalId()
        {
            return this.AddField("externalId");
        }

        public BrokerNode FieldId()
        {
            return this.AddField("id");
        }

        public BrokerNode FieldName()
        {
            return this.AddField("name");
        }
    }
}