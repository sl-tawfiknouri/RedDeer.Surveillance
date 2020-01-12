using FakeItEasy;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using RedDeer.Etl.SqlSriptExecutor.Services;
using RedDeer.Etl.SqlSriptExecutor.Services.Interfaces;
using RedDeer.Etl.SqlSriptExecutor.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedDeer.Etl.SqlSriptExecutor.Tests.Services
{
    public class SqlSriptExecutorServiceUnitTests
    {
        private SqlSriptExecutorService _sqlSriptExecutorService;

        private IS3ClientService _s3ClientService = null;
        private IAthenaClientService _athenaClientService = null;

        [SetUp]
        public void SetUp()
        {
            _s3ClientService = A.Fake<IS3ClientService>();
            _athenaClientService = A.Fake<IAthenaClientService>();

            _sqlSriptExecutorService = new SqlSriptExecutorService(_s3ClientService, _athenaClientService, new NullLogger<SqlSriptExecutorService>());
        }

        [Test]
        public async Task ExecuteAsync_WhenExecuted_StartsQueryExecutionsSavedOnS3AndPoolsStatus()
        {
            var scriptDataA = CreateSqlSriptData("a");
            var scriptDataB = CreateSqlSriptData("b");

            var scripts = new SqlSriptData[] { scriptDataA, scriptDataB };

            var scriptA = "SELECT ColumnA FROM TableA";
            A.CallTo(() => _s3ClientService.ReadAllText(A<string>.That.Matches(m => m.Equals(scriptDataA.SqlScriptS3Location))))
                .Returns(scriptA);

            A.CallTo(() => _athenaClientService.StartQueryExecutionAsync(A<string>.That.IsEqualTo(scriptDataA.Database), A<string>.That.IsEqualTo(scriptA), A<string>.That.IsEqualTo(scriptDataA.CsvOutputLocation)))
                .Returns("id-a");

            var scriptB = "SELECT ColumnB FROM TableB";
            A.CallTo(() => _s3ClientService.ReadAllText(A<string>.That.Matches(m => m.Equals(scriptDataB.SqlScriptS3Location))))
                .Returns(scriptB);

            A.CallTo(() => _athenaClientService.StartQueryExecutionAsync(A<string>.That.IsEqualTo(scriptDataB.Database), A<string>.That.IsEqualTo(scriptB), A<string>.That.IsEqualTo(scriptDataB.CsvOutputLocation)))
                .Returns("id-b");

            await _sqlSriptExecutorService.ExecuteAsync(scripts);

            A.CallTo(() => _athenaClientService.StartQueryExecutionAsync(A<string>.That.IsEqualTo(scriptDataA.Database), A<string>.That.IsEqualTo(scriptA), A<string>.That.IsEqualTo(scriptDataA.CsvOutputLocation)))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _athenaClientService.StartQueryExecutionAsync(A<string>.That.IsEqualTo(scriptDataB.Database), A<string>.That.IsEqualTo(scriptB), A<string>.That.IsEqualTo(scriptDataB.CsvOutputLocation)))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _athenaClientService.BatchPoolQueryExecutionAsync(A<List<string>>.That.IsSameSequenceAs("id-a", "id-b"), A<int>.That.IsEqualTo(5000)))
                .MustHaveHappenedOnceExactly();
        }

        private SqlSriptData CreateSqlSriptData(string value)
        {
            return new SqlSriptData
            {
                CsvOutputLocation = $"s3://ftp-bucket/table-{value}/",
                Database = $"db-{value}",
                SqlScriptS3Location = $"s3//release-bucket/scripts/script-{value}.sql"
            };
        }
    }
}
