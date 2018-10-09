using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Relay.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
// ReSharper disable InconsistentlySynchronizedField

namespace RedDeer.Relay.Relay.App.ConfigBuilder
{
    public class ConfigBuilder
    {
        private const string DynamoDbKey = "DynamoDbReddeerConfigName";
        private const string DynamoDbTable = "reddeer-config";
        private const string DynamoDbTradeTable = "surveillance-import-trade";
        private const string DynamoDbMarketTable = "surveillance-import-market";

        private readonly IDictionary<string, string> _dynamoConfig;
        private bool _hasFetchedEc2Data;
        private readonly object _lock = new object();

        public ConfigBuilder()
        {
            _dynamoConfig = new Dictionary<string, string>();
        }

        public Configuration Build(IConfigurationRoot configurationBuilder)
        {
            lock (_lock)
            {
                SetEc2Data(configurationBuilder);
            }

            var networkConfiguration = new Configuration
            {
                RelayServiceEquityDomain = GetValue("RelayServiceEquityDomain", configurationBuilder),
                RelayServiceEquityPort = GetValue("RelayServiceEquityPort", configurationBuilder),
                SurveillanceServiceEquityDomain = GetValue("SurveillanceServiceEquityDomain", configurationBuilder),
                SurveillanceServiceEquityPort = GetValue("SurveillanceServiceEquityPort", configurationBuilder),
                RelayServiceTradeDomain = GetValue("RelayServiceTradeDomain", configurationBuilder),
                RelayServiceTradePort = GetValue("RelayServiceTradePort", configurationBuilder),
                SurveillanceServiceTradeDomain = GetValue("SurveillanceServiceTradeDomain", configurationBuilder),
                SurveillanceServiceTradePort = GetValue("SurveillanceServiceTradePort", configurationBuilder),
                RelayTradeFileUploadDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), GetValue("RelayTradeFileUploadDirectoryPath", configurationBuilder)),
                RelayEquityFileUploadDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(),
                    GetValue("RelayEquityFileUploadDirectoryPath", configurationBuilder)),

                // TRADE CONFIG
                OrderTypeFieldName = GetValue("OrderTypeFieldName", configurationBuilder),

                MarketIdentifierCodeFieldName = GetValue("MarketIdentifierCodeFieldName", configurationBuilder),
                MarketNameFieldName = GetValue("MarketNameFieldName", configurationBuilder),

                SecurityNameFieldName = GetValue("SecurityNameFieldName", configurationBuilder),
                SecurityCfiFieldName = GetValue("SecurityCfiFieldName", configurationBuilder),

                SecurityClientIdentifierFieldName = GetValue("SecurityClientIdentifierFieldName", configurationBuilder),
                SecuritySedolFieldName = GetValue("SecuritySedolFieldName", configurationBuilder),
                SecurityIsinFieldName =  GetValue("SecurityIsinFieldName", configurationBuilder),
                SecurityFigiFieldName =  GetValue("SecurityFigiFieldName", configurationBuilder),
                SecurityCusipFieldName = GetValue("SecurityCusipFieldName", configurationBuilder),
                SecurityExchangeSymbolFieldName = GetValue("SecurityExchangeSymbolFieldName", configurationBuilder),
                
                LimitPriceFieldName = GetValue("LimitPriceFieldName", configurationBuilder),
                TradeSubmittedOnFieldName = GetValue("SecurityTradeSubmittedOnFieldName", configurationBuilder),
                StatusChangedOnFieldName = GetValue("StatusChangedOnFieldName", configurationBuilder),
                FulfilledVolumeFieldName = GetValue("FulfilledVolumeFieldName", configurationBuilder),
                OrderPositionFieldName = GetValue("OrderPositionFieldName", configurationBuilder),

                TraderIdFieldName = GetValue("TraderIdFieldName", configurationBuilder),
                TraderClientAttributionIdFieldName = GetValue("TraderClientAttributionIdFieldName", configurationBuilder),
                PartyBrokerIdFieldName = GetValue("PartyBrokerIdFieldName", configurationBuilder),
                CounterPartyBrokerIdFieldName = GetValue("CounterPartyBrokerIdFieldName", configurationBuilder),

                OrderStatusFieldName = GetValue("OrderStatusFieldName", configurationBuilder),
                CurrencyFieldName = GetValue("CurrencyFieldName", configurationBuilder),

                SecurityLei = GetValue("SecurityLei", configurationBuilder),
                SecurityBloombergTickerFieldName = GetValue("SecurityBloombergTickerFieldName", configurationBuilder),
                ExecutedPriceFieldName = GetValue("ExecutedPriceFieldName", configurationBuilder),
                OrderedVolumeFieldName = GetValue("OrderedVolumeFieldName", configurationBuilder),
                AccountIdFieldName = GetValue("AccountIdFieldName", configurationBuilder),
                DealerInstructionsFieldName = GetValue("DealerInstructionsFieldName", configurationBuilder),
                TradeRationaleFieldName = GetValue("TradeRationaleFieldName", configurationBuilder),
                TradeStrategyFieldName = GetValue("TradeStrategyFieldName", configurationBuilder),
                SecurityIssuerIdentifier = GetValue("SecurityIssuerIdentifier", configurationBuilder),

                // TICK CONFIG
                SecurityTickTimestampFieldName = GetValue("SecurityTickTimestampFieldName", configurationBuilder),
                SecurityTickMarketIdentifierCodeFieldName = GetValue("SecurityTickMarketIdentifierCodeFieldName", configurationBuilder),
                SecurityTickMarketNameFieldName = GetValue("SecurityTickMarketNameFieldName", configurationBuilder),

                SecurityTickClientIdentifierFieldName = GetValue("SecurityTickClientIdentifierFieldName", configurationBuilder),
                SecurityTickSedolFieldName = GetValue("SecurityTickSedolFieldName", configurationBuilder),
                SecurityTickIsinFieldName = GetValue("SecurityTickIsinFieldName", configurationBuilder),
                SecurityTickFigiFieldName = GetValue("SecurityTickFigiFieldName", configurationBuilder),
                SecurityTickExchangeSymbolFieldName = GetValue("SecurityTickExchangeSymbolFieldName", configurationBuilder),
                SecurityTickCusipFieldName = GetValue("SecurityTickCusipFieldName", configurationBuilder),

                SecurityTickCfiFieldName = GetValue("SecurityTickCifiFieldName", configurationBuilder),
                SecurityTickSecurityNameFieldName = GetValue("SecurityTickSecurityNameFieldName", configurationBuilder),
                SecurityTickSpreadAskFieldName = GetValue("SecurityTickSpreadAskFieldName", configurationBuilder),
                SecurityTickSpreadBidFieldName = GetValue("SecurityTickSpreadBidFieldName", configurationBuilder),
                SecurityTickSpreadPriceFieldName = GetValue("SecurityTickSpreadPriceFieldName", configurationBuilder),

                SecurityTickVolumeTradedFieldName = GetValue("SecurityTickVolumeTradedFieldName", configurationBuilder),
                SecurityTickCurrencyFieldName = GetValue("SecurityTickSecurityCurrencyFieldName", configurationBuilder),
                SecurityTickMarketCapFieldName = GetValue("SecurityTickMarketCapFieldName", configurationBuilder),
                SecurityTickListedSecuritiesFieldName = GetValue("SecurityTickListedSecuritiesFieldName", configurationBuilder),

                SecurityTickOpenPriceFieldName = GetValue("SecurityTickOpenPriceFieldName", configurationBuilder),
                SecurityTickClosePriceFieldName = GetValue("SecurityTickClosePriceFieldName", configurationBuilder),
                SecurityTickHighPriceFieldName = GetValue("SecurityTickHighPriceFieldName", configurationBuilder),
                SecurityTickLowPriceFieldName = GetValue("SecurityTickLowPriceFieldName", configurationBuilder),

                SecurityIssuerIdentifierFieldName = GetValue("SecurityIssuerIdentifierFieldName", configurationBuilder),
                SecurityLeiFieldName = GetValue("SecurityLeiFieldName", configurationBuilder),
                SecurityBloombergTicker = GetValue("SecurityBloombergTicker", configurationBuilder),
                SecurityDailyVolumeFieldName = GetValue("SecurityDailyVolumeFieldName", configurationBuilder)
            };

            return networkConfiguration;
        }

        private void SetEc2Data(IConfigurationRoot configurationBuilder)
        {
            if (_hasFetchedEc2Data)
            {
                return;
            }

            var environmentClientId = configurationBuilder.GetValue<string>(DynamoDbKey);
            var importDictionary = FetchEc2Data(environmentClientId, DynamoDbTable);

            // 
            var tradeDictionary = FetchEc2Data(environmentClientId, DynamoDbTradeTable);
            var marketDictionary = FetchEc2Data(environmentClientId, DynamoDbMarketTable);

            foreach (var item in importDictionary)
                _dynamoConfig.Add(item);

            foreach (var item in tradeDictionary)
                _dynamoConfig.Add(item);

            foreach (var item in marketDictionary)
                _dynamoConfig.Add(item);
        }

        private IDictionary<string, string> FetchEc2Data(string environmentClientId, string table)
        {
            var client = new AmazonDynamoDBClient(new AmazonDynamoDBConfig
            {
                RegionEndpoint = Amazon.RegionEndpoint.EUWest1,
                ProxyCredentials = CredentialCache.DefaultCredentials
            });

            var query = new QueryRequest
            {
                TableName = table,
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
                        attributes[item.Key] = item.Value.S;
                    }
                }

                _hasFetchedEc2Data = true;
                var casedAttributes = attributes.ToDictionary(i => i.Key?.ToLower(), i => i.Value);

                return casedAttributes;
            }
            catch (Exception e)
            {
                _hasFetchedEc2Data = true;
            }

            return new Dictionary<string, string>();
        }

        private string GetValue(string key, IConfigurationRoot configurationBuilder)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }

            key = key.ToLower();

            if (_dynamoConfig.ContainsKey(key))
            {
                _dynamoConfig.TryGetValue(key, out var value);

                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            return configurationBuilder.GetValue<string>(key) ?? string.Empty;
        }
    }
}