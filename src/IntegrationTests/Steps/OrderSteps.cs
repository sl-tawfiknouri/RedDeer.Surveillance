using RedDeer.Surveillance.IntegrationTests.Runner;
using RedDeer.Surveillance.IntegrationTests.Steps.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace RedDeer.Surveillance.IntegrationTests.Steps
{
    [Binding]
    public sealed class OrderSteps
    {
        private readonly ScenarioContext _context;
        private readonly RuleRunner _ruleRunner;

        public OrderSteps(ScenarioContext context, RuleRunner ruleRunner)
        {
            _context = context;
            _ruleRunner = ruleRunner;
        }

        [Given(@"the orders")]
        public void GivenTheOrders(Table table)
        {
            var rows = TableHelpers.ToEnumerableDictionary(table);

            // expand special columns
            foreach (var row in rows)
            {
                if (row.ContainsKey("_Date"))
                {
                    var value = row["_Date"];
                    row.AddIfNotExists("OrderPlacedDate", value);
                    row.AddIfNotExists("OrderBookedDate", value);
                    row.AddIfNotExists("OrderAmendedDate", value);
                    row.AddIfNotExists("OrderFilledDate", value);
                }

                if (row.ContainsKey("_Volume"))
                {
                    var value = row["_Volume"];
                    row.AddIfNotExists("OrderOrderedVolume", value);
                    row.AddIfNotExists("OrderFilledVolume", value);
                }

                if (row.ContainsKey("_EquitySecurity"))
                {
                    var value = row["_EquitySecurity"];
                    var isin = IdentifierHelpers.ToIsinOrFigi(value);

                    row.AddIfNotExists("MarketIdentifierCode", "XLON");
                    row.AddIfNotExists("MarketType", "STOCKEXCHANGE");
                    row.AddIfNotExists("InstrumentCfi", "e");
                    row.AddIfNotExists("OrderCurrency", "GBP");
                    row.AddIfNotExists("OrderType", "MARKET");
                    row.AddIfNotExists("InstrumentIsin", isin);
                    row.AddIfNotExists("InstrumentFigi", isin);
                }
            }

            _ruleRunner.TradeCsvContent = MakeCsv(rows);
            _ruleRunner.ExpectedOrderCount = rows.Count();

            // default allocations
            var allocationRows = rows.Select(x => new Dictionary<string, string>
            {
                ["OrderId"] = x["OrderId"],
                ["Fund"] = "",
                ["Strategy"] = "",
                ["ClientAccountId"] = "",
                ["OrderFilledVolume"] = x.ValueOrNull("OrderFilledVolume")
            });

            _ruleRunner.AllocationCsvContent = MakeCsv(allocationRows);
            _ruleRunner.ExpectedAllocationCount = allocationRows.Count();
        }
        
        [Given(@"the fixed income orders")]
        public void GivenTheFixedIncomeOrders(Table table)
        {
            var rows = TableHelpers.ToEnumerableDictionary(table);

            // expand special columns
            foreach (var row in rows)
            {
                if (row.ContainsKey("_Date"))
                {
                    var value = row["_Date"];
                    row.AddIfNotExists("OrderPlacedDate", value);
                    row.AddIfNotExists("OrderBookedDate", value);
                    row.AddIfNotExists("OrderAmendedDate", value);
                    row.AddIfNotExists("OrderFilledDate", value);
                }

                if (row.ContainsKey("_Volume"))
                {
                    var value = row["_Volume"];
                    row.AddIfNotExists("OrderOrderedVolume", value);
                    row.AddIfNotExists("OrderFilledVolume", value);
                }

                if (row.ContainsKey("_EquitySecurity"))
                {
                    var value = row["_EquitySecurity"];
                    var isin = IdentifierHelpers.ToIsinOrFigi(value);

                    row.AddIfNotExists("MarketIdentifierCode", "RDFI");
                    row.AddIfNotExists("MarketType", "STOCKEXCHANGE");
                    row.AddIfNotExists("InstrumentCfi", "dc");
                    row.AddIfNotExists("OrderCurrency", "GBP");
                    row.AddIfNotExists("OrderType", "MARKET");
                    row.AddIfNotExists("InstrumentIsin", isin);
                    row.AddIfNotExists("InstrumentRic", "GB10YT=RR");
                }
            }

            _ruleRunner.TradeCsvContent = MakeCsv(rows);
            _ruleRunner.ExpectedOrderCount = rows.Count();

            // default allocations
            var allocationRows = rows.Select(x => new Dictionary<string, string>
            {
                ["OrderId"] = x["OrderId"],
                ["Fund"] = "",
                ["Strategy"] = "",
                ["ClientAccountId"] = "",
                ["OrderFilledVolume"] = x.ValueOrNull("OrderFilledVolume")
            });

            _ruleRunner.AllocationCsvContent = MakeCsv(allocationRows);
            _ruleRunner.ExpectedAllocationCount = allocationRows.Count();
        }

        private string MakeCsv(IEnumerable<IDictionary<string, string>> rows)
        {
            var keys = new HashSet<string>();
            foreach (var row in rows)
            {
                foreach (var key in row.Keys)
                {
                    if (key.First() != '_')
                    {
                        keys.Add(key);
                    }
                }
            }

            var lines = new List<string>();

            lines.Add(string.Join(",", keys));

            foreach (var row in rows)
            {
                var cells = new List<string>();
                foreach (var key in keys)
                {
                    if (row.ContainsKey(key))
                    {
                        cells.Add(row[key]);
                    }
                    else
                    {
                        cells.Add("");
                    }
                }

                lines.Add(string.Join(",", cells));
            }

            return string.Join("\n", lines);
        }
    }
}
