using System.ComponentModel;

namespace Domain.Core.Trading.Orders
{
    public enum OrderDirections
    {
        [Description("None")]
        NONE,
        [Description("Buy")]
        BUY,
        [Description("Sell")]
        SELL,
        [Description("Short")]
        SHORT,
        [Description("Cover")]
        COVER
    }
}
