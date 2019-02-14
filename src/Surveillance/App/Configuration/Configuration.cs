using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.EC2;
using Amazon.EC2.Model;
using Microsoft.Extensions.Configuration;
using Surveillance.Auditing.DataLayer;
using Surveillance.Auditing.DataLayer.Interfaces;
using Surveillance.DataLayer.Configuration;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.Engine.DataCoordinator.Configuration;
using Surveillance.Engine.DataCoordinator.Configuration.Interfaces;

// ReSharper disable InconsistentlySynchronizedField

namespace RedDeer.Surveillance.App.Configuration
{
    public class Configuration
    {
        private const string DynamoDbTable = "reddeer-config";

        private IDictionary<string, string> _dynamoConfig;
        private bool _hasFetchedEc2Data;
        private readonly object _lock = new object();

        public static bool IsEC2Instance { get; private set; }
        public static bool IsUnitTest { get; private set; }

        public Configuration()
        {
            _dynamoConfig = new Dictionary<string, string>();
        }

        public IDataLayerConfiguration BuildDatabaseConfiguration(IConfigurationRoot configurationBuilder)
        {
            lock (_lock)
            {
                Ec2Check();

                var networkConfiguration = new DataLayerConfiguration
                {
                    IsEc2Instance = Amazon.Util.EC2InstanceMetadata.InstanceId != null,
                    ScheduledRuleQueueName = GetValue("ScheduledRuleQueueName", configurationBuilder),
                    ScheduleRuleDistributedWorkQueueName = GetValue("ScheduleRuleDistributedWorkQueueName", configurationBuilder),
                    CaseMessageQueueName = GetValue("CaseMessageQueueName", configurationBuilder),
                    DataSynchroniserRequestQueueName = GetValue("DataSynchronizerQueueName", configurationBuilder),
                    ClientServiceUrl = GetValue("ClientServiceUrlAndPort", configurationBuilder),
                    TestRuleRunUpdateQueueName = GetValue("TestRuleRunUpdateQueueName", configurationBuilder),
                    SurveillanceUserApiAccessToken = GetValue("SurveillanceUserApiAccessToken", configurationBuilder),
                    AuroraConnectionString = GetValue("AuroraConnectionString", configurationBuilder),
                    BmllServiceUrl = GetValue($"BmllServiceUrlAndPort", configurationBuilder),
                    UploadCoordinatorQueueName = GetValue($"UploadCoordinatorQueueName", configurationBuilder)
                };

                return networkConfiguration;
            }
        }

        public IRuleConfiguration BuildRuleConfiguration(IConfigurationRoot configurationBuilder)
        {
            lock (_lock)
            {
                Ec2Check();

                var autoScheduleRules = GetValue("AutoScheduleRules", configurationBuilder);
                bool.TryParse(autoScheduleRules, out var autoScheduleRulesValue);

                var ruleConfiguration = new RuleConfiguration
                {
                    AutoScheduleRules = autoScheduleRulesValue
                };

                return ruleConfiguration;
            }
        }

        public ISystemDataLayerConfig BuildDataLayerConfig(IConfigurationRoot configurationBuilder)
        {
            lock (_lock)
            {
                Ec2Check();
            
                var ruleConfiguration = new SystemDataLayerConfig
                {
                    SurveillanceAuroraConnectionString = GetValue("AuroraConnectionString", configurationBuilder)
                };

                return ruleConfiguration;
            }
        }

        private void Ec2Check()
        {
            if (_hasFetchedEc2Data)
            {
                return;
            }

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
                var dynamoDbConfigKey = $"{environment}-surveillance-{GetTag("Customer")}".ToLower();
                _dynamoConfig = FetchEc2Data(dynamoDbConfigKey);
            }

            _hasFetchedEc2Data = true;
        }

        private string GetValue(string field, IConfigurationRoot root)
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                return string.Empty;
            }

            field = field.ToLower();

            // ReSharper disable once InvertIf
            if (_dynamoConfig.ContainsKey(field))
            {
                _dynamoConfig.TryGetValue(field, out var value);

                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            return root.GetValue<string>(field);
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
