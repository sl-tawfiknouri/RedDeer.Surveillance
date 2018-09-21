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
                DateTime = frame.TimeStamp,
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
                .Select(sec => new ReddeerSecurityDocument
                {
                    SecurityName = sec?.Security?.Name ?? string.Empty,

                    SecurityClientIdentifier = sec?.Security?.Identifiers.ClientIdentifier ?? string.Empty,
                    SecuritySedol = sec?.Security?.Identifiers.Sedol ?? string.Empty,
                    SecurityIsin = sec?.Security?.Identifiers.Isin ?? string.Empty,
                    SecurityFigi = sec?.Security?.Identifiers.Figi ?? string.Empty,
                    SecurityCusip = sec?.Security?.Identifiers.Cusip ?? string.Empty,
                    SecurityExchangeSymbol = sec?.Security.Identifiers.ExchangeSymbol,

                    SecurityCfi = sec?.Security.Cfi,

                    SpreadBuy = sec?.Spread.Bid.Value,
                    SpreadBuyCurrency = sec?.Spread.Bid.Currency,
                    SpreadSell = sec?.Spread.Ask.Value,
                    SpreadSellCurrency = sec?.Spread.Ask.Currency,
                    SpreadPrice = sec?.Spread.Price.Value,
                    SpreadPriceCurrency = sec?.Spread.Price.Currency,

                    OpenPrice = sec?.IntradayPrices?.Open?.Value,
                    ClosePrice = sec?.IntradayPrices?.Close?.Value,
                    HighPrice = sec?.IntradayPrices?.High?.Value,
                    LowPrice = sec?.IntradayPrices?.Low?.Value,

                    Volume = sec?.Volume.Traded,
                    ListedSecurities = sec?.ListedSecurities,
                    TimeStamp = sec?.TimeStamp ?? DateTime.UtcNow,
                    MarketCap = sec?.MarketCap
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
