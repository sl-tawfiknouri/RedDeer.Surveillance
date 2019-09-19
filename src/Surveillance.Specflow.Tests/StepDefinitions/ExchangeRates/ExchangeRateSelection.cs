namespace Surveillance.Specflow.Tests.StepDefinitions.ExchangeRates
{
    using System;
    using System.Collections.Generic;

    using FakeItEasy;

    using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;

    using Surveillance.Reddeer.ApiClient.ExchangeRate.Interfaces;

    public class ExchangeRateSelection
    {
        public ExchangeRateSelection()
        {
            if (this.ExchangeRateRepository == null)
            {
                this.ExchangeRateRepository = A.Fake<IExchangeRateApiCachingDecorator>();

                var exchangeRateDto = new ExchangeRateDto
                                          {
                                              DateTime = new DateTime(2018, 01, 01),
                                              Name = "GBX/USD",
                                              FixedCurrency = "GBX",
                                              VariableCurrency = "USD",
                                              Rate = 0.02d
                                          };

                var exchangeRateDtoEur = new ExchangeRateDto
                                             {
                                                 DateTime = new DateTime(2018, 01, 01),
                                                 Name = "GBX/EUR",
                                                 FixedCurrency = "GBX",
                                                 VariableCurrency = "EUR",
                                                 Rate = 0.015d
                                             };

                A.CallTo(() => this.ExchangeRateRepository.GetAsync(A<DateTime>.Ignored, A<DateTime>.Ignored)).Returns(
                    new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>
                        {
                            { new DateTime(2018, 01, 01), new[] { exchangeRateDto, exchangeRateDtoEur } }
                        });
            }
        }

        public IExchangeRateApiCachingDecorator ExchangeRateRepository { get; set; }
    }
}