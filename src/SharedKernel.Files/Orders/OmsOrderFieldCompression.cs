namespace SharedKernel.Files.Orders
{
    using System;

    using Domain.Core.Trading.Orders;

    using SharedKernel.Files.Orders.Interfaces;

    public class OmsOrderFieldCompression : IOmsOrderFieldCompression
    {
        public Order Compress(Order orderToCompress, Order orderToRetain)
        {
            if (orderToRetain == null) return orderToCompress;

            if (orderToCompress == null) return orderToRetain;

            this.MapProperty(orderToCompress, orderToRetain, i => i.AmendedDate, (x, y) => y.AmendedDate = x);
            this.MapProperty(orderToCompress, orderToRetain, i => i.BookedDate, (x, y) => y.BookedDate = x);
            this.MapProperty(orderToCompress, orderToRetain, i => i.CancelledDate, (x, y) => y.CancelledDate = x);
            this.MapProperty(orderToCompress, orderToRetain, i => i.CreatedDate, (x, y) => y.CreatedDate = x);
            this.MapProperty(orderToCompress, orderToRetain, i => i.FilledDate, (x, y) => y.FilledDate = x);
            this.MapProperty(orderToCompress, orderToRetain, i => i.PlacedDate, (x, y) => y.PlacedDate = x);
            this.MapProperty(orderToCompress, orderToRetain, i => i.RejectedDate, (x, y) => y.RejectedDate = x);

            this.MapProperty(
                orderToCompress,
                orderToRetain,
                i => i.OrderOptionStrikePrice,
                (x, y) => y.OrderOptionStrikePrice = x);
            this.MapProperty(
                orderToCompress,
                orderToRetain,
                i => i.OrderOptionExpirationDate,
                (x, y) => y.OrderOptionExpirationDate = x);
            this.MapProperty(
                orderToCompress,
                orderToRetain,
                i => i.OrderOptionEuropeanAmerican,
                (x, y) => y.OrderOptionEuropeanAmerican = x);

            this.MapProperty(orderToCompress, orderToRetain, i => i.OrderType, (x, y) => y.OrderType = x);
            this.MapProperty(orderToCompress, orderToRetain, i => i.OrderDirection, (x, y) => y.OrderDirection = x);
            this.MapProperty(orderToCompress, orderToRetain, i => i.OrderCurrency, (x, y) => y.OrderCurrency = x);
            this.MapProperty(
                orderToCompress,
                orderToRetain,
                i => i.OrderSettlementCurrency,
                (x, y) => y.OrderSettlementCurrency = x);
            this.MapProperty(orderToCompress, orderToRetain, i => i.OrderCleanDirty, (x, y) => y.OrderCleanDirty = x);
            this.MapProperty(
                orderToCompress,
                orderToRetain,
                i => i.OrderAccumulatedInterest,
                (x, y) => y.OrderAccumulatedInterest = x);

            this.MapProperty(orderToCompress, orderToRetain, i => i.OrderLimitPrice, (x, y) => y.OrderLimitPrice = x);
            this.MapProperty(
                orderToCompress,
                orderToRetain,
                i => i.OrderAverageFillPrice,
                (x, y) => y.OrderAverageFillPrice = x);

            this.MapProperty(orderToCompress, orderToRetain, i => i.OrderTraderId, (x, y) => y.OrderTraderId = x);
            this.MapProperty(orderToCompress, orderToRetain, i => i.OrderTraderName, (x, y) => y.OrderTraderName = x);
            this.MapProperty(
                orderToCompress,
                orderToRetain,
                i => i.OrderClearingAgent,
                (x, y) => y.OrderClearingAgent = x);
            this.MapProperty(
                orderToCompress,
                orderToRetain,
                i => i.OrderDealingInstructions,
                (x, y) => y.OrderDealingInstructions = x);

            this.MapProperty(
                orderToCompress,
                orderToRetain,
                i => i.OrderOrderedVolume,
                (x, y) => y.OrderOrderedVolume = x);
            this.MapProperty(
                orderToCompress,
                orderToRetain,
                i => i.OrderFilledVolume,
                (x, y) => y.OrderFilledVolume = x);

            return orderToRetain;
        }

        /// <summary>
        ///     Perform side effect of writing to order to retain
        /// </summary>
        private void MapProperty<T>(
            Order orderToCompress,
            Order orderToRetain,
            Func<Order, T> readFunc,
            Action<T, Order> writeFunc)
        {
            var compressor = readFunc(orderToCompress);
            var compressee = readFunc(orderToRetain);

            if (compressee != null) return;

            if (compressor == null) return;

            writeFunc(compressor, orderToRetain);
        }
    }
}