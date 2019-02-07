using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Surveillance.Systems.DataLayer.Interfaces;
using Surveillance.Systems.DataLayer.Repositories.Interfaces;

namespace Surveillance.Systems.DataLayer.Repositories
{
    public class MigrationRepository : IMigrationRepository
    {
        private readonly IConnectionStringFactory _dbConnectionFactory;
        private readonly ILogger _logger;
        private const string MigrationFolder = "Migrations";

        private string MigrationFolders()
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), MigrationFolder);
        }

        private const string HighestMigrationSql =
            @"SELECT Max(Id) FROM Migrations;";

        public MigrationRepository(
            IConnectionStringFactory connectionStringFactory,
            ILogger<ISystemProcessRepository> logger)
        {
            _dbConnectionFactory =
                connectionStringFactory
                ?? throw new ArgumentNullException(nameof(connectionStringFactory));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> LatestMigrationVersion()
        {
            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                dbConnection.Open();

                _logger.LogTrace($"MigrationRepository GetLatestMigrations about to fetch migration records from the database");
                using (var conn = dbConnection.ExecuteScalarAsync<int>(HighestMigrationSql))
                {
                    var highestMigrationExecuted = await conn;
                    _logger.LogTrace($"MigrationRepository checking migrations found {highestMigrationExecuted} in the database");

                    return highestMigrationExecuted;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error in migration repository get latest version - {e.Message}");
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }

            _logger.LogError($"MigrationRepository LatestMigrationVersion returning 0 on a bad code path");
            return 0;
        }

        public int LatestMigrationAvailable()
        {
            if (!Directory.Exists(MigrationFolders()))
            {
                _logger.LogError($"MigrationRepository could not find the migration folder. {MigrationFolders()} Check application permissions.");
                return 0;
            }

            var entries = Directory.GetFiles(MigrationFolders()).Select(Path.GetFileName).ToList();

            if (!entries.Any())
            {
                _logger.LogError("MigrationRepository could not find any files in the migration folder. Check application permissions.");
                return 0;
            }

            var availableMigrationIds = entries.Select(IndexPrefix).Select(ent => ent.GetValueOrDefault(0)).ToList();
            if (!availableMigrationIds.Any())
            {
                _logger.LogInformation("MigrationRepository LatestMigrationAvailable had 0 available migrations. OK.");

                return 0;
            }

            var result = availableMigrationIds.Max();
            _logger.LogInformation($"MigrationRepository checked for most recent migration available and found {result}");
            return result;
        }

        private int? IndexPrefix(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }

            var file = fileName.Split('.').SelectMany(sm => sm.Split(' ')).FirstOrDefault();
            var parseable = int.TryParse(file, out var result);

            if (!parseable)
            {
                _logger.LogWarning("Migration Repository unparseable migration found in the migration folder!");
                return null;
            }

            return result;
        }

        public async Task UpdateMigrations()
        {
            var databaseSet = await LatestMigrationVersion();
            var migrationSet = LatestMigrationAvailable();

            if (databaseSet >= migrationSet)
            {
                return;
            }

            _logger.LogInformation($"Migration Repository believes that your database version is {databaseSet} whilst the latest migration script available is {migrationSet}. Beginning database migrations process.");

            var scriptIds = Enumerable.Range(databaseSet + 1, migrationSet - databaseSet).OrderBy(x => x).ToList();

            if (!Directory.Exists(MigrationFolders()))
            {
                _logger.LogError("MigrationRepository could not find the migration folder. Check application permissions.");
                return;
            }

            var entries = Directory.GetFiles(MigrationFolders()).Select(Path.GetFileName).ToList();

            if (!entries.Any())
            {
                _logger.LogError("MigrationRepository could not find any files in the migration folder. Check application permissions.");
                return;
            }

            var availableMigrationIds =
                entries
                    .Select(x => 
                        new PairFileNameAndScriptIndex
                        {
                            FileName = x,
                            ScriptIndex = IndexPrefix(x).GetValueOrDefault()
                        })
                    .Where(i => scriptIds.Contains(i.ScriptIndex))
                    .OrderBy(i => i.ScriptIndex)
                    .ToList();

            foreach (var migration in availableMigrationIds)
            {
                _logger.LogInformation($"MigrationRepository UpdateMigrations about to run the migration {migration?.ScriptIndex} {migration?.FileName}");
                GetScriptAndExecute(migration).Wait();
                _logger.LogInformation($"MigrationRepository UpdateMigrations has ran the migration {migration?.ScriptIndex} {migration?.FileName}");
            }
        }

        private async Task GetScriptAndExecute(PairFileNameAndScriptIndex name)
        {
            if (name == null)
            {
                _logger.LogError($"MigrationRepository passed a null name in GetScriptAndExecute for some reason.");
                return;
            }

            _logger.LogInformation($"MigrationRepository get script and execute for migration {name.ScriptIndex} {name.FileName}");
            var dbConnection = _dbConnectionFactory.BuildConn();

            try
            {
                var file = File.ReadAllText(Path.Combine(MigrationFolders(), name.FileName));

                dbConnection.Open();

                using (var conn = dbConnection.ExecuteAsync(file))
                {
                    await conn;
                    _logger.LogInformation($"MigrationRepository get script and execute for migration {name?.FileName} has completed db operation");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"MigrationRepository IMPORTANT something went wrong reading and writing the file {name.FileName}" + e.Message);
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        protected class PairFileNameAndScriptIndex
        {
            public int ScriptIndex { get; set; }
            public string FileName { get; set; }
        }
    }
}
