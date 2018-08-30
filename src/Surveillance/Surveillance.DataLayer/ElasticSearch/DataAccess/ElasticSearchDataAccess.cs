using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nest;
using Surveillance.DataLayer.ElasticSearch.Interfaces;
using Surveillance.ElasticSearchDtos;

namespace Surveillance.DataLayer.ElasticSearch
{
    /// <summary>
    /// Data access type; more primitive than a repository
    /// This handles pure elastic search type logic
    /// </summary>
    public class ElasticSearchDataAccess : IElasticSearchDataAccess
    {
        private readonly ElasticClient _elasticClient;

        private ConcurrentDictionary<string, string> _indexExistsCache;
        private SemaphoreSlim _indexExistsCacheLock;

        public string RuleBreachIndexName => "rule-breach";

        public ElasticSearchDataAccess()
        {
            var url = "localhost:9200";
            var settings = new ConnectionSettings(new Uri(url));

#if DEBUG
            settings = settings.DisableDirectStreaming(); // to capture request/response bytes
#endif

            _elasticClient = new ElasticClient(settings);

            _indexExistsCache = new ConcurrentDictionary<string, string>();
            _indexExistsCacheLock = new SemaphoreSlim(1, 1);
        }

        public async Task DeleteIndexesAsync(string name, CancellationToken cancellationToken)
        {
            await _indexExistsCacheLock.WaitAsync(cancellationToken);
            try
            {
                var deleteIndexResponse = await _elasticClient.DeleteIndexAsync(name, cancellationToken: cancellationToken);
                HandleResponseErrors(deleteIndexResponse);

                _indexExistsCache.Clear();
            }
            finally
            {
                _indexExistsCacheLock.Release();
            }
        }


        public async Task<string> GetOrCreateDateBasedIndexAsync<T>(
            string name,
            DateTime dateTime,
            CancellationToken cancellationToken)
            where T : class
        {
            var indexName = $"Surveillance-{name.ToLower()}-{dateTime.ToString("yyyy-MM-dd")}";
            if (!_indexExistsCache.ContainsKey(indexName))
            {
                await _indexExistsCacheLock.WaitAsync();
                try
                {
                    if (!_indexExistsCache.ContainsKey(indexName))
                    {
                        var existsResponse = await _elasticClient.IndexExistsAsync(indexName, cancellationToken: cancellationToken);
                        HandleResponseErrors(existsResponse);

                        if (existsResponse.Exists == false)
                        {
                            var createIndexResponse = await _elasticClient.CreateIndexAsync(indexName, i => i
                                .InitializeUsing(new IndexState
                                {
                                    Settings = new IndexSettings
                                    {
                                        //NumberOfReplicas = 1,
                                        //NumberOfShards = 2
                                    }
                                })
                                .Mappings(ms => ms
                                    .Map<T>(m => m
                                        .AutoMap()
                                    )
                                ), cancellationToken: cancellationToken);
                            HandleResponseErrors(createIndexResponse);
                        }

                        _indexExistsCache[indexName] = indexName;
                    }
                }
                finally
                {
                    _indexExistsCacheLock.Release();
                }
            }

            return indexName;
        }

        public async Task IndexRuleBreachAsync(RuleBreachDocument ruleBreachDocument, CancellationToken cancellationToken)
        {
            if (ruleBreachDocument == null)
            {
                return;
            }

            var document = ruleBreachDocument;

            var index = await GetOrCreateDateBasedIndexAsync<RuleBreachDocument>(
                RuleBreachIndexName,
                ruleBreachDocument.BreachRaisedOn,
                cancellationToken);

            var indexResponse = await _elasticClient.IndexAsync(
                document,
                i => i.Index(index),
                cancellationToken: cancellationToken
            );

            HandleResponseErrors(indexResponse);
        }

        public void HandleResponseErrors(IResponse response)
        {
#if DEBUG

            var requestJson = response.ApiCall?.RequestBodyInBytes != null ?
                Encoding.UTF8.GetString(response.ApiCall.RequestBodyInBytes) :
                null;

            var responseJson = response.ApiCall?.ResponseBodyInBytes != null ?
                Encoding.UTF8.GetString(response.ApiCall.ResponseBodyInBytes) :
                null;
#endif

            var error = response.ServerError?.Error;

            if (error != null)
            {
                throw new Exception($"ElasticSearch: {error}");
            }

            var originalException = response.ApiCall?.OriginalException;
            if (originalException != null)
            {
                throw originalException;
            }

            if (response.IsValid == false)
            {
                throw new Exception($"ElasticSearch: Invalid Response.");
            }
        }
    }
}
