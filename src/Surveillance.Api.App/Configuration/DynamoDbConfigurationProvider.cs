namespace Surveillance.Api.App.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    using Amazon;
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.DocumentModel;
    using Amazon.DynamoDBv2.Model;
    using Amazon.EC2;
    using Amazon.EC2.Model;
    using Amazon.Util;

    using global::Infrastructure.Network.Extensions;

    using NLog;

    using Surveillance.Api.App.Configuration.Interfaces;

    using Filter = Amazon.EC2.Model.Filter;

    public class DynamoDbConfigurationProvider
    {
        private const string DynamoDbTable = "reddeer-config";

        private readonly IAmazonDynamoDB _client;

        private readonly IEnvironmentService _environmentService;

        private readonly object _lock = new object();

        private IDictionary<string, string> _dynamoConfig;

        private string _dynamoJsonConfig;

        private bool _hasAttemptedToFetchDynamoJsonData;

        private bool _hasAttemptedToFetchEc2Data;

        public DynamoDbConfigurationProvider(
            IEnvironmentService environmentService,
            IAmazonDynamoDB client,
            Logger logger)
        {
            this._dynamoConfig = new Dictionary<string, string>();
            this._environmentService =
                environmentService ?? throw new ArgumentNullException(nameof(environmentService));
            this._client = client ?? throw new ArgumentNullException(nameof(client));
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Logger Logger { get; }

        public IEnumerable<KeyValuePair<string, string>> Build()
        {
            lock (this._lock)
            {
                if (this._hasAttemptedToFetchEc2Data) return this._dynamoConfig;

                if (this._environmentService.IsEc2Instance())
                {
                    var environment = this.GetTag("Environment");
                    var customer = this.GetTag("Customer");
                    var dynamoDbConfigKey = $"{environment}-surveillanceapi-{customer}".ToLower();

                    this._dynamoConfig = this.FetchEc2Data(dynamoDbConfigKey);
                }

                this._hasAttemptedToFetchEc2Data = true;

                return this._dynamoConfig;
            }
        }

        public string GetJson()
        {
            lock (this._lock)
            {
                if (this._hasAttemptedToFetchDynamoJsonData) return this._dynamoJsonConfig;

                if (!this._environmentService.IsEc2Instance()) return null;

                var environment = this.GetTag("Environment");
                var customer = this.GetTag("Customer");
                var dynamoDbConfigKey = $"{environment}-surveillanceapi-{customer}".ToLower();
                var json = this.GetJson(dynamoDbConfigKey);

                this._dynamoJsonConfig = json;
                this._hasAttemptedToFetchDynamoJsonData = true;

                return this._dynamoJsonConfig;
            }
        }

        private IDictionary<string, string> FetchEc2Data(string environmentClientId)
        {
            try
            {
                var response = this.QueryData(environmentClientId);

                var casedAttributes = new Dictionary<string, string>();

                if (response.Items.Any())
                    foreach (var item in response.Items.First())
                        if (!string.IsNullOrWhiteSpace(item.Value.S))
                            casedAttributes[item.Key.ToLower()] = item.Value.S;

                this._hasAttemptedToFetchEc2Data = true;

                return casedAttributes;
            }
            catch (Exception e)
            {
                this._hasAttemptedToFetchEc2Data = true;
                this.Logger.Log(
                    LogLevel.Error,
                    $"Configuration encountered an exception on fetching from EC2 {e.Message} {e.InnerException?.Message}");
                return new Dictionary<string, string>();
            }
        }

        private string GetJson(string environmentClientId)
        {
            try
            {
                this.Logger.Log(
                    LogLevel.Info,
                    $"Loading configuration from {DynamoDbTable} table {environmentClientId}.");

                var response = this.QueryData(environmentClientId);

                var attributeKeyValuePairs = response.Items.Single();
                var document = Document.FromAttributeMap(attributeKeyValuePairs);
                var jsonResult = document.ToJson();

                this.Logger.Log(
                    LogLevel.Info,
                    $"Loaded configuration from {DynamoDbTable} table {environmentClientId}. JSON length: {jsonResult?.Length}.");
                return jsonResult;
            }
            catch (Exception e)
            {
                this._hasAttemptedToFetchDynamoJsonData = true;
                this.Logger.Log(
                    LogLevel.Error,
                    $"Configuration encountered an exception on fetching from EC2 {e.Message} {e.InnerException?.Message}");
                return null;
            }
        }

        private string GetTag(string name)
        {
            var instanceId = EC2InstanceMetadata.InstanceId;
            var client = new AmazonEC2Client(
                new AmazonEC2Config
                    {
                        RegionEndpoint = RegionEndpoint.EUWest1, ProxyCredentials = CredentialCache.DefaultCredentials
                    });

            var describeTagsResponse = client.DescribeTagsAsync(
                new DescribeTagsRequest
                    {
                        Filters = new List<Filter>
                                      {
                                          new Filter("resource-id", new List<string> { instanceId }),
                                          new Filter("key", new List<string> { name })
                                      }
                    }).Result;

            var tags = describeTagsResponse?.Tags?.Select(s => s.Value).ToList();

            this.Logger.Log(
                LogLevel.Info,
                $"Describe Tags for ResourceId '{instanceId}' with key '{name}' returned tags {string.Join(",", tags ?? new List<string>())}. Response HttpCode '{describeTagsResponse?.HttpStatusCode}'.");

            return tags?.FirstOrDefault();
        }

        private QueryResponse QueryData(string environmentClientId)
        {
            this.Logger.Log(LogLevel.Info, $"Fetching configuration '{environmentClientId}'.");

            var query = new QueryRequest
                            {
                                TableName = DynamoDbTable,
                                KeyConditionExpression = "#NameAttribute = :NameValue",
                                ExpressionAttributeNames =
                                    new Dictionary<string, string> { { "#NameAttribute", "name" } },
                                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                                                                {
                                                                    {
                                                                        ":NameValue",
                                                                        new AttributeValue(environmentClientId)
                                                                    }
                                                                }
                            };

            var response = this._client.QueryAsync(query).Result;
            response.HttpStatusCode.EnsureSuccessStatusCode();

            return response;
        }
    }
}