using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Financial.Assets;
using Domain.Core.Financial.Money;
using Domain.Core.Markets;
using Domain.Core.Markets.Collections;
using Domain.Core.Markets.Timebars;
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

        public IReadOnlyCollection<EquityIntraDayTimeBarCollection> OrderedDailyFrames()
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
                            new EquityInstrumentIntraDayTimeBar(
                                new FinancialInstrument(
                                    InstrumentTypes.Equity,
                                    new InstrumentIdentifiers(
                                        string.Empty,
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
                                    SetCfi(sm.Cfi),
                                    sm.SecurityCurrency,
                                    sm.IssuerIdentifier), 
                                new SpreadTimeBar(
                                    new Money(
                                        smp.Value.OpenPrice,
                                        SetCurrency(sm.SecurityCurrency)),
                                    new Money(
                                        smp.Value.OpenPrice,
                                        SetCurrency(sm.SecurityCurrency)),
                                    new Money(
                                        smp.Value.OpenPrice,
                                        SetCurrency(sm.SecurityCurrency)),
                                    new Volume(volume((double)CalculateADailyVolume(smp.Value)))
                                    ),
                                new DailySummaryTimeBar(
                                    smp.Value.MarketCapUsd,
                                    "USD",
                                    new IntradayPrices(
                                        new Money(
                                            smp.Value.OpenPrice,
                                            SetCurrency(sm.SecurityCurrency)),
                                        new Money(
                                            smp.Value.ClosePrice,
                                            SetCurrency(sm.SecurityCurrency)),
                                        new Money(
                                            smp.Value.HighIntradayPrice,
                                            SetCurrency(sm.SecurityCurrency)),
                                        new Money(
                                            smp.Value.LowIntradayPrice,
                                            SetCurrency(sm.SecurityCurrency))),
                                    null,
                                    new Volume(CalculateADailyVolume(smp.Value)),
                                    smp.Value.Epoch.Date.Add(_market.MarketOpenTime)),
                                smp.Value.Epoch.Date.Add(_market.MarketOpenTime),
                                new Market(
                                    null,
                                    _market.Code,
                                    _market.Name,
                                    MarketTypes.STOCKEXCHANGE))))
                    .GroupBy(x => x.TimeStamp)
                    .OrderBy(i => i.Key)
                    .ToList();

            var frames = 
                initialTicks
                    .Select(it =>
                        new EquityIntraDayTimeBarCollection(
                            new Market(
                                null,
                                _market.Code,
                                _market.Name,
                                MarketTypes.STOCKEXCHANGE),
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

        private string SetCfi(string cfi)
        {
            if (!string.IsNullOrWhiteSpace(cfi))
            {
                return cfi;
            }

            return "entspb";
        }

        private string SetCurrency(string currency)
        {
            if (!string.IsNullOrWhiteSpace(currency))
            {
                return currency;
            }

            return "GBP";
        }
    }
}
