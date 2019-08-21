namespace RedDeer.Surveillance.Api.Client.Nodes
{
    using RedDeer.Surveillance.Api.Client.Infrastructure;

    public class MarketNode : Node<MarketNode>
    {
        public MarketNode(Parent parent)
            : base(parent)
        {
        }

        public MarketNode FieldId()
        {
            return this.AddField("id");
        }

        public MarketNode FieldMarketId()
        {
            return this.AddField("marketId");
        }

        public MarketNode FieldMarketName()
        {
            return this.AddField("marketName");
        }
    }
}