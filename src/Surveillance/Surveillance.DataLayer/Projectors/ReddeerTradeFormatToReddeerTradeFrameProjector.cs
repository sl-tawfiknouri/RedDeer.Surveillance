using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Equity;
using Domain.Market;
using Domain.Trades.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Projectors.Interfaces;
using Surveillance.ElasticSearchDtos.Trades;

namespace Surveillance.DataLayer.Projectors
{
    public class ReddeerTradeFormatToReddeerTradeFrameProjector : IReddeerTradeFormatToReddeerTradeFrameProjector
    {
        private readonly ILogger<ReddeerTradeFormatToReddeerTradeFrameProjector> _logger;

        public ReddeerTradeFormatToReddeerTradeFrameProjector(
            ILogger<ReddeerTradeFormatToReddeerTradeFrameProjector> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IReadOnlyCollection<TradeOrderFrame> Project(
            IReadOnlyCollection<ReddeerTradeDocument> tradeDocuments)
        {
            if (tradeDocuments == null
                || tradeDocuments.All(td => td == null))
            {
                return new TradeOrderFrame[0];
            }

            return tradeDocuments
                .Where(td => td != null)
                .Select(ProjectDocument)
                .Where(td => td != null)
                .ToList();
        }

        private TradeOrderFrame ProjectDocument(ReddeerTradeDocument document)
        {
            if (document == null)
            {
                return null;
            }

            try
            {
                var orderType = (OrderType)document.OrderTypeId;

                var stockExchange =
                    new StockExchange(
                        new Market.MarketId(document.MarketId),
                        document.MarketName);

                var security =
                    new Security(
                        new SecurityIdentifiers(
                            document.SecurityClientIdentifier,
                            document.SecuritySedol,
                            document.SecurityIsin,
                            document.SecurityFigi),
                        document.SecurityName);

                var limit =
                    document.Limit != null && orderType == OrderType.Limit
                        ? (Price?)new Price(document.Limit.Value, document.LimitCurrency)
                        : null;

                var volume = document.Volume;
                var orderDirection = (OrderDirection)document.OrderDirectionId;
                var orderStatus = (OrderStatus)document.OrderStatusId;
                var date = document.StatusChangedOn;

                var frame =
                    new TradeOrderFrame(
                        orderType,
                        stockExchange,
                        security,
                        limit,
                        volume,
                        orderDirection,
                        orderStatus,
                        date);

                return frame;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return null;
        }
    }
}
