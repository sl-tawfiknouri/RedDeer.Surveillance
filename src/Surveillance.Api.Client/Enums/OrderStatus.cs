using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace RedDeer.Surveillance.Api.Client.Enums
{
    public enum OrderStatus
    {
        Unknown = 0,
        Placed = 1,
        Booked = 2,
        Amended = 3,
        Rejected = 4,
        Cancelled = 5,
        Filled = 6
    }
}
