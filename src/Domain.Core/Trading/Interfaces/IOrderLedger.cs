﻿using System.Collections.Generic;
using Domain.Trading;

namespace Domain.Core.Trading
{
    public interface IOrderLedger
    {
        void Add(Order order);
        IReadOnlyCollection<Order> FullLedger();
    }
}