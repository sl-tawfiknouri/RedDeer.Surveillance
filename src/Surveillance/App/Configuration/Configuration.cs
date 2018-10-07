using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Configuration;
using Surveillance.Configuration;
using Surveillance.Configuration.Interfaces;
using Surveillance.DataLayer.Configuration;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.System.DataLayer;
using Surveillance.System.DataLayer.Interfaces;
// ReSharper disable InconsistentlySynchronizedField

namespace RedDeer.Surveillance.App.Configuration
{
    public class Configuration
    {
        private const string DynamoDbKey = "EnvironmentClientDeployment";
        private const string DynamoDbTable = "surveillance-application";

        private IDictionary<string, string> _dynamoConfig;
        private bool _hasFetchedEc2Data;
        private readonly object _lock = new object();

        public Configuration()
        {
            _dynamoConfig = new Dictionary<string, string>();
        }

        public INetworkConfiguration BuildNetworkConfiguration(IConfigurationRoot configurationBuilder)
        {
            lock (_lock)
            {
                Ec2Check(configurationBuilder);

                var networkConfiguration = new NetworkConfiguration
                {
                    SurveillanceServiceEquityDomain = GetValue("SurveillanceServiceEquityDomain", configurationBuilder),
                    SurveillanceServiceEquityPort = GetValue("SurveillanceServiceEquityPort", configurationBuilder),
                    SurveillanceServiceTradeDomain = GetValue("SurveillanceServiceTradeDomain", configurationBuilder),
                    SurveillanceServiceTradePort = GetValue("SurveillanceServiceTradePort", configurationBuilder),
                };

                return networkConfiguration;
            }
        }

        public IDataLayerConfiguration BuildDatabaseConfiguration(IConfigurationRoot configurationBuilder)
        {
            lock (_lock)
            {
                Ec2Check(configurationBuilder);

                var networkConfiguration = new DataLayerConfiguration
                {
                    IsEc2Instance = Amazon.Util.EC2InstanceMetadata.InstanceId != null,
                    ScheduledRuleQueueName = GetValue("ScheduledRuleQueueName", configurationBuilder),
                    ScheduleRuleDistributedWorkQueueName = GetValue("ScheduleRuleDistributedWorkQueueName", configurationBuilder),
                    CaseMessageQueueName = GetValue("CaseMessageQueueName", configurationBuilder),
                    ElasticSearchProtocol = GetValue("ElasticSearchProtocol", configurationBuilder),
                    ElasticSearchDomain = GetValue("ElasticSearchDomain", configurationBuilder),
                    ElasticSearchPort = GetValue("ElasticSearchPort", configurationBuilder),
                    ClientServiceUrl = GetValue("ClientServiceUrlAndPort", configurationBuilder),
                    SurveillanceUserApiAccessToken = GetValue("SurveillanceUserApiAccessToken", configurationBuilder)
                };

                return networkConfiguration;
            }
        }

        public IRuleConfiguration BuildRuleConfiguration(IConfigurationRoot configurationBuilder)
        {
            lock (_lock)
            {
                Ec2Check(configurationBuilder);

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
                Ec2Check(configurationBuilder);

                var ruleConfiguration = new SystemDataLayerConfig
                {
                    SurveillanceAuroraConnectionString = GetValue("AuroraConnectionString", configurationBuilder)
                };

                return ruleConfiguration;
            }
        }

        private void Ec2Check(IConfigurationRoot configurationBuilder)
        {
            if (_hasFetchedEc2Data)
            {
                return;
            }

            var environmentClientId = configurationBuilder.GetValue<string>(DynamoDbKey);
            _dynamoConfig = FetchEc2Data(environmentClientId);
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

                return value ?? string.Empty;
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
                KeyConditionExpression = "#EnvironmentClientDeploymentAttribute = :EnvironmentClientDeploymentValue",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#EnvironmentClientDeploymentAttribute", "name" }
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":EnvironmentClientDeploymentValue", new AttributeValue(environmentClientId)},
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
                        attributes[item.Key] = item.Value.S;
                    }
                }

                _hasFetchedEc2Data = true;
                var casedAttributes = attributes.ToDictionary(i => i.Key?.ToLower(), i => i.Value);

                return casedAttributes;
            }
            catch (Exception e)
            {
                //
            }

            return new Dictionary<string, string>();
        }
    }
}
