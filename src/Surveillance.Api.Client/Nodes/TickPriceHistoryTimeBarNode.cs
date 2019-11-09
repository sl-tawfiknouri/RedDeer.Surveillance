using RedDeer.Surveillance.Api.Client.Infrastructure;

namespace RedDeer.Surveillance.Api.Client.Nodes
{
    public class TickPriceHistoryTimeBarNode 
        : Node<TickPriceHistoryTimeBarNode>
    {
        public TickPriceHistoryTimeBarNode(Parent parent)
            : base(parent)
        {
        }

        public TickPriceHistoryTimeBarNode FieldRic() 
            => this.AddField("ric");

        public TickPriceHistoryTimeBarNode FieldEpochUtc()
            => this.AddField("epochUtc");

        public TickPriceHistoryTimeBarNode FieldCurrencyCode()
            => this.AddField("currencyCode");

        public TickPriceHistoryTimeBarNode FieldClose()
            => this.AddField("close");

        public TickPriceHistoryTimeBarNode FieldCloseAsk()
            => this.AddField("closeAsk");

        public TickPriceHistoryTimeBarNode FieldHigh()
            => this.AddField("high");

        public TickPriceHistoryTimeBarNode FieldLow()
            => this.AddField("low");

        public TickPriceHistoryTimeBarNode FieldHighAsk()
            => this.AddField("highAsk");
    }
}
