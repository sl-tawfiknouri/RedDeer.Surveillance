namespace Surveillance.Auditing.DataLayer.Repositories
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Dapper;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.DataLayer.Interfaces;
    using Surveillance.Auditing.DataLayer.Repositories.Interfaces;

    public class MigrationRepository : IMigrationRepository
    {
        private const string HighestMigrationSql = @"SELECT Max(Id) FROM Migrations;";

        private const string MigrationFolder = "Migrations";

        private readonly IConnectionStringFactory _dbConnectionFactory;

        private readonly ILogger _logger;

        public MigrationRepository(
            IConnectionStringFactory connectionStringFactory,
            ILogger<ISystemProcessRepository> logger)
        {
            this._dbConnectionFactory = connectionStringFactory
                                        ?? throw new ArgumentNullException(nameof(connectionStringFactory));

            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public int LatestMigrationAvailable()
        {
            if (!Directory.Exists(this.MigrationFolders()))
            {
                this._logger.LogError(
                    $"MigrationRepository could not find the migration folder. {this.MigrationFolders()} Check application permissions.");
                return 0;
            }

            var entries = Directory.GetFiles(this.MigrationFolders()).Select(Path.GetFileName).ToList();

            if (!entries.Any())
            {
                this._logger.LogError(
                    "MigrationRepository could not find any files in the migration folder. Check application permissions.");
                return 0;
            }

            var availableMigrationIds =
                entries.Select(this.IndexPrefix).Select(ent => ent.GetValueOrDefault(0)).ToList();
            if (!availableMigrationIds.Any())
            {
                this._logger.LogInformation(
                    "MigrationRepository LatestMigrationAvailable had 0 available migrations. OK.");

                return 0;
            }

            var result = availableMigrationIds.Max();
            this._logger.LogInformation(
                $"MigrationRepository checked for most recent migration available and found {result}");
            return result;
        }

        public async Task<int> LatestMigrationVersion()
        {
            try
            {
                this._logger.LogTrace(
                    "MigrationRepository GetLatestMigrations about to fetch migration records from the database");
                using (var dbConnection = this._dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.ExecuteScalarAsync<int>(HighestMigrationSql))
                {
                    var highestMigrationExecuted = await conn;
                    this._logger.LogTrace(
                        $"MigrationRepository checking migrations found {highestMigrationExecuted} in the database");

                    return highestMigrationExecuted;
                }
            }
            catch (Exception e)
            {
                this._logger.LogError($"Error in migration repository get latest version - {e.Message}");
            }

            this._logger.LogError("MigrationRepository LatestMigrationVersion returning 0 on a bad code path");
            return 0;
        }

        public async Task UpdateMigrations()
        {
            var databaseSet = await this.LatestMigrationVersion();
            var migrationSet = this.LatestMigrationAvailable();

            if (databaseSet >= migrationSet) return;

            this._logger.LogInformation(
                $"Migration Repository believes that your database version is {databaseSet} whilst the latest migration script available is {migrationSet}. Beginning database migrations process.");

            var scriptIds = Enumerable.Range(databaseSet + 1, migrationSet - databaseSet).OrderBy(x => x).ToList();

            if (!Directory.Exists(this.MigrationFolders()))
            {
                this._logger.LogError(
                    "MigrationRepository could not find the migration folder. Check application permissions.");
                return;
            }

            var entries = Directory.GetFiles(this.MigrationFolders()).Select(Path.GetFileName).ToList();

            if (!entries.Any())
            {
                this._logger.LogError(
                    "MigrationRepository could not find any files in the migration folder. Check application permissions.");
                return;
            }

            var availableMigrationIds = entries
                .Select(
                    x => new PairFileNameAndScriptIndex
                             {
                                 FileName = x, ScriptIndex = this.IndexPrefix(x).GetValueOrDefault()
                             }).Where(i => scriptIds.Contains(i.ScriptIndex)).OrderBy(i => i.ScriptIndex).ToList();

            foreach (var migration in availableMigrationIds)
            {
                this._logger.LogInformation(
                    $"MigrationRepository UpdateMigrations about to run the migration {migration?.ScriptIndex} {migration?.FileName}");
                this.GetScriptAndExecute(migration).Wait();
                this._logger.LogInformation(
                    $"MigrationRepository UpdateMigrations has ran the migration {migration?.ScriptIndex} {migration?.FileName}");
            }
        }

        private async Task GetScriptAndExecute(PairFileNameAndScriptIndex name)
        {
            if (name == null)
            {
                this._logger.LogError("MigrationRepository passed a null name in GetScriptAndExecute for some reason.");
                return;
            }

            this._logger.LogInformation(
                $"MigrationRepository get script and execute for migration {name.ScriptIndex} {name.FileName}");

            try
            {
                var file = File.ReadAllText(Path.Combine(this.MigrationFolders(), name.FileName));

                using (var dbConnection = this._dbConnectionFactory.BuildConn())
                using (var conn = dbConnection.ExecuteAsync(file))
                {
                    await conn;
                    this._logger.LogInformation(
                        $"MigrationRepository get script and execute for migration {name?.FileName} has completed db operation");
                }
            }
            catch (Exception e)
            {
                this._logger.LogError(
                    $"MigrationRepository IMPORTANT something went wrong reading and writing the file {name.FileName}"
                    + e.Message);
            }
        }

        private int? IndexPrefix(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return null;

            var file = fileName.Split('.').SelectMany(sm => sm.Split(' ')).FirstOrDefault();
            var parseable = int.TryParse(file, out var result);

            if (!parseable)
            {
                this._logger.LogWarning("Migration Repository unparseable migration found in the migration folder!");
                return null;
            }

            return result;
        }

        private string MigrationFolders()
        {
            var overrideMigrationsFolder = this._dbConnectionFactory.OverrideMigrationsFolder();
            if (!string.IsNullOrWhiteSpace(overrideMigrationsFolder))
            {
                return overrideMigrationsFolder;
            }

            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), MigrationFolder);
        }

        protected class PairFileNameAndScriptIndex
        {
            public string FileName { get; set; }

            public int ScriptIndex { get; set; }
        }
    }
}