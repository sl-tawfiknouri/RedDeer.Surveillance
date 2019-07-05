using System;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Interfaces;

namespace Surveillance.DataLayer.Aurora.Judgements
{
    public class JudgementRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<JudgementRepository> _logger;

        public JudgementRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<JudgementRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }





    }
}
