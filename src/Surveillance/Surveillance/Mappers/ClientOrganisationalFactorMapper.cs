using System;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.Mappers.Interfaces;
using Surveillance.RuleParameters.OrganisationalFactors;

namespace Surveillance.Mappers
{
    public class ClientOrganisationalFactorMapper : IClientOrganisationalFactorMapper
    {
        private readonly ILogger<ClientOrganisationalFactorMapper> _logger;

        public ClientOrganisationalFactorMapper(ILogger<ClientOrganisationalFactorMapper> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ClientOrganisationalFactors Map(OrganisationalFactors factor)
        {
            switch (factor)
            {
                case OrganisationalFactors.Unknown:
                    return ClientOrganisationalFactors.Unknown;
                case OrganisationalFactors.None:
                    return ClientOrganisationalFactors.None;
                case OrganisationalFactors.Trader:
                    return ClientOrganisationalFactors.Trader;
                case OrganisationalFactors.PortfolioManager:
                    return ClientOrganisationalFactors.PortfolioManager;
                case OrganisationalFactors.Fund:
                    return ClientOrganisationalFactors.Fund;
                case OrganisationalFactors.Strategy:
                    return ClientOrganisationalFactors.Strategy;
                default:
                    _logger.LogWarning($"ClientOrganisationalFactorMapper received argument of {factor} and could not map it to an existing entry.");
                    return ClientOrganisationalFactors.Unknown;
            }
        }
    }
}
