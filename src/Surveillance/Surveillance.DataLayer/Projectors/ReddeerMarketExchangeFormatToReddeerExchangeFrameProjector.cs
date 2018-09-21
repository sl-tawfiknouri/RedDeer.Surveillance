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

            return new ExchangeFrame(exchange, document.DateTime, securities);
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
            var intradayPrices = IntradayPrices(doc);

            return new SecurityTick(
                security,
                spread,
                volume,
                doc.TimeStamp,
                doc.MarketCap,
                intradayPrices,
                doc.ListedSecurities);
        }

        private IntradayPrices IntradayPrices(ReddeerSecurityDocument doc)
        {
            var openPrice =
                doc.OpenPrice != null
                ? (Price?)new Price(doc.OpenPrice.Value, doc.SpreadPriceCurrency)
                : null;

            var closePrice =
                doc.ClosePrice != null
                ? (Price?)new Price(doc.ClosePrice.Value, doc.SpreadPriceCurrency)
                : null;

            var highPrice =
                doc.HighPrice != null
                ? (Price?)new Price(doc.HighPrice.Value, doc.SpreadPriceCurrency)
                : null;

            var lowPrice =
                doc.LowPrice != null
                ? (Price?)new Price(doc.LowPrice.Value, doc.SpreadPriceCurrency)
                : null;

            var intradayPrices =
                new IntradayPrices(
                    openPrice,
                    closePrice,
                    highPrice,
                    lowPrice);

            return intradayPrices;
        }

        private Security ParseEsSecurity(ReddeerSecurityDocument doc)
        {
            var securityIdentifiers =
                new SecurityIdentifiers(
                    doc.SecurityClientIdentifier,
                    doc.SecuritySedol,
                    doc.SecurityIsin,
                    doc.SecurityFigi,
                    doc.SecurityCusip,
                    doc.SecurityExchangeSymbol);

            return new Security(securityIdentifiers, doc.SecurityName, doc.SecurityCfi);
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
