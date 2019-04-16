using Domain.Core.Trading.Orders;
using SharedKernel.Files.Orders.Interfaces;
using System;

namespace SharedKernel.Files.Orders
{
    public class OmsOrderFieldCompression : IOmsOrderFieldCompression
    {
        public Order Compress(
            Order orderToCompress,
            Order orderToRetain)
        {
            if (orderToRetain == null)
            {
                return orderToCompress;
            }

            if (orderToCompress == null)
            {
                return orderToRetain;
            }

            MapProperty(orderToCompress, orderToRetain, i => i.AmendedDate, (x, y) => y.AmendedDate = x);
            MapProperty(orderToCompress, orderToRetain, i => i.BookedDate, (x, y) => y.BookedDate = x);
            MapProperty(orderToCompress, orderToRetain, i => i.CancelledDate, (x, y) => y.CancelledDate = x);
            MapProperty(orderToCompress, orderToRetain, i => i.CreatedDate, (x, y) => y.CreatedDate = x);
            MapProperty(orderToCompress, orderToRetain, i => i.FilledDate, (x, y) => y.FilledDate = x);
            MapProperty(orderToCompress, orderToRetain, i => i.PlacedDate, (x, y) => y.PlacedDate = x);
            MapProperty(orderToCompress, orderToRetain, i => i.RejectedDate, (x, y) => y.RejectedDate = x);

            MapProperty(orderToCompress, orderToRetain, i => i.OrderOptionStrikePrice, (x, y) => y.OrderOptionStrikePrice = x);
            MapProperty(orderToCompress, orderToRetain, i => i.OrderOptionExpirationDate, (x, y) => y.OrderOptionExpirationDate = x);
            MapProperty(orderToCompress, orderToRetain, i => i.OrderOptionEuropeanAmerican, (x, y) => y.OrderOptionEuropeanAmerican = x);

            MapProperty(orderToCompress, orderToRetain, i => i.OrderType, (x, y) => y.OrderType = x);
            MapProperty(orderToCompress, orderToRetain, i => i.OrderDirection, (x, y) => y.OrderDirection = x);
            MapProperty(orderToCompress, orderToRetain, i => i.OrderCurrency, (x, y) => y.OrderCurrency = x);
            MapProperty(orderToCompress, orderToRetain, i => i.OrderSettlementCurrency, (x, y) => y.OrderSettlementCurrency = x);
            MapProperty(orderToCompress, orderToRetain, i => i.OrderCleanDirty, (x, y) => y.OrderCleanDirty = x);
            MapProperty(orderToCompress, orderToRetain, i => i.OrderAccumulatedInterest, (x, y) => y.OrderAccumulatedInterest = x);

            MapProperty(orderToCompress, orderToRetain, i => i.OrderLimitPrice, (x, y) => y.OrderLimitPrice = x);
            MapProperty(orderToCompress, orderToRetain, i => i.OrderAverageFillPrice, (x, y) => y.OrderAverageFillPrice = x);

            MapProperty(orderToCompress, orderToRetain, i => i.OrderTraderId, (x, y) => y.OrderTraderId = x);
            MapProperty(orderToCompress, orderToRetain, i => i.OrderTraderName, (x, y) => y.OrderTraderName = x);
            MapProperty(orderToCompress, orderToRetain, i => i.OrderClearingAgent, (x, y) => y.OrderClearingAgent = x);
            MapProperty(orderToCompress, orderToRetain, i => i.OrderDealingInstructions, (x, y) => y.OrderDealingInstructions = x);

            MapProperty(orderToCompress, orderToRetain, i => i.OrderOrderedVolume, (x, y) => y.OrderOrderedVolume = x);
            MapProperty(orderToCompress, orderToRetain, i => i.OrderFilledVolume, (x, y) => y.OrderFilledVolume = x);

            return orderToRetain;
        }

        /// <summary>
        /// Perform side effect of writing to order to retain
        /// </summary>
        private void MapProperty<T>(
            Order orderToCompress,
            Order orderToRetain,
            Func<Order, T> readFunc,
            Action<T, Order> writeFunc)
        {
            var compressor = readFunc(orderToCompress);
            var compressee = readFunc(orderToRetain);

            if (compressee != null)
            {
                return;
            }

            if (compressor == null)
            {
                return;
            }

            writeFunc(compressor, orderToRetain);
        }
    }
}
