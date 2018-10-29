using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Surveillance.System.DataLayer.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.DataLayer.Repositories
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

                using (var conn = dbConnection.ExecuteScalarAsync<int>(HighestMigrationSql))
                {
                    return await conn;
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
                return 0;
            }

            return availableMigrationIds.Max();
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
                await GetScriptAndExecute(migration);
            }
        }

        private async Task GetScriptAndExecute(PairFileNameAndScriptIndex name)
        {
            if (name == null)
            {
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
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"MigrationRepository something went horribly wrong reading and writing the file {name.FileName}" + e.Message);
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
