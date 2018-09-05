﻿using StructureMap;
using Surveillance.DataLayer.ElasticSearch.DataAccess;
using Surveillance.DataLayer.ElasticSearch.DataAccess.Interfaces;
using Surveillance.DataLayer.ElasticSearch.Market;
using Surveillance.DataLayer.ElasticSearch.Market.Interfaces;
using Surveillance.DataLayer.ElasticSearch.Rules;
using Surveillance.DataLayer.ElasticSearch.Rules.Interfaces;
using Surveillance.DataLayer.ElasticSearch.Trade;
using Surveillance.DataLayer.ElasticSearch.Trade.Interfaces;

namespace Surveillance.DataLayer
{
    public class DataLayerRegistry : Registry
    {
        public DataLayerRegistry()
        {
            For<IElasticSearchDataAccess>().Use<ElasticSearchDataAccess>();
            For<IRuleBreachRepository>().Use<RuleBreachRepository>();
            For<IRedDeerTradeFormatRepository>().Use<RedDeerTradeFormatRepository>();
            For<IRedDeerMarketExchangeFormatRepository>().Use<RedDeerMarketExchangeFormatRepository>();
        }
    }
}
