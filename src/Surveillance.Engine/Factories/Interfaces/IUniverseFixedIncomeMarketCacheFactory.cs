using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Engine.Rules.Factories.Interfaces
{
    public interface IUniverseFixedIncomeMarketCacheFactory
    {
        IUniverseFixedIncomeInterDayCache BuildInterday(RuleRunMode runMode);

        IUniverseFixedIncomeIntraDayCache BuildIntraday(TimeSpan window, RuleRunMode runMode);
    }
}
