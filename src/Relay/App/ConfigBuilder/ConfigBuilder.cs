using Microsoft.Extensions.Configuration;
using Relay.Configuration;
using System.IO;

namespace RedDeer.Relay.Relay.App.ConfigBuilder
{
    internal static class ConfigBuilder
    {
        public static Configuration Build(IConfigurationRoot configurationBuilder)
        {
            var networkConfiguration = new Configuration
            {
                RelayServiceEquityDomain = configurationBuilder.GetValue<string>("RelayServiceEquityDomain"),
                RelayServiceEquityPort = configurationBuilder.GetValue<string>("RelayServiceEquityPort"),
                SurveillanceServiceEquityDomain = configurationBuilder.GetValue<string>("SurveillanceServiceEquityDomain"),
                SurveillanceServiceEquityPort = configurationBuilder.GetValue<string>("SurveillanceServiceEquityPort"),

                RelayServiceTradeDomain = configurationBuilder.GetValue<string>("RelayServiceTradeDomain"),
                RelayServiceTradePort = configurationBuilder.GetValue<string>("RelayServiceTradePort"),
                SurveillanceServiceTradeDomain = configurationBuilder.GetValue<string>("SurveillanceServiceTradeDomain"),
                SurveillanceServiceTradePort = configurationBuilder.GetValue<string>("SurveillanceServiceTradePort"),

                RelayTradeFileUploadDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), configurationBuilder.GetValue<string>("RelayTradeFileUploadDirectoryPath")),
                RelayEquityFileUploadDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(),
        configurationBuilder.GetValue<string>("RelayEquityFileUploadDirectoryPath")),

                // TRADE CONFIG
                OrderTypeFieldName = configurationBuilder.GetValue<string>("OrderTypeFieldName"),

                MarketIdentifierCodeFieldName = configurationBuilder.GetValue<string>("MarketIdentifierCodeFieldName"),
                MarketNameFieldName = configurationBuilder.GetValue<string>("MarketNameFieldName"),

                SecurityNameFieldName = configurationBuilder.GetValue<string>("SecurityNameFieldName"),
                SecurityCfiFieldName = configurationBuilder.GetValue<string>("SecurityCfiFieldName"),

                SecurityClientIdentifierFieldName = configurationBuilder.GetValue<string>("SecurityClientIdentifierFieldName"),
                SecuritySedolFieldName = configurationBuilder.GetValue<string>("SecuritySedolFieldName"),
                SecurityIsinFieldName = configurationBuilder.GetValue<string>("SecurityIsinFieldName"),
                SecurityFigiFieldName = configurationBuilder.GetValue<string>("SecurityFigiFieldName"),
                SecurityCusipFieldName = configurationBuilder.GetValue<string>("SecurityCusipFieldName"),
                SecurityExchangeSymbolFieldName = configurationBuilder.GetValue<string>("SecurityExchangeSymbolFieldName"),

                LimitPriceFieldName = configurationBuilder.GetValue<string>("LimitPriceFieldName"),
                TradeSubmittedOnFieldName = configurationBuilder.GetValue<string>("SecurityTradeSubmittedOnFieldName"),
                StatusChangedOnFieldName = configurationBuilder.GetValue<string>("StatusChangedOnFieldName"),
                FulfilledVolumeFieldName = configurationBuilder.GetValue<string>("VolumeFieldName"),
                OrderPositionFieldName = configurationBuilder.GetValue<string>("OrderPositionFieldName"),

                TraderIdFieldName = configurationBuilder.GetValue<string>("TraderIdFieldName"),
                TraderClientAttributionIdFieldName = configurationBuilder.GetValue<string>("TraderClientAttributionIdFieldName"),
                PartyBrokerIdFieldName = configurationBuilder.GetValue<string>("PartyBrokerIdFieldName"),
                CounterPartyBrokerIdFieldName = configurationBuilder.GetValue<string>("CounterPartyBrokerIdFieldName"),

                OrderStatusFieldName = configurationBuilder.GetValue<string>("OrderStatusFieldName"),
                CurrencyFieldName = configurationBuilder.GetValue<string>("CurrencyFieldName"),

                // TICK CONFIG
                SecurityTickTimestampFieldName = configurationBuilder.GetValue<string>("SecurityTickTimestampFieldName"),
                SecurityTickMarketIdentifierCodeFieldName = configurationBuilder.GetValue<string>("SecurityTickMarketIdentifierCodeFieldName"),
                SecurityTickMarketNameFieldName = configurationBuilder.GetValue<string>("SecurityTickMarketNameFieldName"),

                SecurityTickClientIdentifierFieldName = configurationBuilder.GetValue<string>("SecurityTickClientIdentifierFieldName"),
                SecurityTickSedolFieldName = configurationBuilder.GetValue<string>("SecurityTickSedolFieldName"),
                SecurityTickIsinFieldName = configurationBuilder.GetValue<string>("SecurityTickIsinFieldName"),
                SecurityTickFigiFieldName = configurationBuilder.GetValue<string>("SecurityTickFigiFieldName"),
                SecurityTickExchangeSymbolFieldName = configurationBuilder.GetValue<string>("SecurityTickExchangeSymbolFieldName"),
                SecurityTickCusipFieldName = configurationBuilder.GetValue<string>("SecurityTickCusipFieldName"),

                SecurityTickCfiFieldName = configurationBuilder.GetValue<string>("SecurityTickCifiFieldName"),
                SecurityTickSecurityNameFieldName = configurationBuilder.GetValue<string>("SecurityTickSecurityNameFieldName"),
                SecurityTickSpreadAskFieldName = configurationBuilder.GetValue<string>("SecurityTickSpreadAskFieldName"),
                SecurityTickSpreadBidFieldName = configurationBuilder.GetValue<string>("SecurityTickSpreadBidFieldName"),
                SecurityTickSpreadPriceFieldName = configurationBuilder.GetValue<string>("SecurityTickSpreadPriceFieldName"),

                SecurityTickVolumeTradedFieldName = configurationBuilder.GetValue<string>("SecurityTickVolumeTradedFieldName"),
                SecurityTickCurrencyFieldName = configurationBuilder.GetValue<string>("SecurityTickSecurityCurrencyFieldName"),
                SecurityTickMarketCapFieldName = configurationBuilder.GetValue<string>("SecurityTickMarketCapFieldName"),
                SecurityTickListedSecuritiesFieldName = configurationBuilder.GetValue<string>("SecurityTickListedSecuritiesFieldName"),

                SecurityTickOpenPriceFieldName = configurationBuilder.GetValue<string>("SecurityTickOpenPriceFieldName"),
                SecurityTickClosePriceFieldName = configurationBuilder.GetValue<string>("SecurityTickClosePriceFieldName"),
                SecurityTickHighPriceFieldName = configurationBuilder.GetValue<string>("SecurityTickHighPriceFieldName"),
                SecurityTickLowPriceFieldName = configurationBuilder.GetValue<string>("SecurityTickLowPriceFieldName"),
            };

            return networkConfiguration;
        }
    }
}