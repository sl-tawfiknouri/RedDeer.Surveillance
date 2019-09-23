namespace Surveillance.DataLayer.Aurora.Tuning
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Dapper;

    using Domain.Surveillance.Rules.Tuning;

    using Microsoft.Extensions.Logging;

    using Surveillance.DataLayer.Aurora.Interfaces;
    using Surveillance.DataLayer.Aurora.Tuning.Interfaces;

    public class TuningRepository : ITuningRepository
    {
        private const string SaveTuning =
            @"INSERT IGNORE INTO RuleParameterTuning(BaseRunId, ParameterTuningId, RuleRunJson, BaseValue, TunedValue, ParameterName, TuningDirection, TuningStrength) VALUES(@BaseRunId, @ParameterTuningId, @RuleRunJson, @BaseValue, @TunedValue, @ParameterName, @TuningDirection, @TuningStrength);";

        private readonly IConnectionStringFactory _dbConnectionFactory;

        private readonly ILogger<TuningRepository> _logger;

        public TuningRepository(IConnectionStringFactory dbConnectionFactory, ILogger<TuningRepository> logger)
        {
            this._dbConnectionFactory =
                dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SaveTasks(IReadOnlyCollection<TuningPair> tuningRuns)
        {
            this._logger?.LogInformation("About to save parameter tuning task");

            tuningRuns = tuningRuns?.Where(_ => _ != null).ToList();

            if (tuningRuns == null || !tuningRuns.Any()) return;

            var dtos = tuningRuns.Select(_ => new TuningDto(_.TunedParam, _.TunedJson)).ToList();

            try
            {
                this._logger?.LogInformation(
                    $"SaveTasks opened db connection and about to save {tuningRuns.Count} tuning runs");
                using (var dbConnection = this._dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.ExecuteAsync(SaveTuning, dtos))
                {
                    await conn;
                    this._logger?.LogInformation($"SaveTasks saved {tuningRuns.Count} tuning runs");
                }
            }
            catch (Exception e)
            {
                this._logger?.LogError(
                    $"exception in save tasks for tuning runs {e.Message} {e?.InnerException?.Message}");
            }

            this._logger?.LogInformation("Completed saving a parameter tuning run");
        }

        public class TuningPair
        {
            public TuningPair(TunedParameter<string> tunedParam, string tunedJson)
            {
                this.TunedParam = tunedParam;
                this.TunedJson = tunedJson;
            }

            public string TunedJson { get; set; }

            public TunedParameter<string> TunedParam { get; set; }
        }

        private class TuningDto
        {
            public TuningDto(TunedParameter<string> tunedParam, string ruleRunJson)
            {
                if (tunedParam == null) return;

                this.BaseRunId = tunedParam.BaseId;
                this.ParameterTuningId = tunedParam.TuningParameterId;

                this.BaseValue = tunedParam.BaseValue;
                this.TunedValue = tunedParam.TunedValue;
                this.ParameterName = tunedParam.ParameterName;

                this.TuningDirection = (int)tunedParam.TuningDirection;
                this.TuningStrength = (int)tunedParam.TuningStrength;

                this.RuleRunJson = ruleRunJson;
            }

            public TuningDto()
            {
                // leave in situ for dapper
            }

            public string BaseRunId { get; }

            public string BaseValue { get; }

            public string ParameterName { get; }

            public string ParameterTuningId { get; }

            public string RuleRunJson { get; }

            public string TunedValue { get; }

            public int TuningDirection { get; }

            public int TuningStrength { get; }
        }
    }
}