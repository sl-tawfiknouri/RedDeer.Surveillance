using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.EC2;
using Amazon.EC2.Model;
using Surveillance.Api.App.Configuration.Interfaces;
using Infrastructure.Network.Extensions;

namespace Surveillance.Api.App.Configuration
{
    public class DynamoDbConfigurationProvider
    {
        private const string DynamoDbTable = "reddeer-config";

        private IDictionary<string, string> _dynamoConfig;
        private bool _hasAttemptedToFetchEc2Data;

        private string _dynamoJsonConfig;
        private bool _hasAttemptedToFetchDynamoJsonData = false;

        private readonly object _lock = new object();
        private readonly IEnvironmentService _environmentService;
        private readonly IAmazonDynamoDB _client;
        public NLog.Logger Logger { get; }

        public DynamoDbConfigurationProvider(
            IEnvironmentService environmentService,
            IAmazonDynamoDB client,
            NLog.Logger logger)
        {
            _dynamoConfig = new Dictionary<string, string>();
            _environmentService = environmentService ?? throw new ArgumentNullException(nameof(environmentService));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string GetJson()
        {
            lock (_lock)
            {
                if (_hasAttemptedToFetchDynamoJsonData)
                {
                    return _dynamoJsonConfig;
                }

                if (!_environmentService.IsEc2Instance())
                {
                    return null;
                }

                var environment = GetTag("Environment");
                var dynamoDbConfigKey = $"{environment}-surveillanceapi-{GetTag("Customer")}".ToLower();
                var json = GetJson(dynamoDbConfigKey);

                _dynamoJsonConfig = json;
                _hasAttemptedToFetchDynamoJsonData = true;

                return _dynamoJsonConfig;
            }
        }

        private string GetJson(string environmentClientId)
        {
            try
            {
                var table = Amazon.DynamoDBv2.DocumentModel.Table.LoadTable(_client, DynamoDbTable);
                var getItemResponse = table.GetItemAsync(new Amazon.DynamoDBv2.DocumentModel.Primitive(environmentClientId)).Result;

                var jsonResult = getItemResponse.ToJson();
                return jsonResult;
            }
            catch (Exception e)
            {
                _hasAttemptedToFetchDynamoJsonData = true;
                Logger.Log(NLog.LogLevel.Error, $"Configuration encountered an exception on fetching from EC2 {e.Message} {e.InnerException?.Message}");
                return null;
            }
        }

        public IEnumerable<KeyValuePair<string, string>> Build()
        {
            lock (_lock)
            {
                if (_hasAttemptedToFetchEc2Data)
                {
                    return _dynamoConfig;
                }
               
                if (_environmentService.IsEc2Instance())
                {
                    var environment = GetTag("Environment");
                    var dynamoDbConfigKey = $"{environment}-surveillanceapi-{GetTag("Customer")}".ToLower();

                    _dynamoConfig = FetchEc2Data(dynamoDbConfigKey);
                }

                _hasAttemptedToFetchEc2Data = true;

                return _dynamoConfig;
            }
        }

        private IDictionary<string, string> FetchEc2Data(string environmentClientId)
        {
            var query = new QueryRequest
            {
                TableName = DynamoDbTable,
                KeyConditionExpression = "#NameAttribute = :NameValue",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#NameAttribute", "name" }
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":NameValue", new AttributeValue(environmentClientId)},
                }
            };

            try
            {
                var response = _client.QueryAsync(query).Result;
                response.HttpStatusCode.EnsureSuccessStatusCode();

                var casedAttributes = new Dictionary<string, string>();

                if (response.Items.Any())
                {
                    foreach (var item in response.Items.First())
                    {
                        if (!string.IsNullOrWhiteSpace(item.Value.S))
                        {
                            casedAttributes[item.Key.ToLower()] = item.Value.S;
                        }
                    }
                }

                _hasAttemptedToFetchEc2Data = true;

                return casedAttributes;
            }
            catch (Exception e)
            {
                _hasAttemptedToFetchEc2Data = true;
                Logger.Log(NLog.LogLevel.Error, $"Configuration encountered an exception on fetching from EC2 {e.Message} {e.InnerException?.Message}");
                return new Dictionary<string, string>();
            }
        }

        private string GetTag(string name)
        {
            var instanceId = Amazon.Util.EC2InstanceMetadata.InstanceId;
            var client = new AmazonEC2Client(new AmazonEC2Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.EUWest1,
                ProxyCredentials = CredentialCache.DefaultCredentials
            });

            var tags = client.DescribeTagsAsync(new DescribeTagsRequest
            {
                Filters = new List<Filter>
                {
                    new Filter("resource-id", new List<string> { instanceId }),
                    new Filter("key", new List<string> { name }),
                }
            }).Result.Tags;

            return tags?.FirstOrDefault()?.Value;
        }
    }
}
