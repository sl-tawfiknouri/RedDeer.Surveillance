

// ReSharper disable InconsistentlySynchronizedField
namespace RedDeer.Surveillance.App.Configuration
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

    using global::Surveillance.Auditing.DataLayer;
    using global::Surveillance.Auditing.DataLayer.Interfaces;
    using global::Surveillance.DataLayer.Configuration;
    using global::Surveillance.DataLayer.Configuration.Interfaces;
    using global::Surveillance.Engine.DataCoordinator.Configuration;
    using global::Surveillance.Engine.DataCoordinator.Configuration.Interfaces;
    using global::Surveillance.Reddeer.ApiClient.Configuration;
    using global::Surveillance.Reddeer.ApiClient.Configuration.Interfaces;

    using Microsoft.Extensions.Configuration;

    public class Configuration
    {
        private const string DynamoDbTable = "reddeer-config";

        private readonly object _lock = new object();

        private IDictionary<string, string> _dynamoConfig;

        private bool _hasFetchedEc2Data;

        public Configuration()
        {
            this._dynamoConfig = new Dictionary<string, string>();
        }

        public static bool IsEC2Instance { get; private set; }

        public static bool IsUnitTest { get; private set; }

        public IApiClientConfiguration BuildApiClientConfiguration(IConfigurationRoot configurationBuilder)
        {
            lock (this._lock)
            {
                this.Ec2Check();

                var apiClient = new ApiClientConfiguration
                                    {
                                        IsEc2Instance = EC2InstanceMetadata.InstanceId != null,
                                        ScheduledRuleQueueName =
                                            this.GetValue("ScheduledRuleQueueName", configurationBuilder),
                                        ScheduleRuleDistributedWorkQueueName =
                                            this.GetValue("ScheduleRuleDistributedWorkQueueName", configurationBuilder),
                                        CaseMessageQueueName =
                                            this.GetValue("CaseMessageQueueName", configurationBuilder),
                                        DataSynchroniserRequestQueueName =
                                            this.GetValue("DataSynchronizerQueueName", configurationBuilder),
                                        ClientServiceUrl =
                                            this.GetValue("ClientServiceUrlAndPort", configurationBuilder),
                                        TestRuleRunUpdateQueueName =
                                            this.GetValue("TestRuleRunUpdateQueueName", configurationBuilder),
                                        SurveillanceUserApiAccessToken =
                                            this.GetValue("SurveillanceUserApiAccessToken", configurationBuilder),
                                        AuroraConnectionString =
                                            this.GetValue("AuroraConnectionString", configurationBuilder),
                                        BmllServiceUrl = this.GetValue("BmllServiceUrlAndPort", configurationBuilder),
                                        UploadCoordinatorQueueName =
                                            this.GetValue("UploadCoordinatorQueueName", configurationBuilder),
                                        ScheduleRuleCancellationQueueName =
                                            this.GetValue("ScheduleRuleCancellationQueueName", configurationBuilder),
                                        ScheduleDelayedRuleRunQueueName = this.GetValue(
                                            "ScheduleDelayedRuleRunQueueName",
                                            configurationBuilder)
                                    };

                return apiClient;
            }
        }

        public IDataLayerConfiguration BuildDatabaseConfiguration(IConfigurationRoot configurationBuilder)
        {
            lock (this._lock)
            {
                this.Ec2Check();

                var networkConfiguration = new DataLayerConfiguration
                                               {
                                                   IsEc2Instance = EC2InstanceMetadata.InstanceId != null,
                                                   ScheduledRuleQueueName =
                                                       this.GetValue("ScheduledRuleQueueName", configurationBuilder),
                                                   ScheduleRuleDistributedWorkQueueName =
                                                       this.GetValue(
                                                           "ScheduleRuleDistributedWorkQueueName",
                                                           configurationBuilder),
                                                   CaseMessageQueueName =
                                                       this.GetValue("CaseMessageQueueName", configurationBuilder),
                                                   DataSynchroniserRequestQueueName =
                                                       this.GetValue("DataSynchronizerQueueName", configurationBuilder),
                                                   ClientServiceUrl =
                                                       this.GetValue("ClientServiceUrlAndPort", configurationBuilder),
                                                   TestRuleRunUpdateQueueName =
                                                       this.GetValue(
                                                           "TestRuleRunUpdateQueueName",
                                                           configurationBuilder),
                                                   SurveillanceUserApiAccessToken =
                                                       this.GetValue(
                                                           "SurveillanceUserApiAccessToken",
                                                           configurationBuilder),
                                                   AuroraConnectionString =
                                                       this.GetValue("AuroraConnectionString", configurationBuilder),
                                                   BmllServiceUrl =
                                                       this.GetValue("BmllServiceUrlAndPort", configurationBuilder),
                                                   UploadCoordinatorQueueName =
                                                       this.GetValue(
                                                           "UploadCoordinatorQueueName",
                                                           configurationBuilder),
                                                   ScheduleRuleCancellationQueueName =
                                                       this.GetValue(
                                                           "ScheduleRuleCancellationQueueName",
                                                           configurationBuilder),
                                                   ScheduleDelayedRuleRunQueueName = this.GetValue(
                                                       "ScheduleDelayedRuleRunQueueName",
                                                       configurationBuilder)
                                               };

                return networkConfiguration;
            }
        }

        public ISystemDataLayerConfig BuildDataLayerConfig(IConfigurationRoot configurationBuilder)
        {
            lock (this._lock)
            {
                this.Ec2Check();

                var ruleConfiguration = new SystemDataLayerConfig
                                            {
                                                SurveillanceAuroraConnectionString = this.GetValue(
                                                    "AuroraConnectionString",
                                                    configurationBuilder)
                                            };

                return ruleConfiguration;
            }
        }

        public IRuleConfiguration BuildRuleConfiguration(IConfigurationRoot configurationBuilder)
        {
            lock (this._lock)
            {
                this.Ec2Check();

                var autoScheduleRules = this.GetValue("AutoScheduleRules", configurationBuilder);
                bool.TryParse(autoScheduleRules, out var autoScheduleRulesValue);

                var alwaysRequireAllocations = this.GetValue("AlwaysRequireAllocations", configurationBuilder);
                bool.TryParse(alwaysRequireAllocations, out var alwaysRequireAllocationValue);

                var ruleConfiguration = new RuleConfiguration
                                            {
                                                AutoScheduleRules = autoScheduleRulesValue,
                                                AlwaysRequireAllocations = alwaysRequireAllocationValue
                                            };

                return ruleConfiguration;
            }
        }

        private void Ec2Check()
        {
            if (this._hasFetchedEc2Data) return;

            IsUnitTest = AppDomain.CurrentDomain.GetAssemblies()
                .Any(a => a.FullName.ToLowerInvariant().StartsWith("nunit.framework"));

            IsEC2Instance = IsUnitTest == false && EC2InstanceMetadata.InstanceId != null;

            if (IsEC2Instance)
            {
                var environment = this.GetTag("Environment");
                var dynamoDbConfigKey = $"{environment}-surveillance-{this.GetTag("Customer")}".ToLower();
                this._dynamoConfig = this.FetchEc2Data(dynamoDbConfigKey);
            }

            this._hasFetchedEc2Data = true;
        }

        private IDictionary<string, string> FetchEc2Data(string environmentClientId)
        {
            var client = new AmazonDynamoDBClient(
                new AmazonDynamoDBConfig
                    {
                        RegionEndpoint = RegionEndpoint.EUWest1, ProxyCredentials = CredentialCache.DefaultCredentials
                    });

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

            var attributes = new Dictionary<string, string>();

            try
            {
                var response = client.QueryAsync(query).Result;

                if (response.Items.Any())
                    foreach (var item in response.Items.First())
                        if (!string.IsNullOrWhiteSpace(item.Value.S))
                            attributes[item.Key] = item.Value.S;

                this._hasFetchedEc2Data = true;
                var casedAttributes = attributes.ToDictionary(i => i.Key?.ToLower(), i => i.Value);

                return casedAttributes;
            }
            catch (Exception)
            {
                this._hasFetchedEc2Data = true;
            }

            return new Dictionary<string, string>();
        }

        private string GetTag(string name)
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

        private string GetValue(string field, IConfigurationRoot root)
        {
            if (string.IsNullOrWhiteSpace(field)) return string.Empty;

            field = field.ToLower();

            // ReSharper disable once InvertIf
            if (this._dynamoConfig.ContainsKey(field))
            {
                this._dynamoConfig.TryGetValue(field, out var value);

                if (!string.IsNullOrWhiteSpace(value)) return value;
            }

            return root.GetValue<string>(field);
        }
    }
}