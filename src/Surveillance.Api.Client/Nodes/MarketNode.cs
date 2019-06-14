using RedDeer.Surveillance.Api.Client.Infrastructure;

namespace RedDeer.Surveillance.Api.Client.Nodes
{
    public class MarketNode : Node<MarketNode>
    {
        public MarketNode(Parent parent) : base(parent) { }

        public MarketNode FieldId() => AddField("id");
        public MarketNode FieldMarketId() => AddField("marketId");
        public MarketNode FieldMarketName() => AddField("marketName");
    }
}
