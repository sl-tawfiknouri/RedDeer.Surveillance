﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nest;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.DataLayer.ElasticSearch.DataAccess.Interfaces;
using Surveillance.ElasticSearchDtos.Market;
using Surveillance.ElasticSearchDtos.Rules;
using Surveillance.ElasticSearchDtos.Trades;

namespace Surveillance.DataLayer.ElasticSearch.DataAccess
{
    /// <summary>
    /// Data access type; more primitive than a repository
    /// This handles pure elastic search type logic
    /// </summary>
    public class ElasticSearchDataAccess : IElasticSearchDataAccess
    {
        private readonly ElasticClient _elasticClient;

        private readonly ConcurrentDictionary<string, string> _indexExistsCache;
        private readonly SemaphoreSlim _indexExistsCacheLock;

        public string RuleBreachIndexName => "rule-breach";
        public string ReddeerTradeFormatIndexName => "reddeer-trade";
        public string ReddeerStockExchangeFormatIndexName => "reddeer-stock-market";

        public ElasticSearchDataAccess(IElasticSearchConfiguration elasticSearchConfiguration)
        {
            var url =
                $"{elasticSearchConfiguration.ElasticSearchProtocol}://{elasticSearchConfiguration.ElasticSearchDomain}:{elasticSearchConfiguration.ElasticSearchPort}";
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
            var indexName = $"surveillance-{name.ToLower()}-{dateTime:yyyy-MM-dd}";

            if (_indexExistsCache.ContainsKey(indexName))
            {
                return indexName;
            }

            await _indexExistsCacheLock.WaitAsync(cancellationToken);
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
                                Settings = new IndexSettings()
                            })
                            .Mappings(ms => ms
                                .Map<T>(m => m
                                    .AutoMap()
                                )
                            ), cancellationToken);
                        HandleResponseErrors(createIndexResponse);
                    }

                    _indexExistsCache[indexName] = indexName;
                }
            }
            finally
            {
                _indexExistsCacheLock.Release();
            }

            return indexName;
        }

        /// <summary>
        /// Use fully formatted index name i.e. with surveillance-name-date
        /// </summary>
        public async Task IndexDocumentAsync<T>(
            string indexName,
            T indexableDocument, 
            DateTime timestamp,
            CancellationToken cancellationToken)
            where T : class
        {
            if (indexableDocument == null)
            {
                return;
            }

            var indexResponse = await _elasticClient.IndexAsync(
                indexableDocument,
                i => i.Index(indexName),
                cancellationToken: cancellationToken
            );

            HandleResponseErrors(indexResponse);
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

        public async Task<IReadOnlyCollection<ReddeerTradeDocument>> GetDocuments(
            DateTime start,
            DateTime end,
            CancellationToken cancellationToken)
        {
            var result = await _elasticClient.SearchAsync<ReddeerTradeDocument>(
                q => q
                    .Index("surveillance-reddeer-trade-*")
                    .Size(10000)
                    .MatchAll()
                    .Query(qu =>
                        qu.DateRange(ra =>
                            ra.Field(fi => fi.StatusChangedOn)
                                .GreaterThanOrEquals(start.ToString("yyyy-MM-dd HH:mm:ss"))
                                .LessThanOrEquals(end.ToString("yyyy-MM-dd HH:mm:ss"))
                                .Format("yyyy-MM-dd HH:mm:ss"))), cancellationToken);

            return result?.Documents ?? new List<ReddeerTradeDocument>();
        }

        public async Task<IReadOnlyCollection<ReddeerMarketDocument>> GetMarketDocuments(
            DateTime start,
            DateTime end,
            CancellationToken cancellationToken)
        {
            var result = await _elasticClient.SearchAsync<ReddeerMarketDocument>(
                q => q
                    .Index("surveillance-reddeer-stock-market*")
                    .Size(10000)
                    .MatchAll()
                    .Query(qu =>
                        qu.DateRange(ra =>
                            ra.Field(fi => fi.DateTime)
                                .GreaterThanOrEquals(start.ToString("yyyy-MM-dd HH:mm:ss"))
                                .LessThanOrEquals(end.ToString("yyyy-MM-dd HH:mm:ss"))
                                .Format("yyyy-MM-dd HH:mm:ss"))), cancellationToken);

            return result?.Documents ?? new List<ReddeerMarketDocument>();
        }

        public void HandleResponseErrors(IResponse response)
        { 
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
                throw new Exception("ElasticSearch: Invalid Response.");
            }
        }
    }
}
