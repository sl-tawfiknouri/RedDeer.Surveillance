using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.EC2;
using Amazon.EC2.Model;

// ReSharper disable InconsistentlySynchronizedField
namespace Surveillance.Api.App.Configuration
{
    public class Configuration
    {
        private const string DynamoDbTable = "reddeer-config";

        private IDictionary<string, string> _dynamoConfig;
        private bool _hasFetchedEc2Data;
        private readonly object _lock = new object();

        public static bool IsEc2Instance { get; private set; }
        public static bool IsUnitTest { get; private set; }

        public Configuration()
        {
            _dynamoConfig = new Dictionary<string, string>();
        }

        public IEnumerable<KeyValuePair<string, string>> Build()
        {
            lock (_lock)
            {
                if (_hasFetchedEc2Data)
                {
                    return _dynamoConfig;
                }

                IsUnitTest = AppDomain
                    .CurrentDomain
                    .GetAssemblies()
                    .Any(a => a.FullName.ToLowerInvariant().StartsWith("nunit.framework"));

                IsEc2Instance =
                    IsUnitTest == false &&
                    Amazon.Util.EC2InstanceMetadata.InstanceId != null;

                if (IsEc2Instance)
                {
                    var environment = GetTag("Environment");
                    var dynamoDbConfigKey = $"{environment}-surveillanceapi-{GetTag("Customer")}".ToLower();
                    _dynamoConfig = FetchEc2Data(dynamoDbConfigKey);
                }

                _hasFetchedEc2Data = true;

                return _dynamoConfig;
            }
        }

        private IDictionary<string, string> FetchEc2Data(string environmentClientId)
        {
            var client = new AmazonDynamoDBClient(new AmazonDynamoDBConfig
            {
                RegionEndpoint = Amazon.RegionEndpoint.EUWest1,
                ProxyCredentials = CredentialCache.DefaultCredentials
            });

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

            var attributes = new Dictionary<string, string>();

            try
            {
                var response = client.QueryAsync(query).Result;

                if (response.Items.Any())
                {
                    foreach (var item in response.Items.First())
                    {
                        if (!string.IsNullOrWhiteSpace(item.Value.S))
                        {
                            attributes[item.Key] = item.Value.S;
                        }
                    }
                }

                _hasFetchedEc2Data = true;
                var casedAttributes = attributes.ToDictionary(i => i.Key?.ToLower(), i => i.Value);

                return casedAttributes;
            }
            catch (Exception)
            {
                _hasFetchedEc2Data = true;
            }

            return new Dictionary<string, string>();
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
