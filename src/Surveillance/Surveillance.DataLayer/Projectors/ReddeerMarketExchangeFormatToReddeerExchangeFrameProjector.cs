using System.Collections.Generic;
using System.Linq;
using Domain.Equity;
using Domain.Equity.Frames;
using Domain.Market;
using Surveillance.DataLayer.Projectors.Interfaces;
using Surveillance.ElasticSearchDtos.Market;

namespace Surveillance.DataLayer.Projectors
{
    public class ReddeerMarketExchangeFormatToReddeerExchangeFrameProjector : IReddeerMarketExchangeFormatToReddeerExchangeFrameProjector
    {
        public IReadOnlyCollection<ExchangeFrame> Project(IReadOnlyCollection<ReddeerMarketDocument> documents)
        {
            if (documents == null)
            {
                return new ExchangeFrame[0];
            }

            return documents.Select(Project).ToList();
        }

        public ExchangeFrame Project(ReddeerMarketDocument document)
        {
            if (document == null)
            {
                return null;
            }

            var exchange = ParseStockExchange(document);
            var securities = ParseSecurities(document);

            return new ExchangeFrame(exchange, securities);
        }

        private StockExchange ParseStockExchange(ReddeerMarketDocument document)
        {
            var exchangeId = document.MarketId;
            var exchangeMic = document.MarketName;

            return new StockExchange(new Market.MarketId(exchangeId), exchangeMic);
        }

        private IReadOnlyCollection<SecurityTick> ParseSecurities(ReddeerMarketDocument document)
        {
            if (document.Securities == null
                || !document.Securities.Any())
            {
                return new List<SecurityTick>();
            }

            var securities =
                document
                    .Securities
                    .Select(ParseSecurity)
                    .Where(sec => sec != null)
                    .ToList();

            return securities;
        }

        private SecurityTick ParseSecurity(ReddeerSecurityDocument doc)
        {
            if (doc == null)
            {
                return null;
            }

            var security = ParseEsSecurity(doc);
            var spread = ParseSpread(doc);
            var volume = new Volume(doc.Volume.GetValueOrDefault(0));

            return new SecurityTick(security, doc.SecurityCfi, doc.TickerSymbol, spread, volume, doc.TimeStamp, doc.MarketCap);
        }

        private Security ParseEsSecurity(ReddeerSecurityDocument doc)
        {
            var securityIdentifiers =
                new SecurityIdentifiers(
                    doc.SecurityClientIdentifier,
                    doc.SecuritySedol,
                    doc.SecurityIsin,
                    doc.SecurityFigi);

            return new Security(securityIdentifiers, doc.SecurityName);
        }

        private Spread ParseSpread(ReddeerSecurityDocument doc)
        {
            return new Spread(
                new Price(
                    doc.SpreadBuy.GetValueOrDefault(0),
                    doc.SpreadBuyCurrency),
                new Price(
                    doc.SpreadSell.GetValueOrDefault(0),
                    doc.SpreadSellCurrency),
                new Price(
                    doc.SpreadPrice.GetValueOrDefault(0),
                    doc.SpreadPriceCurrency));
        }
    }
}
