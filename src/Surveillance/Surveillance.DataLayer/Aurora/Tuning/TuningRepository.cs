using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Domain.Surveillance.Rules.Tuning;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Interfaces;
using Surveillance.DataLayer.Aurora.Tuning.Interfaces;

namespace Surveillance.DataLayer.Aurora.Tuning
{
    public class TuningRepository : ITuningRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger<TuningRepository> _logger;

        private const string SaveTuning = @"INSERT IGNORE INTO RuleParameterTuning(BaseRunId, ParameterTuningId, RuleRunJson, BaseValue, TunedValue, ParameterName, TuningDirection, TuningStrength) VALUES(@BaseRunId, @ParameterTuningId, @RuleRunJson, @BaseValue, @TunedValue, @ParameterName, @TuningDirection, @TuningStrength);";

        public TuningRepository(
            IConnectionStringFactory dbConnectionFactory,
            ILogger<TuningRepository> logger)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SaveTasks(IReadOnlyCollection<TuningPair> tuningRuns)
        {
            _logger?.LogInformation($"About to save parameter tuning task");

            tuningRuns = tuningRuns?.Where(_ => _ != null).ToList();

            if (tuningRuns == null
                || !tuningRuns.Any())
            {
                return;
            }

            var dtos = tuningRuns.Select(_ => new TuningDto(_.TunedParam, _.TunedJson)).ToList();
            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();
                _logger?.LogInformation($"SaveTasks opened db connection and about to save {tuningRuns.Count} tuning runs");

                using (var conn = dbConnection.ExecuteAsync(SaveTuning, dtos))
                {
                    await conn;
                    _logger?.LogInformation($"SaveTasks saved {tuningRuns.Count} tuning runs");
                }
            }
            catch (Exception e)
            {
                _logger?.LogError($"exception in save tasks for tuning runs {e.Message} {e?.InnerException?.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }

            _logger?.LogInformation($"Completed saving a parameter tuning run");
        }

        public class TuningPair
        {
            public TuningPair(TunedParameter<string> tunedParam, string tunedJson)
            {
                TunedParam = tunedParam;
                TunedJson = tunedJson;
            }

            public TunedParameter<string> TunedParam { get; set; }
            public string TunedJson { get; set; }
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
