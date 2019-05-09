using Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.Client.Nodes
{
    public class MarketNode : Node
    {
        public MarketNode(NodeParent parent) : base(parent) { }

        public MarketNode FieldId() => AddField("id", this);
        public MarketNode FieldMarketId() => AddField("marketId", this);
        public MarketNode FieldMarketName() => AddField("marketName", this);
    }
}
