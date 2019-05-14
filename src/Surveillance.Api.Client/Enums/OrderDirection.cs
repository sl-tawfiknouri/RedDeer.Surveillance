using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Surveillance.Api.Client.Enums
{
    public enum OrderDirection
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
