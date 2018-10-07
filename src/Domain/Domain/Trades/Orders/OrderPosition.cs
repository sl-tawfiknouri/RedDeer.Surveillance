using System.ComponentModel;

namespace Domain.Trades.Orders
{
    public enum OrderPosition
    {
        [Description("Buy")]
        Buy = 0, // enter position
        [Description("Sell")]
        Sell = 1, // sell long
    }
}