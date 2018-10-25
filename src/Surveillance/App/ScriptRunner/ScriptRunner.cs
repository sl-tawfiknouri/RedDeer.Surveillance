using RedDeer.Surveillance.App.ScriptRunner.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace RedDeer.Surveillance.App.ScriptRunner
{
    public class ScriptRunner : IScriptRunner
    {
        public ScriptRunner(IMigrationRepository migrationRepository)
        {
            var migration = migrationRepository.UpdateMigrations();
            migration.Wait();
        }
    }
}