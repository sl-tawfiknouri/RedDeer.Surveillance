﻿using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Equity.Frames;
using Domain.Equity.Streams.Interfaces;
using Domain.Trades.Orders;
using Domain.Trades.Streams.Interfaces;
using TestHarness.Engine.OrderGenerator.Interfaces;

namespace TestHarness.Engine.OrderGenerator
{
    public class OrderDataGeneratorSedolFilteringDecorator : IOrderDataGenerator
    {
        private readonly IOrderDataGenerator _baseGenerator;
        private readonly IReadOnlyCollection<string> _sedols;

        public OrderDataGeneratorSedolFilteringDecorator(
            IOrderDataGenerator baseGenerator,
            IReadOnlyCollection<string> sedols)
        {
            _baseGenerator = baseGenerator ?? throw new ArgumentNullException(nameof(baseGenerator));
            _sedols = sedols?.Where(sed => !string.IsNullOrWhiteSpace(sed)).ToList() ?? new List<string>();
        }

        public void OnCompleted()
        {
            _baseGenerator.OnCompleted();
        }

        public void OnError(Exception error)
        {
            _baseGenerator.OnError(error);
        }

        public void OnNext(ExchangeFrame value)
        {
            if (value == null)
            {
                _baseGenerator.OnNext(null);
                return;
            }

            var filteredSecurities =
                value
                    .Securities
                    ?.Where(sec =>
                        !_sedols.Contains(sec?.Security.Identifiers.Sedol, StringComparer.CurrentCultureIgnoreCase))
                    .ToList()
                ?? new List<SecurityTick>();

            var filteredFrame = new ExchangeFrame(value.Exchange, value.TimeStamp, filteredSecurities);

            _baseGenerator.OnNext(filteredFrame);
        }

        public void InitiateTrading(IStockExchangeStream stockStream, ITradeOrderStream<TradeOrderFrame> tradeStream)
        {
            _baseGenerator.InitiateTrading(stockStream, tradeStream);
        }

        public void TerminateTrading()
        {
            _baseGenerator.TerminateTrading();
        }
    }
}
