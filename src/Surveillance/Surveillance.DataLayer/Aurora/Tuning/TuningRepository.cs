using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Surveillance.Rules.Tuning;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Interfaces;

namespace Surveillance.DataLayer.Aurora.Tuning
{
    public class TuningRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<TuningRepository> _logger;

        private const string SaveTuning = @"INSERT IGNORE INTO RuleParameterTuning(ParameterRunId, BaseRunId, )";

        public TuningRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<TuningRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SaveTasks(IReadOnlyCollection<object> tuningRuns)
        {
            _logger?.LogInformation($"About to save parameter tuning task");



            _logger?.LogInformation($"Completed saving a parameter tuning run");
        }



        private class TuningDto
        {
            public TuningDto(TunedParameter<string> tunedParam, string ruleRunJson)
            {
                if (tunedParam == null)
                {
                    return;
                }

                BaseRunId = tunedParam.BaseId;
                ParameterTuningId = tunedParam.TuningParameterId;

                BaseValue = tunedParam.BaseValue;
                TunedValue = tunedParam.TunedValue;
                ParameterName = tunedParam.ParameterName;

                TuningDirection = (int)tunedParam.TuningDirection;
                TuningStrength = (int)tunedParam.TuningStrength;

                RuleRunJson = ruleRunJson;
            }

            public TuningDto()
            {
                // leave in situ for dapper
            }

            public string BaseRunId { get; set; }
            public string ParameterTuningId { get; set; }


            public string RuleRunJson { get; set; }

            public string BaseValue { get; set; }
            public string TunedValue { get; set; }
            public string ParameterName { get; set; }

            public int TuningDirection { get; set; }
            public int TuningStrength { get; set; }


        }
    }
}
