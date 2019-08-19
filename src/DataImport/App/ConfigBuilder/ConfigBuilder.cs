

// ReSharper disable InconsistentlySynchronizedField
namespace RedDeer.DataImport.DataImport.App.ConfigBuilder
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;

    using Amazon;
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.Model;
    using Amazon.EC2;
    using Amazon.EC2.Model;
    using Amazon.Util;

    using global::DataImport.Configuration;

    using Microsoft.Extensions.Configuration;

    using Surveillance.DataLayer.Configuration;
    using Surveillance.DataLayer.Configuration.Interfaces;
    using Surveillance.Reddeer.ApiClient.Configuration;
    using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;

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

        public Configuration Build(IConfigurationRoot configurationBuilder)
        {
            this.SetDynamoConfig();

            var networkConfiguration = new Configuration
                                           {
                                               DataImportTradeFileUploadDirectoryPath =
                                                   Path.Combine(
                                                       Directory.GetCurrentDirectory(),
                                                       this.GetSetting(
                                                           "DataImportTradeFileUploadDirectoryPath",
                                                           configurationBuilder)),
                                               DataImportEquityFileUploadDirectoryPath =
                                                   Path.Combine(
                                                       Directory.GetCurrentDirectory(),
                                                       this.GetSetting(
                                                           "DataImportEquityFileUploadDirectoryPath",
                                                           configurationBuilder)),
                                               SurveillanceAuroraConnectionString =
                                                   this.GetSetting("AuroraConnectionString", configurationBuilder),
                                               DataImportS3UploadQueueName =
                                                   this.GetSetting("DataImportS3UploadQueueName", configurationBuilder),
                                               DataImportTradeFileFtpDirectoryPath =
                                                   this.GetSetting(
                                                       "DataImportTradeFileFtpDirectoryPath",
                                                       configurationBuilder),
                                               DataImportEquityFileFtpDirectoryPath =
                                                   this.GetSetting(
                                                       "DataImportEquityFileFtpDirectoryPath",
                                                       configurationBuilder),
                                               DataImportAllocationFileUploadDirectoryPath =
                                                   this.GetSetting(
                                                       "DataImportAllocationFileUploadDirectoryPath",
                                                       configurationBuilder),
                                               DataImportAllocationFileFtpDirectoryPath =
                                                   this.GetSetting(
                                                       "DataImportAllocationFileFtpDirectoryPath",
                                                       configurationBuilder),
                                               DataImportEtlFileUploadDirectoryPath =
                                                   this.GetSetting(
                                                       "DataImportEtlFileUploadDirectoryPath",
                                                       configurationBuilder),
                                               DataImportEtlFileFtpDirectoryPath =
                                                   this.GetSetting(
                                                       "DataImportEtlFileFtpDirectoryPath",
                                                       configurationBuilder),
                                               DataImportEtlFailureNotifications = this.GetSetting(
                                                   "DataImportEtlFailureNotifications",
                                                   configurationBuilder)
                                           };

            return networkConfiguration;
        }

        public IApiClientConfiguration BuildApi(IConfigurationRoot configurationBuilder)
        {
            this.SetDynamoConfig();

            return new ApiClientConfiguration
                       {
                           ScheduledRuleQueueName = this.GetSetting("ScheduledRuleQueueName", configurationBuilder),
                           ScheduleRuleDistributedWorkQueueName =
                               this.GetSetting("ScheduleRuleDistributedWorkQueueName", configurationBuilder),
                           CaseMessageQueueName = this.GetSetting("CaseMessageQueueName", configurationBuilder),
                           ClientServiceUrl = this.GetSetting("ClientServiceUrlAndPort", configurationBuilder),
                           SurveillanceUserApiAccessToken =
                               this.GetSetting("SurveillanceUserApiAccessToken", configurationBuilder),
                           AuroraConnectionString = this.GetSetting("AuroraConnectionString", configurationBuilder),
                           BmllServiceUrl = this.GetSetting("BmllServiceUrlAndPort", configurationBuilder),
                           UploadCoordinatorQueueName =
                               this.GetSetting("UploadCoordinatorQueueName", configurationBuilder),
                           EmailServiceSendEmailQueueName = this.GetSetting(
                               "EmailServiceSendEmailQueueName",
                               configurationBuilder)
                       };
        }

        public IDataLayerConfiguration BuildData(IConfigurationRoot configurationBuilder)
        {
            this.SetDynamoConfig();

            return new DataLayerConfiguration
                       {
                           ScheduledRuleQueueName = this.GetSetting("ScheduledRuleQueueName", configurationBuilder),
                           ScheduleRuleDistributedWorkQueueName =
                               this.GetSetting("ScheduleRuleDistributedWorkQueueName", configurationBuilder),
                           CaseMessageQueueName = this.GetSetting("CaseMessageQueueName", configurationBuilder),
                           ClientServiceUrl = this.GetSetting("ClientServiceUrlAndPort", configurationBuilder),
                           SurveillanceUserApiAccessToken =
                               this.GetSetting("SurveillanceUserApiAccessToken", configurationBuilder),
                           AuroraConnectionString = this.GetSetting("AuroraConnectionString", configurationBuilder),
                           BmllServiceUrl = this.GetSetting("BmllServiceUrlAndPort", configurationBuilder),
                           UploadCoordinatorQueueName =
                               this.GetSetting("UploadCoordinatorQueueName", configurationBuilder),
                           EmailServiceSendEmailQueueName = this.GetSetting(
                               "EmailServiceSendEmailQueueName",
                               configurationBuilder)
                       };
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

        private void SetDynamoConfig()
        {
            lock (this._lock)
            {
                IsUnitTest = AppDomain.CurrentDomain.GetAssemblies()
                    .Any(a => a.FullName.ToLowerInvariant().StartsWith("nunit.framework"));

                IsEC2Instance = IsUnitTest == false && EC2InstanceMetadata.InstanceId != null;

                if (IsEC2Instance)
                {
                    var environment = GetTag("Environment");
                    var dynamoDBName = $"{environment}-data-import-{GetTag("Customer")}".ToLower();
                    this._dynamoConfig = GetDynamoDBAttributes(dynamoDBName);
                }
            }
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