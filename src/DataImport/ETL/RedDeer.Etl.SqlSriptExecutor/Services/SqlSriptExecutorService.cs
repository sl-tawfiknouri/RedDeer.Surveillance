using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RedDeer.Etl.SqlSriptExecutor.Services.Interfaces;
using RedDeer.Etl.SqlSriptExecutor.Services.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedDeer.Etl.SqlSriptExecutor.Services
{
    public class SqlSriptExecutorService 
        : ISqlSriptExecutorService
    {
        private readonly IS3ClientService _s3ClientService;
        private readonly IAthenaClientService _athenaClientService;
        private readonly ILogger<SqlSriptExecutorService> _logger;

        public SqlSriptExecutorService(
            IS3ClientService s3ClientService,
            IAthenaClientService athenaClientService,
            ILogger<SqlSriptExecutorService> logger)
        {
            _s3ClientService = s3ClientService;
            _athenaClientService = athenaClientService;
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

            var requestJson = request != null ? JsonConvert.SerializeObject(request) : "";
            _logger.LogDebug($"Request: '{requestJson}'");

            var queryExecutionIds = new List<string>();

            foreach (var script in request.Scripts)
            {
                _logger.LogDebug($"Executing script: '{JsonConvert.SerializeObject(script)}'");

                var queryString = await _s3ClientService.ReadAllText(script.SqlScriptS3Location);
                var queryExecutionId = await _athenaClientService.StartQueryExecutionAsync(script.Database, queryString, script.CsvOutputLocation);
                queryExecutionIds.Add(queryExecutionId);

                _logger.LogDebug($"Executed script: '{JsonConvert.SerializeObject(script)}' with ExecutionId '{queryExecutionId}'.");
            }

            await _athenaClientService.BatchPoolQueryExecutionAsync(queryExecutionIds);

            _logger.LogDebug($"Completed QueryExecutionIds '{string.Join(", ", queryExecutionIds)}'.");

            return await Task.FromResult(true);
        }
    }
}
