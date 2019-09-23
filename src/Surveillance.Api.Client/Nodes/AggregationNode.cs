namespace RedDeer.Surveillance.Api.Client.Nodes
{
    using RedDeer.Surveillance.Api.Client.Infrastructure;

    public class AggregationNode : Node<AggregationNode>
    {
        public AggregationNode(Parent parent)
            : base(parent)
        {
        }

        public AggregationNode FieldCount()
        {
            return this.AddField("count");
        }

        public AggregationNode FieldKey()
        {
            return this.AddField("key");
        }
    }
}