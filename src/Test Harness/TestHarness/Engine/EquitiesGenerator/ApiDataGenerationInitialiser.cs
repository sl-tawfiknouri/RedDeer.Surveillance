using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Equity;
using Domain.Equity.Frames;
using Domain.Market;
using RedDeer.Contracts.SurveillanceService.Api.Markets;
using RedDeer.Contracts.SurveillanceService.Api.SecurityPrices;
using TestHarness.Engine.EquitiesGenerator.Interfaces;

namespace TestHarness.Engine.EquitiesGenerator
{
    public class ApiDataGenerationInitialiser : IApiDataGenerationInitialiser
    {
        private readonly ExchangeDto _market;
        private readonly IReadOnlyCollection<SecurityPriceDto> _securityPrices;

        public ApiDataGenerationInitialiser(
            ExchangeDto market,
            IReadOnlyCollection<SecurityPriceDto> securityPrices)
        {
            _market = market ?? throw new ArgumentNullException(nameof(market));
            _securityPrices = securityPrices ?? throw new ArgumentNullException(nameof(securityPrices));
        }

        public IReadOnlyCollection<ExchangeFrame> OrderedDailyFrames()
        {
 
            var initialTicks =
                _securityPrices
                    .SelectMany(sm =>
                        sm.Prices.Select(smp =>
                            new SecurityTick(
                                new Security(
                                    new SecurityIdentifiers(
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
                                    new Price(
                                        smp.Value.OpenPrice,
                                        sm.SecurityCurrency),
                                    new Price(
                                        smp.Value.OpenPrice,
                                        sm.SecurityCurrency),
                                    new Price(
                                        smp.Value.OpenPrice,
                                        sm.SecurityCurrency)),
                                new Volume(smp.Value.DailyVolume),
                                new Volume(smp.Value.DailyVolume),
                                smp.Value.Epoch.Date.Add(_market.MarketOpenTime),
                                smp.Value.MarketCapUsd,
                                new IntradayPrices(
                                    new Price(
                                        smp.Value.OpenPrice,
                                        sm.SecurityCurrency),
                                    new Price(
                                        smp.Value.ClosePrice,
                                        sm.SecurityCurrency),
                                    new Price(
                                        smp.Value.HighIntradayPrice,
                                        sm.SecurityCurrency),
                                    new Price(
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
    }
}
