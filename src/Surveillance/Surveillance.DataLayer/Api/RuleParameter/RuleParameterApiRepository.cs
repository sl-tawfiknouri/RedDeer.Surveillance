﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.DataLayer.Api.RuleParameter.Interfaces;
using Surveillance.DataLayer.Configuration.Interfaces;

namespace Surveillance.DataLayer.Api.RuleParameter
{
    public class RuleParameterApiRepository : BaseApiRepository, IRuleParameterApiRepository
    {
        private const string Route = "api/surveillanceruleparameter/get/v1";
        private readonly ILogger _logger;

        public RuleParameterApiRepository(
            IDataLayerConfiguration dataLayerConfiguration,
            ILogger<RuleParameterApiRepository> logger) 
            : base(dataLayerConfiguration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<RuleParameterDto> Get()
        {
            var httpClient = BuildHttpClient();

            try
            {
                var response = await httpClient.GetAsync(Route);

                if (response == null
                    || !response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Unsuccessful rule parameter api repository GET request. {response?.StatusCode}");

                    return new RuleParameterDto();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var deserialisedResponse = JsonConvert.DeserializeObject<RuleParameterDto>(jsonResponse);

                return deserialisedResponse ?? new RuleParameterDto();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return new RuleParameterDto();
        }
    }
}
