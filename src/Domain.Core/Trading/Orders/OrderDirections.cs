namespace Domain.Core.Trading.Orders
{
    using System.ComponentModel;

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