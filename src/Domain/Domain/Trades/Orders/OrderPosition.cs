using System.ComponentModel;

namespace Domain.Trades.Orders
{
    public enum OrderPosition
    {
        [Description("Buy")]
        BuyLong, // enter position
        [Description("Sell")]
        SellLong, // sell long
        [Description("Buy Short")]
        BuyShort, // enter position
        [Description("Sell Short")]
        SellShort // sell short
    }
}