using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Equity;
using Domain.Equity.Frames;
using Domain.Finance;
using Domain.Market;
using MathNet.Numerics.Distributions;
using RedDeer.Contracts.SurveillanceService.Api.Markets;
using RedDeer.Contracts.SurveillanceService.Api.SecurityPrices;
using TestHarness.Engine.EquitiesGenerator.Interfaces;

namespace TestHarness.Engine.EquitiesGenerator
{
    public class ApiDataGenerationInitialiser : IApiDataGenerationInitialiser
    {
        private readonly ExchangeDto _market;
        private readonly TimeSpan _tickFrequency;
        private readonly IReadOnlyCollection<SecurityPriceDto> _securityPrices;

        public ApiDataGenerationInitialiser(
            ExchangeDto market,
            TimeSpan tickFrequency,
            IReadOnlyCollection<SecurityPriceDto> securityPrices)
        {
            _market = market ?? throw new ArgumentNullException(nameof(market));
            _tickFrequency = tickFrequency;
            _securityPrices = securityPrices ?? throw new ArgumentNullException(nameof(securityPrices));
        }

        public IReadOnlyCollection<ExchangeFrame> OrderedDailyFrames()
        {
            var close = _market.MarketCloseTime;
            var open = _market.MarketOpenTime;
            var openFor = close - open;
            var openMinutes = openFor.TotalMinutes;
            var ticksInDay = 0;

            if (openMinutes > 0 && _tickFrequency.TotalMinutes > 0)
            {
               ticksInDay = (int)Math.Ceiling(openMinutes / _tickFrequency.TotalMinutes);
            }

            Func<double, int> volume = (x) => ticksInDay == 0
                ? (int)x
                : (int)Math.Ceiling(x * ((double) 1 / (double) ticksInDay));

            var initialTicks =
                _securityPrices
                    .SelectMany(sm =>
                        sm.Prices.Select(smp =>
                            new SecurityTick(
                                new Security(
                                    new SecurityIdentifiers(
                                        string.Empty,
                                        string.Empty,
                                        string.Empty,
                                        sm.Sedol,
                                        sm.Isin,
                                        sm.Figi,
                                        sm.Cusip,
                                        sm.ExchangeSymbol,
                                        sm.Lei,
                                        sm.BloombergTicker),
                                    sm.SecurityName,
                                    sm.Cfi,
                                    sm.IssuerIdentifier),
                                new Spread(
                                    new CurrencyAmount(
                                        smp.Value.OpenPrice,
                                        sm.SecurityCurrency),
                                    new CurrencyAmount(
                                        smp.Value.OpenPrice,
                                        sm.SecurityCurrency),
                                    new CurrencyAmount(
                                        smp.Value.OpenPrice,
                                        sm.SecurityCurrency)),
                                new Volume(volume((double)CalculateADailyVolume(smp.Value))),
                                new Volume(CalculateADailyVolume(smp.Value)),
                                smp.Value.Epoch.Date.Add(_market.MarketOpenTime),
                                smp.Value.MarketCapUsd,
                                new IntradayPrices(
                                    new CurrencyAmount(
                                        smp.Value.OpenPrice,
                                        sm.SecurityCurrency),
                                    new CurrencyAmount(
                                        smp.Value.ClosePrice,
                                        sm.SecurityCurrency),
                                    new CurrencyAmount(
                                        smp.Value.HighIntradayPrice,
                                        sm.SecurityCurrency),
                                    new CurrencyAmount(
                                        smp.Value.LowIntradayPrice,
                                        sm.SecurityCurrency)),
                                null,
                                new StockExchange(
                                    new Market.MarketId(_market.Code),
                                    _market.Name))))
                    .GroupBy(x => x.TimeStamp)
                    .OrderBy(i => i.Key)
                    .ToList();

            var frames = 
                initialTicks
                    .Select(it =>
                        new ExchangeFrame(
                            new StockExchange(
                                new Market.MarketId(_market.Code),
                                _market.Name),
                            it.Key,
                            it.ToList()))
                        .ToList();

            return frames;
        }

        private long CalculateADailyVolume(SecurityPriceSnapshotDto dto)
        {
            if (dto.DailyVolume > 0)
            {
                return dto.DailyVolume;
            }

            if (dto.MarketCapUsd == 0
                || dto.OpenPrice == 0)
            {
                // just pick a random daily volume between a million and ten million
                return DiscreteUniform.Sample(1000000, 10000000);
            }

            return (long)(dto.MarketCapUsd / dto.OpenPrice);
        }
    }
}
