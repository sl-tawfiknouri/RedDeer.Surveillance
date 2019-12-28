using Amazon.Athena;
using Amazon.Athena.Model;
using Microsoft.Extensions.Logging;
using RedDeer.Etl.SqlSriptExecutor.Extensions;
using RedDeer.Etl.SqlSriptExecutor.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedDeer.Etl.SqlSriptExecutor.Services
{
    public class AthenaService 
        : IAthenaService
    {
        private readonly IAmazonAthenaClientFactory _amazonAthenaClientFactory;
        private readonly ILogger<AthenaService> _logger; 

        public AthenaService(
            IAmazonAthenaClientFactory amazonAthenaClientFactory,
            ILogger<AthenaService> logger)
        {
            _amazonAthenaClientFactory = amazonAthenaClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// https://docs.aws.amazon.com/athena/latest/ug/querying.html#saving-query-results
        /// </summary>
        public async Task<string> StartQueryExecutionAsync(string database, string queryString, string outputLocation)
        {
            _logger.LogDebug($"StartQueryExecutionAsync (Database: '{database}', 'QueryString': '{queryString}', 'OutputLocation': '{outputLocation}')");

            var startQueryExecutionRequest = new StartQueryExecutionRequest
            {
                QueryString = queryString,
                QueryExecutionContext = new QueryExecutionContext { Database = database },
                ResultConfiguration = new ResultConfiguration { OutputLocation = outputLocation }
            };

            var client = _amazonAthenaClientFactory.Create();
            var startQueryExecutionResponse = await client.StartQueryExecutionAsync(startQueryExecutionRequest);
            startQueryExecutionResponse.EnsureSuccessStatusCode();

            var queryExecutionId = startQueryExecutionResponse.QueryExecutionId;

            _logger.LogDebug($"QueryExecution '{queryExecutionId}' started (Database: '{database}', 'QueryString': '{queryString}', 'OutputLocation': '{outputLocation}')");

            return queryExecutionId;
        }

        /// <summary>
        ///  Poll API to determine when the query completed
        /// </summary>
        public async Task PoolQueryExecutionAsync(string queryExecutionId)
        {
            QueryExecution queryExecution = null;

            var executingStates = new string[] { QueryExecutionState.RUNNING, QueryExecutionState.QUEUED };
            var unsuccessStates = new string[] { QueryExecutionState.CANCELLED, QueryExecutionState.FAILED };

            do
            {
                try
                {
                    _logger.LogDebug($"PoolQueryExecutionAsync for 'queryExecutionId': '{queryExecutionId}'.");

                    var getQueryExecutionRequest = new GetQueryExecutionRequest()
                    {
                        QueryExecutionId = queryExecutionId
                    };

                    var client = _amazonAthenaClientFactory.Create();
                    var getQueryExecutionAsyncResponse = await client.GetQueryExecutionAsync(getQueryExecutionRequest);
                    getQueryExecutionAsyncResponse.EnsureSuccessStatusCode();
                    queryExecution = getQueryExecutionAsyncResponse.QueryExecution;

                    _logger.LogDebug($"PoolQueryExecutionAsync state '{queryExecution.Status.State}', StateChangeReason: '{queryExecution.Status.StateChangeReason}' for 'queryExecutionId': '{queryExecutionId}'.");

                    if (unsuccessStates.Any(state => queryExecution.Status.State.Equals(state)))
                    {
                        throw new Exception($"QueryExecution for 'queryExecutionId': '{queryExecutionId}' failed.",
                            new Exception($"State '{queryExecution.Status.State}', StateChangeReason: '{queryExecution.Status.StateChangeReason}'."));
                    }

                    await Task.Delay(3000); 
                }
                catch (InvalidRequestException e)
                {
                    _logger.LogError(e, $"Exception while executing PoolQueryExecutionAsync for 'queryExecutionId': '{queryExecutionId}'.");
                }

            }
            while (executingStates.Any(state => queryExecution.Status.State.Equals(state)));

            _logger.LogDebug($"Data Scanned for '{queryExecutionId}': '{queryExecution.Statistics.DataScannedInBytes}' Bytes.");
        }

        public async Task<List<Dictionary<string, string>>> GetQueryResultsAsync(string queryExecutionId)
        {
            var items = new List<Dictionary<string, string>>();

            var getQueryResultsRequest = new GetQueryResultsRequest()
            {
                QueryExecutionId = queryExecutionId,
                MaxResults = 1000
            };

            var client = _amazonAthenaClientFactory.Create();
            GetQueryResultsResponse getQueryResultsResponse = null;
            /* Page through results and request additional pages if available */
            do
            {
                getQueryResultsResponse = await client.GetQueryResultsAsync(getQueryResultsRequest);
                getQueryResultsResponse.EnsureSuccessStatusCode();
                /* Loop over result set and create a dictionary with column name for key and data for value */
                foreach (Row row in getQueryResultsResponse.ResultSet.Rows)
                {
                    var dict = new Dictionary<string, string>();
                    for (var i = 0; i < getQueryResultsResponse.ResultSet.ResultSetMetadata.ColumnInfo.Count; i++)
                    {
                        dict.Add(getQueryResultsResponse.ResultSet.ResultSetMetadata.ColumnInfo[i].Name, row.Data[i].VarCharValue);
                    }

                    items.Add(dict);
                }

                if (getQueryResultsResponse.NextToken != null)
                {
                    getQueryResultsRequest.NextToken = getQueryResultsResponse.NextToken;
                }

            }
            while (getQueryResultsResponse.NextToken != null);

            return items;
        }
    }
}
