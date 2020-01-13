using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace RedDeer.Surveillance.IntegrationTests.Steps.Common
{
    public static class TableHelpers
    {
        public static IEnumerable<IDictionary<string, string>> ToEnumerableDictionary(Table table)
        {
            return table.Rows.Select(x => x.ToDictionary(y => y.Key, y => y.Value)).ToList();
        }
    }
}
