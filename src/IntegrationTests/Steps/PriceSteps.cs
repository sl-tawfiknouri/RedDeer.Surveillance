using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;
using RedDeer.Surveillance.IntegrationTests.Runner;
using RedDeer.Surveillance.IntegrationTests.Steps.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace RedDeer.Surveillance.IntegrationTests.Steps
{
    [Binding]
    public sealed class PriceSteps
    {
        private readonly ScenarioContext _context;
        private readonly RuleRunner _ruleRunner;

        public PriceSteps(ScenarioContext context, RuleRunner ruleRunner)
        {
            _context = context;
            _ruleRunner = ruleRunner;
        }

        [Given(@"the equity close prices")]
        public void GivenTheEquityClosePrices(Table table)
        {
            var rows = TableHelpers.ToEnumerableDictionary(table);

            foreach (var row in rows)
            {
                var item = new FactsetSecurityDailyResponseItem
                {
                    Epoch = DateTime.Parse(row["Date"], null, DateTimeStyles.AssumeUniversal),
                    ClosePrice = Convert.ToDecimal(row["ClosePrice"]),
                    Figi = IdentifierHelpers.ToIsinOrFigi(row["_EquitySecurity"]),
                    Currency = "GBP"
                };

                _ruleRunner.EquityClosePriceMock.Add(item);
            }
        }

    }
}
