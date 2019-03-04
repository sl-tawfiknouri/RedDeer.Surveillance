using System;
using System.Collections.Generic;
using FakeItEasy;
using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;
using Surveillance.DataLayer.Api.ExchangeRate.Interfaces;

namespace Surveillance.Specflow.Tests.StepDefinitions.ExchangeRates
{
    public class ExchangeRateSelection
    {
        public ExchangeRateSelection()
        {
            if (ExchangeRateRepository == null)
            {
                ExchangeRateRepository = A.Fake<IExchangeRateApiCachingDecoratorRepository>();

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

                A.CallTo(() =>
                        ExchangeRateRepository.Get(A<DateTime>.Ignored, A<DateTime>.Ignored))
                    .Returns(new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>
                    {
                        { new DateTime(2018, 01, 01), new ExchangeRateDto[] { exchangeRateDto, exchangeRateDtoEur }}
                    });
            }
        }

        public IExchangeRateApiCachingDecoratorRepository ExchangeRateRepository { get; set; }

    }
}
