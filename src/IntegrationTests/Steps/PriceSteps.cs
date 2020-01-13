using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;
using RedDeer.Surveillance.IntegrationTests.Runner;
using RedDeer.Surveillance.IntegrationTests.Steps.Common;
using System;
using System.Globalization;
using Firefly.Service.Data.TickPriceHistory.Shared.Protos;
using Google.Protobuf.WellKnownTypes;
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
                    Currency = "GBP",
                    DailyVolume = row.ValueOrNull("DailyVolume") != null ? Convert.ToInt64(row.ValueOrNull("DailyVolume")) : 0
                };

                _ruleRunner.EquityClosePriceMock.Add(item);
            }
        }

        [Given(@"the fixed income close prices")]
        public void GivenTheFixedIncomeClosePrices(Table table)
        {
            var rows = TableHelpers.ToEnumerableDictionary(table);

            foreach (var row in rows)
            {
                var item = new SecurityTimeBarQuerySubResponse
                {
                    Identifiers = new SecurityIdentifiers
                    {
                        Ric = row["_FixedIncomeSecurity"],
                        Isin = row["_FixedIncomeSecurity"],
                        Sedol = row["_FixedIncomeSecurity"],
                        ExternalIdentifiers = row["_FixedIncomeSecurity"]
                    },
                    Timebars =
                    {
                        new TimeBar
                        {
                            CloseAsk = double.Parse(row["ClosePrice"]),
                            CloseBid = double.Parse(row["ClosePrice"]),
                            CurrencyCode = "GBP",
                            EpochUtc = DateTime.Parse(row["Date"]).ToUniversalTime().ToTimestamp()
                        }
                    }
                };
                
                _ruleRunner.FixedIncomeClosePriceMock.Add(item);
            }
        }
    }
}