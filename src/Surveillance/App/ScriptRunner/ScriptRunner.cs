namespace RedDeer.Surveillance.App.ScriptRunner
{
    using global::Surveillance.Auditing.DataLayer.Repositories.Interfaces;

    using Microsoft.Extensions.Logging;

    using RedDeer.Surveillance.App.ScriptRunner.Interfaces;

    public class ScriptRunner : IScriptRunner
    {
        public ScriptRunner(IMigrationRepository migrationRepository, ILogger<ScriptRunner> logger)
        {
            logger.LogInformation("ScriptRunner initiating update migrations");
            var migration = migrationRepository.UpdateMigrations();
            migration.Wait();
            logger.LogInformation("ScriptRunner completed update migrations");
        }
    }
}