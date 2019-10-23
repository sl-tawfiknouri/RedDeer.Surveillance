namespace TestHarness.Engine.EquitiesGenerator
{
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

    public class ApiDataGenerationInitialiser : IApiDataGenerationInitialiser
    {
        private readonly ExchangeDto _market;

        private readonly IReadOnlyCollection<SecurityPriceDto> _securityPrices;

        private readonly TimeSpan _tickFrequency;

        public ApiDataGenerationInitialiser(
            ExchangeDto market,
            TimeSpan tickFrequency,
            IReadOnlyCollection<SecurityPriceDto> securityPrices)
        {
            this._market = market ?? throw new ArgumentNullException(nameof(market));
            this._tickFrequency = tickFrequency;
            this._securityPrices = securityPrices ?? throw new ArgumentNullException(nameof(securityPrices));
        }

        public IReadOnlyCollection<EquityIntraDayTimeBarCollection> OrderedDailyFrames()
        {
            var close = this._market.MarketCloseTime;
            var open = this._market.MarketOpenTime;
            var openFor = close - open;
            var openMinutes = openFor.TotalMinutes;
            var ticksInDay = 0;

            if (openMinutes > 0 && this._tickFrequency.TotalMinutes > 0)
                ticksInDay = (int)Math.Ceiling(openMinutes / this._tickFrequency.TotalMinutes);

            Func<double, int> volume = x => ticksInDay == 0 ? (int)x : (int)Math.Ceiling(x * (1 / (double)ticksInDay));

            var initialTicks = this._securityPrices.SelectMany(
                    sm => sm.Prices.Select(
                        smp => new EquityInstrumentIntraDayTimeBar(
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
                                    sm.BloombergTicker,
                                    sm.Ric),
                                sm.SecurityName,
                                this.SetCfi(sm.Cfi),
                                sm.SecurityCurrency,
                                sm.IssuerIdentifier),
                            new SpreadTimeBar(
                                new Money(smp.Value.OpenPrice, this.SetCurrency(sm.SecurityCurrency)),
                                new Money(smp.Value.OpenPrice, this.SetCurrency(sm.SecurityCurrency)),
                                new Money(smp.Value.OpenPrice, this.SetCurrency(sm.SecurityCurrency)),
                                new Volume(volume(this.CalculateADailyVolume(smp.Value)))),
                            new DailySummaryTimeBar(
                                smp.Value.MarketCapUsd,
                                    "USD",
                                new IntradayPrices(
                                    new Money(smp.Value.OpenPrice, this.SetCurrency(sm.SecurityCurrency)),
                                    new Money(smp.Value.ClosePrice, this.SetCurrency(sm.SecurityCurrency)),
                                    new Money(smp.Value.HighIntradayPrice, this.SetCurrency(sm.SecurityCurrency)),
                                    new Money(smp.Value.LowIntradayPrice, this.SetCurrency(sm.SecurityCurrency))),
                                null,
                                new Volume(this.CalculateADailyVolume(smp.Value)),
                                smp.Value.Epoch.Date.Add(this._market.MarketOpenTime)),
                            smp.Value.Epoch.Date.Add(this._market.MarketOpenTime),
                            new Market(null, this._market.Code, this._market.Name, MarketTypes.STOCKEXCHANGE))))
                .GroupBy(x => x.TimeStamp).OrderBy(i => i.Key).ToList();

            var frames = initialTicks.Select(
                it => new EquityIntraDayTimeBarCollection(
                    new Market(null, this._market.Code, this._market.Name, MarketTypes.STOCKEXCHANGE),
                    it.Key,
                    it.ToList())).ToList();

            return frames;
        }

        private long CalculateADailyVolume(SecurityPriceSnapshotDto dto)
        {
            if (dto.DailyVolume > 0) return dto.DailyVolume;

            if (dto.MarketCapUsd == 0 || dto.OpenPrice == 0) return DiscreteUniform.Sample(1000000, 10000000);

            return (long)(dto.MarketCapUsd / dto.OpenPrice);
        }

        private string SetCfi(string cfi)
        {
            if (!string.IsNullOrWhiteSpace(cfi)) return cfi;

            return "entspb";
        }

        private string SetCurrency(string currency)
        {
            if (!string.IsNullOrWhiteSpace(currency)) return currency;

            return "GBP";
        }
    }
}