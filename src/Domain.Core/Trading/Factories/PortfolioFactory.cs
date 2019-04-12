﻿using Domain.Core.Trading.Factories.Interfaces;
using Domain.Core.Trading.Interfaces;

namespace Domain.Core.Trading.Factories
{
    public class PortfolioFactory : IPortfolioFactory
    {
        public IPortfolio Build()
        {
            return new Portfolio(
                new OrderLedger());
        }
    }
}