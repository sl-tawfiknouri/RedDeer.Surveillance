using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RedDeer.Etl.SqlSriptExecutor.Services.Interfaces;
using RedDeer.Etl.SqlSriptExecutor.Services.Models;
using System;
using System.Threading.Tasks;

namespace RedDeer.Etl.SqlSriptExecutor.Services
{
    public class SqlSriptExecutorService 
        : ISqlSriptExecutorService
    {
        private readonly IS3ClientService _s3ClientService;
        private readonly IAthenaService _athenaService;
        private readonly ILogger<SqlSriptExecutorService> _logger;

        public SqlSriptExecutorService(
            IS3ClientService s3ClientService,
            IAthenaService athenaService,
            ILogger<SqlSriptExecutorService> logger)
        {
            _s3ClientService = s3ClientService;
            _athenaService = athenaService;
            _logger = logger;
        }

        public async Task<bool> ExecuteAsync(SqlSriptExecutorRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Scripts == null)
            {
                throw new ArgumentNullException(nameof(request.Scripts));
            }

            //request.Scripts = new SqlSriptData[]
            //{
            //    new SqlSriptData
            //    {
            //        CsvOutputLocation = "s3://rd-dev-client-mantasmasidlauskas-eu-west-1/surveillance-etl/spike/results",
            //        Database = "mm-spike",
            //        SqlScriptS3Location = "script"
            //    }
            //};

            var requestJson = request != null ? JsonConvert.SerializeObject(request) : "";
            _logger.LogDebug($"Request: '{requestJson}'");
            
            foreach (var script in request.Scripts)
            {
                _logger.LogDebug($"Executing script: '{JsonConvert.SerializeObject(script)}'");

                var queryString = await _s3ClientService.ReadAllText(script.SqlScriptS3Location);
                var queryExecutionId = await _athenaService.StartQueryExecutionAsync(script.Database, queryString, script.CsvOutputLocation);
                await _athenaService.PoolQueryExecutionAsync(queryExecutionId);

                _logger.LogDebug($"Executed script: '{JsonConvert.SerializeObject(script)}' with ExecutionId '{queryExecutionId}'.");
            }

            return await Task.FromResult(true);
        }
    }
}
