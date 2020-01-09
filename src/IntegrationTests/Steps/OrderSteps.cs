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
                    var identifier = IdentifierHelpers.ToIsinOrFigi(value);
                    
                    row.AddIfNotExists("MarketType", "STOCKEXCHANGE");
                    row.AddIfNotExists("OrderCurrency", "GBP");
                    row.AddIfNotExists("OrderType", "MARKET");
                    row.AddIfNotExists("InstrumentIsin", identifier);
                    row.AddIfNotExists("InstrumentFigi", identifier);
                    row.AddIfNotExists("MarketIdentifierCode", "XLON");
                    row.AddIfNotExists("InstrumentCfi", "e");
                }

                if (row.ContainsKey("_FixedIncomeSecurity"))
                {
                    var value = row["_FixedIncomeSecurity"];
                    
                    row.AddIfNotExists("MarketType", "STOCKEXCHANGE");
                    row.AddIfNotExists("OrderCurrency", "GBP");
                    row.AddIfNotExists("OrderType", "MARKET");
                    row.AddIfNotExists("InstrumentRic", value);
                    row.AddIfNotExists("MarketIdentifierCode", "RDFI");
                    row.AddIfNotExists("MarketName", "RDFI");
                    row.AddIfNotExists("InstrumentCfi", "d");
                    
                    var identifier = IdentifierHelpers.ToIsinOrFigi(value);
                    row.AddIfNotExists("InstrumentIsin", identifier);
                }
            }

            _ruleRunner.TradeCsvContent = MakeCsv(rows);

            // default allocations
            if (_ruleRunner.AllocationCsvContent == null)
            {
                var allocationRows = rows.Select(x => new Dictionary<string, string>
                {
                    ["OrderId"] = x["OrderId"],
                    ["Fund"] = "",
                    ["Strategy"] = "",
                    ["ClientAccountId"] = "",
                    ["OrderFilledVolume"] = x.ValueOrNull("OrderFilledVolume")
                });

                _ruleRunner.AllocationCsvContent = MakeCsv(allocationRows);
            }
        }

        [Given(@"the allocations")]
        public void GivenTheAllocations(Table table)
        {
            var rows = TableHelpers.ToEnumerableDictionary(table);
            _ruleRunner.AllocationCsvContent = MakeCsv(rows);
        }

        [When(@"the data importer is run")]
        public void WhenTheDataImporterIsRun()
        {
            _ruleRunner.RunDataImport();
        }

        [When(@"the auto scheduler is run")]
        public void WhenTheAutoSchedulerIsRun()
        {
            _ruleRunner.RunAutoScheduler();
        }

        [Then(@"there should be a single order with id ""(.*)"" and autoscheduled ""(.*)""")]
        public void ThenThereShouldBeASingleOrderWithIdAndAutoscheduled(string id, bool autoscheduled)
        {
            using (var dbContext = _ruleRunner.BuildDbContext())
            {
                var orders = dbContext
                    .Orders
                    .ToList();

                if (orders.Count > 1)
                {
                    throw new Exception("There are more than 1 orders in the database");
                }

                var order = orders.Where(x => x.ClientOrderId == id && x.Autoscheduled == autoscheduled).SingleOrDefault();

                if (order == null)
                {
                    throw new Exception($"Could not find order with client id \"{id}\" and autoscheduled \"{autoscheduled}\"");
                }
            }
        }

        [Then(@"there should be a single allocation with OrderId ""(.*)"" and autoscheduled ""(.*)""")]
        public void ThenThereShouldBeASingleAllocationWithOrderIdAndAutoscheduled(string orderId, bool autoscheduled)
        {
            using (var dbContext = _ruleRunner.BuildDbContext())
            {
                var allocations = dbContext
                    .OrdersAllocation
                    .ToList();

                if (allocations.Count > 1)
                {
                    throw new Exception("There are more than 1 allocations in the database");
                }

                var allocation = allocations.Where(x => x.OrderId == orderId && x.AutoScheduled == autoscheduled).SingleOrDefault();

                if (allocation == null)
                {
                    throw new Exception($"Could not find allocation with order id \"{orderId}\" and autoscheduled \"{autoscheduled}\"");
                }
            }
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
