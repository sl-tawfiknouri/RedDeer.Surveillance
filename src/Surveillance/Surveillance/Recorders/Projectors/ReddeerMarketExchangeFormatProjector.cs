using Surveillance.ElasticSearchDtos.Market;
using Surveillance.Factories.Interfaces;
using Surveillance.Recorders.Projectors.Interfaces;
using System;
using System.Linq;
using Domain.Equity.Frames;

namespace Surveillance.Recorders.Projectors
{
    public class ReddeerMarketExchangeFormatProjector : IReddeerMarketExchangeFormatProjector
    {
        private readonly IOriginFactory _originFactory;

        public ReddeerMarketExchangeFormatProjector(IOriginFactory originFactory)
        {
            _originFactory = originFactory ?? throw new ArgumentNullException(nameof(originFactory));
        }

        public ReddeerMarketDocument Project(ExchangeFrame frame)
        {
            if (frame == null)
            {
                return null;
            }

            return new ReddeerMarketDocument
            {
                Id = GenerateDate(),
                Origin = _originFactory.Origin(),
                MarketId = frame.Exchange?.Id?.Id ?? string.Empty,
                MarketName = frame.Exchange?.Name ?? string.Empty,
                DateTime = DateTime.UtcNow,
                Securities = MapSecuritiesToDocument(frame)
            };

        }

        private ReddeerSecurityDocument[] MapSecuritiesToDocument(ExchangeFrame frame)
        {
            if (frame?.Securities == null)
            {
                return new ReddeerSecurityDocument[0];
            }

            return frame
                .Securities
                .Select(sec =>
                {
                return new ReddeerSecurityDocument
                {
                    SecurityName = sec?.Security?.Name ?? string.Empty,

                    SecurityClientIdentifier = sec?.Security?.Identifiers.ClientIdentifier ?? string.Empty,
                    SecuritySedol = sec?.Security?.Identifiers.Sedol ?? string.Empty,
                    SecurityIsin = sec?.Security?.Identifiers.Isin ?? string.Empty,
                    SecurityFigi = sec?.Security?.Identifiers.Figi ?? string.Empty,

                    SpreadBuy = sec?.Spread.Bid.Value,
                    SpreadBuyCurrency = sec?.Spread.Bid.Currency,
                    SpreadSell = sec?.Spread.Ask.Value,
                    SpreadSellCurrency = sec?.Spread.Ask.Currency,
                    SpreadPrice = sec?.Spread.Price.Value,
                    SpreadPriceCurrency = sec?.Spread.Price.Currency,

                    TimeStamp = sec?.TimeStamp ?? DateTime.UtcNow,
                    Volume = sec?.Volume.Traded
                };
            }).ToArray();
        }

        private string GenerateDate()
        {
            var id = Guid.NewGuid().ToString();
            id += "." + DateTime.UtcNow;

            return id;
        }
    }
}
