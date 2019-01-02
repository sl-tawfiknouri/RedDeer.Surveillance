using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.EC2;
using Amazon.EC2.Model;
using Microsoft.Extensions.Configuration;
using ThirdPartySurveillanceDataSynchroniser.Configuration;

// ReSharper disable InconsistentlySynchronizedField
namespace RedDeer.ThirdPartySurveillanceDataSynchroniser.App.ConfigBuilder
{
    public class ConfigBuilder
    {
        private IDictionary<string, string> _dynamoConfig;
        private bool _hasFetchedEc2Data;

        private readonly object _lock = new object();

        public static bool IsEC2Instance { get; private set; }
        public static bool IsUnitTest { get; private set; }

        public ConfigBuilder()
        {
            _dynamoConfig = new Dictionary<string, string>();
        }

        public Config Build(IConfigurationRoot configurationBuilder)
        {
            lock (_lock)
            {
                IsUnitTest = AppDomain
                    .CurrentDomain
                    .GetAssemblies()
                    .Any(a => a.FullName.ToLowerInvariant().StartsWith("nunit.framework"));

                IsEC2Instance =
                    IsUnitTest == false &&
                    Amazon.Util.EC2InstanceMetadata.InstanceId != null;

                if (IsEC2Instance)
                {
                    var environment = GetTag("Environment");
                    var dynamoDBName = $"{environment}-data-import-{GetTag("Customer")}".ToLower();
                    _dynamoConfig = GetDynamoDBAttributes(dynamoDBName);

                    var marketTableName = $"{environment}-surveillance-import-market-{GetTag("Customer")}".ToLower();
                    var marketAttributes = GetDynamoDbAttributesTable(marketTableName);
                    var tradeTableName = $"{environment}-surveillance-import-trade-{GetTag("Customer")}".ToLower();
                    var tradeAttributes = GetDynamoDbAttributesTable(tradeTableName);

                    foreach (var kvp in marketAttributes)
                        _dynamoConfig.Add(kvp);

                    foreach (var kvp in tradeAttributes)
                        _dynamoConfig.Add(kvp);
                }
            }

        var config = new Config
            {
                DataSynchroniserRequestQueueName = GetSetting("DataSynchronizerQueueName", configurationBuilder), // american english for dev ops
                ScheduledRuleQueueName = GetSetting("ScheduledRuleQueueName", configurationBuilder),
                ScheduleRuleDistributedWorkQueueName = GetSetting("ScheduleRuleDistributedWorkQueueName", configurationBuilder),
                CaseMessageQueueName = GetSetting("CaseMessageQueueName", configurationBuilder),
                AuroraConnectionString = GetSetting("AuroraConnectionString", configurationBuilder),
                SurveillanceAuroraConnectionString = GetSetting("AuroraConnectionString", configurationBuilder),
                SurveillanceUserApiAccessToken = GetSetting("SurveillanceUserApiAccessToken", configurationBuilder),
                ClientServiceUrl = GetSetting("ClientServiceUrlAndPort", configurationBuilder)
            };

            return config;
        }

        private string GetSetting(string name, IConfigurationRoot config)
        {
            if (TryGetSetting(name, out var setting, config))
            {
                return setting;
            }

            return string.Empty;
        }

        private bool TryGetSetting(string name, out string setting, IConfigurationRoot config)
        {
            if (IsEC2Instance)
            {
                if (_dynamoConfig.ContainsKey(name))
                {
                    setting = _dynamoConfig[name];
                    return true;
                }
            }
            else
            {
                setting = config.GetValue<string>(name);
                return true;
            }

            setting = string.Empty;
            return false;
        }

        private static string GetTag(string name)
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

        public static Dictionary<string, string> GetDynamoDBAttributes(string dynamoDBName)
        {
            var client = new AmazonDynamoDBClient(new AmazonDynamoDBConfig
            {
                RegionEndpoint = Amazon.RegionEndpoint.EUWest1,
                ProxyCredentials = CredentialCache.DefaultCredentials
            });

            var query = new QueryRequest
            {
                TableName = "reddeer-config",
                KeyConditionExpression = "#nameAttribute = :nameValue",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#nameAttribute", "name" }
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":nameValue", new AttributeValue(dynamoDBName)},
                }
            };

            var attributes = new Dictionary<string, string>();
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

            return attributes;
        }

        public static Dictionary<string, string> GetDynamoDbAttributesTable(string dynamoDbName)
        {
            var client = new AmazonDynamoDBClient(new AmazonDynamoDBConfig
            {
                RegionEndpoint = Amazon.RegionEndpoint.EUWest1,
                ProxyCredentials = CredentialCache.DefaultCredentials
            });

            var query = new QueryRequest
            {
                TableName = dynamoDbName,
                KeyConditionExpression = "#datetimeAttribute = :datetimeValue",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#datetimeAttribute", "Datetime" }
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":datetimeValue", new AttributeValue(dynamoDbName)},
                },
                ScanIndexForward = false
            };

            var attributes = new Dictionary<string, string>();
            var response = client.QueryAsync(query).Result;
            if (response.Items.Any())
            {
                foreach (var item in response.Items.First())
                {
                    if (!string.IsNullOrWhiteSpace(item.Value.S)
                    && !string.Equals(item.Key, "Datetime", StringComparison.InvariantCultureIgnoreCase))
                    {
                        attributes[item.Key] = item.Value.S;
                    }
                }
            }

            return attributes;
        }
    }
}