﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RedDeer.Contracts.SurveillanceService.Api.ExchangeRate;
using Surveillance.DataLayer.Api.ExchangeRate.Interfaces;
using Surveillance.DataLayer.Configuration.Interfaces;

namespace Surveillance.DataLayer.Api.ExchangeRate
{
    public class ExchangeRateApiRepository : BaseApiRepository, IExchangeRateApiRepository
    {
        private const string HeartbeatRoute = "api/exchangerates/heartbeat";
        private const string Route = "api/exchangerates/get/v1";
        private readonly ILogger _logger;

        public ExchangeRateApiRepository(
            IDataLayerConfiguration dataLayerConfiguration,
            ILogger<ExchangeRateApiRepository> logger)
            : base(dataLayerConfiguration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IDictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>> Get(DateTime commencement, DateTime termination)
        {
            var httpClient = BuildHttpClient();

            try
            {
                // US date format as that's default when deserialising on a UK machine as asp.net mvc core
                // If this starts breaking the culture of the machine the client service is on would be worth investigating
                // for posterity this url worked @ 29/09/2018 (http://localhost:8080/api/exchangerates/get/v1?commencement=09/27/2017&termination=09/26/2018)

                var routeWithQString = $"{Route}?commencement={commencement.ToString("MM/dd/yyyy")}&termination={termination.ToString("MM/dd/yyyy")}";

                var response = await httpClient.GetAsync(routeWithQString);

                if (response == null
                    || !response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Unsuccessful exchange rate api repository GET request. {response?.StatusCode}");

                    return new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var deserialisedResponse = JsonConvert.DeserializeObject<ExchangeRateDto[]>(jsonResponse);

                if (deserialisedResponse == null
                    || !deserialisedResponse.Any())
                {
                    return new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>();
                }

                var result = deserialisedResponse.GroupBy(dr => dr.DateTime).ToDictionary(i => i.Key, i => i.ToList() as IReadOnlyCollection<ExchangeRateDto>);

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return new Dictionary<DateTime, IReadOnlyCollection<ExchangeRateDto>>();
        }

        public async Task<bool> HeartBeating(CancellationToken token)
        {
            var httpClient = BuildHttpClient();

            var response = await httpClient.GetAsync(HeartbeatRoute, token);

            return response.IsSuccessStatusCode;
        }
    }
}