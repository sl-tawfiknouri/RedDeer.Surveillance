using Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.Client.Nodes
{
    public class MarketNode : Node<MarketNode>
    {
        public MarketNode(NodeParent parent) : base(parent) { }

        public MarketNode FieldId() => AddField("id");
        public MarketNode FieldMarketId() => AddField("marketId");
        public MarketNode FieldMarketName() => AddField("marketName");
    }
}
