using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.EC2;
using Amazon.EC2.Model;
using DataImport.Configuration;
using Microsoft.Extensions.Configuration;

// ReSharper disable InconsistentlySynchronizedField

namespace RedDeer.DataImport.DataImport.App.ConfigBuilder
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

        public Configuration Build(IConfigurationRoot configurationBuilder)
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
                    var dynamoDBName = $"{environment}-relay-{GetTag("Customer")}".ToLower();
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

            var networkConfiguration = new Configuration
            {
                RelayServiceEquityDomain = GetSetting("RelayServiceEquityDomain", configurationBuilder),
                RelayServiceEquityPort = GetSetting("RelayServiceEquityPort", configurationBuilder),
                SurveillanceServiceEquityDomain = GetSetting("SurveillanceServiceEquityDomain", configurationBuilder),
                SurveillanceServiceEquityPort = GetSetting("SurveillanceServiceEquityPort", configurationBuilder),
                RelayServiceTradeDomain = GetSetting("RelayServiceTradeDomain", configurationBuilder),
                RelayServiceTradePort = GetSetting("RelayServiceTradePort", configurationBuilder),
                SurveillanceServiceTradeDomain = GetSetting("SurveillanceServiceTradeDomain", configurationBuilder),
                SurveillanceServiceTradePort = GetSetting("SurveillanceServiceTradePort", configurationBuilder),
                RelayTradeFileUploadDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), GetSetting("RelayTradeFileUploadDirectoryPath", configurationBuilder)),
                RelayEquityFileUploadDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(),
                    GetSetting("RelayEquityFileUploadDirectoryPath", configurationBuilder)),
                SurveillanceAuroraConnectionString = GetSetting("SurveillanceAuroraConnectionString", configurationBuilder),
                IsDeployedOnClientMachine = GetSetting("IsDeployedOnClientMachine", configurationBuilder),
                RelayS3UploadQueueName = GetSetting("RelayS3UploadQueueName", configurationBuilder),
                RelayTradeFileFtpDirectoryPath = GetSetting("RelayTradeFileFtpDirectoryPath", configurationBuilder),
                RelayEquityFileFtpDirectoryPath = GetSetting("RelayEquityFileFtpDirectoryPath", configurationBuilder),

                // TRADE CONFIG
                OrderTypeFieldName = GetSetting("OrderTypeFieldName", configurationBuilder),

                MarketIdentifierCodeFieldName = GetSetting("MarketIdentifierCodeFieldName", configurationBuilder),
                MarketNameFieldName = GetSetting("MarketNameFieldName", configurationBuilder),

                SecurityNameFieldName = GetSetting("SecurityNameFieldName", configurationBuilder),
                SecurityCfiFieldName = GetSetting("SecurityCfiFieldName", configurationBuilder),

                SecurityClientIdentifierFieldName = GetSetting("SecurityClientIdentifierFieldName", configurationBuilder),
                SecuritySedolFieldName = GetSetting("SecuritySedolFieldName", configurationBuilder),
                SecurityIsinFieldName = GetSetting("SecurityIsinFieldName", configurationBuilder),
                SecurityFigiFieldName = GetSetting("SecurityFigiFieldName", configurationBuilder),
                SecurityCusipFieldName = GetSetting("SecurityCusipFieldName", configurationBuilder),
                SecurityExchangeSymbolFieldName = GetSetting("SecurityExchangeSymbolFieldName", configurationBuilder),
                
                LimitPriceFieldName = GetSetting("LimitPriceFieldName", configurationBuilder),
                TradeSubmittedOnFieldName = GetSetting("SecurityTradeSubmittedOnFieldName", configurationBuilder),
                StatusChangedOnFieldName = GetSetting("StatusChangedOnFieldName", configurationBuilder),
                FulfilledVolumeFieldName = GetSetting("FulfilledVolumeFieldName", configurationBuilder),
                OrderPositionFieldName = GetSetting("OrderPositionFieldName", configurationBuilder),

                TraderIdFieldName = GetSetting("TraderIdFieldName", configurationBuilder),
                TraderClientAttributionIdFieldName = GetSetting("TraderClientAttributionIdFieldName", configurationBuilder),
                PartyBrokerIdFieldName = GetSetting("PartyBrokerIdFieldName", configurationBuilder),
                CounterPartyBrokerIdFieldName = GetSetting("CounterPartyBrokerIdFieldName", configurationBuilder),

                OrderStatusFieldName = GetSetting("OrderStatusFieldName", configurationBuilder),
                CurrencyFieldName = GetSetting("CurrencyFieldName", configurationBuilder),

                SecurityLei = GetSetting("SecurityLei", configurationBuilder),
                SecurityBloombergTickerFieldName = GetSetting("SecurityBloombergTickerFieldName", configurationBuilder),
                ExecutedPriceFieldName = GetSetting("ExecutedPriceFieldName", configurationBuilder),
                OrderedVolumeFieldName = GetSetting("OrderedVolumeFieldName", configurationBuilder),
                AccountIdFieldName = GetSetting("AccountIdFieldName", configurationBuilder),
                DealerInstructionsFieldName = GetSetting("DealerInstructionsFieldName", configurationBuilder),
                TradeRationaleFieldName = GetSetting("TradeRationaleFieldName", configurationBuilder),
                TradeStrategyFieldName = GetSetting("TradeStrategyFieldName", configurationBuilder),
                SecurityIssuerIdentifier = GetSetting("SecurityIssuerIdentifier", configurationBuilder),

                // TICK CONFIG
                SecurityTickTimestampFieldName = GetSetting("SecurityTickTimestampFieldName", configurationBuilder),
                SecurityTickMarketIdentifierCodeFieldName = GetSetting("SecurityTickMarketIdentifierCodeFieldName", configurationBuilder),
                SecurityTickMarketNameFieldName = GetSetting("SecurityTickMarketNameFieldName", configurationBuilder),

                SecurityTickClientIdentifierFieldName = GetSetting("SecurityTickClientIdentifierFieldName", configurationBuilder),
                SecurityTickSedolFieldName = GetSetting("SecurityTickSedolFieldName", configurationBuilder),
                SecurityTickIsinFieldName = GetSetting("SecurityTickIsinFieldName", configurationBuilder),
                SecurityTickFigiFieldName = GetSetting("SecurityTickFigiFieldName", configurationBuilder),
                SecurityTickExchangeSymbolFieldName = GetSetting("SecurityTickExchangeSymbolFieldName", configurationBuilder),
                SecurityTickCusipFieldName = GetSetting("SecurityTickCusipFieldName", configurationBuilder),

                SecurityTickCfiFieldName = GetSetting("SecurityTickCifiFieldName", configurationBuilder),
                SecurityTickSecurityNameFieldName = GetSetting("SecurityTickSecurityNameFieldName", configurationBuilder),
                SecurityTickSpreadAskFieldName = GetSetting("SecurityTickSpreadAskFieldName", configurationBuilder),
                SecurityTickSpreadBidFieldName = GetSetting("SecurityTickSpreadBidFieldName", configurationBuilder),
                SecurityTickSpreadPriceFieldName = GetSetting("SecurityTickSpreadPriceFieldName", configurationBuilder),

                SecurityTickVolumeTradedFieldName = GetSetting("SecurityTickVolumeTradedFieldName", configurationBuilder),
                SecurityTickCurrencyFieldName = GetSetting("SecurityTickSecurityCurrencyFieldName", configurationBuilder),
                SecurityTickMarketCapFieldName = GetSetting("SecurityTickMarketCapFieldName", configurationBuilder),
                SecurityTickListedSecuritiesFieldName = GetSetting("SecurityTickListedSecuritiesFieldName", configurationBuilder),

                SecurityTickOpenPriceFieldName = GetSetting("SecurityTickOpenPriceFieldName", configurationBuilder),
                SecurityTickClosePriceFieldName = GetSetting("SecurityTickClosePriceFieldName", configurationBuilder),
                SecurityTickHighPriceFieldName = GetSetting("SecurityTickHighPriceFieldName", configurationBuilder),
                SecurityTickLowPriceFieldName = GetSetting("SecurityTickLowPriceFieldName", configurationBuilder),

                SecurityIssuerIdentifierFieldName = GetSetting("SecurityIssuerIdentifierFieldName", configurationBuilder),
                SecurityLeiFieldName = GetSetting("SecurityLeiFieldName", configurationBuilder),
                SecurityBloombergTicker = GetSetting("SecurityBloombergTicker", configurationBuilder),
                SecurityDailyVolumeFieldName = GetSetting("SecurityDailyVolumeFieldName", configurationBuilder)
            };

            return networkConfiguration;
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