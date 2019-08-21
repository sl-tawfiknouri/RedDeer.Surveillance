namespace RedDeer.Surveillance.Api.Client.Nodes
{
    using RedDeer.Surveillance.Api.Client.Infrastructure;

    public class TraderNode : Node<TraderNode>
    {
        public TraderNode(Parent parent)
            : base(parent)
        {
        }

        public TraderNode FieldId()
        {
            return this.AddField("id");
        }

        public TraderNode FieldName()
        {
            return this.AddField("name");
        }
    }
}