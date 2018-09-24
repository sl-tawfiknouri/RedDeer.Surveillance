using System.ComponentModel;

namespace Domain.Trades.Orders
{
    public enum OrderPosition
    {
        [Description("Buy")]
        Buy, // enter position
        [Description("Sell")]
        Sell, // sell long
    }
}