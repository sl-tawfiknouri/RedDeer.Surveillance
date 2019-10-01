namespace Surveillance.Specflow.Tests.StepDefinitions.ExchangeRates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FakeItEasy;

    using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;

    using Surveillance.Reddeer.ApiClient.ExchangeRate.Interfaces;

    using TechTalk.SpecFlow;
    using TechTalk.SpecFlow.Assist;

    [Binding]
    public sealed class ExchangeRateSteps
    {
        private readonly ScenarioContext _ctx;

        private readonly ExchangeRateSelection _exchangeRateSelection;

        public ExchangeRateSteps(ScenarioContext ctx, ExchangeRateSelection exchangeRateSelection)
        {
            this._ctx = ctx;
            this._exchangeRateSelection = exchangeRateSelection;
        }

        [Given(@"I have the exchange rates :")]
        public void GivenIHaveTheExchangeRateTable(Table ruleParameters)
        {
            if (ruleParameters == null)
            {
                this._ctx.Pending();
                return;
            }

            this._exchangeRateSelection.ExchangeRateRepository = A.Fake<IExchangeRateApiCachingDecorator>();
            var parameters = ruleParameters.CreateSet<ExchangeRateApiParameters>();
            var groupedParams = parameters.GroupBy(i => i.Date.Date);

            var exchangeRateDtos = new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>();
            foreach (var grp in groupedParams)
            {
                var dtos = grp.Select(
                    i => new ExchangeRateDto
                             {
                                 DateTime = grp.Key,
                                 FixedCurrency = i.Fixed,
                                 Name = i.Name,
                                 Rate = i.Rate,
                                 VariableCurrency = i.Variable
                             }).ToArray();

                exchangeRateDtos.Add(grp.Key, dtos);
            }

            A.CallTo(
                    () => this._exchangeRateSelection.ExchangeRateRepository.GetAsync(
                        A<DateTime>.Ignored,
                        A<DateTime>.Ignored))
                .Returns(exchangeRateDtos);
        }
    }
}