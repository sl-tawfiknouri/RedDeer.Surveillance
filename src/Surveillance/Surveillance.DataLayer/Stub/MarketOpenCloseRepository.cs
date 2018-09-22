using System;
using System.Collections.Generic;
using System.Linq;
using Surveillance.DataLayer.Stub.Interfaces;

namespace Surveillance.DataLayer.Stub
{
    public class MarketOpenCloseRepository : IMarketOpenCloseRepository
    {
        private readonly List<MarketOpenClose> _stubMarketData =
            new List<MarketOpenClose>
            {
                new MarketOpenClose("XLON", new DateTime(2000, 1, 1, 9, 0, 0), new DateTime(2000, 1, 1, 16, 0, 0)),
                new MarketOpenClose("LSE", new DateTime(2000, 1, 1, 9, 0, 0), new DateTime(2000, 1, 1, 16, 0, 0)),
                new MarketOpenClose("NASDAQ", new DateTime(2000, 1, 1, 8, 0, 0), new DateTime(2000, 1, 1, 17, 0, 0)),
            };

        public IReadOnlyCollection<MarketOpenClose> GetAll()
        {
            return _stubMarketData;
        }

        public IReadOnlyCollection<MarketOpenClose> Get(IReadOnlyCollection<string> marketIds)
        {
            if (!marketIds?.Any() ?? true)
            {
                return new MarketOpenClose[0];
            }

            marketIds = marketIds.Where(mi => !string.IsNullOrWhiteSpace(mi)).ToList();

            var queryResults = _stubMarketData
                .Where(smd => marketIds.Contains(smd.MarketId, StringComparer.InvariantCultureIgnoreCase))
                .ToList();

            return queryResults;
        }
    }
}