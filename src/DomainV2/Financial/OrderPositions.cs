﻿using System.ComponentModel;

namespace DomainV2.Financial
{
    public enum OrderPositions
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