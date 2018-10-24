using System;
using System.Threading.Tasks;
using RedDeer.Surveillance.App.ScriptRunner.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace RedDeer.Surveillance.App.ScriptRunner
{
    public class ScriptRunner : IScriptRunner
    {
        private readonly IMigrationRepository _migrationRepository;

        public ScriptRunner(IMigrationRepository migrationRepository)
        {
            _migrationRepository = migrationRepository ?? throw new ArgumentNullException(nameof(migrationRepository));
        }

        public async Task Run()
        {
            await _migrationRepository.UpdateMigrations();
        }
    }
}
