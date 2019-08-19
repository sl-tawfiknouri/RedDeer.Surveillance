

// ReSharper disable InconsistentlySynchronizedField
namespace DataSynchroniser.App.ConfigBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    using Amazon;
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.Model;
    using Amazon.EC2;
    using Amazon.EC2.Model;
    using Amazon.Util;

    using DataSynchroniser.Configuration;

    using Microsoft.Extensions.Configuration;

    public class ConfigBuilder
    {
        private readonly object _lock = new object();

        private IDictionary<string, string> _dynamoConfig;

        public ConfigBuilder()
        {
            this._dynamoConfig = new Dictionary<string, string>();
        }

        public static bool IsEC2Instance { get; private set; }

        public static bool IsUnitTest { get; private set; }

        public static Dictionary<string, string> GetDynamoDBAttributes(string dynamoDBName)
        {
            var client = new AmazonDynamoDBClient(
                new AmazonDynamoDBConfig
                    {
                        RegionEndpoint = RegionEndpoint.EUWest1, ProxyCredentials = CredentialCache.DefaultCredentials
                    });

            var query = new QueryRequest
                            {
                                TableName = "reddeer-config",
                                KeyConditionExpression = "#nameAttribute = :nameValue",
                                ExpressionAttributeNames =
                                    new Dictionary<string, string> { { "#nameAttribute", "name" } },
                                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                                                                {
                                                                    { ":nameValue", new AttributeValue(dynamoDBName) }
                                                                }
                            };

            var attributes = new Dictionary<string, string>();
            var response = client.QueryAsync(query).Result;
            if (response.Items.Any())
                foreach (var item in response.Items.First())
                    if (!string.IsNullOrWhiteSpace(item.Value.S))
                        attributes[item.Key] = item.Value.S;

            return attributes;
        }

        public static Dictionary<string, string> GetDynamoDbAttributesTable(string dynamoDbName)
        {
            var client = new AmazonDynamoDBClient(
                new AmazonDynamoDBConfig
                    {
                        RegionEndpoint = RegionEndpoint.EUWest1, ProxyCredentials = CredentialCache.DefaultCredentials
                    });

            var query = new QueryRequest
                            {
                                TableName = dynamoDbName,
                                KeyConditionExpression = "#datetimeAttribute = :datetimeValue",
                                ExpressionAttributeNames =
                                    new Dictionary<string, string> { { "#datetimeAttribute", "Datetime" } },
                                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                                                                {
                                                                    {
                                                                        ":datetimeValue",
                                                                        new AttributeValue(dynamoDbName)
                                                                    }
                                                                },
                                ScanIndexForward = false
                            };

            var attributes = new Dictionary<string, string>();
            var response = client.QueryAsync(query).Result;
            if (response.Items.Any())
                foreach (var item in response.Items.First())
                    if (!string.IsNullOrWhiteSpace(item.Value.S) && !string.Equals(
                            item.Key,
                            "Datetime",
                            StringComparison.InvariantCultureIgnoreCase))
                        attributes[item.Key] = item.Value.S;

            return attributes;
        }

        public Config Build(IConfigurationRoot configurationBuilder)
        {
            lock (this._lock)
            {
                IsUnitTest = AppDomain.CurrentDomain.GetAssemblies()
                    .Any(a => a.FullName.ToLowerInvariant().StartsWith("nunit.framework"));

                IsEC2Instance = IsUnitTest == false && EC2InstanceMetadata.InstanceId != null;

                if (IsEC2Instance)
                {
                    var environment = GetTag("Environment");
                    var dynamoDBName = $"{environment}-data-synchronizer-{GetTag("Customer")}".ToLower();
                    this._dynamoConfig = GetDynamoDBAttributes(dynamoDBName);
                }
            }

            var config = new Config
                             {
                                 DataSynchroniserRequestQueueName =
                                     this.GetSetting(
                                         "DataSynchronizerQueueName",
                                         configurationBuilder), // american english for dev ops
                                 ScheduledRuleQueueName =
                                     this.GetSetting("ScheduledRuleQueueName", configurationBuilder),
                                 ScheduleRuleDistributedWorkQueueName =
                                     this.GetSetting("ScheduleRuleDistributedWorkQueueName", configurationBuilder),
                                 CaseMessageQueueName = this.GetSetting("CaseMessageQueueName", configurationBuilder),
                                 AuroraConnectionString =
                                     this.GetSetting("AuroraConnectionString", configurationBuilder),
                                 SurveillanceAuroraConnectionString =
                                     this.GetSetting("AuroraConnectionString", configurationBuilder),
                                 SurveillanceUserApiAccessToken =
                                     this.GetSetting("SurveillanceUserApiAccessToken", configurationBuilder),
                                 ClientServiceUrl = this.GetSetting("ClientServiceUrlAndPort", configurationBuilder),
                                 BmllServiceUrl = this.GetSetting("BmllServiceUrlAndPort", configurationBuilder)
                             };

            return config;
        }

        private static string GetTag(string name)
        {
            var instanceId = EC2InstanceMetadata.InstanceId;
            var client = new AmazonEC2Client(
                new AmazonEC2Config
                    {
                        RegionEndpoint = RegionEndpoint.EUWest1, ProxyCredentials = CredentialCache.DefaultCredentials
                    });

            var tags = client.DescribeTagsAsync(
                new DescribeTagsRequest
                    {
                        Filters = new List<Filter>
                                      {
                                          new Filter("resource-id", new List<string> { instanceId }),
                                          new Filter("key", new List<string> { name })
                                      }
                    }).Result.Tags;

            return tags?.FirstOrDefault()?.Value;
        }

        private string GetSetting(string name, IConfigurationRoot config)
        {
            if (this.TryGetSetting(name, out var setting, config)) return setting;

            return string.Empty;
        }

        private bool TryGetSetting(string name, out string setting, IConfigurationRoot config)
        {
            if (IsEC2Instance)
            {
                if (this._dynamoConfig.ContainsKey(name))
                {
                    setting = this._dynamoConfig[name];
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
    }
}