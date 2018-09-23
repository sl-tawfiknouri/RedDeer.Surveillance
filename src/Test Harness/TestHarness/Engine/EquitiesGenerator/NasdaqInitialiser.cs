using Domain.Market;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Equity;
using Domain.Equity.Frames;
using TestHarness.Engine.EquitiesGenerator.Interfaces;

namespace TestHarness.Engine.EquitiesGenerator
{
    public class NasdaqInitialiser : IExchangeSeriesInitialiser
    {
        private string _nasdaqCurrency = "USD";

        public ExchangeFrame InitialFrame()
        {
            var exchange = new StockExchange(new Market.MarketId("NASDAQ"), "NASDAQ");
            var nasdaqRaw = JsonConvert.DeserializeObject<NasdaqData[]>(InitialNasdaqDataJson);
            var securities = ProjectToSecurities(nasdaqRaw);

            var tick = new ExchangeFrame(exchange, DateTime.UtcNow, securities);

            return tick;
        }

        private List<SecurityTick> ProjectToSecurities(NasdaqData[] nasdaqRaw)
        {
            var rnd = new Random();
            var volume = rnd.Next(5000, 1000000);

            return nasdaqRaw.Select(raw =>
                new SecurityTick(
                    new Security(
                        new SecurityIdentifiers(
                            raw.Symbol,
                            raw.Symbol,
                            raw.Symbol,
                            raw.Symbol,
                            raw.Symbol,
                            raw.Symbol),
                        raw.Symbol,
                        "CFI"),
                     new Spread(
                        new Price(decimal.Parse(raw.Buy), _nasdaqCurrency),
                        new Price(decimal.Parse(raw.Sell), _nasdaqCurrency),
                        new Price(decimal.Parse(raw.Buy), _nasdaqCurrency)),
                    new Volume(volume),
                    DateTime.UtcNow,
                    decimal.Parse(raw.Buy) * volume,
                    new IntradayPrices(
                        new Price(decimal.Parse(raw.Buy), _nasdaqCurrency),
                        new Price(decimal.Parse(raw.Sell), _nasdaqCurrency),
                        new Price(decimal.Parse(raw.Buy) * 1.2m, _nasdaqCurrency),
                        new Price(decimal.Parse(raw.Sell) * 0.7m, _nasdaqCurrency)),
                    volume * 3))
                .ToList();
        }

        private class NasdaqData
        {
            public string Symbol { get; set; }
            public string Buy { get; set; }
            public string Sell { get; set; }
        }

        private static readonly string InitialNasdaqDataJson = @"
        [
          {
            'Symbol': 'AABA',
            'Buy': 69.84,
            'Sell': 69.14
          },
          {
            'Symbol': 'AAL',
            'Buy': 37.75,
            'Sell': 37.37
          },
          {
            'Symbol': 'AAME',
            'Buy': 2.55,
            'Sell': 2.52
          },
          {
            'Symbol': 'AAOI',
            'Buy': 39.45,
            'Sell': 39.06
          },
          {
            'Symbol': 'AAON',
            'Buy': 39.25,
            'Sell': 38.86
          },
          {
            'Symbol': 'AAPL',
            'Buy': 207.36,
            'Sell': 205.29
          },
          {
            'Symbol': 'AAWW',
            'Buy': 61.4,
            'Sell': 60.79
          },
          {
            'Symbol': 'AAXJ',
            'Buy': 71.28,
            'Sell': 70.57
          },
          {
            'Symbol': 'AAXN',
            'Buy': 59.27,
            'Sell': 58.68
          },
          {
            'Symbol': 'ABAC',
            'Buy': 2.27,
            'Sell': 2.25
          },
          {
            'Symbol': 'ABCB',
            'Buy': 48.6,
            'Sell': 48.11
          },
          {
            'Symbol': 'ABCD',
            'Buy': 11.76,
            'Sell': 11.64
          },
          {
            'Symbol': 'ABDC',
            'Buy': 6.17,
            'Sell': 6.11
          },
          {
            'Symbol': 'ABEO',
            'Buy': 14.3,
            'Sell': 14.16
          },
          {
            'Symbol': 'ABEOW',
            'Buy': 8.9,
            'Sell': 8.81
          },
          {
            'Symbol': 'ABIL',
            'Buy': 5.05,
            'Sell': 5
          },
          {
            'Symbol': 'ABIO',
            'Buy': 0.67,
            'Sell': 0.66
          },
          {
            'Symbol': 'ABMD',
            'Buy': 379.46,
            'Sell': 375.67
          },
          {
            'Symbol': 'ABTX',
            'Buy': 44.45,
            'Sell': 44.01
          },
          {
            'Symbol': 'ABUS',
            'Buy': 9.8,
            'Sell': 9.7
          },
          {
            'Symbol': 'ACAD',
            'Buy': 13.83,
            'Sell': 13.69
          },
          {
            'Symbol': 'ACBI',
            'Buy': 18.05,
            'Sell': 17.87
          },
          {
            'Symbol': 'ACER',
            'Buy': 30.07,
            'Sell': 29.77
          },
          {
            'Symbol': 'ACET',
            'Buy': 3.24,
            'Sell': 3.21
          },
          {
            'Symbol': 'ACGL',
            'Buy': 30.17,
            'Sell': 29.87
          },
          {
            'Symbol': 'ACGLO',
            'Buy': 24.56,
            'Sell': 24.31
          },
          {
            'Symbol': 'ACGLP',
            'Buy': 23.92,
            'Sell': 23.68
          },
          {
            'Symbol': 'ACHC',
            'Buy': 38.73,
            'Sell': 38.34
          },
          {
            'Symbol': 'ACHN',
            'Buy': 2.72,
            'Sell': 2.69
          },
          {
            'Symbol': 'ACHV',
            'Buy': 3.1,
            'Sell': 3.07
          },
          {
            'Symbol': 'ACIA',
            'Buy': 38.44,
            'Sell': 38.06
          },
          {
            'Symbol': 'ACIU',
            'Buy': 9.9,
            'Sell': 9.8
          },
          {
            'Symbol': 'ACIW',
            'Buy': 27.11,
            'Sell': 26.84
          },
          {
            'Symbol': 'ACLS',
            'Buy': 21.6,
            'Sell': 21.38
          },
          {
            'Symbol': 'ACMR',
            'Buy': 14.55,
            'Sell': 14.4
          },
          {
            'Symbol': 'ACNB',
            'Buy': 34.1,
            'Sell': 33.76
          },
          {
            'Symbol': 'ACOR',
            'Buy': 26.7,
            'Sell': 26.43
          },
          {
            'Symbol': 'ACRS',
            'Buy': 17.43,
            'Sell': 17.26
          },
          {
            'Symbol': 'ACRX',
            'Buy': 3.05,
            'Sell': 3.02
          },
          {
            'Symbol': 'ACSF',
            'Buy': 12.2,
            'Sell': 12.08
          },
          {
            'Symbol': 'ACST',
            'Buy': 0.55,
            'Sell': 0.54
          },
          {
            'Symbol': 'ACT',
            'Buy': 25.68,
            'Sell': 25.42
          },
          {
            'Symbol': 'ACTG',
            'Buy': 3.85,
            'Sell': 3.81
          },
          {
            'Symbol': 'ACWI',
            'Buy': 72.72,
            'Sell': 71.99
          },
          {
            'Symbol': 'ACWX',
            'Buy': 47.24,
            'Sell': 46.77
          },
          {
            'Symbol': 'ACXM',
            'Buy': 43.66,
            'Sell': 43.22
          },
          {
            'Symbol': 'ADAP',
            'Buy': 8.96,
            'Sell': 8.87
          },
          {
            'Symbol': 'ADBE',
            'Buy': 252,
            'Sell': 249.48
          },
          {
            'Symbol': 'ADES',
            'Buy': 11.28,
            'Sell': 11.17
          },
          {
            'Symbol': 'ADI',
            'Buy': 95.33,
            'Sell': 94.38
          },
          {
            'Symbol': 'ADIL',
            'Buy': 3.49,
            'Sell': 3.46
          },
          {
            'Symbol': 'ADILW',
            'Buy': 0.3,
            'Sell': 0.3
          },
          {
            'Symbol': 'ADMA',
            'Buy': 6.16,
            'Sell': 6.1
          },
          {
            'Symbol': 'ADMP',
            'Buy': 3.2,
            'Sell': 3.17
          },
          {
            'Symbol': 'ADMS',
            'Buy': 24.16,
            'Sell': 23.92
          },
          {
            'Symbol': 'ADOM',
            'Buy': 0.715,
            'Sell': 0.71
          },
          {
            'Symbol': 'ADP',
            'Buy': 137.77,
            'Sell': 136.39
          },
          {
            'Symbol': 'ADRA',
            'Buy': 32.85,
            'Sell': 32.52
          },
          {
            'Symbol': 'ADRD',
            'Buy': 22.45,
            'Sell': 22.23
          },
          {
            'Symbol': 'ADRE',
            'Buy': 41.1,
            'Sell': 40.69
          },
          {
            'Symbol': 'ADRO',
            'Buy': 5.3,
            'Sell': 5.25
          },
          {
            'Symbol': 'ADSK',
            'Buy': 134.27,
            'Sell': 132.93
          },
          {
            'Symbol': 'ADTN',
            'Buy': 15.85,
            'Sell': 15.69
          },
          {
            'Symbol': 'ADUS',
            'Buy': 64.5,
            'Sell': 63.86
          },
          {
            'Symbol': 'ADVM',
            'Buy': 5,
            'Sell': 4.95
          },
          {
            'Symbol': 'ADXS',
            'Buy': 1.45,
            'Sell': 1.44
          },
          {
            'Symbol': 'AEGN',
            'Buy': 24.89,
            'Sell': 24.64
          },
          {
            'Symbol': 'AEHR',
            'Buy': 2.31,
            'Sell': 2.29
          },
          {
            'Symbol': 'AEIS',
            'Buy': 60.49,
            'Sell': 59.89
          },
          {
            'Symbol': 'AEMD',
            'Buy': 1.2,
            'Sell': 1.19
          },
          {
            'Symbol': 'AERI',
            'Buy': 63.8,
            'Sell': 63.16
          },
          {
            'Symbol': 'AETI',
            'Buy': 1.19,
            'Sell': 1.18
          },
          {
            'Symbol': 'AEY',
            'Buy': 1.46,
            'Sell': 1.45
          },
          {
            'Symbol': 'AEZS',
            'Buy': 1.65,
            'Sell': 1.63
          },
          {
            'Symbol': 'AFH',
            'Buy': 11.05,
            'Sell': 10.94
          },
          {
            'Symbol': 'AFHBL',
            'Buy': 25.35,
            'Sell': 25.1
          },
          {
            'Symbol': 'AFIN',
            'Buy': 14.81,
            'Sell': 14.66
          },
          {
            'Symbol': 'AFMD',
            'Buy': 1.55,
            'Sell': 1.53
          },
          {
            'Symbol': 'AFSI',
            'Buy': 14.37,
            'Sell': 14.23
          },
          {
            'Symbol': 'AGEN',
            'Buy': 1.76,
            'Sell': 1.74
          },
          {
            'Symbol': 'AGFS',
            'Buy': 6.82,
            'Sell': 6.75
          },
          {
            'Symbol': 'AGFSW',
            'Buy': 0.3,
            'Sell': 0.3
          },
          {
            'Symbol': 'AGIO',
            'Buy': 77.91,
            'Sell': 77.13
          },
          {
            'Symbol': 'AGLE',
            'Buy': 9.5,
            'Sell': 9.41
          },
          {
            'Symbol': 'AGMH',
            'Buy': 14.25,
            'Sell': 14.11
          },
          {
            'Symbol': 'AGNC',
            'Buy': 18.94,
            'Sell': 18.75
          },
          {
            'Symbol': 'AGNCB',
            'Buy': 25.91,
            'Sell': 25.65
          },
          {
            'Symbol': 'AGNCN',
            'Buy': 25.98,
            'Sell': 25.72
          },
          {
            'Symbol': 'AGND',
            'Buy': 44.045,
            'Sell': 43.6
          },
          {
            'Symbol': 'AGRX',
            'Buy': 0.28,
            'Sell': 0.28
          },
          {
            'Symbol': 'AGTC',
            'Buy': 4.1,
            'Sell': 4.06
          },
          {
            'Symbol': 'AGYS',
            'Buy': 15.76,
            'Sell': 15.6
          },
          {
            'Symbol': 'AGZD',
            'Buy': 47.92,
            'Sell': 47.44
          },
          {
            'Symbol': 'AHPA',
            'Buy': 10.18,
            'Sell': 10.08
          },
          {
            'Symbol': 'AHPAW',
            'Buy': 0.41,
            'Sell': 0.41
          },
          {
            'Symbol': 'AHPI',
            'Buy': 2.4,
            'Sell': 2.38
          },
          {
            'Symbol': 'AIA',
            'Buy': 62.43,
            'Sell': 61.81
          },
          {
            'Symbol': 'AIHS',
            'Buy': 4.28,
            'Sell': 4.24
          },
          {
            'Symbol': 'AIMC',
            'Buy': 41.4,
            'Sell': 40.99
          },
          {
            'Symbol': 'AIMT',
            'Buy': 28.32,
            'Sell': 28.04
          },
          {
            'Symbol': 'AINV',
            'Buy': 5.73,
            'Sell': 5.67
          },
          {
            'Symbol': 'AIPT',
            'Buy': 1.27,
            'Sell': 1.26
          },
          {
            'Symbol': 'AIQ',
            'Buy': 15.32,
            'Sell': 15.17
          },
          {
            'Symbol': 'AIRG',
            'Buy': 11,
            'Sell': 10.89
          },
          {
            'Symbol': 'AIRR',
            'Buy': 27.64,
            'Sell': 27.36
          },
          {
            'Symbol': 'AIRT',
            'Buy': 33.477,
            'Sell': 33.14
          },
          {
            'Symbol': 'AKAM',
            'Buy': 75.9,
            'Sell': 75.14
          },
          {
            'Symbol': 'AKAO',
            'Buy': 6.34,
            'Sell': 6.28
          },
          {
            'Symbol': 'AKBA',
            'Buy': 8.2,
            'Sell': 8.12
          },
          {
            'Symbol': 'AKCA',
            'Buy': 36.53,
            'Sell': 36.16
          },
          {
            'Symbol': 'AKER',
            'Buy': 0.25,
            'Sell': 0.25
          },
          {
            'Symbol': 'AKRX',
            'Buy': 18.79,
            'Sell': 18.6
          },
          {
            'Symbol': 'AKTS',
            'Buy': 7.94,
            'Sell': 7.86
          },
          {
            'Symbol': 'AKTX',
            'Buy': 1.89,
            'Sell': 1.87
          },
          {
            'Symbol': 'ALBO',
            'Buy': 31.7,
            'Sell': 31.38
          },
          {
            'Symbol': 'ALCO',
            'Buy': 31.85,
            'Sell': 31.53
          },
          {
            'Symbol': 'ALDR',
            'Buy': 19.5,
            'Sell': 19.31
          },
          {
            'Symbol': 'ALDX',
            'Buy': 7.55,
            'Sell': 7.47
          },
          {
            'Symbol': 'ALGN',
            'Buy': 363.71,
            'Sell': 360.07
          },
          {
            'Symbol': 'ALGRU',
            'Buy': 10.19,
            'Sell': 10.09
          },
          {
            'Symbol': 'ALGT',
            'Buy': 130.05,
            'Sell': 128.75
          },
          {
            'Symbol': 'ALIM',
            'Buy': 0.86,
            'Sell': 0.85
          },
          {
            'Symbol': 'ALJJ',
            'Buy': 2.11,
            'Sell': 2.09
          },
          {
            'Symbol': 'ALKS',
            'Buy': 44.59,
            'Sell': 44.14
          },
          {
            'Symbol': 'ALLK',
            'Buy': 36.71,
            'Sell': 36.34
          },
          {
            'Symbol': 'ALLT',
            'Buy': 6.28,
            'Sell': 6.22
          },
          {
            'Symbol': 'ALNA',
            'Buy': 10.7,
            'Sell': 10.59
          },
          {
            'Symbol': 'ALNY',
            'Buy': 94,
            'Sell': 93.06
          },
          {
            'Symbol': 'ALOT',
            'Buy': 17.95,
            'Sell': 17.77
          },
          {
            'Symbol': 'ALPN',
            'Buy': 6.87,
            'Sell': 6.8
          },
          {
            'Symbol': 'ALQA',
            'Buy': 2.213,
            'Sell': 2.19
          },
          {
            'Symbol': 'ALRM',
            'Buy': 49.15,
            'Sell': 48.66
          },
          {
            'Symbol': 'ALRN',
            'Buy': 2.48,
            'Sell': 2.46
          },
          {
            'Symbol': 'ALSK',
            'Buy': 1.73,
            'Sell': 1.71
          },
          {
            'Symbol': 'ALT',
            'Buy': 0.33,
            'Sell': 0.33
          },
          {
            'Symbol': 'ALTR',
            'Buy': 37.5,
            'Sell': 37.13
          },
          {
            'Symbol': 'ALTY',
            'Buy': 15.4,
            'Sell': 15.25
          },
          {
            'Symbol': 'ALXN',
            'Buy': 119.54,
            'Sell': 118.34
          },
          {
            'Symbol': 'AMAG',
            'Buy': 25.2,
            'Sell': 24.95
          },
          {
            'Symbol': 'AMAL',
            'Buy': 16.39,
            'Sell': 16.23
          },
          {
            'Symbol': 'AMAT',
            'Buy': 48.18,
            'Sell': 47.7
          },
          {
            'Symbol': 'AMBA',
            'Buy': 39.12,
            'Sell': 38.73
          },
          {
            'Symbol': 'AMBC',
            'Buy': 21.62,
            'Sell': 21.4
          },
          {
            'Symbol': 'AMBCW',
            'Buy': 10.43,
            'Sell': 10.33
          },
          {
            'Symbol': 'AMCN',
            'Buy': 0.5,
            'Sell': 0.5
          },
          {
            'Symbol': 'AMCX',
            'Buy': 60.32,
            'Sell': 59.72
          },
          {
            'Symbol': 'AMD',
            'Buy': 19.09,
            'Sell': 18.9
          },
          {
            'Symbol': 'AMDA',
            'Buy': 0.48,
            'Sell': 0.48
          },
          {
            'Symbol': 'AMED',
            'Buy': 112.33,
            'Sell': 111.21
          },
          {
            'Symbol': 'AMEH',
            'Buy': 23.52,
            'Sell': 23.28
          },
          {
            'Symbol': 'AMGN',
            'Buy': 193.3,
            'Sell': 191.37
          },
          {
            'Symbol': 'AMKR',
            'Buy': 9.12,
            'Sell': 9.03
          },
          {
            'Symbol': 'AMMA',
            'Buy': 0.17,
            'Sell': 0.17
          },
          {
            'Symbol': 'AMNB',
            'Buy': 40.7,
            'Sell': 40.29
          },
          {
            'Symbol': 'AMOT',
            'Buy': 49.73,
            'Sell': 49.23
          },
          {
            'Symbol': 'AMPH',
            'Buy': 17.11,
            'Sell': 16.94
          },
          {
            'Symbol': 'AMR',
            'Buy': 6.12,
            'Sell': 6.06
          },
          {
            'Symbol': 'AMRB',
            'Buy': 15.65,
            'Sell': 15.49
          },
          {
            'Symbol': 'AMRH',
            'Buy': 1.62,
            'Sell': 1.6
          },
          {
            'Symbol': 'AMRK',
            'Buy': 13.63,
            'Sell': 13.49
          },
          {
            'Symbol': 'AMRN',
            'Buy': 2.86,
            'Sell': 2.83
          },
          {
            'Symbol': 'AMRS',
            'Buy': 7.88,
            'Sell': 7.8
          },
          {
            'Symbol': 'AMRWW',
            'Buy': 1,
            'Sell': 0.99
          },
          {
            'Symbol': 'AMSC',
            'Buy': 6.45,
            'Sell': 6.39
          },
          {
            'Symbol': 'AMSF',
            'Buy': 62.7,
            'Sell': 62.07
          },
          {
            'Symbol': 'AMSWA',
            'Buy': 15.83,
            'Sell': 15.67
          },
          {
            'Symbol': 'AMTD',
            'Buy': 58.21,
            'Sell': 57.63
          },
          {
            'Symbol': 'AMTX',
            'Buy': 1.35,
            'Sell': 1.34
          },
          {
            'Symbol': 'AMWD',
            'Buy': 85.1,
            'Sell': 84.25
          },
          {
            'Symbol': 'AMZN',
            'Buy': 1888.51,
            'Sell': 1869.62
          },
          {
            'Symbol': 'ANAB',
            'Buy': 81.33,
            'Sell': 80.52
          },
          {
            'Symbol': 'ANAT',
            'Buy': 128.6,
            'Sell': 127.31
          },
          {
            'Symbol': 'ANCB',
            'Buy': 29.43,
            'Sell': 29.14
          },
          {
            'Symbol': 'ANCX',
            'Buy': 27.55,
            'Sell': 27.27
          },
          {
            'Symbol': 'ANDE',
            'Buy': 40.25,
            'Sell': 39.85
          },
          {
            'Symbol': 'ANGI',
            'Buy': 18.23,
            'Sell': 18.05
          },
          {
            'Symbol': 'ANGO',
            'Buy': 21.46,
            'Sell': 21.25
          },
          {
            'Symbol': 'ANIK',
            'Buy': 41.75,
            'Sell': 41.33
          },
          {
            'Symbol': 'ANIP',
            'Buy': 56.31,
            'Sell': 55.75
          },
          {
            'Symbol': 'ANSS',
            'Buy': 171.16,
            'Sell': 169.45
          },
          {
            'Symbol': 'ANY',
            'Buy': 0.31,
            'Sell': 0.31
          },
          {
            'Symbol': 'AOBC',
            'Buy': 9.56,
            'Sell': 9.46
          },
          {
            'Symbol': 'AOSL',
            'Buy': 15.44,
            'Sell': 15.29
          },
          {
            'Symbol': 'APDN',
            'Buy': 1.5,
            'Sell': 1.49
          },
          {
            'Symbol': 'APDNW',
            'Buy': 0.25,
            'Sell': 0.25
          },
          {
            'Symbol': 'APEI',
            'Buy': 34.15,
            'Sell': 33.81
          },
          {
            'Symbol': 'APEN',
            'Buy': 7.8,
            'Sell': 7.72
          },
          {
            'Symbol': 'APLS',
            'Buy': 18.07,
            'Sell': 17.89
          },
          {
            'Symbol': 'APOG',
            'Buy': 49.5,
            'Sell': 49.01
          },
          {
            'Symbol': 'APOP',
            'Buy': 5.26,
            'Sell': 5.21
          },
          {
            'Symbol': 'APPF',
            'Buy': 76.05,
            'Sell': 75.29
          },
          {
            'Symbol': 'APPN',
            'Buy': 35.23,
            'Sell': 34.88
          },
          {
            'Symbol': 'APPS',
            'Buy': 1.42,
            'Sell': 1.41
          },
          {
            'Symbol': 'APRI',
            'Buy': 0.28,
            'Sell': 0.28
          },
          {
            'Symbol': 'APTI',
            'Buy': 35.73,
            'Sell': 35.37
          },
          {
            'Symbol': 'APTO',
            'Buy': 3.03,
            'Sell': 3
          },
          {
            'Symbol': 'APTX',
            'Buy': 21.41,
            'Sell': 21.2
          },
          {
            'Symbol': 'APVO',
            'Buy': 4.61,
            'Sell': 4.56
          },
          {
            'Symbol': 'APWC',
            'Buy': 2.65,
            'Sell': 2.62
          },
          {
            'Symbol': 'AQB',
            'Buy': 2.49,
            'Sell': 2.47
          },
          {
            'Symbol': 'AQMS',
            'Buy': 2.29,
            'Sell': 2.27
          },
          {
            'Symbol': 'AQST',
            'Buy': 15.03,
            'Sell': 14.88
          },
          {
            'Symbol': 'AQXP',
            'Buy': 3.13,
            'Sell': 3.1
          },
          {
            'Symbol': 'ARAY',
            'Buy': 3.95,
            'Sell': 3.91
          },
          {
            'Symbol': 'ARCB',
            'Buy': 47.75,
            'Sell': 47.27
          },
          {
            'Symbol': 'ARCC',
            'Buy': 17.38,
            'Sell': 17.21
          },
          {
            'Symbol': 'ARCI',
            'Buy': 0.631,
            'Sell': 0.62
          },
          {
            'Symbol': 'ARCT',
            'Buy': 7.77,
            'Sell': 7.69
          },
          {
            'Symbol': 'ARCW',
            'Buy': 2.045,
            'Sell': 2.02
          },
          {
            'Symbol': 'ARDM',
            'Buy': 1.36,
            'Sell': 1.35
          },
          {
            'Symbol': 'ARDX',
            'Buy': 4.2,
            'Sell': 4.16
          },
          {
            'Symbol': 'AREX',
            'Buy': 2.34,
            'Sell': 2.32
          },
          {
            'Symbol': 'ARGX',
            'Buy': 90.75,
            'Sell': 89.84
          },
          {
            'Symbol': 'ARII',
            'Buy': 46.81,
            'Sell': 46.34
          },
          {
            'Symbol': 'ARKR',
            'Buy': 22.06,
            'Sell': 21.84
          },
          {
            'Symbol': 'ARLP',
            'Buy': 20,
            'Sell': 19.8
          },
          {
            'Symbol': 'ARLZ',
            'Buy': 0.06,
            'Sell': 0.06
          },
          {
            'Symbol': 'ARNA',
            'Buy': 37.24,
            'Sell': 36.87
          },
          {
            'Symbol': 'AROW',
            'Buy': 39.35,
            'Sell': 38.96
          },
          {
            'Symbol': 'ARPO',
            'Buy': 3.89,
            'Sell': 3.85
          },
          {
            'Symbol': 'ARQL',
            'Buy': 5.44,
            'Sell': 5.39
          },
          {
            'Symbol': 'ARRS',
            'Buy': 24.55,
            'Sell': 24.3
          },
          {
            'Symbol': 'ARRY',
            'Buy': 14.89,
            'Sell': 14.74
          },
          {
            'Symbol': 'ARTNA',
            'Buy': 36.93,
            'Sell': 36.56
          },
          {
            'Symbol': 'ARTW',
            'Buy': 2.8,
            'Sell': 2.77
          },
          {
            'Symbol': 'ARTX',
            'Buy': 3.55,
            'Sell': 3.51
          },
          {
            'Symbol': 'ARWR',
            'Buy': 16.34,
            'Sell': 16.18
          },
          {
            'Symbol': 'ASCMA',
            'Buy': 2.52,
            'Sell': 2.49
          },
          {
            'Symbol': 'ASET',
            'Buy': 28.02,
            'Sell': 27.74
          },
          {
            'Symbol': 'ASFI',
            'Buy': 3.1,
            'Sell': 3.07
          },
          {
            'Symbol': 'ASLN',
            'Buy': 7.6,
            'Sell': 7.52
          },
          {
            'Symbol': 'ASMB',
            'Buy': 39.24,
            'Sell': 38.85
          },
          {
            'Symbol': 'ASML',
            'Buy': 204.74,
            'Sell': 202.69
          },
          {
            'Symbol': 'ASNA',
            'Buy': 4.14,
            'Sell': 4.1
          },
          {
            'Symbol': 'ASND',
            'Buy': 66.21,
            'Sell': 65.55
          },
          {
            'Symbol': 'ASNS',
            'Buy': 2.23,
            'Sell': 2.21
          },
          {
            'Symbol': 'ASPS',
            'Buy': 34.75,
            'Sell': 34.4
          },
          {
            'Symbol': 'ASPU',
            'Buy': 6.8,
            'Sell': 6.73
          },
          {
            'Symbol': 'ASRV',
            'Buy': 4.5,
            'Sell': 4.46
          },
          {
            'Symbol': 'ASRVP',
            'Buy': 28.75,
            'Sell': 28.46
          },
          {
            'Symbol': 'ASTC',
            'Buy': 3.95,
            'Sell': 3.91
          },
          {
            'Symbol': 'ASTE',
            'Buy': 45.58,
            'Sell': 45.12
          },
          {
            'Symbol': 'ASUR',
            'Buy': 15.71,
            'Sell': 15.55
          },
          {
            'Symbol': 'ASV',
            'Buy': 6.9,
            'Sell': 6.83
          },
          {
            'Symbol': 'ASYS',
            'Buy': 5.4,
            'Sell': 5.35
          },
          {
            'Symbol': 'ATAC',
            'Buy': 10.6,
            'Sell': 10.49
          },
          {
            'Symbol': 'ATACR',
            'Buy': 1.02,
            'Sell': 1.01
          },
          {
            'Symbol': 'ATAI',
            'Buy': 5.99,
            'Sell': 5.93
          },
          {
            'Symbol': 'ATAX',
            'Buy': 6.35,
            'Sell': 6.29
          },
          {
            'Symbol': 'ATEC',
            'Buy': 3.12,
            'Sell': 3.09
          },
          {
            'Symbol': 'ATHN',
            'Buy': 150.81,
            'Sell': 149.3
          },
          {
            'Symbol': 'ATHX',
            'Buy': 2,
            'Sell': 1.98
          },
          {
            'Symbol': 'ATIS',
            'Buy': 0.375,
            'Sell': 0.37
          },
          {
            'Symbol': 'ATLC',
            'Buy': 1.71,
            'Sell': 1.69
          },
          {
            'Symbol': 'ATLO',
            'Buy': 30,
            'Sell': 29.7
          },
          {
            'Symbol': 'ATNI',
            'Buy': 68.73,
            'Sell': 68.04
          },
          {
            'Symbol': 'ATNX',
            'Buy': 18.35,
            'Sell': 18.17
          },
          {
            'Symbol': 'ATOM',
            'Buy': 5.75,
            'Sell': 5.69
          },
          {
            'Symbol': 'ATOS',
            'Buy': 2.26,
            'Sell': 2.24
          },
          {
            'Symbol': 'ATRA',
            'Buy': 37.35,
            'Sell': 36.98
          },
          {
            'Symbol': 'ATRC',
            'Buy': 30.7,
            'Sell': 30.39
          },
          {
            'Symbol': 'ATRI',
            'Buy': 641.45,
            'Sell': 635.04
          },
          {
            'Symbol': 'ATRO',
            'Buy': 44.31,
            'Sell': 43.87
          },
          {
            'Symbol': 'ATRS',
            'Buy': 2.94,
            'Sell': 2.91
          },
          {
            'Symbol': 'ATSG',
            'Buy': 20.82,
            'Sell': 20.61
          },
          {
            'Symbol': 'ATTU',
            'Buy': 18.66,
            'Sell': 18.47
          },
          {
            'Symbol': 'ATVI',
            'Buy': 70.1,
            'Sell': 69.4
          },
          {
            'Symbol': 'ATXI',
            'Buy': 3.92,
            'Sell': 3.88
          },
          {
            'Symbol': 'AUBN',
            'Buy': 46.69,
            'Sell': 46.22
          },
          {
            'Symbol': 'AUDC',
            'Buy': 10.25,
            'Sell': 10.15
          },
          {
            'Symbol': 'AUPH',
            'Buy': 5.46,
            'Sell': 5.41
          },
          {
            'Symbol': 'AUTL',
            'Buy': 24.64,
            'Sell': 24.39
          },
          {
            'Symbol': 'AUTO',
            'Buy': 3.64,
            'Sell': 3.6
          },
          {
            'Symbol': 'AVAV',
            'Buy': 78.63,
            'Sell': 77.84
          },
          {
            'Symbol': 'AVDL',
            'Buy': 5.11,
            'Sell': 5.06
          },
          {
            'Symbol': 'AVEO',
            'Buy': 2.24,
            'Sell': 2.22
          },
          {
            'Symbol': 'AVGO',
            'Buy': 212.81,
            'Sell': 210.68
          },
          {
            'Symbol': 'AVGR',
            'Buy': 1.19,
            'Sell': 1.18
          },
          {
            'Symbol': 'AVHI',
            'Buy': 21.4,
            'Sell': 21.19
          },
          {
            'Symbol': 'AVID',
            'Buy': 5.04,
            'Sell': 4.99
          },
          {
            'Symbol': 'AVNW',
            'Buy': 15.25,
            'Sell': 15.1
          },
          {
            'Symbol': 'AVRO',
            'Buy': 37.7,
            'Sell': 37.32
          },
          {
            'Symbol': 'AVT',
            'Buy': 47,
            'Sell': 46.53
          },
          {
            'Symbol': 'AVXL',
            'Buy': 2.79,
            'Sell': 2.76
          },
          {
            'Symbol': 'AWRE',
            'Buy': 3.9,
            'Sell': 3.86
          },
          {
            'Symbol': 'AWSM',
            'Buy': 4.24,
            'Sell': 4.2
          },
          {
            'Symbol': 'AXAS',
            'Buy': 2.15,
            'Sell': 2.13
          },
          {
            'Symbol': 'AXDX',
            'Buy': 22.2,
            'Sell': 21.98
          },
          {
            'Symbol': 'AXGN',
            'Buy': 39.15,
            'Sell': 38.76
          },
          {
            'Symbol': 'AXON',
            'Buy': 2.05,
            'Sell': 2.03
          },
          {
            'Symbol': 'AXSM',
            'Buy': 2.55,
            'Sell': 2.52
          },
          {
            'Symbol': 'AXTI',
            'Buy': 7.8,
            'Sell': 7.72
          },
          {
            'Symbol': 'AY',
            'Buy': 20.78,
            'Sell': 20.57
          },
          {
            'Symbol': 'AYTU',
            'Buy': 0.28,
            'Sell': 0.28
          },
          {
            'Symbol': 'AZPN',
            'Buy': 104.88,
            'Sell': 103.83
          },
          {
            'Symbol': 'AZRX',
            'Buy': 2.3,
            'Sell': 2.28
          },
          {
            'Symbol': 'BABY',
            'Buy': 35.75,
            'Sell': 35.39
          },
          {
            'Symbol': 'BAND',
            'Buy': 42.96,
            'Sell': 42.53
          },
          {
            'Symbol': 'BANF',
            'Buy': 62.2,
            'Sell': 61.58
          },
          {
            'Symbol': 'BANFP',
            'Buy': 27.245,
            'Sell': 26.97
          },
          {
            'Symbol': 'BANR',
            'Buy': 64.69,
            'Sell': 64.04
          },
          {
            'Symbol': 'BANX',
            'Buy': 21.757,
            'Sell': 21.54
          },
          {
            'Symbol': 'BASI',
            'Buy': 1.6,
            'Sell': 1.58
          },
          {
            'Symbol': 'BATRA',
            'Buy': 25.99,
            'Sell': 25.73
          },
          {
            'Symbol': 'BATRK',
            'Buy': 25.9,
            'Sell': 25.64
          },
          {
            'Symbol': 'BBBY',
            'Buy': 18.6,
            'Sell': 18.41
          },
          {
            'Symbol': 'BBGI',
            'Buy': 6.8,
            'Sell': 6.73
          },
          {
            'Symbol': 'BBH',
            'Buy': 129.46,
            'Sell': 128.17
          },
          {
            'Symbol': 'BBOX',
            'Buy': 1.45,
            'Sell': 1.44
          },
          {
            'Symbol': 'BBSI',
            'Buy': 71.92,
            'Sell': 71.2
          },
          {
            'Symbol': 'BCBP',
            'Buy': 14.75,
            'Sell': 14.6
          },
          {
            'Symbol': 'BCLI',
            'Buy': 3.99,
            'Sell': 3.95
          },
          {
            'Symbol': 'BCML',
            'Buy': 26.14,
            'Sell': 25.88
          },
          {
            'Symbol': 'BCOR',
            'Buy': 34.35,
            'Sell': 34.01
          },
          {
            'Symbol': 'BCOV',
            'Buy': 8.05,
            'Sell': 7.97
          },
          {
            'Symbol': 'BCPC',
            'Buy': 100.45,
            'Sell': 99.45
          },
          {
            'Symbol': 'BCRX',
            'Buy': 7.14,
            'Sell': 7.07
          },
          {
            'Symbol': 'BCTF',
            'Buy': 15.25,
            'Sell': 15.1
          },
          {
            'Symbol': 'BDGE',
            'Buy': 34.9,
            'Sell': 34.55
          },
          {
            'Symbol': 'BDSI',
            'Buy': 2.75,
            'Sell': 2.72
          },
          {
            'Symbol': 'BEAT',
            'Buy': 56.65,
            'Sell': 56.08
          },
          {
            'Symbol': 'BECN',
            'Buy': 37.61,
            'Sell': 37.23
          },
          {
            'Symbol': 'BELFB',
            'Buy': 24.45,
            'Sell': 24.21
          },
          {
            'Symbol': 'BFIN',
            'Buy': 15.59,
            'Sell': 15.43
          },
          {
            'Symbol': 'BFIT',
            'Buy': 19.3,
            'Sell': 19.11
          },
          {
            'Symbol': 'BFRA',
            'Buy': 13.35,
            'Sell': 13.22
          },
          {
            'Symbol': 'BFST',
            'Buy': 25.08,
            'Sell': 24.83
          },
          {
            'Symbol': 'BGCP',
            'Buy': 11.07,
            'Sell': 10.96
          },
          {
            'Symbol': 'BGFV',
            'Buy': 5.65,
            'Sell': 5.59
          },
          {
            'Symbol': 'BGNE',
            'Buy': 171.68,
            'Sell': 169.96
          },
          {
            'Symbol': 'BHACR',
            'Buy': 0.16,
            'Sell': 0.16
          },
          {
            'Symbol': 'BHACW',
            'Buy': 0.12,
            'Sell': 0.12
          },
          {
            'Symbol': 'BHBK',
            'Buy': 21.85,
            'Sell': 21.63
          },
          {
            'Symbol': 'BHF',
            'Buy': 41.76,
            'Sell': 41.34
          },
          {
            'Symbol': 'BHTG',
            'Buy': 4.05,
            'Sell': 4.01
          },
          {
            'Symbol': 'BIB',
            'Buy': 64.92,
            'Sell': 64.27
          },
          {
            'Symbol': 'BICK',
            'Buy': 27.22,
            'Sell': 26.95
          },
          {
            'Symbol': 'BIDU',
            'Buy': 217.5,
            'Sell': 215.33
          },
          {
            'Symbol': 'BIIB',
            'Buy': 344.93,
            'Sell': 341.48
          },
          {
            'Symbol': 'BILI',
            'Buy': 11.25,
            'Sell': 11.14
          },
          {
            'Symbol': 'BIOC',
            'Buy': 3.97,
            'Sell': 3.93
          },
          {
            'Symbol': 'BIOL',
            'Buy': 1.22,
            'Sell': 1.21
          },
          {
            'Symbol': 'BIOS',
            'Buy': 2.79,
            'Sell': 2.76
          },
          {
            'Symbol': 'BIS',
            'Buy': 17.04,
            'Sell': 16.87
          },
          {
            'Symbol': 'BJRI',
            'Buy': 63.5,
            'Sell': 62.87
          },
          {
            'Symbol': 'BKCC',
            'Buy': 6.07,
            'Sell': 6.01
          },
          {
            'Symbol': 'BKEP',
            'Buy': 2.85,
            'Sell': 2.82
          },
          {
            'Symbol': 'BKEPP',
            'Buy': 6.62,
            'Sell': 6.55
          },
          {
            'Symbol': 'BKNG',
            'Buy': 1909.85,
            'Sell': 1890.75
          },
          {
            'Symbol': 'BKSC',
            'Buy': 20,
            'Sell': 19.8
          },
          {
            'Symbol': 'BKYI',
            'Buy': 1.81,
            'Sell': 1.79
          },
          {
            'Symbol': 'BL',
            'Buy': 47.06,
            'Sell': 46.59
          },
          {
            'Symbol': 'BLBD',
            'Buy': 22.75,
            'Sell': 22.52
          },
          {
            'Symbol': 'BLCM',
            'Buy': 6.23,
            'Sell': 6.17
          },
          {
            'Symbol': 'BLCN',
            'Buy': 23.03,
            'Sell': 22.8
          },
          {
            'Symbol': 'BLDP',
            'Buy': 3.21,
            'Sell': 3.18
          },
          {
            'Symbol': 'BLDR',
            'Buy': 17.87,
            'Sell': 17.69
          },
          {
            'Symbol': 'BLFS',
            'Buy': 19.07,
            'Sell': 18.88
          },
          {
            'Symbol': 'BLIN',
            'Buy': 1.35,
            'Sell': 1.34
          },
          {
            'Symbol': 'BLKB',
            'Buy': 95.78,
            'Sell': 94.82
          },
          {
            'Symbol': 'BLMN',
            'Buy': 18.47,
            'Sell': 18.29
          },
          {
            'Symbol': 'BLMT',
            'Buy': 34.1,
            'Sell': 33.76
          },
          {
            'Symbol': 'BLNK',
            'Buy': 3.92,
            'Sell': 3.88
          },
          {
            'Symbol': 'BLNKW',
            'Buy': 1.16,
            'Sell': 1.15
          },
          {
            'Symbol': 'BLPH',
            'Buy': 0.68,
            'Sell': 0.67
          },
          {
            'Symbol': 'BLRX',
            'Buy': 0.86,
            'Sell': 0.85
          },
          {
            'Symbol': 'BLUE',
            'Buy': 155.15,
            'Sell': 153.6
          },
          {
            'Symbol': 'BMCH',
            'Buy': 22.5,
            'Sell': 22.28
          },
          {
            'Symbol': 'BMRA',
            'Buy': 3.8,
            'Sell': 3.76
          },
          {
            'Symbol': 'BMRC',
            'Buy': 87,
            'Sell': 86.13
          },
          {
            'Symbol': 'BMRN',
            'Buy': 101.32,
            'Sell': 100.31
          },
          {
            'Symbol': 'BMTC',
            'Buy': 48.4,
            'Sell': 47.92
          },
          {
            'Symbol': 'BNCL',
            'Buy': 18.15,
            'Sell': 17.97
          },
          {
            'Symbol': 'BND',
            'Buy': 79.02,
            'Sell': 78.23
          },
          {
            'Symbol': 'BNDX',
            'Buy': 54.74,
            'Sell': 54.19
          },
          {
            'Symbol': 'BNFT',
            'Buy': 35.7,
            'Sell': 35.34
          },
          {
            'Symbol': 'BNSO',
            'Buy': 3.27,
            'Sell': 3.24
          },
          {
            'Symbol': 'BNTC',
            'Buy': 2.66,
            'Sell': 2.63
          },
          {
            'Symbol': 'BOCH',
            'Buy': 12.5,
            'Sell': 12.38
          },
          {
            'Symbol': 'BOFI',
            'Buy': 36.92,
            'Sell': 36.55
          },
          {
            'Symbol': 'BOJA',
            'Buy': 13.75,
            'Sell': 13.61
          },
          {
            'Symbol': 'BOKF',
            'Buy': 98.16,
            'Sell': 97.18
          },
          {
            'Symbol': 'BOKFL',
            'Buy': 25,
            'Sell': 24.75
          },
          {
            'Symbol': 'BOLD',
            'Buy': 36.12,
            'Sell': 35.76
          },
          {
            'Symbol': 'BOMN',
            'Buy': 22.56,
            'Sell': 22.33
          },
          {
            'Symbol': 'BOOM',
            'Buy': 42.75,
            'Sell': 42.32
          },
          {
            'Symbol': 'BOSC',
            'Buy': 2.52,
            'Sell': 2.49
          },
          {
            'Symbol': 'BOTJ',
            'Buy': 16.05,
            'Sell': 15.89
          },
          {
            'Symbol': 'BOTZ',
            'Buy': 22.21,
            'Sell': 21.99
          },
          {
            'Symbol': 'BOXL',
            'Buy': 4,
            'Sell': 3.96
          },
          {
            'Symbol': 'BPFH',
            'Buy': 14,
            'Sell': 13.86
          },
          {
            'Symbol': 'BPMC',
            'Buy': 68.32,
            'Sell': 67.64
          },
          {
            'Symbol': 'BPOP',
            'Buy': 49.89,
            'Sell': 49.39
          },
          {
            'Symbol': 'BPOPM',
            'Buy': 24.2,
            'Sell': 23.96
          },
          {
            'Symbol': 'BPOPN',
            'Buy': 25.3,
            'Sell': 25.05
          },
          {
            'Symbol': 'BPRN',
            'Buy': 33.41,
            'Sell': 33.08
          },
          {
            'Symbol': 'BPTH',
            'Buy': 1.63,
            'Sell': 1.61
          },
          {
            'Symbol': 'BPY',
            'Buy': 19.97,
            'Sell': 19.77
          },
          {
            'Symbol': 'BRAC',
            'Buy': 9.81,
            'Sell': 9.71
          },
          {
            'Symbol': 'BRACW',
            'Buy': 0.48,
            'Sell': 0.48
          },
          {
            'Symbol': 'BREW',
            'Buy': 19.45,
            'Sell': 19.26
          },
          {
            'Symbol': 'BRID',
            'Buy': 14.5,
            'Sell': 14.36
          },
          {
            'Symbol': 'BRKL',
            'Buy': 17.85,
            'Sell': 17.67
          },
          {
            'Symbol': 'BRKR',
            'Buy': 33.93,
            'Sell': 33.59
          },
          {
            'Symbol': 'BRKS',
            'Buy': 29.57,
            'Sell': 29.27
          },
          {
            'Symbol': 'BRPAR',
            'Buy': 0.435,
            'Sell': 0.43
          },
          {
            'Symbol': 'BRQS',
            'Buy': 4.4,
            'Sell': 4.36
          },
          {
            'Symbol': 'BRY',
            'Buy': 13.01,
            'Sell': 12.88
          },
          {
            'Symbol': 'BSET',
            'Buy': 23.2,
            'Sell': 22.97
          },
          {
            'Symbol': 'BSPM',
            'Buy': 1.91,
            'Sell': 1.89
          },
          {
            'Symbol': 'BSQR',
            'Buy': 2.1,
            'Sell': 2.08
          },
          {
            'Symbol': 'BSRR',
            'Buy': 29.24,
            'Sell': 28.95
          },
          {
            'Symbol': 'BSTC',
            'Buy': 47.68,
            'Sell': 47.2
          },
          {
            'Symbol': 'BTAI',
            'Buy': 9.94,
            'Sell': 9.84
          },
          {
            'Symbol': 'BTEC',
            'Buy': 34.96,
            'Sell': 34.61
          },
          {
            'Symbol': 'BURG',
            'Buy': 2.69,
            'Sell': 2.66
          },
          {
            'Symbol': 'BUSE',
            'Buy': 31.12,
            'Sell': 30.81
          },
          {
            'Symbol': 'BVSN',
            'Buy': 2.17,
            'Sell': 2.15
          },
          {
            'Symbol': 'BVXV',
            'Buy': 5.99,
            'Sell': 5.93
          },
          {
            'Symbol': 'BWB',
            'Buy': 12.51,
            'Sell': 12.38
          },
          {
            'Symbol': 'BWEN',
            'Buy': 2.19,
            'Sell': 2.17
          },
          {
            'Symbol': 'BWFG',
            'Buy': 31.8,
            'Sell': 31.48
          },
          {
            'Symbol': 'BYFC',
            'Buy': 1.941,
            'Sell': 1.92
          },
          {
            'Symbol': 'BYSI',
            'Buy': 25,
            'Sell': 24.75
          },
          {
            'Symbol': 'BZUN',
            'Buy': 55.66,
            'Sell': 55.1
          },
          {
            'Symbol': 'CA',
            'Buy': 43.32,
            'Sell': 42.89
          },
          {
            'Symbol': 'CAAS',
            'Buy': 3.84,
            'Sell': 3.8
          },
          {
            'Symbol': 'CAC',
            'Buy': 44.76,
            'Sell': 44.31
          },
          {
            'Symbol': 'CACC',
            'Buy': 440.81,
            'Sell': 436.4
          },
          {
            'Symbol': 'CACG',
            'Buy': 31.03,
            'Sell': 30.72
          },
          {
            'Symbol': 'CADC',
            'Buy': 6.14,
            'Sell': 6.08
          },
          {
            'Symbol': 'CAKE',
            'Buy': 49.1,
            'Sell': 48.61
          },
          {
            'Symbol': 'CALA',
            'Buy': 4.5,
            'Sell': 4.46
          },
          {
            'Symbol': 'CALL',
            'Buy': 8.25,
            'Sell': 8.17
          },
          {
            'Symbol': 'CALM',
            'Buy': 45.7,
            'Sell': 45.24
          },
          {
            'Symbol': 'CAMP',
            'Buy': 23.14,
            'Sell': 22.91
          },
          {
            'Symbol': 'CAMT',
            'Buy': 9.25,
            'Sell': 9.16
          },
          {
            'Symbol': 'CAPR',
            'Buy': 1.17,
            'Sell': 1.16
          },
          {
            'Symbol': 'CAR',
            'Buy': 33.99,
            'Sell': 33.65
          },
          {
            'Symbol': 'CARA',
            'Buy': 19.25,
            'Sell': 19.06
          },
          {
            'Symbol': 'CARB',
            'Buy': 37.45,
            'Sell': 37.08
          },
          {
            'Symbol': 'CARG',
            'Buy': 53.54,
            'Sell': 53
          },
          {
            'Symbol': 'CARO',
            'Buy': 41.62,
            'Sell': 41.2
          },
          {
            'Symbol': 'CARV',
            'Buy': 5.76,
            'Sell': 5.7
          },
          {
            'Symbol': 'CARZ',
            'Buy': 36.95,
            'Sell': 36.58
          },
          {
            'Symbol': 'CASA',
            'Buy': 15.73,
            'Sell': 15.57
          },
          {
            'Symbol': 'CASH',
            'Buy': 90,
            'Sell': 89.1
          },
          {
            'Symbol': 'CASI',
            'Buy': 7.35,
            'Sell': 7.28
          },
          {
            'Symbol': 'CASM',
            'Buy': 2.69,
            'Sell': 2.66
          },
          {
            'Symbol': 'CASS',
            'Buy': 70.36,
            'Sell': 69.66
          },
          {
            'Symbol': 'CASY',
            'Buy': 112.11,
            'Sell': 110.99
          },
          {
            'Symbol': 'CATB',
            'Buy': 0.68,
            'Sell': 0.67
          },
          {
            'Symbol': 'CATC',
            'Buy': 92.67,
            'Sell': 91.74
          },
          {
            'Symbol': 'CATH',
            'Buy': 35.24,
            'Sell': 34.89
          },
          {
            'Symbol': 'CATM',
            'Buy': 30,
            'Sell': 29.7
          },
          {
            'Symbol': 'CATS',
            'Buy': 7.74,
            'Sell': 7.66
          },
          {
            'Symbol': 'CATY',
            'Buy': 41.99,
            'Sell': 41.57
          },
          {
            'Symbol': 'CBAK',
            'Buy': 0.63,
            'Sell': 0.62
          },
          {
            'Symbol': 'CBAN',
            'Buy': 17.38,
            'Sell': 17.21
          },
          {
            'Symbol': 'CBAY',
            'Buy': 11.97,
            'Sell': 11.85
          },
          {
            'Symbol': 'CBFV',
            'Buy': 31.8,
            'Sell': 31.48
          },
          {
            'Symbol': 'CBIO',
            'Buy': 10.15,
            'Sell': 10.05
          },
          {
            'Symbol': 'CBLI',
            'Buy': 2.33,
            'Sell': 2.31
          },
          {
            'Symbol': 'CBLK',
            'Buy': 24.1,
            'Sell': 23.86
          },
          {
            'Symbol': 'CBMG',
            'Buy': 20.8,
            'Sell': 20.59
          },
          {
            'Symbol': 'CBOE',
            'Buy': 91.95,
            'Sell': 91.03
          },
          {
            'Symbol': 'CBPO',
            'Buy': 94.96,
            'Sell': 94.01
          },
          {
            'Symbol': 'CBRL',
            'Buy': 145.56,
            'Sell': 144.1
          },
          {
            'Symbol': 'CBSH',
            'Buy': 68.83,
            'Sell': 68.14
          },
          {
            'Symbol': 'CBSHP',
            'Buy': 25.6,
            'Sell': 25.34
          },
          {
            'Symbol': 'CBTX',
            'Buy': 37.96,
            'Sell': 37.58
          },
          {
            'Symbol': 'CCB',
            'Buy': 16.16,
            'Sell': 16
          },
          {
            'Symbol': 'CCBG',
            'Buy': 24.19,
            'Sell': 23.95
          },
          {
            'Symbol': 'CCCL',
            'Buy': 1.58,
            'Sell': 1.56
          },
          {
            'Symbol': 'CCD',
            'Buy': 21.9,
            'Sell': 21.68
          },
          {
            'Symbol': 'CCIH',
            'Buy': 1.16,
            'Sell': 1.15
          },
          {
            'Symbol': 'CCLP',
            'Buy': 6.03,
            'Sell': 5.97
          },
          {
            'Symbol': 'CCMP',
            'Buy': 121.63,
            'Sell': 120.41
          },
          {
            'Symbol': 'CCNE',
            'Buy': 30.99,
            'Sell': 30.68
          },
          {
            'Symbol': 'CCNI',
            'Buy': 5.75,
            'Sell': 5.69
          },
          {
            'Symbol': 'CCOI',
            'Buy': 49.85,
            'Sell': 49.35
          },
          {
            'Symbol': 'CCRC',
            'Buy': 8.5,
            'Sell': 8.42
          },
          {
            'Symbol': 'CCRN',
            'Buy': 9.45,
            'Sell': 9.36
          },
          {
            'Symbol': 'CCXI',
            'Buy': 11.89,
            'Sell': 11.77
          },
          {
            'Symbol': 'CDC',
            'Buy': 47.27,
            'Sell': 46.8
          },
          {
            'Symbol': 'CDEV',
            'Buy': 18.48,
            'Sell': 18.3
          },
          {
            'Symbol': 'CDK',
            'Buy': 63.86,
            'Sell': 63.22
          },
          {
            'Symbol': 'CDL',
            'Buy': 46.12,
            'Sell': 45.66
          },
          {
            'Symbol': 'CDLX',
            'Buy': 19.4,
            'Sell': 19.21
          },
          {
            'Symbol': 'CDMO',
            'Buy': 5.56,
            'Sell': 5.5
          },
          {
            'Symbol': 'CDMOP',
            'Buy': 25.7,
            'Sell': 25.44
          },
          {
            'Symbol': 'CDNA',
            'Buy': 17.5,
            'Sell': 17.33
          },
          {
            'Symbol': 'CDNS',
            'Buy': 45.12,
            'Sell': 44.67
          },
          {
            'Symbol': 'CDTI',
            'Buy': 0.47,
            'Sell': 0.47
          },
          {
            'Symbol': 'CDTX',
            'Buy': 4.45,
            'Sell': 4.41
          },
          {
            'Symbol': 'CDW',
            'Buy': 85.08,
            'Sell': 84.23
          },
          {
            'Symbol': 'CDXC',
            'Buy': 4.88,
            'Sell': 4.83
          },
          {
            'Symbol': 'CDXS',
            'Buy': 14.4,
            'Sell': 14.26
          },
          {
            'Symbol': 'CDZI',
            'Buy': 12.4,
            'Sell': 12.28
          },
          {
            'Symbol': 'CECE',
            'Buy': 8.18,
            'Sell': 8.1
          },
          {
            'Symbol': 'CECO',
            'Buy': 16.84,
            'Sell': 16.67
          },
          {
            'Symbol': 'CELC',
            'Buy': 25.77,
            'Sell': 25.51
          },
          {
            'Symbol': 'CELG',
            'Buy': 91.5,
            'Sell': 90.59
          },
          {
            'Symbol': 'CELGZ',
            'Buy': 1.98,
            'Sell': 1.96
          },
          {
            'Symbol': 'CELH',
            'Buy': 3.75,
            'Sell': 3.71
          },
          {
            'Symbol': 'CEMI',
            'Buy': 11.15,
            'Sell': 11.04
          },
          {
            'Symbol': 'CENT',
            'Buy': 40.88,
            'Sell': 40.47
          },
          {
            'Symbol': 'CENTA',
            'Buy': 36.39,
            'Sell': 36.03
          },
          {
            'Symbol': 'CENX',
            'Buy': 11.96,
            'Sell': 11.84
          },
          {
            'Symbol': 'CERC',
            'Buy': 4.24,
            'Sell': 4.2
          },
          {
            'Symbol': 'CERN',
            'Buy': 65.91,
            'Sell': 65.25
          },
          {
            'Symbol': 'CERS',
            'Buy': 6.86,
            'Sell': 6.79
          },
          {
            'Symbol': 'CETV',
            'Buy': 3.6,
            'Sell': 3.56
          },
          {
            'Symbol': 'CETX',
            'Buy': 2.04,
            'Sell': 2.02
          },
          {
            'Symbol': 'CETXW',
            'Buy': 0.19,
            'Sell': 0.19
          },
          {
            'Symbol': 'CEVA',
            'Buy': 29.5,
            'Sell': 29.21
          },
          {
            'Symbol': 'CEY',
            'Buy': 24.17,
            'Sell': 23.93
          },
          {
            'Symbol': 'CEZ',
            'Buy': 27.52,
            'Sell': 27.24
          },
          {
            'Symbol': 'CFA',
            'Buy': 51.22,
            'Sell': 50.71
          },
          {
            'Symbol': 'CFBI',
            'Buy': 11.6,
            'Sell': 11.48
          },
          {
            'Symbol': 'CFBK',
            'Buy': 2.545,
            'Sell': 2.52
          },
          {
            'Symbol': 'CFFI',
            'Buy': 61.35,
            'Sell': 60.74
          },
          {
            'Symbol': 'CFFN',
            'Buy': 13.11,
            'Sell': 12.98
          },
          {
            'Symbol': 'CFMS',
            'Buy': 0.94,
            'Sell': 0.93
          },
          {
            'Symbol': 'CFO',
            'Buy': 51.3,
            'Sell': 50.79
          },
          {
            'Symbol': 'CFRX',
            'Buy': 1.89,
            'Sell': 1.87
          },
          {
            'Symbol': 'CG',
            'Buy': 23.45,
            'Sell': 23.22
          },
          {
            'Symbol': 'CGBD',
            'Buy': 17.3,
            'Sell': 17.13
          },
          {
            'Symbol': 'CGEN',
            'Buy': 3.05,
            'Sell': 3.02
          },
          {
            'Symbol': 'CGIX',
            'Buy': 0.951,
            'Sell': 0.94
          },
          {
            'Symbol': 'CGNX',
            'Buy': 52.41,
            'Sell': 51.89
          },
          {
            'Symbol': 'CGO',
            'Buy': 15.06,
            'Sell': 14.91
          },
          {
            'Symbol': 'CHCI',
            'Buy': 2.66,
            'Sell': 2.63
          },
          {
            'Symbol': 'CHCO',
            'Buy': 79.98,
            'Sell': 79.18
          },
          {
            'Symbol': 'CHDN',
            'Buy': 279.3,
            'Sell': 276.51
          },
          {
            'Symbol': 'CHEF',
            'Buy': 28.85,
            'Sell': 28.56
          },
          {
            'Symbol': 'CHEK',
            'Buy': 3.21,
            'Sell': 3.18
          },
          {
            'Symbol': 'CHEKW',
            'Buy': 0.125,
            'Sell': 0.12
          },
          {
            'Symbol': 'CHEKZ',
            'Buy': 0.51,
            'Sell': 0.5
          },
          {
            'Symbol': 'CHFC',
            'Buy': 56.94,
            'Sell': 56.37
          },
          {
            'Symbol': 'CHFN',
            'Buy': 23.94,
            'Sell': 23.7
          },
          {
            'Symbol': 'CHFS',
            'Buy': 1.1,
            'Sell': 1.09
          },
          {
            'Symbol': 'CHI',
            'Buy': 12.25,
            'Sell': 12.13
          },
          {
            'Symbol': 'CHKE',
            'Buy': 0.87,
            'Sell': 0.86
          },
          {
            'Symbol': 'CHKP',
            'Buy': 113.41,
            'Sell': 112.28
          },
          {
            'Symbol': 'CHMA',
            'Buy': 1.3,
            'Sell': 1.29
          },
          {
            'Symbol': 'CHMG',
            'Buy': 44.96,
            'Sell': 44.51
          },
          {
            'Symbol': 'CHNR',
            'Buy': 1.96,
            'Sell': 1.94
          },
          {
            'Symbol': 'CHRS',
            'Buy': 18.5,
            'Sell': 18.32
          },
          {
            'Symbol': 'CHRW',
            'Buy': 94.4,
            'Sell': 93.46
          },
          {
            'Symbol': 'CHSCL',
            'Buy': 27.5,
            'Sell': 27.23
          },
          {
            'Symbol': 'CHSCM',
            'Buy': 26.53,
            'Sell': 26.26
          },
          {
            'Symbol': 'CHSCN',
            'Buy': 27.72,
            'Sell': 27.44
          },
          {
            'Symbol': 'CHSCO',
            'Buy': 28.6,
            'Sell': 28.31
          },
          {
            'Symbol': 'CHSCP',
            'Buy': 29.62,
            'Sell': 29.32
          },
          {
            'Symbol': 'CHTR',
            'Buy': 302.86,
            'Sell': 299.83
          },
          {
            'Symbol': 'CHUY',
            'Buy': 28,
            'Sell': 27.72
          },
          {
            'Symbol': 'CHW',
            'Buy': 9.08,
            'Sell': 8.99
          },
          {
            'Symbol': 'CHY',
            'Buy': 12.89,
            'Sell': 12.76
          },
          {
            'Symbol': 'CIBR',
            'Buy': 26.92,
            'Sell': 26.65
          },
          {
            'Symbol': 'CID',
            'Buy': 33.91,
            'Sell': 33.57
          },
          {
            'Symbol': 'CIDM',
            'Buy': 1.555,
            'Sell': 1.54
          },
          {
            'Symbol': 'CIFS',
            'Buy': 14.95,
            'Sell': 14.8
          },
          {
            'Symbol': 'CIGI',
            'Buy': 79.3,
            'Sell': 78.51
          },
          {
            'Symbol': 'CIL',
            'Buy': 39.29,
            'Sell': 38.9
          },
          {
            'Symbol': 'CINF',
            'Buy': 74.43,
            'Sell': 73.69
          },
          {
            'Symbol': 'CIVB',
            'Buy': 24.5,
            'Sell': 24.26
          },
          {
            'Symbol': 'CIZ',
            'Buy': 33.78,
            'Sell': 33.44
          },
          {
            'Symbol': 'CJJD',
            'Buy': 1.275,
            'Sell': 1.26
          },
          {
            'Symbol': 'CKPT',
            'Buy': 3.22,
            'Sell': 3.19
          },
          {
            'Symbol': 'CLAR',
            'Buy': 9.5,
            'Sell': 9.41
          },
          {
            'Symbol': 'CLBK',
            'Buy': 16.72,
            'Sell': 16.55
          },
          {
            'Symbol': 'CLBS',
            'Buy': 5.11,
            'Sell': 5.06
          },
          {
            'Symbol': 'CLCT',
            'Buy': 13.39,
            'Sell': 13.26
          },
          {
            'Symbol': 'CLDC',
            'Buy': 1.24,
            'Sell': 1.23
          },
          {
            'Symbol': 'CLDX',
            'Buy': 0.44,
            'Sell': 0.44
          },
          {
            'Symbol': 'CLFD',
            'Buy': 13.5,
            'Sell': 13.37
          },
          {
            'Symbol': 'CLGN',
            'Buy': 5.5,
            'Sell': 5.45
          },
          {
            'Symbol': 'CLIR',
            'Buy': 1.85,
            'Sell': 1.83
          },
          {
            'Symbol': 'CLLS',
            'Buy': 28.29,
            'Sell': 28.01
          },
          {
            'Symbol': 'CLMT',
            'Buy': 7.35,
            'Sell': 7.28
          },
          {
            'Symbol': 'CLNE',
            'Buy': 2.77,
            'Sell': 2.74
          },
          {
            'Symbol': 'CLPS',
            'Buy': 14.05,
            'Sell': 13.91
          },
          {
            'Symbol': 'CLRB',
            'Buy': 3.07,
            'Sell': 3.04
          },
          {
            'Symbol': 'CLRBZ',
            'Buy': 0.1,
            'Sell': 0.1
          },
          {
            'Symbol': 'CLRG',
            'Buy': 25.81,
            'Sell': 25.55
          },
          {
            'Symbol': 'CLRO',
            'Buy': 3.25,
            'Sell': 3.22
          },
          {
            'Symbol': 'CLSD',
            'Buy': 7.55,
            'Sell': 7.47
          },
          {
            'Symbol': 'CLSN',
            'Buy': 2.7,
            'Sell': 2.67
          },
          {
            'Symbol': 'CLUB',
            'Buy': 9.65,
            'Sell': 9.55
          },
          {
            'Symbol': 'CLVS',
            'Buy': 34.63,
            'Sell': 34.28
          },
          {
            'Symbol': 'CLWT',
            'Buy': 3.94,
            'Sell': 3.9
          },
          {
            'Symbol': 'CLXT',
            'Buy': 17.4,
            'Sell': 17.23
          },
          {
            'Symbol': 'CMCO',
            'Buy': 40.82,
            'Sell': 40.41
          },
          {
            'Symbol': 'CMCSA',
            'Buy': 35.26,
            'Sell': 34.91
          },
          {
            'Symbol': 'CMCT',
            'Buy': 15,
            'Sell': 14.85
          },
          {
            'Symbol': 'CME',
            'Buy': 162.26,
            'Sell': 160.64
          },
          {
            'Symbol': 'CMFN',
            'Buy': 9.05,
            'Sell': 8.96
          },
          {
            'Symbol': 'CMFNL',
            'Buy': 25.32,
            'Sell': 25.07
          },
          {
            'Symbol': 'CMPR',
            'Buy': 143.53,
            'Sell': 142.09
          },
          {
            'Symbol': 'CMRX',
            'Buy': 4.39,
            'Sell': 4.35
          },
          {
            'Symbol': 'CMSS',
            'Buy': 9.94,
            'Sell': 9.84
          },
          {
            'Symbol': 'CMTA',
            'Buy': 8.83,
            'Sell': 8.74
          },
          {
            'Symbol': 'CMTL',
            'Buy': 34.42,
            'Sell': 34.08
          },
          {
            'Symbol': 'CNAT',
            'Buy': 4.16,
            'Sell': 4.12
          },
          {
            'Symbol': 'CNBKA',
            'Buy': 76.4,
            'Sell': 75.64
          },
          {
            'Symbol': 'CNCE',
            'Buy': 16.53,
            'Sell': 16.36
          },
          {
            'Symbol': 'CNCR',
            'Buy': 24.08,
            'Sell': 23.84
          },
          {
            'Symbol': 'CNET',
            'Buy': 2.05,
            'Sell': 2.03
          },
          {
            'Symbol': 'CNMD',
            'Buy': 76.01,
            'Sell': 75.25
          },
          {
            'Symbol': 'CNOB',
            'Buy': 25.1,
            'Sell': 24.85
          },
          {
            'Symbol': 'CNSL',
            'Buy': 11.25,
            'Sell': 11.14
          },
          {
            'Symbol': 'CNST',
            'Buy': 9.36,
            'Sell': 9.27
          },
          {
            'Symbol': 'CNTF',
            'Buy': 2.02,
            'Sell': 2
          },
          {
            'Symbol': 'CNTY',
            'Buy': 7.9,
            'Sell': 7.82
          },
          {
            'Symbol': 'CNXN',
            'Buy': 37.34,
            'Sell': 36.97
          },
          {
            'Symbol': 'COBZ',
            'Buy': 22.15,
            'Sell': 21.93
          },
          {
            'Symbol': 'COCP',
            'Buy': 4.15,
            'Sell': 4.11
          },
          {
            'Symbol': 'CODA',
            'Buy': 4.53,
            'Sell': 4.48
          },
          {
            'Symbol': 'CODX',
            'Buy': 3.52,
            'Sell': 3.48
          },
          {
            'Symbol': 'COHR',
            'Buy': 174.75,
            'Sell': 173
          },
          {
            'Symbol': 'COHU',
            'Buy': 26.13,
            'Sell': 25.87
          },
          {
            'Symbol': 'COKE',
            'Buy': 165.5,
            'Sell': 163.85
          },
          {
            'Symbol': 'COLB',
            'Buy': 41.45,
            'Sell': 41.04
          },
          {
            'Symbol': 'COLL',
            'Buy': 17.5,
            'Sell': 17.33
          },
          {
            'Symbol': 'COLM',
            'Buy': 89.97,
            'Sell': 89.07
          },
          {
            'Symbol': 'COMM',
            'Buy': 31.8,
            'Sell': 31.48
          },
          {
            'Symbol': 'COMT',
            'Buy': 37.94,
            'Sell': 37.56
          },
          {
            'Symbol': 'CONE',
            'Buy': 65.42,
            'Sell': 64.77
          },
          {
            'Symbol': 'CONN',
            'Buy': 34.85,
            'Sell': 34.5
          },
          {
            'Symbol': 'COOL',
            'Buy': 23.41,
            'Sell': 23.18
          },
          {
            'Symbol': 'CORE',
            'Buy': 30.81,
            'Sell': 30.5
          },
          {
            'Symbol': 'CORI',
            'Buy': 7.41,
            'Sell': 7.34
          },
          {
            'Symbol': 'CORT',
            'Buy': 12,
            'Sell': 11.88
          },
          {
            'Symbol': 'CORV',
            'Buy': 4.89,
            'Sell': 4.84
          },
          {
            'Symbol': 'COST',
            'Buy': 218.14,
            'Sell': 215.96
          },
          {
            'Symbol': 'COUP',
            'Buy': 66.25,
            'Sell': 65.59
          },
          {
            'Symbol': 'COWN',
            'Buy': 14.75,
            'Sell': 14.6
          },
          {
            'Symbol': 'COWNL',
            'Buy': 25.15,
            'Sell': 24.9
          },
          {
            'Symbol': 'CPAH',
            'Buy': 2.11,
            'Sell': 2.09
          },
          {
            'Symbol': 'CPHC',
            'Buy': 14.9,
            'Sell': 14.75
          },
          {
            'Symbol': 'CPIX',
            'Buy': 5.8,
            'Sell': 5.74
          },
          {
            'Symbol': 'CPLP',
            'Buy': 3.05,
            'Sell': 3.02
          },
          {
            'Symbol': 'CPRT',
            'Buy': 59.7,
            'Sell': 59.1
          },
          {
            'Symbol': 'CPRX',
            'Buy': 3.01,
            'Sell': 2.98
          },
          {
            'Symbol': 'CPSH',
            'Buy': 1.57,
            'Sell': 1.55
          },
          {
            'Symbol': 'CPSI',
            'Buy': 26.2,
            'Sell': 25.94
          },
          {
            'Symbol': 'CPSS',
            'Buy': 3.43,
            'Sell': 3.4
          },
          {
            'Symbol': 'CPST',
            'Buy': 1.33,
            'Sell': 1.32
          },
          {
            'Symbol': 'CPTA',
            'Buy': 8.8,
            'Sell': 8.71
          },
          {
            'Symbol': 'CPTAG',
            'Buy': 24.96,
            'Sell': 24.71
          },
          {
            'Symbol': 'CPTAL',
            'Buy': 25.03,
            'Sell': 24.78
          },
          {
            'Symbol': 'CRAI',
            'Buy': 55.49,
            'Sell': 54.94
          },
          {
            'Symbol': 'CRAY',
            'Buy': 21.1,
            'Sell': 20.89
          },
          {
            'Symbol': 'CRBP',
            'Buy': 4.8,
            'Sell': 4.75
          },
          {
            'Symbol': 'CREE',
            'Buy': 50.81,
            'Sell': 50.3
          },
          {
            'Symbol': 'CREG',
            'Buy': 1.29,
            'Sell': 1.28
          },
          {
            'Symbol': 'CRESY',
            'Buy': 15.5,
            'Sell': 15.35
          },
          {
            'Symbol': 'CRIS',
            'Buy': 1.59,
            'Sell': 1.57
          },
          {
            'Symbol': 'CRMT',
            'Buy': 65.5,
            'Sell': 64.85
          },
          {
            'Symbol': 'CRNT',
            'Buy': 3.41,
            'Sell': 3.38
          },
          {
            'Symbol': 'CRNX',
            'Buy': 30,
            'Sell': 29.7
          },
          {
            'Symbol': 'CRON',
            'Buy': 6.08,
            'Sell': 6.02
          },
          {
            'Symbol': 'CROX',
            'Buy': 18.75,
            'Sell': 18.56
          },
          {
            'Symbol': 'CRSP',
            'Buy': 48.09,
            'Sell': 47.61
          },
          {
            'Symbol': 'CRTO',
            'Buy': 27.5,
            'Sell': 27.23
          },
          {
            'Symbol': 'CRUS',
            'Buy': 42.44,
            'Sell': 42.02
          },
          {
            'Symbol': 'CRVL',
            'Buy': 55.45,
            'Sell': 54.9
          },
          {
            'Symbol': 'CRVS',
            'Buy': 9.31,
            'Sell': 9.22
          },
          {
            'Symbol': 'CRWS',
            'Buy': 5.7,
            'Sell': 5.64
          },
          {
            'Symbol': 'CRZO',
            'Buy': 24.41,
            'Sell': 24.17
          },
          {
            'Symbol': 'CSA',
            'Buy': 50,
            'Sell': 49.5
          },
          {
            'Symbol': 'CSB',
            'Buy': 47.49,
            'Sell': 47.02
          },
          {
            'Symbol': 'CSBR',
            'Buy': 7.8,
            'Sell': 7.72
          },
          {
            'Symbol': 'CSCO',
            'Buy': 43.69,
            'Sell': 43.25
          },
          {
            'Symbol': 'CSF',
            'Buy': 48.44,
            'Sell': 47.96
          },
          {
            'Symbol': 'CSFL',
            'Buy': 29.57,
            'Sell': 29.27
          },
          {
            'Symbol': 'CSGP',
            'Buy': 420.72,
            'Sell': 416.51
          },
          {
            'Symbol': 'CSGS',
            'Buy': 37.66,
            'Sell': 37.28
          },
          {
            'Symbol': 'CSII',
            'Buy': 37.73,
            'Sell': 37.35
          },
          {
            'Symbol': 'CSIQ',
            'Buy': 14.39,
            'Sell': 14.25
          },
          {
            'Symbol': 'CSML',
            'Buy': 28.79,
            'Sell': 28.5
          },
          {
            'Symbol': 'CSOD',
            'Buy': 52.1,
            'Sell': 51.58
          },
          {
            'Symbol': 'CSPI',
            'Buy': 10.66,
            'Sell': 10.55
          },
          {
            'Symbol': 'CSQ',
            'Buy': 13.19,
            'Sell': 13.06
          },
          {
            'Symbol': 'CSSE',
            'Buy': 9.09,
            'Sell': 9
          },
          {
            'Symbol': 'CSSEP',
            'Buy': 25.66,
            'Sell': 25.4
          },
          {
            'Symbol': 'CSTE',
            'Buy': 18.35,
            'Sell': 18.17
          },
          {
            'Symbol': 'CSTR',
            'Buy': 17.84,
            'Sell': 17.66
          },
          {
            'Symbol': 'CSWC',
            'Buy': 18.66,
            'Sell': 18.47
          },
          {
            'Symbol': 'CSWCL',
            'Buy': 25.5,
            'Sell': 25.25
          },
          {
            'Symbol': 'CSWI',
            'Buy': 54.65,
            'Sell': 54.1
          },
          {
            'Symbol': 'CSX',
            'Buy': 71.51,
            'Sell': 70.79
          },
          {
            'Symbol': 'CTAS',
            'Buy': 209.2,
            'Sell': 207.11
          },
          {
            'Symbol': 'CTBI',
            'Buy': 49,
            'Sell': 48.51
          },
          {
            'Symbol': 'CTG',
            'Buy': 5.84,
            'Sell': 5.78
          },
          {
            'Symbol': 'CTHR',
            'Buy': 1.04,
            'Sell': 1.03
          },
          {
            'Symbol': 'CTIB',
            'Buy': 3.98,
            'Sell': 3.94
          },
          {
            'Symbol': 'CTIC',
            'Buy': 2.08,
            'Sell': 2.06
          },
          {
            'Symbol': 'CTMX',
            'Buy': 24.8,
            'Sell': 24.55
          },
          {
            'Symbol': 'CTRE',
            'Buy': 17.63,
            'Sell': 17.45
          },
          {
            'Symbol': 'CTRL',
            'Buy': 31.16,
            'Sell': 30.85
          },
          {
            'Symbol': 'CTRN',
            'Buy': 28.55,
            'Sell': 28.26
          },
          {
            'Symbol': 'CTRP',
            'Buy': 40.13,
            'Sell': 39.73
          },
          {
            'Symbol': 'CTRV',
            'Buy': 0.75,
            'Sell': 0.74
          },
          {
            'Symbol': 'CTSH',
            'Buy': 75.97,
            'Sell': 75.21
          },
          {
            'Symbol': 'CTSO',
            'Buy': 11,
            'Sell': 10.89
          },
          {
            'Symbol': 'CTWS',
            'Buy': 69.36,
            'Sell': 68.67
          },
          {
            'Symbol': 'CTXR',
            'Buy': 1.56,
            'Sell': 1.54
          },
          {
            'Symbol': 'CTXS',
            'Buy': 110.81,
            'Sell': 109.7
          },
          {
            'Symbol': 'CUBA',
            'Buy': 6.57,
            'Sell': 6.5
          },
          {
            'Symbol': 'CUE',
            'Buy': 8.31,
            'Sell': 8.23
          },
          {
            'Symbol': 'CUI',
            'Buy': 2.18,
            'Sell': 2.16
          },
          {
            'Symbol': 'CUR',
            'Buy': 1.23,
            'Sell': 1.22
          },
          {
            'Symbol': 'CUTR',
            'Buy': 34.25,
            'Sell': 33.91
          },
          {
            'Symbol': 'CVBF',
            'Buy': 23.94,
            'Sell': 23.7
          },
          {
            'Symbol': 'CVCO',
            'Buy': 227.8,
            'Sell': 225.52
          },
          {
            'Symbol': 'CVCY',
            'Buy': 21.65,
            'Sell': 21.43
          },
          {
            'Symbol': 'CVGI',
            'Buy': 9.02,
            'Sell': 8.93
          },
          {
            'Symbol': 'CVGW',
            'Buy': 95.85,
            'Sell': 94.89
          },
          {
            'Symbol': 'CVLT',
            'Buy': 66.95,
            'Sell': 66.28
          },
          {
            'Symbol': 'CVLY',
            'Buy': 31.7,
            'Sell': 31.38
          },
          {
            'Symbol': 'CVON',
            'Buy': 9.35,
            'Sell': 9.26
          },
          {
            'Symbol': 'CVTI',
            'Buy': 30.19,
            'Sell': 29.89
          },
          {
            'Symbol': 'CVV',
            'Buy': 7.25,
            'Sell': 7.18
          },
          {
            'Symbol': 'CWAY',
            'Buy': 27.7,
            'Sell': 27.42
          },
          {
            'Symbol': 'CWBC',
            'Buy': 12.08,
            'Sell': 11.96
          },
          {
            'Symbol': 'CWBR',
            'Buy': 6.8,
            'Sell': 6.73
          },
          {
            'Symbol': 'CWCO',
            'Buy': 14.1,
            'Sell': 13.96
          },
          {
            'Symbol': 'CWST',
            'Buy': 29.32,
            'Sell': 29.03
          },
          {
            'Symbol': 'CXDC',
            'Buy': 3.45,
            'Sell': 3.42
          },
          {
            'Symbol': 'CXSE',
            'Buy': 75.37,
            'Sell': 74.62
          },
          {
            'Symbol': 'CY',
            'Buy': 17.33,
            'Sell': 17.16
          },
          {
            'Symbol': 'CYAD',
            'Buy': 29.39,
            'Sell': 29.1
          },
          {
            'Symbol': 'CYAN',
            'Buy': 3.655,
            'Sell': 3.62
          },
          {
            'Symbol': 'CYBE',
            'Buy': 18.95,
            'Sell': 18.76
          },
          {
            'Symbol': 'CYBR',
            'Buy': 67.9,
            'Sell': 67.22
          },
          {
            'Symbol': 'CYCC',
            'Buy': 1.56,
            'Sell': 1.54
          },
          {
            'Symbol': 'CYHHZ',
            'Buy': 0.026,
            'Sell': 0.03
          },
          {
            'Symbol': 'CYOU',
            'Buy': 13.84,
            'Sell': 13.7
          },
          {
            'Symbol': 'CYRN',
            'Buy': 3.05,
            'Sell': 3.02
          },
          {
            'Symbol': 'CYRX',
            'Buy': 15.6,
            'Sell': 15.44
          },
          {
            'Symbol': 'CYRXW',
            'Buy': 12.5,
            'Sell': 12.38
          },
          {
            'Symbol': 'CYTK',
            'Buy': 6.95,
            'Sell': 6.88
          },
          {
            'Symbol': 'CYTR',
            'Buy': 1.14,
            'Sell': 1.13
          },
          {
            'Symbol': 'CYTX',
            'Buy': 0.43,
            'Sell': 0.43
          },
          {
            'Symbol': 'CYTXW',
            'Buy': 0.0088,
            'Sell': 0.01
          },
          {
            'Symbol': 'CZNC',
            'Buy': 27.26,
            'Sell': 26.99
          },
          {
            'Symbol': 'CZR',
            'Buy': 9.8,
            'Sell': 9.7
          },
          {
            'Symbol': 'CZWI',
            'Buy': 14.2,
            'Sell': 14.06
          },
          {
            'Symbol': 'DAIO',
            'Buy': 5.27,
            'Sell': 5.22
          },
          {
            'Symbol': 'DAKT',
            'Buy': 8.56,
            'Sell': 8.47
          },
          {
            'Symbol': 'DALI',
            'Buy': 20.68,
            'Sell': 20.47
          },
          {
            'Symbol': 'DARE',
            'Buy': 1.17,
            'Sell': 1.16
          },
          {
            'Symbol': 'DAVE',
            'Buy': 6.7,
            'Sell': 6.63
          },
          {
            'Symbol': 'DAX',
            'Buy': 28.47,
            'Sell': 28.19
          },
          {
            'Symbol': 'DBVT',
            'Buy': 18.16,
            'Sell': 17.98
          },
          {
            'Symbol': 'DBX',
            'Buy': 31.76,
            'Sell': 31.44
          },
          {
            'Symbol': 'DCAR',
            'Buy': 1.02,
            'Sell': 1.01
          },
          {
            'Symbol': 'DCIX',
            'Buy': 1.53,
            'Sell': 1.51
          },
          {
            'Symbol': 'DCOM',
            'Buy': 17.75,
            'Sell': 17.57
          },
          {
            'Symbol': 'DCPH',
            'Buy': 36.14,
            'Sell': 35.78
          },
          {
            'Symbol': 'DDBI',
            'Buy': 29.11,
            'Sell': 28.82
          },
          {
            'Symbol': 'DELT',
            'Buy': 0.83,
            'Sell': 0.82
          },
          {
            'Symbol': 'DENN',
            'Buy': 14.92,
            'Sell': 14.77
          },
          {
            'Symbol': 'DEPO',
            'Buy': 7.57,
            'Sell': 7.49
          },
          {
            'Symbol': 'DERM',
            'Buy': 9.82,
            'Sell': 9.72
          },
          {
            'Symbol': 'DEST',
            'Buy': 4.46,
            'Sell': 4.42
          },
          {
            'Symbol': 'DFBG',
            'Buy': 4.6,
            'Sell': 4.55
          },
          {
            'Symbol': 'DFBHU',
            'Buy': 10.2,
            'Sell': 10.1
          },
          {
            'Symbol': 'DFBHW',
            'Buy': 1.5,
            'Sell': 1.49
          },
          {
            'Symbol': 'DFFN',
            'Buy': 0.37,
            'Sell': 0.37
          },
          {
            'Symbol': 'DFNL',
            'Buy': 24.69,
            'Sell': 24.44
          },
          {
            'Symbol': 'DFRG',
            'Buy': 9.5,
            'Sell': 9.41
          },
          {
            'Symbol': 'DFVS',
            'Buy': 35.75,
            'Sell': 35.39
          },
          {
            'Symbol': 'DGICA',
            'Buy': 14.01,
            'Sell': 13.87
          },
          {
            'Symbol': 'DGII',
            'Buy': 13.1,
            'Sell': 12.97
          },
          {
            'Symbol': 'DGLD',
            'Buy': 55.03,
            'Sell': 54.48
          },
          {
            'Symbol': 'DGLY',
            'Buy': 2.45,
            'Sell': 2.43
          },
          {
            'Symbol': 'DGRE',
            'Buy': 24.74,
            'Sell': 24.49
          },
          {
            'Symbol': 'DGRS',
            'Buy': 37.53,
            'Sell': 37.15
          },
          {
            'Symbol': 'DGRW',
            'Buy': 42.96,
            'Sell': 42.53
          },
          {
            'Symbol': 'DHIL',
            'Buy': 183.45,
            'Sell': 181.62
          },
          {
            'Symbol': 'DHXM',
            'Buy': 1.95,
            'Sell': 1.93
          },
          {
            'Symbol': 'DINT',
            'Buy': 18.96,
            'Sell': 18.77
          },
          {
            'Symbol': 'DIOD',
            'Buy': 37.42,
            'Sell': 37.05
          },
          {
            'Symbol': 'DISCA',
            'Buy': 26,
            'Sell': 25.74
          },
          {
            'Symbol': 'DISCK',
            'Buy': 24.17,
            'Sell': 23.93
          },
          {
            'Symbol': 'DISH',
            'Buy': 36.3,
            'Sell': 35.94
          },
          {
            'Symbol': 'DJCO',
            'Buy': 232.01,
            'Sell': 229.69
          },
          {
            'Symbol': 'DLBS',
            'Buy': 20.04,
            'Sell': 19.84
          },
          {
            'Symbol': 'DLPN',
            'Buy': 2.95,
            'Sell': 2.92
          },
          {
            'Symbol': 'DLTH',
            'Buy': 24.95,
            'Sell': 24.7
          },
          {
            'Symbol': 'DLTR',
            'Buy': 92.06,
            'Sell': 91.14
          },
          {
            'Symbol': 'DMLP',
            'Buy': 18.65,
            'Sell': 18.46
          },
          {
            'Symbol': 'DMPI',
            'Buy': 0.52,
            'Sell': 0.51
          },
          {
            'Symbol': 'DMRC',
            'Buy': 30.05,
            'Sell': 29.75
          },
          {
            'Symbol': 'DNBF',
            'Buy': 34.59,
            'Sell': 34.24
          },
          {
            'Symbol': 'DNJR',
            'Buy': 7.4,
            'Sell': 7.33
          },
          {
            'Symbol': 'DNKN',
            'Buy': 71.52,
            'Sell': 70.8
          },
          {
            'Symbol': 'DNLI',
            'Buy': 14.78,
            'Sell': 14.63
          },
          {
            'Symbol': 'DOCU',
            'Buy': 60.3,
            'Sell': 59.7
          },
          {
            'Symbol': 'DOGZ',
            'Buy': 2.91,
            'Sell': 2.88
          },
          {
            'Symbol': 'DOMO',
            'Buy': 17.53,
            'Sell': 17.35
          },
          {
            'Symbol': 'DORM',
            'Buy': 75.47,
            'Sell': 74.72
          },
          {
            'Symbol': 'DOTAW',
            'Buy': 0.8,
            'Sell': 0.79
          },
          {
            'Symbol': 'DOVA',
            'Buy': 22.28,
            'Sell': 22.06
          },
          {
            'Symbol': 'DOX',
            'Buy': 64.39,
            'Sell': 63.75
          },
          {
            'Symbol': 'DRAD',
            'Buy': 1.9,
            'Sell': 1.88
          },
          {
            'Symbol': 'DRIO',
            'Buy': 1.24,
            'Sell': 1.23
          },
          {
            'Symbol': 'DRIV',
            'Buy': 14.63,
            'Sell': 14.48
          },
          {
            'Symbol': 'DRNA',
            'Buy': 14.53,
            'Sell': 14.38
          },
          {
            'Symbol': 'DRRX',
            'Buy': 1.37,
            'Sell': 1.36
          },
          {
            'Symbol': 'DRYS',
            'Buy': 5,
            'Sell': 4.95
          },
          {
            'Symbol': 'DSGX',
            'Buy': 33.9,
            'Sell': 33.56
          },
          {
            'Symbol': 'DSKE',
            'Buy': 8.98,
            'Sell': 8.89
          },
          {
            'Symbol': 'DSKEW',
            'Buy': 1,
            'Sell': 0.99
          },
          {
            'Symbol': 'DSLV',
            'Buy': 30,
            'Sell': 29.7
          },
          {
            'Symbol': 'DSPG',
            'Buy': 12.3,
            'Sell': 12.18
          },
          {
            'Symbol': 'DSWL',
            'Buy': 3.43,
            'Sell': 3.4
          },
          {
            'Symbol': 'DTEA',
            'Buy': 2.5,
            'Sell': 2.48
          },
          {
            'Symbol': 'DTRM',
            'Buy': 1.45,
            'Sell': 1.44
          },
          {
            'Symbol': 'DTUS',
            'Buy': 39.9,
            'Sell': 39.5
          },
          {
            'Symbol': 'DTYL',
            'Buy': 73.23,
            'Sell': 72.5
          },
          {
            'Symbol': 'DTYS',
            'Buy': 22.38,
            'Sell': 22.16
          },
          {
            'Symbol': 'DUSA',
            'Buy': 25.4,
            'Sell': 25.15
          },
          {
            'Symbol': 'DVAX',
            'Buy': 13.3,
            'Sell': 13.17
          },
          {
            'Symbol': 'DVCR',
            'Buy': 6.9,
            'Sell': 6.83
          },
          {
            'Symbol': 'DVY',
            'Buy': 99.74,
            'Sell': 98.74
          },
          {
            'Symbol': 'DWAQ',
            'Buy': 112.02,
            'Sell': 110.9
          },
          {
            'Symbol': 'DWAS',
            'Buy': 55.487,
            'Sell': 54.93
          },
          {
            'Symbol': 'DWCH',
            'Buy': 13.1,
            'Sell': 12.97
          },
          {
            'Symbol': 'DWFI',
            'Buy': 23.61,
            'Sell': 23.37
          },
          {
            'Symbol': 'DWIN',
            'Buy': 25.32,
            'Sell': 25.07
          },
          {
            'Symbol': 'DWLD',
            'Buy': 26.56,
            'Sell': 26.29
          },
          {
            'Symbol': 'DWLV',
            'Buy': 33.16,
            'Sell': 32.83
          },
          {
            'Symbol': 'DWMC',
            'Buy': 25.5,
            'Sell': 25.25
          },
          {
            'Symbol': 'DWSN',
            'Buy': 5.93,
            'Sell': 5.87
          },
          {
            'Symbol': 'DWTR',
            'Buy': 30.65,
            'Sell': 30.34
          },
          {
            'Symbol': 'DXCM',
            'Buy': 122,
            'Sell': 120.78
          },
          {
            'Symbol': 'DXGE',
            'Buy': 30.43,
            'Sell': 30.13
          },
          {
            'Symbol': 'DXJS',
            'Buy': 42.93,
            'Sell': 42.5
          },
          {
            'Symbol': 'DXLG',
            'Buy': 2.05,
            'Sell': 2.03
          },
          {
            'Symbol': 'DXPE',
            'Buy': 47.41,
            'Sell': 46.94
          },
          {
            'Symbol': 'DXYN',
            'Buy': 1.9,
            'Sell': 1.88
          },
          {
            'Symbol': 'DYNT',
            'Buy': 2.85,
            'Sell': 2.82
          },
          {
            'Symbol': 'DYSL',
            'Buy': 1.4,
            'Sell': 1.39
          },
          {
            'Symbol': 'DZSI',
            'Buy': 9.7,
            'Sell': 9.6
          },
          {
            'Symbol': 'EA',
            'Buy': 128.2,
            'Sell': 126.92
          },
          {
            'Symbol': 'EACQW',
            'Buy': 1.04,
            'Sell': 1.03
          },
          {
            'Symbol': 'EAGL',
            'Buy': 9.8,
            'Sell': 9.7
          },
          {
            'Symbol': 'EARS',
            'Buy': 0.27,
            'Sell': 0.27
          },
          {
            'Symbol': 'EAST',
            'Buy': 7.41,
            'Sell': 7.34
          },
          {
            'Symbol': 'EASTW',
            'Buy': 1.99,
            'Sell': 1.97
          },
          {
            'Symbol': 'EBAY',
            'Buy': 33.55,
            'Sell': 33.21
          },
          {
            'Symbol': 'EBAYL',
            'Buy': 26.05,
            'Sell': 25.79
          },
          {
            'Symbol': 'EBIX',
            'Buy': 78.65,
            'Sell': 77.86
          },
          {
            'Symbol': 'EBSB',
            'Buy': 18.25,
            'Sell': 18.07
          },
          {
            'Symbol': 'EBTC',
            'Buy': 36.6,
            'Sell': 36.23
          },
          {
            'Symbol': 'ECHO',
            'Buy': 32.45,
            'Sell': 32.13
          },
          {
            'Symbol': 'ECOL',
            'Buy': 69,
            'Sell': 68.31
          },
          {
            'Symbol': 'ECOR',
            'Buy': 13.02,
            'Sell': 12.89
          },
          {
            'Symbol': 'ECPG',
            'Buy': 39.65,
            'Sell': 39.25
          },
          {
            'Symbol': 'ECYT',
            'Buy': 16.1,
            'Sell': 15.94
          },
          {
            'Symbol': 'EDAP',
            'Buy': 3.14,
            'Sell': 3.11
          },
          {
            'Symbol': 'EDBI',
            'Buy': 29.38,
            'Sell': 29.09
          },
          {
            'Symbol': 'EDGE',
            'Buy': 0.92,
            'Sell': 0.91
          },
          {
            'Symbol': 'EDGW',
            'Buy': 5.14,
            'Sell': 5.09
          },
          {
            'Symbol': 'EDIT',
            'Buy': 28.5,
            'Sell': 28.22
          },
          {
            'Symbol': 'EDRY',
            'Buy': 7.18,
            'Sell': 7.11
          },
          {
            'Symbol': 'EDUC',
            'Buy': 20,
            'Sell': 19.8
          },
          {
            'Symbol': 'EEFT',
            'Buy': 93.86,
            'Sell': 92.92
          },
          {
            'Symbol': 'EEI',
            'Buy': 13.85,
            'Sell': 13.71
          },
          {
            'Symbol': 'EEMA',
            'Buy': 68.86,
            'Sell': 68.17
          },
          {
            'Symbol': 'EFAS',
            'Buy': 17.01,
            'Sell': 16.84
          },
          {
            'Symbol': 'EFII',
            'Buy': 34.03,
            'Sell': 33.69
          },
          {
            'Symbol': 'EFOI',
            'Buy': 2.26,
            'Sell': 2.24
          },
          {
            'Symbol': 'EFSC',
            'Buy': 56.2,
            'Sell': 55.64
          },
          {
            'Symbol': 'EGAN',
            'Buy': 14.35,
            'Sell': 14.21
          },
          {
            'Symbol': 'EGBN',
            'Buy': 54.2,
            'Sell': 53.66
          },
          {
            'Symbol': 'EGC',
            'Buy': 8.99,
            'Sell': 8.9
          },
          {
            'Symbol': 'EGLE',
            'Buy': 5.35,
            'Sell': 5.3
          },
          {
            'Symbol': 'EGLT',
            'Buy': 0.378,
            'Sell': 0.37
          },
          {
            'Symbol': 'EGOV',
            'Buy': 16.5,
            'Sell': 16.34
          },
          {
            'Symbol': 'EGRX',
            'Buy': 78.34,
            'Sell': 77.56
          },
          {
            'Symbol': 'EHTH',
            'Buy': 24.6,
            'Sell': 24.35
          },
          {
            'Symbol': 'EIDX',
            'Buy': 16.39,
            'Sell': 16.23
          },
          {
            'Symbol': 'EIGI',
            'Buy': 9.05,
            'Sell': 8.96
          },
          {
            'Symbol': 'EIGR',
            'Buy': 9.8,
            'Sell': 9.7
          },
          {
            'Symbol': 'EKSO',
            'Buy': 2.76,
            'Sell': 2.73
          },
          {
            'Symbol': 'ELGX',
            'Buy': 3.71,
            'Sell': 3.67
          },
          {
            'Symbol': 'ELON',
            'Buy': 8.38,
            'Sell': 8.3
          },
          {
            'Symbol': 'ELOX',
            'Buy': 14.27,
            'Sell': 14.13
          },
          {
            'Symbol': 'ELTK',
            'Buy': 3.65,
            'Sell': 3.61
          },
          {
            'Symbol': 'EMB',
            'Buy': 106.6,
            'Sell': 105.53
          },
          {
            'Symbol': 'EMCG',
            'Buy': 22.48,
            'Sell': 22.26
          },
          {
            'Symbol': 'EMCI',
            'Buy': 25.74,
            'Sell': 25.48
          },
          {
            'Symbol': 'EMIF',
            'Buy': 28.67,
            'Sell': 28.38
          },
          {
            'Symbol': 'EMKR',
            'Buy': 4.9,
            'Sell': 4.85
          },
          {
            'Symbol': 'EML',
            'Buy': 27.45,
            'Sell': 27.18
          },
          {
            'Symbol': 'EMMS',
            'Buy': 5.05,
            'Sell': 5
          },
          {
            'Symbol': 'EMXC',
            'Buy': 49.44,
            'Sell': 48.95
          },
          {
            'Symbol': 'ENDP',
            'Buy': 15.88,
            'Sell': 15.72
          },
          {
            'Symbol': 'ENFC',
            'Buy': 28.29,
            'Sell': 28.01
          },
          {
            'Symbol': 'ENG',
            'Buy': 0.9,
            'Sell': 0.89
          },
          {
            'Symbol': 'ENPH',
            'Buy': 5.64,
            'Sell': 5.58
          },
          {
            'Symbol': 'ENSG',
            'Buy': 35.73,
            'Sell': 35.37
          },
          {
            'Symbol': 'ENT',
            'Buy': 2.8,
            'Sell': 2.77
          },
          {
            'Symbol': 'ENTA',
            'Buy': 92.05,
            'Sell': 91.13
          },
          {
            'Symbol': 'ENTG',
            'Buy': 35.65,
            'Sell': 35.29
          },
          {
            'Symbol': 'ENTX',
            'Buy': 5.69,
            'Sell': 5.63
          },
          {
            'Symbol': 'ENTXW',
            'Buy': 1,
            'Sell': 0.99
          },
          {
            'Symbol': 'ENZL',
            'Buy': 46.95,
            'Sell': 46.48
          },
          {
            'Symbol': 'EOLS',
            'Buy': 20.2,
            'Sell': 20
          },
          {
            'Symbol': 'EPAY',
            'Buy': 58,
            'Sell': 57.42
          },
          {
            'Symbol': 'EPIX',
            'Buy': 3.16,
            'Sell': 3.13
          },
          {
            'Symbol': 'EPZM',
            'Buy': 9.95,
            'Sell': 9.85
          },
          {
            'Symbol': 'EQBK',
            'Buy': 41.25,
            'Sell': 40.84
          },
          {
            'Symbol': 'EQIX',
            'Buy': 442.7,
            'Sell': 438.27
          },
          {
            'Symbol': 'EQRR',
            'Buy': 50.12,
            'Sell': 49.62
          },
          {
            'Symbol': 'ERI',
            'Buy': 43.2,
            'Sell': 42.77
          },
          {
            'Symbol': 'ERIC',
            'Buy': 7.8,
            'Sell': 7.72
          },
          {
            'Symbol': 'ERIE',
            'Buy': 125.27,
            'Sell': 124.02
          },
          {
            'Symbol': 'ERII',
            'Buy': 9.68,
            'Sell': 9.58
          },
          {
            'Symbol': 'ERYP',
            'Buy': 10.45,
            'Sell': 10.35
          },
          {
            'Symbol': 'ESBK',
            'Buy': 20.25,
            'Sell': 20.05
          },
          {
            'Symbol': 'ESCA',
            'Buy': 13.4,
            'Sell': 13.27
          },
          {
            'Symbol': 'ESEA',
            'Buy': 1.74,
            'Sell': 1.72
          },
          {
            'Symbol': 'ESES',
            'Buy': 0.66,
            'Sell': 0.65
          },
          {
            'Symbol': 'ESGD',
            'Buy': 64.95,
            'Sell': 64.3
          },
          {
            'Symbol': 'ESGE',
            'Buy': 33.93,
            'Sell': 33.59
          },
          {
            'Symbol': 'ESGG',
            'Buy': 97.42,
            'Sell': 96.45
          },
          {
            'Symbol': 'ESGR',
            'Buy': 208,
            'Sell': 205.92
          },
          {
            'Symbol': 'ESGRP',
            'Buy': 25.97,
            'Sell': 25.71
          },
          {
            'Symbol': 'ESGU',
            'Buy': 62.48,
            'Sell': 61.86
          },
          {
            'Symbol': 'ESIO',
            'Buy': 22.44,
            'Sell': 22.22
          },
          {
            'Symbol': 'ESLT',
            'Buy': 119.46,
            'Sell': 118.27
          },
          {
            'Symbol': 'ESND',
            'Buy': 15.72,
            'Sell': 15.56
          },
          {
            'Symbol': 'ESPR',
            'Buy': 46.44,
            'Sell': 45.98
          },
          {
            'Symbol': 'ESQ',
            'Buy': 25.49,
            'Sell': 25.24
          },
          {
            'Symbol': 'ESRX',
            'Buy': 83.06,
            'Sell': 82.23
          },
          {
            'Symbol': 'ESSA',
            'Buy': 15.96,
            'Sell': 15.8
          },
          {
            'Symbol': 'ESTA',
            'Buy': 25.65,
            'Sell': 25.39
          },
          {
            'Symbol': 'ESTR',
            'Buy': 7.65,
            'Sell': 7.57
          },
          {
            'Symbol': 'ESTRW',
            'Buy': 0.44,
            'Sell': 0.44
          },
          {
            'Symbol': 'ESXB',
            'Buy': 9.05,
            'Sell': 8.96
          },
          {
            'Symbol': 'ETFC',
            'Buy': 60.92,
            'Sell': 60.31
          },
          {
            'Symbol': 'ETSY',
            'Buy': 43.35,
            'Sell': 42.92
          },
          {
            'Symbol': 'EUFN',
            'Buy': 20.03,
            'Sell': 19.83
          },
          {
            'Symbol': 'EVBG',
            'Buy': 49.89,
            'Sell': 49.39
          },
          {
            'Symbol': 'EVER',
            'Buy': 15.12,
            'Sell': 14.97
          },
          {
            'Symbol': 'EVFM',
            'Buy': 1.92,
            'Sell': 1.9
          },
          {
            'Symbol': 'EVGN',
            'Buy': 2.83,
            'Sell': 2.8
          },
          {
            'Symbol': 'EVK',
            'Buy': 3.15,
            'Sell': 3.12
          },
          {
            'Symbol': 'EVLO',
            'Buy': 12.75,
            'Sell': 12.62
          },
          {
            'Symbol': 'EVLV',
            'Buy': 1.23,
            'Sell': 1.22
          },
          {
            'Symbol': 'EVOK',
            'Buy': 2.65,
            'Sell': 2.62
          },
          {
            'Symbol': 'EVOL',
            'Buy': 2.54,
            'Sell': 2.51
          },
          {
            'Symbol': 'EVOP',
            'Buy': 20.35,
            'Sell': 20.15
          },
          {
            'Symbol': 'EWBC',
            'Buy': 64.06,
            'Sell': 63.42
          },
          {
            'Symbol': 'EWZS',
            'Buy': 13,
            'Sell': 12.87
          },
          {
            'Symbol': 'EXAS',
            'Buy': 50.25,
            'Sell': 49.75
          },
          {
            'Symbol': 'EXEL',
            'Buy': 20.6,
            'Sell': 20.39
          },
          {
            'Symbol': 'EXFO',
            'Buy': 3.9,
            'Sell': 3.86
          },
          {
            'Symbol': 'EXLS',
            'Buy': 58.44,
            'Sell': 57.86
          },
          {
            'Symbol': 'EXPD',
            'Buy': 72.76,
            'Sell': 72.03
          },
          {
            'Symbol': 'EXPE',
            'Buy': 132.16,
            'Sell': 130.84
          },
          {
            'Symbol': 'EXPI',
            'Buy': 16.65,
            'Sell': 16.48
          },
          {
            'Symbol': 'EXPO',
            'Buy': 50.15,
            'Sell': 49.65
          },
          {
            'Symbol': 'EXTR',
            'Buy': 6.2,
            'Sell': 6.14
          },
          {
            'Symbol': 'EYE',
            'Buy': 44.43,
            'Sell': 43.99
          },
          {
            'Symbol': 'EYEG',
            'Buy': 0.52,
            'Sell': 0.51
          },
          {
            'Symbol': 'EYEN',
            'Buy': 5.65,
            'Sell': 5.59
          },
          {
            'Symbol': 'EYES',
            'Buy': 1.55,
            'Sell': 1.53
          },
          {
            'Symbol': 'EYESW',
            'Buy': 0.65,
            'Sell': 0.64
          },
          {
            'Symbol': 'EYPT',
            'Buy': 2.18,
            'Sell': 2.16
          },
          {
            'Symbol': 'EZPW',
            'Buy': 11.75,
            'Sell': 11.63
          },
          {
            'Symbol': 'FAAR',
            'Buy': 28.39,
            'Sell': 28.11
          },
          {
            'Symbol': 'FAB',
            'Buy': 57.12,
            'Sell': 56.55
          },
          {
            'Symbol': 'FAD',
            'Buy': 73.961,
            'Sell': 73.22
          },
          {
            'Symbol': 'FALN',
            'Buy': 26.405,
            'Sell': 26.14
          },
          {
            'Symbol': 'FAMI',
            'Buy': 3.03,
            'Sell': 3
          },
          {
            'Symbol': 'FANG',
            'Buy': 132.06,
            'Sell': 130.74
          },
          {
            'Symbol': 'FANH',
            'Buy': 28.36,
            'Sell': 28.08
          },
          {
            'Symbol': 'FARM',
            'Buy': 28.5,
            'Sell': 28.22
          },
          {
            'Symbol': 'FARO',
            'Buy': 66.75,
            'Sell': 66.08
          },
          {
            'Symbol': 'FAST',
            'Buy': 57.35,
            'Sell': 56.78
          },
          {
            'Symbol': 'FAT',
            'Buy': 7.2,
            'Sell': 7.13
          },
          {
            'Symbol': 'FATE',
            'Buy': 11.37,
            'Sell': 11.26
          },
          {
            'Symbol': 'FB',
            'Buy': 182.04,
            'Sell': 180.22
          },
          {
            'Symbol': 'FBIO',
            'Buy': 2.25,
            'Sell': 2.23
          },
          {
            'Symbol': 'FBIOP',
            'Buy': 18,
            'Sell': 17.82
          },
          {
            'Symbol': 'FBIZ',
            'Buy': 23.01,
            'Sell': 22.78
          },
          {
            'Symbol': 'FBMS',
            'Buy': 39.3,
            'Sell': 38.91
          },
          {
            'Symbol': 'FBNC',
            'Buy': 41.13,
            'Sell': 40.72
          },
          {
            'Symbol': 'FBNK',
            'Buy': 31.4,
            'Sell': 31.09
          },
          {
            'Symbol': 'FBSS',
            'Buy': 22.86,
            'Sell': 22.63
          },
          {
            'Symbol': 'FBZ',
            'Buy': 12.04,
            'Sell': 11.92
          },
          {
            'Symbol': 'FCA',
            'Buy': 28.21,
            'Sell': 27.93
          },
          {
            'Symbol': 'FCAL',
            'Buy': 50.36,
            'Sell': 49.86
          },
          {
            'Symbol': 'FCAN',
            'Buy': 25.58,
            'Sell': 25.32
          },
          {
            'Symbol': 'FCAP',
            'Buy': 38.28,
            'Sell': 37.9
          },
          {
            'Symbol': 'FCBC',
            'Buy': 33.03,
            'Sell': 32.7
          },
          {
            'Symbol': 'FCBP',
            'Buy': 27.25,
            'Sell': 26.98
          },
          {
            'Symbol': 'FCCO',
            'Buy': 25.75,
            'Sell': 25.49
          },
          {
            'Symbol': 'FCCY',
            'Buy': 21.9,
            'Sell': 21.68
          },
          {
            'Symbol': 'FCEF',
            'Buy': 22.083,
            'Sell': 21.86
          },
          {
            'Symbol': 'FCEL',
            'Buy': 1.2,
            'Sell': 1.19
          },
          {
            'Symbol': 'FCNCA',
            'Buy': 437.65,
            'Sell': 433.27
          },
          {
            'Symbol': 'FCSC',
            'Buy': 1.98,
            'Sell': 1.96
          },
          {
            'Symbol': 'FCVT',
            'Buy': 30.32,
            'Sell': 30.02
          },
          {
            'Symbol': 'FDBC',
            'Buy': 61.01,
            'Sell': 60.4
          },
          {
            'Symbol': 'FDEF',
            'Buy': 31.9,
            'Sell': 31.58
          },
          {
            'Symbol': 'FDIV',
            'Buy': 49.8,
            'Sell': 49.3
          },
          {
            'Symbol': 'FDT',
            'Buy': 59.12,
            'Sell': 58.53
          },
          {
            'Symbol': 'FDTS',
            'Buy': 41.06,
            'Sell': 40.65
          },
          {
            'Symbol': 'FDUS',
            'Buy': 14.87,
            'Sell': 14.72
          },
          {
            'Symbol': 'FEIM',
            'Buy': 8.53,
            'Sell': 8.44
          },
          {
            'Symbol': 'FELE',
            'Buy': 48.65,
            'Sell': 48.16
          },
          {
            'Symbol': 'FEM',
            'Buy': 25.37,
            'Sell': 25.12
          },
          {
            'Symbol': 'FEMB',
            'Buy': 37.39,
            'Sell': 37.02
          },
          {
            'Symbol': 'FEMS',
            'Buy': 37.84,
            'Sell': 37.46
          },
          {
            'Symbol': 'FENC',
            'Buy': 9.9,
            'Sell': 9.8
          },
          {
            'Symbol': 'FEP',
            'Buy': 38.3,
            'Sell': 37.92
          },
          {
            'Symbol': 'FEUZ',
            'Buy': 41.96,
            'Sell': 41.54
          },
          {
            'Symbol': 'FEX',
            'Buy': 61.13,
            'Sell': 60.52
          },
          {
            'Symbol': 'FEYE',
            'Buy': 15.2,
            'Sell': 15.05
          },
          {
            'Symbol': 'FFBC',
            'Buy': 30.8,
            'Sell': 30.49
          },
          {
            'Symbol': 'FFBW',
            'Buy': 11.29,
            'Sell': 11.18
          },
          {
            'Symbol': 'FFHL',
            'Buy': 2.45,
            'Sell': 2.43
          },
          {
            'Symbol': 'FFIC',
            'Buy': 25.15,
            'Sell': 24.9
          },
          {
            'Symbol': 'FFIN',
            'Buy': 57.8,
            'Sell': 57.22
          },
          {
            'Symbol': 'FFIV',
            'Buy': 179.15,
            'Sell': 177.36
          },
          {
            'Symbol': 'FFKT',
            'Buy': 56.1,
            'Sell': 55.54
          },
          {
            'Symbol': 'FFNW',
            'Buy': 17.43,
            'Sell': 17.26
          },
          {
            'Symbol': 'FFWM',
            'Buy': 15.51,
            'Sell': 15.35
          },
          {
            'Symbol': 'FGBI',
            'Buy': 25.934,
            'Sell': 25.67
          },
          {
            'Symbol': 'FGEN',
            'Buy': 59.25,
            'Sell': 58.66
          },
          {
            'Symbol': 'FGM',
            'Buy': 46.97,
            'Sell': 46.5
          },
          {
            'Symbol': 'FHB',
            'Buy': 28.63,
            'Sell': 28.34
          },
          {
            'Symbol': 'FHK',
            'Buy': 37.82,
            'Sell': 37.44
          },
          {
            'Symbol': 'FIBK',
            'Buy': 43.2,
            'Sell': 42.77
          },
          {
            'Symbol': 'FINX',
            'Buy': 27.18,
            'Sell': 26.91
          },
          {
            'Symbol': 'FISI',
            'Buy': 31.45,
            'Sell': 31.14
          },
          {
            'Symbol': 'FISV',
            'Buy': 78.3,
            'Sell': 77.52
          },
          {
            'Symbol': 'FITB',
            'Buy': 29.15,
            'Sell': 28.86
          },
          {
            'Symbol': 'FITBI',
            'Buy': 27.48,
            'Sell': 27.21
          },
          {
            'Symbol': 'FIVE',
            'Buy': 103.99,
            'Sell': 102.95
          },
          {
            'Symbol': 'FIVN',
            'Buy': 42.5,
            'Sell': 42.08
          },
          {
            'Symbol': 'FIXD',
            'Buy': 49.27,
            'Sell': 48.78
          },
          {
            'Symbol': 'FIXX',
            'Buy': 16.29,
            'Sell': 16.13
          },
          {
            'Symbol': 'FIZZ',
            'Buy': 109.75,
            'Sell': 108.65
          },
          {
            'Symbol': 'FJP',
            'Buy': 54.93,
            'Sell': 54.38
          },
          {
            'Symbol': 'FKO',
            'Buy': 24.8,
            'Sell': 24.55
          },
          {
            'Symbol': 'FLDM',
            'Buy': 6.83,
            'Sell': 6.76
          },
          {
            'Symbol': 'FLEX',
            'Buy': 13.8,
            'Sell': 13.66
          },
          {
            'Symbol': 'FLGT',
            'Buy': 4.7,
            'Sell': 4.65
          },
          {
            'Symbol': 'FLIC',
            'Buy': 22.1,
            'Sell': 21.88
          },
          {
            'Symbol': 'FLIR',
            'Buy': 59.38,
            'Sell': 58.79
          },
          {
            'Symbol': 'FLKS',
            'Buy': 0.56,
            'Sell': 0.55
          },
          {
            'Symbol': 'FLL',
            'Buy': 2.97,
            'Sell': 2.94
          },
          {
            'Symbol': 'FLN',
            'Buy': 19.12,
            'Sell': 18.93
          },
          {
            'Symbol': 'FLNT',
            'Buy': 2.25,
            'Sell': 2.23
          },
          {
            'Symbol': 'FLWS',
            'Buy': 14.45,
            'Sell': 14.31
          },
          {
            'Symbol': 'FLXN',
            'Buy': 22.19,
            'Sell': 21.97
          },
          {
            'Symbol': 'FLXS',
            'Buy': 36.4,
            'Sell': 36.04
          },
          {
            'Symbol': 'FMAO',
            'Buy': 45.15,
            'Sell': 44.7
          },
          {
            'Symbol': 'FMB',
            'Buy': 52.68,
            'Sell': 52.15
          },
          {
            'Symbol': 'FMBH',
            'Buy': 40.76,
            'Sell': 40.35
          },
          {
            'Symbol': 'FMBI',
            'Buy': 26.64,
            'Sell': 26.37
          },
          {
            'Symbol': 'FMCIU',
            'Buy': 9.99,
            'Sell': 9.89
          },
          {
            'Symbol': 'FMHI',
            'Buy': 50.43,
            'Sell': 49.93
          },
          {
            'Symbol': 'FMK',
            'Buy': 35.88,
            'Sell': 35.52
          },
          {
            'Symbol': 'FMNB',
            'Buy': 16.1,
            'Sell': 15.94
          },
          {
            'Symbol': 'FNCB',
            'Buy': 8.44,
            'Sell': 8.36
          },
          {
            'Symbol': 'FNHC',
            'Buy': 24.4,
            'Sell': 24.16
          },
          {
            'Symbol': 'FNJN',
            'Buy': 3.73,
            'Sell': 3.69
          },
          {
            'Symbol': 'FNK',
            'Buy': 36.81,
            'Sell': 36.44
          },
          {
            'Symbol': 'FNKO',
            'Buy': 20.61,
            'Sell': 20.4
          },
          {
            'Symbol': 'FNLC',
            'Buy': 29.44,
            'Sell': 29.15
          },
          {
            'Symbol': 'FNSR',
            'Buy': 18.5,
            'Sell': 18.32
          },
          {
            'Symbol': 'FNWB',
            'Buy': 15.92,
            'Sell': 15.76
          },
          {
            'Symbol': 'FNX',
            'Buy': 70.4,
            'Sell': 69.7
          },
          {
            'Symbol': 'FNY',
            'Buy': 44.97,
            'Sell': 44.52
          },
          {
            'Symbol': 'FOCS',
            'Buy': 39.83,
            'Sell': 39.43
          },
          {
            'Symbol': 'FOLD',
            'Buy': 15.07,
            'Sell': 14.92
          },
          {
            'Symbol': 'FOMX',
            'Buy': 5.73,
            'Sell': 5.67
          },
          {
            'Symbol': 'FONE',
            'Buy': 49.27,
            'Sell': 48.78
          },
          {
            'Symbol': 'FONR',
            'Buy': 26.6,
            'Sell': 26.33
          },
          {
            'Symbol': 'FORD',
            'Buy': 1.71,
            'Sell': 1.69
          },
          {
            'Symbol': 'FORK',
            'Buy': 3.2,
            'Sell': 3.17
          },
          {
            'Symbol': 'FORM',
            'Buy': 14.05,
            'Sell': 13.91
          },
          {
            'Symbol': 'FORR',
            'Buy': 45,
            'Sell': 44.55
          },
          {
            'Symbol': 'FOSL',
            'Buy': 24.5,
            'Sell': 24.26
          },
          {
            'Symbol': 'FOX',
            'Buy': 44.9,
            'Sell': 44.45
          },
          {
            'Symbol': 'FOXA',
            'Buy': 45.41,
            'Sell': 44.96
          },
          {
            'Symbol': 'FOXF',
            'Buy': 63.45,
            'Sell': 62.82
          },
          {
            'Symbol': 'FPA',
            'Buy': 31.99,
            'Sell': 31.67
          },
          {
            'Symbol': 'FPAY',
            'Buy': 3.4,
            'Sell': 3.37
          },
          {
            'Symbol': 'FPRX',
            'Buy': 14.81,
            'Sell': 14.66
          },
          {
            'Symbol': 'FPXI',
            'Buy': 34.42,
            'Sell': 34.08
          },
          {
            'Symbol': 'FRAN',
            'Buy': 7.49,
            'Sell': 7.42
          },
          {
            'Symbol': 'FRBA',
            'Buy': 14.15,
            'Sell': 14.01
          },
          {
            'Symbol': 'FRBK',
            'Buy': 7.7,
            'Sell': 7.62
          },
          {
            'Symbol': 'FRED',
            'Buy': 1.82,
            'Sell': 1.8
          },
          {
            'Symbol': 'FRGI',
            'Buy': 28.6,
            'Sell': 28.31
          },
          {
            'Symbol': 'FRME',
            'Buy': 47.29,
            'Sell': 46.82
          },
          {
            'Symbol': 'FRPH',
            'Buy': 65.7,
            'Sell': 65.04
          },
          {
            'Symbol': 'FRPT',
            'Buy': 33.2,
            'Sell': 32.87
          },
          {
            'Symbol': 'FRSH',
            'Buy': 5.36,
            'Sell': 5.31
          },
          {
            'Symbol': 'FRSX',
            'Buy': 2.84,
            'Sell': 2.81
          },
          {
            'Symbol': 'FRTA',
            'Buy': 9.55,
            'Sell': 9.45
          },
          {
            'Symbol': 'FSBC',
            'Buy': 17.66,
            'Sell': 17.48
          },
          {
            'Symbol': 'FSBW',
            'Buy': 60.31,
            'Sell': 59.71
          },
          {
            'Symbol': 'FSCT',
            'Buy': 34.5,
            'Sell': 34.16
          },
          {
            'Symbol': 'FSFG',
            'Buy': 67.1,
            'Sell': 66.43
          },
          {
            'Symbol': 'FSLR',
            'Buy': 53.97,
            'Sell': 53.43
          },
          {
            'Symbol': 'FSNN',
            'Buy': 4.29,
            'Sell': 4.25
          },
          {
            'Symbol': 'FSTR',
            'Buy': 23.95,
            'Sell': 23.71
          },
          {
            'Symbol': 'FSV',
            'Buy': 82.2,
            'Sell': 81.38
          },
          {
            'Symbol': 'FSZ',
            'Buy': 51.43,
            'Sell': 50.92
          },
          {
            'Symbol': 'FTA',
            'Buy': 53.89,
            'Sell': 53.35
          },
          {
            'Symbol': 'FTAG',
            'Buy': 26.46,
            'Sell': 26.2
          },
          {
            'Symbol': 'FTC',
            'Buy': 67.87,
            'Sell': 67.19
          },
          {
            'Symbol': 'FTCS',
            'Buy': 53.19,
            'Sell': 52.66
          },
          {
            'Symbol': 'FTD',
            'Buy': 3.77,
            'Sell': 3.73
          },
          {
            'Symbol': 'FTEK',
            'Buy': 1.02,
            'Sell': 1.01
          },
          {
            'Symbol': 'FTFT',
            'Buy': 1.64,
            'Sell': 1.62
          },
          {
            'Symbol': 'FTGC',
            'Buy': 19.85,
            'Sell': 19.65
          },
          {
            'Symbol': 'FTHI',
            'Buy': 23,
            'Sell': 22.77
          },
          {
            'Symbol': 'FTLB',
            'Buy': 22.66,
            'Sell': 22.43
          },
          {
            'Symbol': 'FTNT',
            'Buy': 74.01,
            'Sell': 73.27
          },
          {
            'Symbol': 'FTR',
            'Buy': 5.23,
            'Sell': 5.18
          },
          {
            'Symbol': 'FTRI',
            'Buy': 12.36,
            'Sell': 12.24
          },
          {
            'Symbol': 'FTSL',
            'Buy': 48.09,
            'Sell': 47.61
          },
          {
            'Symbol': 'FTSM',
            'Buy': 60.06,
            'Sell': 59.46
          },
          {
            'Symbol': 'FTSV',
            'Buy': 14.76,
            'Sell': 14.61
          },
          {
            'Symbol': 'FTXG',
            'Buy': 19.79,
            'Sell': 19.59
          },
          {
            'Symbol': 'FTXH',
            'Buy': 23.4,
            'Sell': 23.17
          },
          {
            'Symbol': 'FTXL',
            'Buy': 32.62,
            'Sell': 32.29
          },
          {
            'Symbol': 'FTXN',
            'Buy': 24.4,
            'Sell': 24.16
          },
          {
            'Symbol': 'FTXO',
            'Buy': 29.57,
            'Sell': 29.27
          },
          {
            'Symbol': 'FULT',
            'Buy': 17.55,
            'Sell': 17.37
          },
          {
            'Symbol': 'FUNC',
            'Buy': 20.25,
            'Sell': 20.05
          },
          {
            'Symbol': 'FUND',
            'Buy': 7.73,
            'Sell': 7.65
          },
          {
            'Symbol': 'FUSB',
            'Buy': 11.3,
            'Sell': 11.19
          },
          {
            'Symbol': 'FUV',
            'Buy': 3.84,
            'Sell': 3.8
          },
          {
            'Symbol': 'FV',
            'Buy': 30.6,
            'Sell': 30.29
          },
          {
            'Symbol': 'FVC',
            'Buy': 28.42,
            'Sell': 28.14
          },
          {
            'Symbol': 'FVE',
            'Buy': 1.2,
            'Sell': 1.19
          },
          {
            'Symbol': 'FWONA',
            'Buy': 33.05,
            'Sell': 32.72
          },
          {
            'Symbol': 'FWONK',
            'Buy': 35.02,
            'Sell': 34.67
          },
          {
            'Symbol': 'FWP',
            'Buy': 2.8,
            'Sell': 2.77
          },
          {
            'Symbol': 'FWRD',
            'Buy': 62.85,
            'Sell': 62.22
          },
          {
            'Symbol': 'FYC',
            'Buy': 49.85,
            'Sell': 49.35
          },
          {
            'Symbol': 'FYT',
            'Buy': 39,
            'Sell': 38.61
          },
          {
            'Symbol': 'FYX',
            'Buy': 67.67,
            'Sell': 66.99
          },
          {
            'Symbol': 'GABC',
            'Buy': 36.57,
            'Sell': 36.2
          },
          {
            'Symbol': 'GAIA',
            'Buy': 17.45,
            'Sell': 17.28
          },
          {
            'Symbol': 'GAIN',
            'Buy': 11.85,
            'Sell': 11.73
          },
          {
            'Symbol': 'GAINM',
            'Buy': 25.305,
            'Sell': 25.05
          },
          {
            'Symbol': 'GAINO',
            'Buy': 25.27,
            'Sell': 25.02
          },
          {
            'Symbol': 'GALT',
            'Buy': 4.5,
            'Sell': 4.46
          },
          {
            'Symbol': 'GARS',
            'Buy': 8.39,
            'Sell': 8.31
          },
          {
            'Symbol': 'GASS',
            'Buy': 3.6,
            'Sell': 3.56
          },
          {
            'Symbol': 'GBCI',
            'Buy': 43.33,
            'Sell': 42.9
          },
          {
            'Symbol': 'GBDC',
            'Buy': 18.93,
            'Sell': 18.74
          },
          {
            'Symbol': 'GBLI',
            'Buy': 39.98,
            'Sell': 39.58
          },
          {
            'Symbol': 'GBLIL',
            'Buy': 25.77,
            'Sell': 25.51
          },
          {
            'Symbol': 'GBLIZ',
            'Buy': 25.55,
            'Sell': 25.29
          },
          {
            'Symbol': 'GBNK',
            'Buy': 31.05,
            'Sell': 30.74
          },
          {
            'Symbol': 'GBT',
            'Buy': 46.4,
            'Sell': 45.94
          },
          {
            'Symbol': 'GCBC',
            'Buy': 34,
            'Sell': 33.66
          },
          {
            'Symbol': 'GCVRZ',
            'Buy': 0.55,
            'Sell': 0.54
          },
          {
            'Symbol': 'GDEN',
            'Buy': 25.69,
            'Sell': 25.43
          },
          {
            'Symbol': 'GDS',
            'Buy': 30.67,
            'Sell': 30.36
          },
          {
            'Symbol': 'GEC',
            'Buy': 3.1,
            'Sell': 3.07
          },
          {
            'Symbol': 'GECC',
            'Buy': 9.41,
            'Sell': 9.32
          },
          {
            'Symbol': 'GECCL',
            'Buy': 25.44,
            'Sell': 25.19
          },
          {
            'Symbol': 'GECCM',
            'Buy': 24.95,
            'Sell': 24.7
          },
          {
            'Symbol': 'GEMP',
            'Buy': 1.03,
            'Sell': 1.02
          },
          {
            'Symbol': 'GENC',
            'Buy': 12.5,
            'Sell': 12.38
          },
          {
            'Symbol': 'GENE',
            'Buy': 1.14,
            'Sell': 1.13
          },
          {
            'Symbol': 'GENY',
            'Buy': 37.73,
            'Sell': 37.35
          },
          {
            'Symbol': 'GEOS',
            'Buy': 13.03,
            'Sell': 12.9
          },
          {
            'Symbol': 'GERN',
            'Buy': 3.7,
            'Sell': 3.66
          },
          {
            'Symbol': 'GEVO',
            'Buy': 3.5,
            'Sell': 3.47
          },
          {
            'Symbol': 'GFED',
            'Buy': 24.85,
            'Sell': 24.6
          },
          {
            'Symbol': 'GFN',
            'Buy': 13,
            'Sell': 12.87
          },
          {
            'Symbol': 'GFNSL',
            'Buy': 25.55,
            'Sell': 25.29
          },
          {
            'Symbol': 'GGAL',
            'Buy': 33.39,
            'Sell': 33.06
          },
          {
            'Symbol': 'GHDX',
            'Buy': 54.93,
            'Sell': 54.38
          },
          {
            'Symbol': 'GIFI',
            'Buy': 9.38,
            'Sell': 9.29
          },
          {
            'Symbol': 'GIGM',
            'Buy': 2.89,
            'Sell': 2.86
          },
          {
            'Symbol': 'GIII',
            'Buy': 45.8,
            'Sell': 45.34
          },
          {
            'Symbol': 'GILD',
            'Buy': 77.52,
            'Sell': 76.74
          },
          {
            'Symbol': 'GILT',
            'Buy': 8.75,
            'Sell': 8.66
          },
          {
            'Symbol': 'GLACU',
            'Buy': 10.18,
            'Sell': 10.08
          },
          {
            'Symbol': 'GLAD',
            'Buy': 9.71,
            'Sell': 9.61
          },
          {
            'Symbol': 'GLADN',
            'Buy': 25.5,
            'Sell': 25.25
          },
          {
            'Symbol': 'GLBS',
            'Buy': 0.43,
            'Sell': 0.43
          },
          {
            'Symbol': 'GLBZ',
            'Buy': 12.95,
            'Sell': 12.82
          },
          {
            'Symbol': 'GLDD',
            'Buy': 5.3,
            'Sell': 5.25
          },
          {
            'Symbol': 'GLDI',
            'Buy': 8.2,
            'Sell': 8.12
          },
          {
            'Symbol': 'GLG',
            'Buy': 0.577,
            'Sell': 0.57
          },
          {
            'Symbol': 'GLIBA',
            'Buy': 48.22,
            'Sell': 47.74
          },
          {
            'Symbol': 'GLIBP',
            'Buy': 24.34,
            'Sell': 24.1
          },
          {
            'Symbol': 'GLMD',
            'Buy': 12.65,
            'Sell': 12.52
          },
          {
            'Symbol': 'GLNG',
            'Buy': 25.03,
            'Sell': 24.78
          },
          {
            'Symbol': 'GLPG',
            'Buy': 106.93,
            'Sell': 105.86
          },
          {
            'Symbol': 'GLPI',
            'Buy': 34.7,
            'Sell': 34.35
          },
          {
            'Symbol': 'GLRE',
            'Buy': 13.2,
            'Sell': 13.07
          },
          {
            'Symbol': 'GLUU',
            'Buy': 6.34,
            'Sell': 6.28
          },
          {
            'Symbol': 'GLYC',
            'Buy': 15.16,
            'Sell': 15.01
          },
          {
            'Symbol': 'GMLP',
            'Buy': 15.12,
            'Sell': 14.97
          },
          {
            'Symbol': 'GMLPP',
            'Buy': 25.5,
            'Sell': 25.25
          },
          {
            'Symbol': 'GNBC',
            'Buy': 24.35,
            'Sell': 24.11
          },
          {
            'Symbol': 'GNCA',
            'Buy': 0.61,
            'Sell': 0.6
          },
          {
            'Symbol': 'GNMA',
            'Buy': 48.41,
            'Sell': 47.93
          },
          {
            'Symbol': 'GNMK',
            'Buy': 7.05,
            'Sell': 6.98
          },
          {
            'Symbol': 'GNMX',
            'Buy': 1,
            'Sell': 0.99
          },
          {
            'Symbol': 'GNPX',
            'Buy': 3.03,
            'Sell': 3
          },
          {
            'Symbol': 'GNRX',
            'Buy': 25.06,
            'Sell': 24.81
          },
          {
            'Symbol': 'GNTX',
            'Buy': 23.52,
            'Sell': 23.28
          },
          {
            'Symbol': 'GNTY',
            'Buy': 30.61,
            'Sell': 30.3
          },
          {
            'Symbol': 'GNUS',
            'Buy': 2.62,
            'Sell': 2.59
          },
          {
            'Symbol': 'GOGL',
            'Buy': 8.84,
            'Sell': 8.75
          },
          {
            'Symbol': 'GOGO',
            'Buy': 4.25,
            'Sell': 4.21
          },
          {
            'Symbol': 'GOLD',
            'Buy': 69.07,
            'Sell': 68.38
          },
          {
            'Symbol': 'GOOD',
            'Buy': 19.76,
            'Sell': 19.56
          },
          {
            'Symbol': 'GOODM',
            'Buy': 25.38,
            'Sell': 25.13
          },
          {
            'Symbol': 'GOODO',
            'Buy': 26.49,
            'Sell': 26.23
          },
          {
            'Symbol': 'GOOG',
            'Buy': 1243,
            'Sell': 1230.57
          },
          {
            'Symbol': 'GOOGL',
            'Buy': 1259.18,
            'Sell': 1246.59
          },
          {
            'Symbol': 'GOV',
            'Buy': 16.02,
            'Sell': 15.86
          },
          {
            'Symbol': 'GOVNI',
            'Buy': 25.45,
            'Sell': 25.2
          },
          {
            'Symbol': 'GPAQ',
            'Buy': 9.81,
            'Sell': 9.71
          },
          {
            'Symbol': 'GPIC',
            'Buy': 8.3,
            'Sell': 8.22
          },
          {
            'Symbol': 'GPOR',
            'Buy': 11.31,
            'Sell': 11.2
          },
          {
            'Symbol': 'GPP',
            'Buy': 15.95,
            'Sell': 15.79
          },
          {
            'Symbol': 'GPRE',
            'Buy': 17.25,
            'Sell': 17.08
          },
          {
            'Symbol': 'GPRO',
            'Buy': 6.13,
            'Sell': 6.07
          },
          {
            'Symbol': 'GRBK',
            'Buy': 10.3,
            'Sell': 10.2
          },
          {
            'Symbol': 'GRFS',
            'Buy': 21.02,
            'Sell': 20.81
          },
          {
            'Symbol': 'GRID',
            'Buy': 49.44,
            'Sell': 48.95
          },
          {
            'Symbol': 'GRIF',
            'Buy': 38,
            'Sell': 37.62
          },
          {
            'Symbol': 'GRIN',
            'Buy': 9,
            'Sell': 8.91
          },
          {
            'Symbol': 'GRMN',
            'Buy': 64.36,
            'Sell': 63.72
          },
          {
            'Symbol': 'GRNQ',
            'Buy': 5.51,
            'Sell': 5.45
          },
          {
            'Symbol': 'GROW',
            'Buy': 1.72,
            'Sell': 1.7
          },
          {
            'Symbol': 'GRPN',
            'Buy': 4.45,
            'Sell': 4.41
          },
          {
            'Symbol': 'GRVY',
            'Buy': 52.5,
            'Sell': 51.98
          },
          {
            'Symbol': 'GSBC',
            'Buy': 60,
            'Sell': 59.4
          },
          {
            'Symbol': 'GSHD',
            'Buy': 25.8,
            'Sell': 25.54
          },
          {
            'Symbol': 'GSHT',
            'Buy': 10.36,
            'Sell': 10.26
          },
          {
            'Symbol': 'GSHTU',
            'Buy': 11.05,
            'Sell': 10.94
          },
          {
            'Symbol': 'GSHTW',
            'Buy': 2.19,
            'Sell': 2.17
          },
          {
            'Symbol': 'GSIT',
            'Buy': 6.56,
            'Sell': 6.49
          },
          {
            'Symbol': 'GSKY',
            'Buy': 16.92,
            'Sell': 16.75
          },
          {
            'Symbol': 'GSM',
            'Buy': 7.45,
            'Sell': 7.38
          },
          {
            'Symbol': 'GSUM',
            'Buy': 6.34,
            'Sell': 6.28
          },
          {
            'Symbol': 'GSVC',
            'Buy': 7.09,
            'Sell': 7.02
          },
          {
            'Symbol': 'GT',
            'Buy': 24.7,
            'Sell': 24.45
          },
          {
            'Symbol': 'GTHX',
            'Buy': 56.22,
            'Sell': 55.66
          },
          {
            'Symbol': 'GTIM',
            'Buy': 4.25,
            'Sell': 4.21
          },
          {
            'Symbol': 'GTLS',
            'Buy': 76.31,
            'Sell': 75.55
          },
          {
            'Symbol': 'GTXI',
            'Buy': 17.7,
            'Sell': 17.52
          },
          {
            'Symbol': 'GULF',
            'Buy': 19.69,
            'Sell': 19.49
          },
          {
            'Symbol': 'GURE',
            'Buy': 1.21,
            'Sell': 1.2
          },
          {
            'Symbol': 'GVP',
            'Buy': 2.95,
            'Sell': 2.92
          },
          {
            'Symbol': 'GWGH',
            'Buy': 8.05,
            'Sell': 7.97
          },
          {
            'Symbol': 'GWPH',
            'Buy': 135.1,
            'Sell': 133.75
          },
          {
            'Symbol': 'GWRS',
            'Buy': 9.6,
            'Sell': 9.5
          },
          {
            'Symbol': 'GYRO',
            'Buy': 20.7,
            'Sell': 20.49
          },
          {
            'Symbol': 'HA',
            'Buy': 41.7,
            'Sell': 41.28
          },
          {
            'Symbol': 'HABT',
            'Buy': 15.35,
            'Sell': 15.2
          },
          {
            'Symbol': 'HAFC',
            'Buy': 25.3,
            'Sell': 25.05
          },
          {
            'Symbol': 'HAIN',
            'Buy': 28.3,
            'Sell': 28.02
          },
          {
            'Symbol': 'HAIR',
            'Buy': 1.48,
            'Sell': 1.47
          },
          {
            'Symbol': 'HALL',
            'Buy': 10.47,
            'Sell': 10.37
          },
          {
            'Symbol': 'HALO',
            'Buy': 17.29,
            'Sell': 17.12
          },
          {
            'Symbol': 'HAS',
            'Buy': 97.77,
            'Sell': 96.79
          },
          {
            'Symbol': 'HAYN',
            'Buy': 39.61,
            'Sell': 39.21
          },
          {
            'Symbol': 'HBAN',
            'Buy': 15.72,
            'Sell': 15.56
          },
          {
            'Symbol': 'HBANN',
            'Buy': 25.65,
            'Sell': 25.39
          },
          {
            'Symbol': 'HBANO',
            'Buy': 26.29,
            'Sell': 26.03
          },
          {
            'Symbol': 'HBCP',
            'Buy': 44.92,
            'Sell': 44.47
          },
          {
            'Symbol': 'HBIO',
            'Buy': 6.65,
            'Sell': 6.58
          },
          {
            'Symbol': 'HBK',
            'Buy': 14.7,
            'Sell': 14.55
          },
          {
            'Symbol': 'HBMD',
            'Buy': 16.5,
            'Sell': 16.34
          },
          {
            'Symbol': 'HBNC',
            'Buy': 20.78,
            'Sell': 20.57
          },
          {
            'Symbol': 'HBP',
            'Buy': 5.01,
            'Sell': 4.96
          },
          {
            'Symbol': 'HCAP',
            'Buy': 11.1,
            'Sell': 10.99
          },
          {
            'Symbol': 'HCAPZ',
            'Buy': 25.56,
            'Sell': 25.3
          },
          {
            'Symbol': 'HCCH',
            'Buy': 9.6,
            'Sell': 9.5
          },
          {
            'Symbol': 'HCCI',
            'Buy': 23.65,
            'Sell': 23.41
          },
          {
            'Symbol': 'HCKT',
            'Buy': 18.99,
            'Sell': 18.8
          },
          {
            'Symbol': 'HCM',
            'Buy': 32.1,
            'Sell': 31.78
          },
          {
            'Symbol': 'HCSG',
            'Buy': 40.29,
            'Sell': 39.89
          },
          {
            'Symbol': 'HDP',
            'Buy': 20.57,
            'Sell': 20.36
          },
          {
            'Symbol': 'HDS',
            'Buy': 43.73,
            'Sell': 43.29
          },
          {
            'Symbol': 'HDSN',
            'Buy': 1.9,
            'Sell': 1.88
          },
          {
            'Symbol': 'HEAR',
            'Buy': 29.23,
            'Sell': 28.94
          },
          {
            'Symbol': 'HEBT',
            'Buy': 1.51,
            'Sell': 1.49
          },
          {
            'Symbol': 'HEES',
            'Buy': 35.63,
            'Sell': 35.27
          },
          {
            'Symbol': 'HELE',
            'Buy': 111.8,
            'Sell': 110.68
          },
          {
            'Symbol': 'HEWG',
            'Buy': 27.66,
            'Sell': 27.38
          },
          {
            'Symbol': 'HFBC',
            'Buy': 17.75,
            'Sell': 17.57
          },
          {
            'Symbol': 'HFBL',
            'Buy': 33.75,
            'Sell': 33.41
          },
          {
            'Symbol': 'HFWA',
            'Buy': 35.2,
            'Sell': 34.85
          },
          {
            'Symbol': 'HGSH',
            'Buy': 1.46,
            'Sell': 1.45
          },
          {
            'Symbol': 'HIBB',
            'Buy': 23.65,
            'Sell': 23.41
          },
          {
            'Symbol': 'HIFS',
            'Buy': 220.25,
            'Sell': 218.05
          },
          {
            'Symbol': 'HIHO',
            'Buy': 3.95,
            'Sell': 3.91
          },
          {
            'Symbol': 'HIIQ',
            'Buy': 45.1,
            'Sell': 44.65
          },
          {
            'Symbol': 'HIMX',
            'Buy': 6.52,
            'Sell': 6.45
          },
          {
            'Symbol': 'HJLI',
            'Buy': 2.43,
            'Sell': 2.41
          },
          {
            'Symbol': 'HLG',
            'Buy': 81.79,
            'Sell': 80.97
          },
          {
            'Symbol': 'HLIT',
            'Buy': 5.2,
            'Sell': 5.15
          },
          {
            'Symbol': 'HLNE',
            'Buy': 46.3,
            'Sell': 45.84
          },
          {
            'Symbol': 'HMHC',
            'Buy': 5.65,
            'Sell': 5.59
          },
          {
            'Symbol': 'HMNF',
            'Buy': 21,
            'Sell': 20.79
          },
          {
            'Symbol': 'HMNY',
            'Buy': 0.06,
            'Sell': 0.06
          },
          {
            'Symbol': 'HMST',
            'Buy': 29.75,
            'Sell': 29.45
          },
          {
            'Symbol': 'HMSY',
            'Buy': 29.95,
            'Sell': 29.65
          },
          {
            'Symbol': 'HMTA',
            'Buy': 13.6,
            'Sell': 13.46
          },
          {
            'Symbol': 'HMTV',
            'Buy': 12.8,
            'Sell': 12.67
          },
          {
            'Symbol': 'HNDL',
            'Buy': 24.27,
            'Sell': 24.03
          },
          {
            'Symbol': 'HNNA',
            'Buy': 16.29,
            'Sell': 16.13
          },
          {
            'Symbol': 'HNRG',
            'Buy': 6.63,
            'Sell': 6.56
          },
          {
            'Symbol': 'HOFT',
            'Buy': 45.65,
            'Sell': 45.19
          },
          {
            'Symbol': 'HOLI',
            'Buy': 23.38,
            'Sell': 23.15
          },
          {
            'Symbol': 'HOLX',
            'Buy': 41.11,
            'Sell': 40.7
          },
          {
            'Symbol': 'HOMB',
            'Buy': 23.35,
            'Sell': 23.12
          },
          {
            'Symbol': 'HONE',
            'Buy': 18.23,
            'Sell': 18.05
          },
          {
            'Symbol': 'HOPE',
            'Buy': 17.21,
            'Sell': 17.04
          },
          {
            'Symbol': 'HPJ',
            'Buy': 3.2,
            'Sell': 3.17
          },
          {
            'Symbol': 'HPT',
            'Buy': 27.86,
            'Sell': 27.58
          },
          {
            'Symbol': 'HQCL',
            'Buy': 8.3,
            'Sell': 8.22
          },
          {
            'Symbol': 'HQY',
            'Buy': 81.89,
            'Sell': 81.07
          },
          {
            'Symbol': 'HRTX',
            'Buy': 38.9,
            'Sell': 38.51
          },
          {
            'Symbol': 'HRZN',
            'Buy': 11.35,
            'Sell': 11.24
          },
          {
            'Symbol': 'HSDT',
            'Buy': 9.11,
            'Sell': 9.02
          },
          {
            'Symbol': 'HSGX',
            'Buy': 2.42,
            'Sell': 2.4
          },
          {
            'Symbol': 'HSIC',
            'Buy': 78.21,
            'Sell': 77.43
          },
          {
            'Symbol': 'HSII',
            'Buy': 43.55,
            'Sell': 43.11
          },
          {
            'Symbol': 'HSKA',
            'Buy': 97.25,
            'Sell': 96.28
          },
          {
            'Symbol': 'HSON',
            'Buy': 1.73,
            'Sell': 1.71
          },
          {
            'Symbol': 'HSTM',
            'Buy': 29.63,
            'Sell': 29.33
          },
          {
            'Symbol': 'HTBI',
            'Buy': 29.65,
            'Sell': 29.35
          },
          {
            'Symbol': 'HTBK',
            'Buy': 14.65,
            'Sell': 14.5
          },
          {
            'Symbol': 'HTBX',
            'Buy': 2.07,
            'Sell': 2.05
          },
          {
            'Symbol': 'HTGM',
            'Buy': 3.63,
            'Sell': 3.59
          },
          {
            'Symbol': 'HTHT',
            'Buy': 34.45,
            'Sell': 34.11
          },
          {
            'Symbol': 'HTLD',
            'Buy': 19.32,
            'Sell': 19.13
          },
          {
            'Symbol': 'HTLF',
            'Buy': 59.4,
            'Sell': 58.81
          },
          {
            'Symbol': 'HUBG',
            'Buy': 52.15,
            'Sell': 51.63
          },
          {
            'Symbol': 'HUNTW',
            'Buy': 0.45,
            'Sell': 0.45
          },
          {
            'Symbol': 'HURC',
            'Buy': 43.3,
            'Sell': 42.87
          },
          {
            'Symbol': 'HURN',
            'Buy': 48.1,
            'Sell': 47.62
          },
          {
            'Symbol': 'HVBC',
            'Buy': 15.465,
            'Sell': 15.31
          },
          {
            'Symbol': 'HWBK',
            'Buy': 22.355,
            'Sell': 22.13
          },
          {
            'Symbol': 'HWC',
            'Buy': 51.05,
            'Sell': 50.54
          },
          {
            'Symbol': 'HWCC',
            'Buy': 8.45,
            'Sell': 8.37
          },
          {
            'Symbol': 'HWCPL',
            'Buy': 25.64,
            'Sell': 25.38
          },
          {
            'Symbol': 'HWKN',
            'Buy': 39.35,
            'Sell': 38.96
          },
          {
            'Symbol': 'HX',
            'Buy': 8.27,
            'Sell': 8.19
          },
          {
            'Symbol': 'HYACU',
            'Buy': 10.4,
            'Sell': 10.3
          },
          {
            'Symbol': 'HYGS',
            'Buy': 6.15,
            'Sell': 6.09
          },
          {
            'Symbol': 'HYLS',
            'Buy': 47.81,
            'Sell': 47.33
          },
          {
            'Symbol': 'HYND',
            'Buy': 21.26,
            'Sell': 21.05
          },
          {
            'Symbol': 'HYRE',
            'Buy': 3.04,
            'Sell': 3.01
          },
          {
            'Symbol': 'HYXE',
            'Buy': 50.55,
            'Sell': 50.04
          },
          {
            'Symbol': 'HYZD',
            'Buy': 24.01,
            'Sell': 23.77
          },
          {
            'Symbol': 'HZNP',
            'Buy': 20.98,
            'Sell': 20.77
          },
          {
            'Symbol': 'IAC',
            'Buy': 182.74,
            'Sell': 180.91
          },
          {
            'Symbol': 'IAM',
            'Buy': 10.05,
            'Sell': 9.95
          },
          {
            'Symbol': 'IART',
            'Buy': 62.26,
            'Sell': 61.64
          },
          {
            'Symbol': 'IBB',
            'Buy': 117,
            'Sell': 115.83
          },
          {
            'Symbol': 'IBCP',
            'Buy': 24.4,
            'Sell': 24.16
          },
          {
            'Symbol': 'IBKC',
            'Buy': 84.25,
            'Sell': 83.41
          },
          {
            'Symbol': 'IBKCP',
            'Buy': 26.65,
            'Sell': 26.38
          },
          {
            'Symbol': 'IBKR',
            'Buy': 61.15,
            'Sell': 60.54
          },
          {
            'Symbol': 'IBOC',
            'Buy': 45.35,
            'Sell': 44.9
          },
          {
            'Symbol': 'IBTX',
            'Buy': 68.8,
            'Sell': 68.11
          },
          {
            'Symbol': 'IBUY',
            'Buy': 52.61,
            'Sell': 52.08
          },
          {
            'Symbol': 'ICAD',
            'Buy': 2.99,
            'Sell': 2.96
          },
          {
            'Symbol': 'ICBK',
            'Buy': 25.63,
            'Sell': 25.37
          },
          {
            'Symbol': 'ICCC',
            'Buy': 6.5,
            'Sell': 6.44
          },
          {
            'Symbol': 'ICCH',
            'Buy': 15.1,
            'Sell': 14.95
          },
          {
            'Symbol': 'ICFI',
            'Buy': 75.55,
            'Sell': 74.79
          },
          {
            'Symbol': 'ICHR',
            'Buy': 22.5,
            'Sell': 22.28
          },
          {
            'Symbol': 'ICLK',
            'Buy': 6.16,
            'Sell': 6.1
          },
          {
            'Symbol': 'ICLN',
            'Buy': 8.81,
            'Sell': 8.72
          },
          {
            'Symbol': 'ICLR',
            'Buy': 143.95,
            'Sell': 142.51
          },
          {
            'Symbol': 'ICON',
            'Buy': 0.49,
            'Sell': 0.49
          },
          {
            'Symbol': 'ICPT',
            'Buy': 114.45,
            'Sell': 113.31
          },
          {
            'Symbol': 'ICUI',
            'Buy': 320.75,
            'Sell': 317.54
          },
          {
            'Symbol': 'IDCC',
            'Buy': 80.85,
            'Sell': 80.04
          },
          {
            'Symbol': 'IDLB',
            'Buy': 29.09,
            'Sell': 28.8
          },
          {
            'Symbol': 'IDRA',
            'Buy': 7.19,
            'Sell': 7.12
          },
          {
            'Symbol': 'IDSA',
            'Buy': 2.43,
            'Sell': 2.41
          },
          {
            'Symbol': 'IDSY',
            'Buy': 6.97,
            'Sell': 6.9
          },
          {
            'Symbol': 'IDTI',
            'Buy': 36.14,
            'Sell': 35.78
          },
          {
            'Symbol': 'IDXG',
            'Buy': 1.07,
            'Sell': 1.06
          },
          {
            'Symbol': 'IDXX',
            'Buy': 242.92,
            'Sell': 240.49
          },
          {
            'Symbol': 'IEA',
            'Buy': 9.95,
            'Sell': 9.85
          },
          {
            'Symbol': 'IEAWW',
            'Buy': 1.5,
            'Sell': 1.49
          },
          {
            'Symbol': 'IEF',
            'Buy': 102.2,
            'Sell': 101.18
          },
          {
            'Symbol': 'IEI',
            'Buy': 119.77,
            'Sell': 118.57
          },
          {
            'Symbol': 'IEP',
            'Buy': 77.76,
            'Sell': 76.98
          },
          {
            'Symbol': 'IESC',
            'Buy': 19.5,
            'Sell': 19.31
          },
          {
            'Symbol': 'IEUS',
            'Buy': 55.39,
            'Sell': 54.84
          },
          {
            'Symbol': 'IFEU',
            'Buy': 39.4,
            'Sell': 39.01
          },
          {
            'Symbol': 'IFGL',
            'Buy': 28.74,
            'Sell': 28.45
          },
          {
            'Symbol': 'IFMK',
            'Buy': 2.9,
            'Sell': 2.87
          },
          {
            'Symbol': 'IFRX',
            'Buy': 30.62,
            'Sell': 30.31
          },
          {
            'Symbol': 'IFV',
            'Buy': 20.77,
            'Sell': 20.56
          },
          {
            'Symbol': 'IGF',
            'Buy': 43.36,
            'Sell': 42.93
          },
          {
            'Symbol': 'IGIB',
            'Buy': 53.21,
            'Sell': 52.68
          },
          {
            'Symbol': 'IGLD',
            'Buy': 2.19,
            'Sell': 2.17
          },
          {
            'Symbol': 'IGOV',
            'Buy': 48.26,
            'Sell': 47.78
          },
          {
            'Symbol': 'IGSB',
            'Buy': 51.84,
            'Sell': 51.32
          },
          {
            'Symbol': 'III',
            'Buy': 4.15,
            'Sell': 4.11
          },
          {
            'Symbol': 'IIIN',
            'Buy': 42.61,
            'Sell': 42.18
          },
          {
            'Symbol': 'IIIV',
            'Buy': 16.86,
            'Sell': 16.69
          },
          {
            'Symbol': 'IIJI',
            'Buy': 9.53,
            'Sell': 9.43
          },
          {
            'Symbol': 'IIN',
            'Buy': 64.9,
            'Sell': 64.25
          },
          {
            'Symbol': 'IIVI',
            'Buy': 44.45,
            'Sell': 44.01
          },
          {
            'Symbol': 'IJT',
            'Buy': 199.6,
            'Sell': 197.6
          },
          {
            'Symbol': 'ILG',
            'Buy': 34,
            'Sell': 33.66
          },
          {
            'Symbol': 'ILMN',
            'Buy': 331.63,
            'Sell': 328.31
          },
          {
            'Symbol': 'ILPT',
            'Buy': 23.66,
            'Sell': 23.42
          },
          {
            'Symbol': 'IMDZ',
            'Buy': 3.6,
            'Sell': 3.56
          },
          {
            'Symbol': 'IMGN',
            'Buy': 8.82,
            'Sell': 8.73
          },
          {
            'Symbol': 'IMI',
            'Buy': 1.18,
            'Sell': 1.17
          },
          {
            'Symbol': 'IMKTA',
            'Buy': 30.2,
            'Sell': 29.9
          },
          {
            'Symbol': 'IMMP',
            'Buy': 2.72,
            'Sell': 2.69
          },
          {
            'Symbol': 'IMMR',
            'Buy': 11.43,
            'Sell': 11.32
          },
          {
            'Symbol': 'IMMU',
            'Buy': 23.15,
            'Sell': 22.92
          },
          {
            'Symbol': 'IMMY',
            'Buy': 2.74,
            'Sell': 2.71
          },
          {
            'Symbol': 'IMOS',
            'Buy': 14.8,
            'Sell': 14.65
          },
          {
            'Symbol': 'IMPV',
            'Buy': 45.7,
            'Sell': 45.24
          },
          {
            'Symbol': 'IMRN',
            'Buy': 10.2,
            'Sell': 10.1
          },
          {
            'Symbol': 'IMTE',
            'Buy': 13.16,
            'Sell': 13.03
          },
          {
            'Symbol': 'IMV',
            'Buy': 5.05,
            'Sell': 5
          },
          {
            'Symbol': 'IMXI',
            'Buy': 9.34,
            'Sell': 9.25
          },
          {
            'Symbol': 'IMXIW',
            'Buy': 1.75,
            'Sell': 1.73
          },
          {
            'Symbol': 'INAP',
            'Buy': 13.89,
            'Sell': 13.75
          },
          {
            'Symbol': 'INBK',
            'Buy': 31.4,
            'Sell': 31.09
          },
          {
            'Symbol': 'INCY',
            'Buy': 63.88,
            'Sell': 63.24
          },
          {
            'Symbol': 'INDB',
            'Buy': 87.6,
            'Sell': 86.72
          },
          {
            'Symbol': 'INDU',
            'Buy': 9.95,
            'Sell': 9.85
          },
          {
            'Symbol': 'INDY',
            'Buy': 37.23,
            'Sell': 36.86
          },
          {
            'Symbol': 'INFI',
            'Buy': 2.05,
            'Sell': 2.03
          },
          {
            'Symbol': 'INFN',
            'Buy': 8.5,
            'Sell': 8.42
          },
          {
            'Symbol': 'INFO',
            'Buy': 53.89,
            'Sell': 53.35
          },
          {
            'Symbol': 'INFR',
            'Buy': 28.35,
            'Sell': 28.07
          },
          {
            'Symbol': 'INGN',
            'Buy': 227.72,
            'Sell': 225.44
          },
          {
            'Symbol': 'INNT',
            'Buy': 5.98,
            'Sell': 5.92
          },
          {
            'Symbol': 'INO',
            'Buy': 4.35,
            'Sell': 4.31
          },
          {
            'Symbol': 'INOD',
            'Buy': 1.23,
            'Sell': 1.22
          },
          {
            'Symbol': 'INOV',
            'Buy': 11.8,
            'Sell': 11.68
          },
          {
            'Symbol': 'INPX',
            'Buy': 0.11,
            'Sell': 0.11
          },
          {
            'Symbol': 'INSE',
            'Buy': 7.5,
            'Sell': 7.43
          },
          {
            'Symbol': 'INSG',
            'Buy': 2.39,
            'Sell': 2.37
          },
          {
            'Symbol': 'INSM',
            'Buy': 21.81,
            'Sell': 21.59
          },
          {
            'Symbol': 'INSY',
            'Buy': 7.39,
            'Sell': 7.32
          },
          {
            'Symbol': 'INTC',
            'Buy': 48.51,
            'Sell': 48.02
          },
          {
            'Symbol': 'INTL',
            'Buy': 55.11,
            'Sell': 54.56
          },
          {
            'Symbol': 'INTU',
            'Buy': 209.59,
            'Sell': 207.49
          },
          {
            'Symbol': 'INTX',
            'Buy': 1.69,
            'Sell': 1.67
          },
          {
            'Symbol': 'INVA',
            'Buy': 13.89,
            'Sell': 13.75
          },
          {
            'Symbol': 'INVE',
            'Buy': 6.5,
            'Sell': 6.44
          },
          {
            'Symbol': 'INWK',
            'Buy': 6.86,
            'Sell': 6.79
          },
          {
            'Symbol': 'IONS',
            'Buy': 47.77,
            'Sell': 47.29
          },
          {
            'Symbol': 'IOSP',
            'Buy': 74.45,
            'Sell': 73.71
          },
          {
            'Symbol': 'IOTS',
            'Buy': 5.7,
            'Sell': 5.64
          },
          {
            'Symbol': 'IOVA',
            'Buy': 13.9,
            'Sell': 13.76
          },
          {
            'Symbol': 'IPAR',
            'Buy': 61.75,
            'Sell': 61.13
          },
          {
            'Symbol': 'IPAS',
            'Buy': 0.28,
            'Sell': 0.28
          },
          {
            'Symbol': 'IPCI',
            'Buy': 0.33,
            'Sell': 0.33
          },
          {
            'Symbol': 'IPDN',
            'Buy': 3.24,
            'Sell': 3.21
          },
          {
            'Symbol': 'IPGP',
            'Buy': 163.09,
            'Sell': 161.46
          },
          {
            'Symbol': 'IPHS',
            'Buy': 43.76,
            'Sell': 43.32
          },
          {
            'Symbol': 'IPIC',
            'Buy': 6.51,
            'Sell': 6.44
          },
          {
            'Symbol': 'IPKW',
            'Buy': 35.36,
            'Sell': 35.01
          },
          {
            'Symbol': 'IPWR',
            'Buy': 0.715,
            'Sell': 0.71
          },
          {
            'Symbol': 'IQ',
            'Buy': 28.6,
            'Sell': 28.31
          },
          {
            'Symbol': 'IRBT',
            'Buy': 85.96,
            'Sell': 85.1
          },
          {
            'Symbol': 'IRCP',
            'Buy': 31.03,
            'Sell': 30.72
          },
          {
            'Symbol': 'IRDM',
            'Buy': 20.3,
            'Sell': 20.1
          },
          {
            'Symbol': 'IRIX',
            'Buy': 7.89,
            'Sell': 7.81
          },
          {
            'Symbol': 'IRMD',
            'Buy': 27.35,
            'Sell': 27.08
          },
          {
            'Symbol': 'IROQ',
            'Buy': 24.26,
            'Sell': 24.02
          },
          {
            'Symbol': 'IRTC',
            'Buy': 82.95,
            'Sell': 82.12
          },
          {
            'Symbol': 'IRWD',
            'Buy': 19.11,
            'Sell': 18.92
          },
          {
            'Symbol': 'ISBC',
            'Buy': 12.59,
            'Sell': 12.46
          },
          {
            'Symbol': 'ISCA',
            'Buy': 43.85,
            'Sell': 43.41
          },
          {
            'Symbol': 'ISHG',
            'Buy': 81.1,
            'Sell': 80.29
          },
          {
            'Symbol': 'ISIG',
            'Buy': 1.92,
            'Sell': 1.9
          },
          {
            'Symbol': 'ISNS',
            'Buy': 4.55,
            'Sell': 4.5
          },
          {
            'Symbol': 'ISRG',
            'Buy': 521.16,
            'Sell': 515.95
          },
          {
            'Symbol': 'ISSC',
            'Buy': 2.55,
            'Sell': 2.52
          },
          {
            'Symbol': 'ISTB',
            'Buy': 49.09,
            'Sell': 48.6
          },
          {
            'Symbol': 'ISTR',
            'Buy': 27.05,
            'Sell': 26.78
          },
          {
            'Symbol': 'ITCI',
            'Buy': 21.35,
            'Sell': 21.14
          },
          {
            'Symbol': 'ITI',
            'Buy': 4.8,
            'Sell': 4.75
          },
          {
            'Symbol': 'ITIC',
            'Buy': 197,
            'Sell': 195.03
          },
          {
            'Symbol': 'ITRI',
            'Buy': 62.85,
            'Sell': 62.22
          },
          {
            'Symbol': 'ITRM',
            'Buy': 8.74,
            'Sell': 8.65
          },
          {
            'Symbol': 'ITRN',
            'Buy': 33.5,
            'Sell': 33.17
          },
          {
            'Symbol': 'ITUS',
            'Buy': 3.35,
            'Sell': 3.32
          },
          {
            'Symbol': 'IUSB',
            'Buy': 49.35,
            'Sell': 48.86
          },
          {
            'Symbol': 'IUSG',
            'Buy': 60.17,
            'Sell': 59.57
          },
          {
            'Symbol': 'IUSV',
            'Buy': 55.52,
            'Sell': 54.96
          },
          {
            'Symbol': 'IVAC',
            'Buy': 4.65,
            'Sell': 4.6
          },
          {
            'Symbol': 'IVTY',
            'Buy': 3.6,
            'Sell': 3.56
          },
          {
            'Symbol': 'IXUS',
            'Buy': 59.87,
            'Sell': 59.27
          },
          {
            'Symbol': 'IZEA',
            'Buy': 1.23,
            'Sell': 1.22
          },
          {
            'Symbol': 'JACK',
            'Buy': 92.77,
            'Sell': 91.84
          },
          {
            'Symbol': 'JAGX',
            'Buy': 1.1,
            'Sell': 1.09
          },
          {
            'Symbol': 'JAKK',
            'Buy': 2.25,
            'Sell': 2.23
          },
          {
            'Symbol': 'JASN',
            'Buy': 2.42,
            'Sell': 2.4
          },
          {
            'Symbol': 'JASNW',
            'Buy': 0.02,
            'Sell': 0.02
          },
          {
            'Symbol': 'JAZZ',
            'Buy': 176.61,
            'Sell': 174.84
          },
          {
            'Symbol': 'JBHT',
            'Buy': 120.96,
            'Sell': 119.75
          },
          {
            'Symbol': 'JBLU',
            'Buy': 18.4,
            'Sell': 18.22
          },
          {
            'Symbol': 'JBSS',
            'Buy': 76.91,
            'Sell': 76.14
          },
          {
            'Symbol': 'JCOM',
            'Buy': 80.12,
            'Sell': 79.32
          },
          {
            'Symbol': 'JCS',
            'Buy': 3.51,
            'Sell': 3.47
          },
          {
            'Symbol': 'JCTCF',
            'Buy': 8.1,
            'Sell': 8.02
          },
          {
            'Symbol': 'JD',
            'Buy': 35.62,
            'Sell': 35.26
          },
          {
            'Symbol': 'JG',
            'Buy': 5.62,
            'Sell': 5.56
          },
          {
            'Symbol': 'JJSF',
            'Buy': 143.45,
            'Sell': 142.02
          },
          {
            'Symbol': 'JKHY',
            'Buy': 140.63,
            'Sell': 139.22
          },
          {
            'Symbol': 'JKI',
            'Buy': 161.97,
            'Sell': 160.35
          },
          {
            'Symbol': 'JMBA',
            'Buy': 12.94,
            'Sell': 12.81
          },
          {
            'Symbol': 'JMU',
            'Buy': 1.37,
            'Sell': 1.36
          },
          {
            'Symbol': 'JNCE',
            'Buy': 7.79,
            'Sell': 7.71
          },
          {
            'Symbol': 'JNP',
            'Buy': 11.5,
            'Sell': 11.39
          },
          {
            'Symbol': 'JOBS',
            'Buy': 72.73,
            'Sell': 72
          },
          {
            'Symbol': 'JOUT',
            'Buy': 95.66,
            'Sell': 94.7
          },
          {
            'Symbol': 'JRJC',
            'Buy': 1.91,
            'Sell': 1.89
          },
          {
            'Symbol': 'JRSH',
            'Buy': 6.52,
            'Sell': 6.45
          },
          {
            'Symbol': 'JRVR',
            'Buy': 39.77,
            'Sell': 39.37
          },
          {
            'Symbol': 'JSM',
            'Buy': 23.47,
            'Sell': 23.24
          },
          {
            'Symbol': 'JSMD',
            'Buy': 44.71,
            'Sell': 44.26
          },
          {
            'Symbol': 'JSML',
            'Buy': 43.62,
            'Sell': 43.18
          },
          {
            'Symbol': 'JSYNW',
            'Buy': 0.17,
            'Sell': 0.17
          },
          {
            'Symbol': 'JTPY',
            'Buy': 1.75,
            'Sell': 1.73
          },
          {
            'Symbol': 'JVA',
            'Buy': 5.24,
            'Sell': 5.19
          },
          {
            'Symbol': 'JYNT',
            'Buy': 8.79,
            'Sell': 8.7
          },
          {
            'Symbol': 'KAAC',
            'Buy': 10.2,
            'Sell': 10.1
          },
          {
            'Symbol': 'KAACU',
            'Buy': 10.75,
            'Sell': 10.64
          },
          {
            'Symbol': 'KAACW',
            'Buy': 1.7,
            'Sell': 1.68
          },
          {
            'Symbol': 'KALA',
            'Buy': 12.75,
            'Sell': 12.62
          },
          {
            'Symbol': 'KALU',
            'Buy': 110.53,
            'Sell': 109.42
          },
          {
            'Symbol': 'KALV',
            'Buy': 12.4,
            'Sell': 12.28
          },
          {
            'Symbol': 'KANG',
            'Buy': 20.4,
            'Sell': 20.2
          },
          {
            'Symbol': 'KBAL',
            'Buy': 17.03,
            'Sell': 16.86
          },
          {
            'Symbol': 'KBLMW',
            'Buy': 0.24,
            'Sell': 0.24
          },
          {
            'Symbol': 'KBSF',
            'Buy': 4.03,
            'Sell': 3.99
          },
          {
            'Symbol': 'KBWB',
            'Buy': 56.4,
            'Sell': 55.84
          },
          {
            'Symbol': 'KBWD',
            'Buy': 23.29,
            'Sell': 23.06
          },
          {
            'Symbol': 'KBWP',
            'Buy': 62.34,
            'Sell': 61.72
          },
          {
            'Symbol': 'KBWR',
            'Buy': 58.19,
            'Sell': 57.61
          },
          {
            'Symbol': 'KBWY',
            'Buy': 34.64,
            'Sell': 34.29
          },
          {
            'Symbol': 'KCAP',
            'Buy': 3.23,
            'Sell': 3.2
          },
          {
            'Symbol': 'KE',
            'Buy': 19.55,
            'Sell': 19.35
          },
          {
            'Symbol': 'KELYA',
            'Buy': 24.26,
            'Sell': 24.02
          },
          {
            'Symbol': 'KEQU',
            'Buy': 34.4,
            'Sell': 34.06
          },
          {
            'Symbol': 'KERX',
            'Buy': 3.49,
            'Sell': 3.46
          },
          {
            'Symbol': 'KEYW',
            'Buy': 7.46,
            'Sell': 7.39
          },
          {
            'Symbol': 'KFFB',
            'Buy': 7.95,
            'Sell': 7.87
          },
          {
            'Symbol': 'KFRC',
            'Buy': 42.5,
            'Sell': 42.08
          },
          {
            'Symbol': 'KGJI',
            'Buy': 1.25,
            'Sell': 1.24
          },
          {
            'Symbol': 'KHC',
            'Buy': 59.76,
            'Sell': 59.16
          },
          {
            'Symbol': 'KIDS',
            'Buy': 27.52,
            'Sell': 27.24
          },
          {
            'Symbol': 'KIN',
            'Buy': 14.5,
            'Sell': 14.36
          },
          {
            'Symbol': 'KINS',
            'Buy': 17.05,
            'Sell': 16.88
          },
          {
            'Symbol': 'KIRK',
            'Buy': 11.13,
            'Sell': 11.02
          },
          {
            'Symbol': 'KLAC',
            'Buy': 115.31,
            'Sell': 114.16
          },
          {
            'Symbol': 'KLIC',
            'Buy': 26.71,
            'Sell': 26.44
          },
          {
            'Symbol': 'KLXI',
            'Buy': 72.65,
            'Sell': 71.92
          },
          {
            'Symbol': 'KMDA',
            'Buy': 5.25,
            'Sell': 5.2
          },
          {
            'Symbol': 'KMPH',
            'Buy': 4.8,
            'Sell': 4.75
          },
          {
            'Symbol': 'KNDI',
            'Buy': 3.8,
            'Sell': 3.76
          },
          {
            'Symbol': 'KNSA',
            'Buy': 14.1,
            'Sell': 13.96
          },
          {
            'Symbol': 'KNSL',
            'Buy': 59.25,
            'Sell': 58.66
          },
          {
            'Symbol': 'KONA',
            'Buy': 2.35,
            'Sell': 2.33
          },
          {
            'Symbol': 'KONE',
            'Buy': 7.26,
            'Sell': 7.19
          },
          {
            'Symbol': 'KOOL',
            'Buy': 0.34,
            'Sell': 0.34
          },
          {
            'Symbol': 'KOPN',
            'Buy': 2.31,
            'Sell': 2.29
          },
          {
            'Symbol': 'KOSS',
            'Buy': 2.75,
            'Sell': 2.72
          },
          {
            'Symbol': 'KPTI',
            'Buy': 18.01,
            'Sell': 17.83
          },
          {
            'Symbol': 'KRMA',
            'Buy': 20.71,
            'Sell': 20.5
          },
          {
            'Symbol': 'KRNT',
            'Buy': 19,
            'Sell': 18.81
          },
          {
            'Symbol': 'KRNY',
            'Buy': 13.55,
            'Sell': 13.41
          },
          {
            'Symbol': 'KRYS',
            'Buy': 16.24,
            'Sell': 16.08
          },
          {
            'Symbol': 'KTCC',
            'Buy': 7.6,
            'Sell': 7.52
          },
          {
            'Symbol': 'KTOS',
            'Buy': 13.01,
            'Sell': 12.88
          },
          {
            'Symbol': 'KTOV',
            'Buy': 1.88,
            'Sell': 1.86
          },
          {
            'Symbol': 'KTOVW',
            'Buy': 0.5,
            'Sell': 0.5
          },
          {
            'Symbol': 'KTWO',
            'Buy': 20.88,
            'Sell': 20.67
          },
          {
            'Symbol': 'KURA',
            'Buy': 19.25,
            'Sell': 19.06
          },
          {
            'Symbol': 'KVHI',
            'Buy': 12.85,
            'Sell': 12.72
          },
          {
            'Symbol': 'KZIA',
            'Buy': 3.24,
            'Sell': 3.21
          },
          {
            'Symbol': 'KZR',
            'Buy': 15.81,
            'Sell': 15.65
          },
          {
            'Symbol': 'LABL',
            'Buy': 65.05,
            'Sell': 64.4
          },
          {
            'Symbol': 'LACQ',
            'Buy': 9.68,
            'Sell': 9.58
          },
          {
            'Symbol': 'LAKE',
            'Buy': 13.7,
            'Sell': 13.56
          },
          {
            'Symbol': 'LALT',
            'Buy': 21.83,
            'Sell': 21.61
          },
          {
            'Symbol': 'LAMR',
            'Buy': 72.42,
            'Sell': 71.7
          },
          {
            'Symbol': 'LANC',
            'Buy': 145.34,
            'Sell': 143.89
          },
          {
            'Symbol': 'LAND',
            'Buy': 12.37,
            'Sell': 12.25
          },
          {
            'Symbol': 'LANDP',
            'Buy': 25.51,
            'Sell': 25.25
          },
          {
            'Symbol': 'LARK',
            'Buy': 29.03,
            'Sell': 28.74
          },
          {
            'Symbol': 'LASR',
            'Buy': 32.48,
            'Sell': 32.16
          },
          {
            'Symbol': 'LAUR',
            'Buy': 16.35,
            'Sell': 16.19
          },
          {
            'Symbol': 'LAWS',
            'Buy': 29.25,
            'Sell': 28.96
          },
          {
            'Symbol': 'LAZY',
            'Buy': 8.35,
            'Sell': 8.27
          },
          {
            'Symbol': 'LBAI',
            'Buy': 19.35,
            'Sell': 19.16
          },
          {
            'Symbol': 'LBC',
            'Buy': 10.62,
            'Sell': 10.51
          },
          {
            'Symbol': 'LBIX',
            'Buy': 1.2,
            'Sell': 1.19
          },
          {
            'Symbol': 'LBRDA',
            'Buy': 79.45,
            'Sell': 78.66
          },
          {
            'Symbol': 'LBRDK',
            'Buy': 79.53,
            'Sell': 78.73
          },
          {
            'Symbol': 'LBTYA',
            'Buy': 27.75,
            'Sell': 27.47
          },
          {
            'Symbol': 'LBTYK',
            'Buy': 26.68,
            'Sell': 26.41
          },
          {
            'Symbol': 'LCA',
            'Buy': 10.15,
            'Sell': 10.05
          },
          {
            'Symbol': 'LCAHW',
            'Buy': 0.85,
            'Sell': 0.84
          },
          {
            'Symbol': 'LCNB',
            'Buy': 20.35,
            'Sell': 20.15
          },
          {
            'Symbol': 'LCUT',
            'Buy': 11.55,
            'Sell': 11.43
          },
          {
            'Symbol': 'LE',
            'Buy': 25.95,
            'Sell': 25.69
          },
          {
            'Symbol': 'LECO',
            'Buy': 92.35,
            'Sell': 91.43
          },
          {
            'Symbol': 'LEDS',
            'Buy': 4.25,
            'Sell': 4.21
          },
          {
            'Symbol': 'LEGR',
            'Buy': 29.35,
            'Sell': 29.06
          },
          {
            'Symbol': 'LENS',
            'Buy': 2.12,
            'Sell': 2.1
          },
          {
            'Symbol': 'LEVL',
            'Buy': 27.41,
            'Sell': 27.14
          },
          {
            'Symbol': 'LEXEA',
            'Buy': 47.21,
            'Sell': 46.74
          },
          {
            'Symbol': 'LFACU',
            'Buy': 10.1,
            'Sell': 10
          },
          {
            'Symbol': 'LFUS',
            'Buy': 220.26,
            'Sell': 218.06
          },
          {
            'Symbol': 'LFVN',
            'Buy': 10.77,
            'Sell': 10.66
          },
          {
            'Symbol': 'LGCY',
            'Buy': 4.4,
            'Sell': 4.36
          },
          {
            'Symbol': 'LGCYO',
            'Buy': 12.21,
            'Sell': 12.09
          },
          {
            'Symbol': 'LGCYP',
            'Buy': 12.49,
            'Sell': 12.37
          },
          {
            'Symbol': 'LGIH',
            'Buy': 58.34,
            'Sell': 57.76
          },
          {
            'Symbol': 'LGND',
            'Buy': 239.12,
            'Sell': 236.73
          },
          {
            'Symbol': 'LHCG',
            'Buy': 92.38,
            'Sell': 91.46
          },
          {
            'Symbol': 'LIFE',
            'Buy': 0.7,
            'Sell': 0.69
          },
          {
            'Symbol': 'LILA',
            'Buy': 18.82,
            'Sell': 18.63
          },
          {
            'Symbol': 'LILAK',
            'Buy': 19.03,
            'Sell': 18.84
          },
          {
            'Symbol': 'LINC',
            'Buy': 2.05,
            'Sell': 2.03
          },
          {
            'Symbol': 'LIND',
            'Buy': 13.7,
            'Sell': 13.56
          },
          {
            'Symbol': 'LINDW',
            'Buy': 3.21,
            'Sell': 3.18
          },
          {
            'Symbol': 'LINK',
            'Buy': 4.05,
            'Sell': 4.01
          },
          {
            'Symbol': 'LION',
            'Buy': 23.73,
            'Sell': 23.49
          },
          {
            'Symbol': 'LITE',
            'Buy': 57.8,
            'Sell': 57.22
          },
          {
            'Symbol': 'LIVE',
            'Buy': 10.734,
            'Sell': 10.63
          },
          {
            'Symbol': 'LIVN',
            'Buy': 122.86,
            'Sell': 121.63
          },
          {
            'Symbol': 'LIVX',
            'Buy': 4.85,
            'Sell': 4.8
          },
          {
            'Symbol': 'LJPC',
            'Buy': 24.11,
            'Sell': 23.87
          },
          {
            'Symbol': 'LKFN',
            'Buy': 48.79,
            'Sell': 48.3
          },
          {
            'Symbol': 'LKOR',
            'Buy': 50.26,
            'Sell': 49.76
          },
          {
            'Symbol': 'LKQ',
            'Buy': 33.84,
            'Sell': 33.5
          },
          {
            'Symbol': 'LLIT',
            'Buy': 1.65,
            'Sell': 1.63
          },
          {
            'Symbol': 'LLNW',
            'Buy': 4.2,
            'Sell': 4.16
          },
          {
            'Symbol': 'LMAT',
            'Buy': 36.62,
            'Sell': 36.25
          },
          {
            'Symbol': 'LMB',
            'Buy': 10.65,
            'Sell': 10.54
          },
          {
            'Symbol': 'LMBS',
            'Buy': 51.19,
            'Sell': 50.68
          },
          {
            'Symbol': 'LMFA',
            'Buy': 0.37,
            'Sell': 0.37
          },
          {
            'Symbol': 'LMNR',
            'Buy': 29.54,
            'Sell': 29.24
          },
          {
            'Symbol': 'LMNX',
            'Buy': 26.86,
            'Sell': 26.59
          },
          {
            'Symbol': 'LMRK',
            'Buy': 12.7,
            'Sell': 12.57
          },
          {
            'Symbol': 'LMRKN',
            'Buy': 22.47,
            'Sell': 22.25
          },
          {
            'Symbol': 'LMRKO',
            'Buy': 23.48,
            'Sell': 23.25
          },
          {
            'Symbol': 'LMRKP',
            'Buy': 24.52,
            'Sell': 24.27
          },
          {
            'Symbol': 'LNDC',
            'Buy': 13.25,
            'Sell': 13.12
          },
          {
            'Symbol': 'LNGR',
            'Buy': 21.08,
            'Sell': 20.87
          },
          {
            'Symbol': 'LNTH',
            'Buy': 13.45,
            'Sell': 13.32
          },
          {
            'Symbol': 'LOAN',
            'Buy': 6.75,
            'Sell': 6.68
          },
          {
            'Symbol': 'LOB',
            'Buy': 29.8,
            'Sell': 29.5
          },
          {
            'Symbol': 'LOCO',
            'Buy': 11.6,
            'Sell': 11.48
          },
          {
            'Symbol': 'LOGI',
            'Buy': 45.54,
            'Sell': 45.08
          },
          {
            'Symbol': 'LOGM',
            'Buy': 81.3,
            'Sell': 80.49
          },
          {
            'Symbol': 'LONE',
            'Buy': 8.64,
            'Sell': 8.55
          },
          {
            'Symbol': 'LOOP',
            'Buy': 9.19,
            'Sell': 9.1
          },
          {
            'Symbol': 'LOPE',
            'Buy': 114.51,
            'Sell': 113.36
          },
          {
            'Symbol': 'LORL',
            'Buy': 42.15,
            'Sell': 41.73
          },
          {
            'Symbol': 'LOVE',
            'Buy': 20.34,
            'Sell': 20.14
          },
          {
            'Symbol': 'LOXO',
            'Buy': 157.81,
            'Sell': 156.23
          },
          {
            'Symbol': 'LPCN',
            'Buy': 1.5,
            'Sell': 1.49
          },
          {
            'Symbol': 'LPLA',
            'Buy': 65.71,
            'Sell': 65.05
          },
          {
            'Symbol': 'LPNT',
            'Buy': 64.6,
            'Sell': 63.95
          },
          {
            'Symbol': 'LPSN',
            'Buy': 23.9,
            'Sell': 23.66
          },
          {
            'Symbol': 'LPTH',
            'Buy': 2.15,
            'Sell': 2.13
          },
          {
            'Symbol': 'LPTX',
            'Buy': 6.41,
            'Sell': 6.35
          },
          {
            'Symbol': 'LQDA',
            'Buy': 12.97,
            'Sell': 12.84
          },
          {
            'Symbol': 'LQDT',
            'Buy': 7.25,
            'Sell': 7.18
          },
          {
            'Symbol': 'LRAD',
            'Buy': 2.82,
            'Sell': 2.79
          },
          {
            'Symbol': 'LRCX',
            'Buy': 180.57,
            'Sell': 178.76
          },
          {
            'Symbol': 'LRGE',
            'Buy': 33.21,
            'Sell': 32.88
          },
          {
            'Symbol': 'LSBK',
            'Buy': 17.25,
            'Sell': 17.08
          },
          {
            'Symbol': 'LSCC',
            'Buy': 7.61,
            'Sell': 7.53
          },
          {
            'Symbol': 'LSTR',
            'Buy': 114.2,
            'Sell': 113.06
          },
          {
            'Symbol': 'LSXMA',
            'Buy': 46.66,
            'Sell': 46.19
          },
          {
            'Symbol': 'LSXMK',
            'Buy': 46.77,
            'Sell': 46.3
          },
          {
            'Symbol': 'LTBR',
            'Buy': 0.89,
            'Sell': 0.88
          },
          {
            'Symbol': 'LTRPA',
            'Buy': 15.7,
            'Sell': 15.54
          },
          {
            'Symbol': 'LTRX',
            'Buy': 3.35,
            'Sell': 3.32
          },
          {
            'Symbol': 'LTXB',
            'Buy': 43.11,
            'Sell': 42.68
          },
          {
            'Symbol': 'LULU',
            'Buy': 127.48,
            'Sell': 126.21
          },
          {
            'Symbol': 'LUNA',
            'Buy': 3.91,
            'Sell': 3.87
          },
          {
            'Symbol': 'LVHD',
            'Buy': 30.65,
            'Sell': 30.34
          },
          {
            'Symbol': 'LWAY',
            'Buy': 3.7,
            'Sell': 3.66
          },
          {
            'Symbol': 'LX',
            'Buy': 12.7,
            'Sell': 12.57
          },
          {
            'Symbol': 'LXRX',
            'Buy': 11.02,
            'Sell': 10.91
          },
          {
            'Symbol': 'LYL',
            'Buy': 2.28,
            'Sell': 2.26
          },
          {
            'Symbol': 'LYTS',
            'Buy': 4.54,
            'Sell': 4.49
          },
          {
            'Symbol': 'MACK',
            'Buy': 5.35,
            'Sell': 5.3
          },
          {
            'Symbol': 'MAGS',
            'Buy': 5.345,
            'Sell': 5.29
          },
          {
            'Symbol': 'MAMS',
            'Buy': 8,
            'Sell': 7.92
          },
          {
            'Symbol': 'MANH',
            'Buy': 50.73,
            'Sell': 50.22
          },
          {
            'Symbol': 'MANT',
            'Buy': 62.46,
            'Sell': 61.84
          },
          {
            'Symbol': 'MAR',
            'Buy': 120.57,
            'Sell': 119.36
          },
          {
            'Symbol': 'MARA',
            'Buy': 0.96,
            'Sell': 0.95
          },
          {
            'Symbol': 'MARK',
            'Buy': 3.93,
            'Sell': 3.89
          },
          {
            'Symbol': 'MARPS',
            'Buy': 4.07,
            'Sell': 4.03
          },
          {
            'Symbol': 'MASI',
            'Buy': 109.06,
            'Sell': 107.97
          },
          {
            'Symbol': 'MAT',
            'Buy': 15.52,
            'Sell': 15.36
          },
          {
            'Symbol': 'MATR',
            'Buy': 2.55,
            'Sell': 2.52
          },
          {
            'Symbol': 'MATW',
            'Buy': 51.35,
            'Sell': 50.84
          },
          {
            'Symbol': 'MAYS',
            'Buy': 39.4,
            'Sell': 39.01
          },
          {
            'Symbol': 'MB',
            'Buy': 36.1,
            'Sell': 35.74
          },
          {
            'Symbol': 'MBB',
            'Buy': 103.92,
            'Sell': 102.88
          },
          {
            'Symbol': 'MBCN',
            'Buy': 50.5,
            'Sell': 50
          },
          {
            'Symbol': 'MBFI',
            'Buy': 47.91,
            'Sell': 47.43
          },
          {
            'Symbol': 'MBFIO',
            'Buy': 25.76,
            'Sell': 25.5
          },
          {
            'Symbol': 'MBII',
            'Buy': 1.94,
            'Sell': 1.92
          },
          {
            'Symbol': 'MBIN',
            'Buy': 25.21,
            'Sell': 24.96
          },
          {
            'Symbol': 'MBIO',
            'Buy': 6.54,
            'Sell': 6.47
          },
          {
            'Symbol': 'MBOT',
            'Buy': 0.619,
            'Sell': 0.61
          },
          {
            'Symbol': 'MBRX',
            'Buy': 1.64,
            'Sell': 1.62
          },
          {
            'Symbol': 'MBSD',
            'Buy': 23.06,
            'Sell': 22.83
          },
          {
            'Symbol': 'MBTF',
            'Buy': 11,
            'Sell': 10.89
          },
          {
            'Symbol': 'MBUU',
            'Buy': 38.38,
            'Sell': 38
          },
          {
            'Symbol': 'MBWM',
            'Buy': 35.49,
            'Sell': 35.14
          },
          {
            'Symbol': 'MCBC',
            'Buy': 12.4,
            'Sell': 12.28
          },
          {
            'Symbol': 'MCEF',
            'Buy': 17.83,
            'Sell': 17.65
          },
          {
            'Symbol': 'MCEP',
            'Buy': 1.62,
            'Sell': 1.6
          },
          {
            'Symbol': 'MCFT',
            'Buy': 25.61,
            'Sell': 25.35
          },
          {
            'Symbol': 'MCHI',
            'Buy': 61.9,
            'Sell': 61.28
          },
          {
            'Symbol': 'MCHP',
            'Buy': 88.4,
            'Sell': 87.52
          },
          {
            'Symbol': 'MCHX',
            'Buy': 2.77,
            'Sell': 2.74
          },
          {
            'Symbol': 'MCRB',
            'Buy': 8.07,
            'Sell': 7.99
          },
          {
            'Symbol': 'MCRI',
            'Buy': 46.73,
            'Sell': 46.26
          },
          {
            'Symbol': 'MDB',
            'Buy': 61.05,
            'Sell': 60.44
          },
          {
            'Symbol': 'MDCA',
            'Buy': 5.2,
            'Sell': 5.15
          },
          {
            'Symbol': 'MDCO',
            'Buy': 38.87,
            'Sell': 38.48
          },
          {
            'Symbol': 'MDGL',
            'Buy': 234.51,
            'Sell': 232.16
          },
          {
            'Symbol': 'MDGS',
            'Buy': 3.04,
            'Sell': 3.01
          },
          {
            'Symbol': 'MDGSW',
            'Buy': 0.53,
            'Sell': 0.52
          },
          {
            'Symbol': 'MDIV',
            'Buy': 18.62,
            'Sell': 18.43
          },
          {
            'Symbol': 'MDLZ',
            'Buy': 42.11,
            'Sell': 41.69
          },
          {
            'Symbol': 'MDRX',
            'Buy': 13.45,
            'Sell': 13.32
          },
          {
            'Symbol': 'MDSO',
            'Buy': 76.58,
            'Sell': 75.81
          },
          {
            'Symbol': 'MDWD',
            'Buy': 6.43,
            'Sell': 6.37
          },
          {
            'Symbol': 'MDXG',
            'Buy': 4.32,
            'Sell': 4.28
          },
          {
            'Symbol': 'MEDP',
            'Buy': 58.32,
            'Sell': 57.74
          },
          {
            'Symbol': 'MEET',
            'Buy': 4.24,
            'Sell': 4.2
          },
          {
            'Symbol': 'MEIP',
            'Buy': 3.64,
            'Sell': 3.6
          },
          {
            'Symbol': 'MELI',
            'Buy': 378.86,
            'Sell': 375.07
          },
          {
            'Symbol': 'MEOH',
            'Buy': 71.45,
            'Sell': 70.74
          },
          {
            'Symbol': 'MERC',
            'Buy': 17.85,
            'Sell': 17.67
          },
          {
            'Symbol': 'MESA',
            'Buy': 12,
            'Sell': 11.88
          },
          {
            'Symbol': 'MESO',
            'Buy': 6.35,
            'Sell': 6.29
          },
          {
            'Symbol': 'METC',
            'Buy': 7.26,
            'Sell': 7.19
          },
          {
            'Symbol': 'MFIN',
            'Buy': 6.24,
            'Sell': 6.18
          },
          {
            'Symbol': 'MFINL',
            'Buy': 25.31,
            'Sell': 25.06
          },
          {
            'Symbol': 'MFNC',
            'Buy': 16,
            'Sell': 15.84
          },
          {
            'Symbol': 'MFSF',
            'Buy': 38.25,
            'Sell': 37.87
          },
          {
            'Symbol': 'MGEE',
            'Buy': 65,
            'Sell': 64.35
          },
          {
            'Symbol': 'MGEN',
            'Buy': 5.86,
            'Sell': 5.8
          },
          {
            'Symbol': 'MGI',
            'Buy': 6.07,
            'Sell': 6.01
          },
          {
            'Symbol': 'MGIC',
            'Buy': 8.7,
            'Sell': 8.61
          },
          {
            'Symbol': 'MGLN',
            'Buy': 75.35,
            'Sell': 74.6
          },
          {
            'Symbol': 'MGNX',
            'Buy': 20.85,
            'Sell': 20.64
          },
          {
            'Symbol': 'MGPI',
            'Buy': 74.64,
            'Sell': 73.89
          },
          {
            'Symbol': 'MGRC',
            'Buy': 57.27,
            'Sell': 56.7
          },
          {
            'Symbol': 'MGTA',
            'Buy': 12.66,
            'Sell': 12.53
          },
          {
            'Symbol': 'MGTX',
            'Buy': 9.07,
            'Sell': 8.98
          },
          {
            'Symbol': 'MGYR',
            'Buy': 12.67,
            'Sell': 12.54
          },
          {
            'Symbol': 'MHLD',
            'Buy': 4.7,
            'Sell': 4.65
          },
          {
            'Symbol': 'MICT',
            'Buy': 1.46,
            'Sell': 1.45
          },
          {
            'Symbol': 'MIDD',
            'Buy': 117.8,
            'Sell': 116.62
          },
          {
            'Symbol': 'MIK',
            'Buy': 20.17,
            'Sell': 19.97
          },
          {
            'Symbol': 'MILN',
            'Buy': 22.79,
            'Sell': 22.56
          },
          {
            'Symbol': 'MIME',
            'Buy': 38.12,
            'Sell': 37.74
          },
          {
            'Symbol': 'MIND',
            'Buy': 4.01,
            'Sell': 3.97
          },
          {
            'Symbol': 'MINDP',
            'Buy': 23.77,
            'Sell': 23.53
          },
          {
            'Symbol': 'MINI',
            'Buy': 45.65,
            'Sell': 45.19
          },
          {
            'Symbol': 'MITK',
            'Buy': 8.6,
            'Sell': 8.51
          },
          {
            'Symbol': 'MITL',
            'Buy': 10.99,
            'Sell': 10.88
          },
          {
            'Symbol': 'MKGI',
            'Buy': 2.06,
            'Sell': 2.04
          },
          {
            'Symbol': 'MKSI',
            'Buy': 91.4,
            'Sell': 90.49
          },
          {
            'Symbol': 'MKTX',
            'Buy': 185.26,
            'Sell': 183.41
          },
          {
            'Symbol': 'MLAB',
            'Buy': 201.01,
            'Sell': 199
          },
          {
            'Symbol': 'MLCO',
            'Buy': 23.05,
            'Sell': 22.82
          },
          {
            'Symbol': 'MLHR',
            'Buy': 37.8,
            'Sell': 37.42
          },
          {
            'Symbol': 'MLNT',
            'Buy': 4.9,
            'Sell': 4.85
          },
          {
            'Symbol': 'MLNX',
            'Buy': 81.1,
            'Sell': 80.29
          },
          {
            'Symbol': 'MLVF',
            'Buy': 24.7,
            'Sell': 24.45
          },
          {
            'Symbol': 'MMAC',
            'Buy': 27.6,
            'Sell': 27.32
          },
          {
            'Symbol': 'MMDM',
            'Buy': 10.02,
            'Sell': 9.92
          },
          {
            'Symbol': 'MMDMW',
            'Buy': 0.69,
            'Sell': 0.68
          },
          {
            'Symbol': 'MMLP',
            'Buy': 12.55,
            'Sell': 12.42
          },
          {
            'Symbol': 'MMSI',
            'Buy': 54.7,
            'Sell': 54.15
          },
          {
            'Symbol': 'MMYT',
            'Buy': 34.35,
            'Sell': 34.01
          },
          {
            'Symbol': 'MNDO',
            'Buy': 2.14,
            'Sell': 2.12
          },
          {
            'Symbol': 'MNGA',
            'Buy': 0.26,
            'Sell': 0.26
          },
          {
            'Symbol': 'MNKD',
            'Buy': 1.21,
            'Sell': 1.2
          },
          {
            'Symbol': 'MNLO',
            'Buy': 8.15,
            'Sell': 8.07
          },
          {
            'Symbol': 'MNOV',
            'Buy': 9.69,
            'Sell': 9.59
          },
          {
            'Symbol': 'MNRO',
            'Buy': 69.3,
            'Sell': 68.61
          },
          {
            'Symbol': 'MNST',
            'Buy': 60.58,
            'Sell': 59.97
          },
          {
            'Symbol': 'MNTA',
            'Buy': 25.7,
            'Sell': 25.44
          },
          {
            'Symbol': 'MNTX',
            'Buy': 11.53,
            'Sell': 11.41
          },
          {
            'Symbol': 'MOBL',
            'Buy': 4.95,
            'Sell': 4.9
          },
          {
            'Symbol': 'MOFG',
            'Buy': 32.44,
            'Sell': 32.12
          },
          {
            'Symbol': 'MOGO',
            'Buy': 3.6,
            'Sell': 3.56
          },
          {
            'Symbol': 'MOMO',
            'Buy': 41.03,
            'Sell': 40.62
          },
          {
            'Symbol': 'MOR',
            'Buy': 30.27,
            'Sell': 29.97
          },
          {
            'Symbol': 'MORN',
            'Buy': 133.58,
            'Sell': 132.24
          },
          {
            'Symbol': 'MOSY',
            'Buy': 1,
            'Sell': 0.99
          },
          {
            'Symbol': 'MOTS',
            'Buy': 6.07,
            'Sell': 6.01
          },
          {
            'Symbol': 'MOXC',
            'Buy': 1.6,
            'Sell': 1.58
          },
          {
            'Symbol': 'MPAA',
            'Buy': 23.44,
            'Sell': 23.21
          },
          {
            'Symbol': 'MPAC',
            'Buy': 10.03,
            'Sell': 9.93
          },
          {
            'Symbol': 'MPACU',
            'Buy': 10.66,
            'Sell': 10.55
          },
          {
            'Symbol': 'MPACW',
            'Buy': 0.8,
            'Sell': 0.79
          },
          {
            'Symbol': 'MPB',
            'Buy': 31.25,
            'Sell': 30.94
          },
          {
            'Symbol': 'MPCT',
            'Buy': 57.32,
            'Sell': 56.75
          },
          {
            'Symbol': 'MPVD',
            'Buy': 2.15,
            'Sell': 2.13
          },
          {
            'Symbol': 'MPWR',
            'Buy': 138,
            'Sell': 136.62
          },
          {
            'Symbol': 'MRAM',
            'Buy': 9.09,
            'Sell': 9
          },
          {
            'Symbol': 'MRBK',
            'Buy': 17.42,
            'Sell': 17.25
          },
          {
            'Symbol': 'MRCC',
            'Buy': 13.82,
            'Sell': 13.68
          },
          {
            'Symbol': 'MRCY',
            'Buy': 48.66,
            'Sell': 48.17
          },
          {
            'Symbol': 'MRIN',
            'Buy': 3.8,
            'Sell': 3.76
          },
          {
            'Symbol': 'MRLN',
            'Buy': 27.3,
            'Sell': 27.03
          },
          {
            'Symbol': 'MRNS',
            'Buy': 5.92,
            'Sell': 5.86
          },
          {
            'Symbol': 'MRSN',
            'Buy': 12.6,
            'Sell': 12.47
          },
          {
            'Symbol': 'MRTN',
            'Buy': 22.1,
            'Sell': 21.88
          },
          {
            'Symbol': 'MRTX',
            'Buy': 58.5,
            'Sell': 57.92
          },
          {
            'Symbol': 'MRUS',
            'Buy': 19.48,
            'Sell': 19.29
          },
          {
            'Symbol': 'MRVL',
            'Buy': 21.15,
            'Sell': 20.94
          },
          {
            'Symbol': 'MSBF',
            'Buy': 21.4,
            'Sell': 21.19
          },
          {
            'Symbol': 'MSBI',
            'Buy': 34.58,
            'Sell': 34.23
          },
          {
            'Symbol': 'MSEX',
            'Buy': 45.17,
            'Sell': 44.72
          },
          {
            'Symbol': 'MSFT',
            'Buy': 109.42,
            'Sell': 108.33
          },
          {
            'Symbol': 'MSON',
            'Buy': 17.3,
            'Sell': 17.13
          },
          {
            'Symbol': 'MSTR',
            'Buy': 139.14,
            'Sell': 137.75
          },
          {
            'Symbol': 'MTBC',
            'Buy': 4.25,
            'Sell': 4.21
          },
          {
            'Symbol': 'MTBCP',
            'Buy': 26.79,
            'Sell': 26.52
          },
          {
            'Symbol': 'MTCH',
            'Buy': 48.98,
            'Sell': 48.49
          },
          {
            'Symbol': 'MTEC',
            'Buy': 9.85,
            'Sell': 9.75
          },
          {
            'Symbol': 'MTEM',
            'Buy': 5.29,
            'Sell': 5.24
          },
          {
            'Symbol': 'MTEX',
            'Buy': 18.6,
            'Sell': 18.41
          },
          {
            'Symbol': 'MTFB',
            'Buy': 8.24,
            'Sell': 8.16
          },
          {
            'Symbol': 'MTGE',
            'Buy': 19.8,
            'Sell': 19.6
          },
          {
            'Symbol': 'MTGEP',
            'Buy': 25.65,
            'Sell': 25.39
          },
          {
            'Symbol': 'MTLS',
            'Buy': 13.35,
            'Sell': 13.22
          },
          {
            'Symbol': 'MTP',
            'Buy': 0.5,
            'Sell': 0.5
          },
          {
            'Symbol': 'MTRX',
            'Buy': 20.35,
            'Sell': 20.15
          },
          {
            'Symbol': 'MTSC',
            'Buy': 51.75,
            'Sell': 51.23
          },
          {
            'Symbol': 'MTSI',
            'Buy': 23.52,
            'Sell': 23.28
          },
          {
            'Symbol': 'MTSL',
            'Buy': 1.75,
            'Sell': 1.73
          },
          {
            'Symbol': 'MU',
            'Buy': 51.25,
            'Sell': 50.74
          },
          {
            'Symbol': 'MVBF',
            'Buy': 17.4,
            'Sell': 17.23
          },
          {
            'Symbol': 'MVIS',
            'Buy': 0.944,
            'Sell': 0.93
          },
          {
            'Symbol': 'MXIM',
            'Buy': 62.24,
            'Sell': 61.62
          },
          {
            'Symbol': 'MXWL',
            'Buy': 3.54,
            'Sell': 3.5
          },
          {
            'Symbol': 'MYFW',
            'Buy': 17.06,
            'Sell': 16.89
          },
          {
            'Symbol': 'MYGN',
            'Buy': 42.94,
            'Sell': 42.51
          },
          {
            'Symbol': 'MYL',
            'Buy': 36.51,
            'Sell': 36.14
          },
          {
            'Symbol': 'MYND',
            'Buy': 1.43,
            'Sell': 1.42
          },
          {
            'Symbol': 'MYNDW',
            'Buy': 0.13,
            'Sell': 0.13
          },
          {
            'Symbol': 'MYOK',
            'Buy': 58.65,
            'Sell': 58.06
          },
          {
            'Symbol': 'MYOS',
            'Buy': 1.255,
            'Sell': 1.24
          },
          {
            'Symbol': 'MYRG',
            'Buy': 35.91,
            'Sell': 35.55
          },
          {
            'Symbol': 'MYSZ',
            'Buy': 0.86,
            'Sell': 0.85
          },
          {
            'Symbol': 'MZOR',
            'Buy': 47.84,
            'Sell': 47.36
          },
          {
            'Symbol': 'NAII',
            'Buy': 9.75,
            'Sell': 9.65
          },
          {
            'Symbol': 'NAKD',
            'Buy': 4.37,
            'Sell': 4.33
          },
          {
            'Symbol': 'NANO',
            'Buy': 41.72,
            'Sell': 41.3
          },
          {
            'Symbol': 'NAOV',
            'Buy': 4.43,
            'Sell': 4.39
          },
          {
            'Symbol': 'NATH',
            'Buy': 88.9,
            'Sell': 88.01
          },
          {
            'Symbol': 'NATI',
            'Buy': 44.19,
            'Sell': 43.75
          },
          {
            'Symbol': 'NATR',
            'Buy': 8.95,
            'Sell': 8.86
          },
          {
            'Symbol': 'NAVG',
            'Buy': 58.55,
            'Sell': 57.96
          },
          {
            'Symbol': 'NAVI',
            'Buy': 13.19,
            'Sell': 13.06
          },
          {
            'Symbol': 'NBEV',
            'Buy': 1.88,
            'Sell': 1.86
          },
          {
            'Symbol': 'NBIX',
            'Buy': 118.59,
            'Sell': 117.4
          },
          {
            'Symbol': 'NBN',
            'Buy': 22.45,
            'Sell': 22.23
          },
          {
            'Symbol': 'NBRV',
            'Buy': 2.56,
            'Sell': 2.53
          },
          {
            'Symbol': 'NBTB',
            'Buy': 39.37,
            'Sell': 38.98
          },
          {
            'Symbol': 'NCBS',
            'Buy': 54.97,
            'Sell': 54.42
          },
          {
            'Symbol': 'NCMI',
            'Buy': 8.6,
            'Sell': 8.51
          },
          {
            'Symbol': 'NCNA',
            'Buy': 22.92,
            'Sell': 22.69
          },
          {
            'Symbol': 'NCOM',
            'Buy': 43.95,
            'Sell': 43.51
          },
          {
            'Symbol': 'NCSM',
            'Buy': 16.74,
            'Sell': 16.57
          },
          {
            'Symbol': 'NCTY',
            'Buy': 0.93,
            'Sell': 0.92
          },
          {
            'Symbol': 'NDAQ',
            'Buy': 92.21,
            'Sell': 91.29
          },
          {
            'Symbol': 'NDLS',
            'Buy': 10.3,
            'Sell': 10.2
          },
          {
            'Symbol': 'NDRA',
            'Buy': 1.99,
            'Sell': 1.97
          },
          {
            'Symbol': 'NDRAW',
            'Buy': 0.52,
            'Sell': 0.51
          },
          {
            'Symbol': 'NDSN',
            'Buy': 132.67,
            'Sell': 131.34
          },
          {
            'Symbol': 'NEBUU',
            'Buy': 10.19,
            'Sell': 10.09
          },
          {
            'Symbol': 'NEO',
            'Buy': 12.07,
            'Sell': 11.95
          },
          {
            'Symbol': 'NEOG',
            'Buy': 83.52,
            'Sell': 82.68
          },
          {
            'Symbol': 'NEON',
            'Buy': 0.345,
            'Sell': 0.34
          },
          {
            'Symbol': 'NEOS',
            'Buy': 6.05,
            'Sell': 5.99
          },
          {
            'Symbol': 'NEPT',
            'Buy': 2.77,
            'Sell': 2.74
          },
          {
            'Symbol': 'NERV',
            'Buy': 8.5,
            'Sell': 8.42
          },
          {
            'Symbol': 'NESR',
            'Buy': 11.15,
            'Sell': 11.04
          },
          {
            'Symbol': 'NESRW',
            'Buy': 1.49,
            'Sell': 1.48
          },
          {
            'Symbol': 'NETE',
            'Buy': 7,
            'Sell': 6.93
          },
          {
            'Symbol': 'NEWA',
            'Buy': 21.45,
            'Sell': 21.24
          },
          {
            'Symbol': 'NEWT',
            'Buy': 21.69,
            'Sell': 21.47
          },
          {
            'Symbol': 'NEWTI',
            'Buy': 25.79,
            'Sell': 25.53
          },
          {
            'Symbol': 'NEWTZ',
            'Buy': 25.6,
            'Sell': 25.34
          },
          {
            'Symbol': 'NEXT',
            'Buy': 6.27,
            'Sell': 6.21
          },
          {
            'Symbol': 'NFBK',
            'Buy': 16.26,
            'Sell': 16.1
          },
          {
            'Symbol': 'NFEC',
            'Buy': 5.86,
            'Sell': 5.8
          },
          {
            'Symbol': 'NFLX',
            'Buy': 346.91,
            'Sell': 343.44
          },
          {
            'Symbol': 'NFTY',
            'Buy': 38.01,
            'Sell': 37.63
          },
          {
            'Symbol': 'NGHC',
            'Buy': 28.01,
            'Sell': 27.73
          },
          {
            'Symbol': 'NGHCN',
            'Buy': 25.22,
            'Sell': 24.97
          },
          {
            'Symbol': 'NGHCO',
            'Buy': 25.29,
            'Sell': 25.04
          },
          {
            'Symbol': 'NGHCP',
            'Buy': 25.22,
            'Sell': 24.97
          },
          {
            'Symbol': 'NGHCZ',
            'Buy': 25.81,
            'Sell': 25.55
          },
          {
            'Symbol': 'NH',
            'Buy': 3.06,
            'Sell': 3.03
          },
          {
            'Symbol': 'NHLD',
            'Buy': 3.26,
            'Sell': 3.23
          },
          {
            'Symbol': 'NHTC',
            'Buy': 24.04,
            'Sell': 23.8
          },
          {
            'Symbol': 'NICE',
            'Buy': 107.61,
            'Sell': 106.53
          },
          {
            'Symbol': 'NICK',
            'Buy': 11,
            'Sell': 10.89
          },
          {
            'Symbol': 'NIHD',
            'Buy': 4.96,
            'Sell': 4.91
          },
          {
            'Symbol': 'NITE',
            'Buy': 17.8,
            'Sell': 17.62
          },
          {
            'Symbol': 'NK',
            'Buy': 3.26,
            'Sell': 3.23
          },
          {
            'Symbol': 'NKSH',
            'Buy': 46.05,
            'Sell': 45.59
          },
          {
            'Symbol': 'NKTR',
            'Buy': 58.35,
            'Sell': 57.77
          },
          {
            'Symbol': 'NLNK',
            'Buy': 3.09,
            'Sell': 3.06
          },
          {
            'Symbol': 'NLST',
            'Buy': 0.1,
            'Sell': 0.1
          },
          {
            'Symbol': 'NMIH',
            'Buy': 21.45,
            'Sell': 21.24
          },
          {
            'Symbol': 'NMRD',
            'Buy': 2.31,
            'Sell': 2.29
          },
          {
            'Symbol': 'NMRK',
            'Buy': 13.1,
            'Sell': 12.97
          },
          {
            'Symbol': 'NNBR',
            'Buy': 19.95,
            'Sell': 19.75
          },
          {
            'Symbol': 'NNDM',
            'Buy': 2.05,
            'Sell': 2.03
          },
          {
            'Symbol': 'NODK',
            'Buy': 16.66,
            'Sell': 16.49
          },
          {
            'Symbol': 'NOVN',
            'Buy': 2.767,
            'Sell': 2.74
          },
          {
            'Symbol': 'NOVT',
            'Buy': 66.4,
            'Sell': 65.74
          },
          {
            'Symbol': 'NRC',
            'Buy': 37.8,
            'Sell': 37.42
          },
          {
            'Symbol': 'NRIM',
            'Buy': 42.45,
            'Sell': 42.03
          },
          {
            'Symbol': 'NSEC',
            'Buy': 16.39,
            'Sell': 16.23
          },
          {
            'Symbol': 'NSIT',
            'Buy': 52.8,
            'Sell': 52.27
          },
          {
            'Symbol': 'NSSC',
            'Buy': 16.15,
            'Sell': 15.99
          },
          {
            'Symbol': 'NSTG',
            'Buy': 12.81,
            'Sell': 12.68
          },
          {
            'Symbol': 'NSYS',
            'Buy': 3.76,
            'Sell': 3.72
          },
          {
            'Symbol': 'NTAP',
            'Buy': 82.23,
            'Sell': 81.41
          },
          {
            'Symbol': 'NTCT',
            'Buy': 26.4,
            'Sell': 26.14
          },
          {
            'Symbol': 'NTEC',
            'Buy': 3.95,
            'Sell': 3.91
          },
          {
            'Symbol': 'NTES',
            'Buy': 226,
            'Sell': 223.74
          },
          {
            'Symbol': 'NTGN',
            'Buy': 12.12,
            'Sell': 12
          },
          {
            'Symbol': 'NTGR',
            'Buy': 66.55,
            'Sell': 65.88
          },
          {
            'Symbol': 'NTIC',
            'Buy': 32.4,
            'Sell': 32.08
          },
          {
            'Symbol': 'NTLA',
            'Buy': 27.45,
            'Sell': 27.18
          },
          {
            'Symbol': 'NTNX',
            'Buy': 52.95,
            'Sell': 52.42
          },
          {
            'Symbol': 'NTRA',
            'Buy': 23.98,
            'Sell': 23.74
          },
          {
            'Symbol': 'NTRI',
            'Buy': 40.6,
            'Sell': 40.19
          },
          {
            'Symbol': 'NTRP',
            'Buy': 10.11,
            'Sell': 10.01
          },
          {
            'Symbol': 'NTRS',
            'Buy': 109.44,
            'Sell': 108.35
          },
          {
            'Symbol': 'NTRSP',
            'Buy': 26.71,
            'Sell': 26.44
          },
          {
            'Symbol': 'NTWK',
            'Buy': 6.05,
            'Sell': 5.99
          },
          {
            'Symbol': 'NUAN',
            'Buy': 16.75,
            'Sell': 16.58
          },
          {
            'Symbol': 'NURO',
            'Buy': 1.21,
            'Sell': 1.2
          },
          {
            'Symbol': 'NUROW',
            'Buy': 0.08,
            'Sell': 0.08
          },
          {
            'Symbol': 'NUVA',
            'Buy': 63.88,
            'Sell': 63.24
          },
          {
            'Symbol': 'NVAX',
            'Buy': 1.47,
            'Sell': 1.46
          },
          {
            'Symbol': 'NVCN',
            'Buy': 0.038,
            'Sell': 0.04
          },
          {
            'Symbol': 'NVCR',
            'Buy': 34.85,
            'Sell': 34.5
          },
          {
            'Symbol': 'NVDA',
            'Buy': 253.15,
            'Sell': 250.62
          },
          {
            'Symbol': 'NVEC',
            'Buy': 115.08,
            'Sell': 113.93
          },
          {
            'Symbol': 'NVEE',
            'Buy': 83,
            'Sell': 82.17
          },
          {
            'Symbol': 'NVFY',
            'Buy': 1.82,
            'Sell': 1.8
          },
          {
            'Symbol': 'NVIV',
            'Buy': 2,
            'Sell': 1.98
          },
          {
            'Symbol': 'NVLN',
            'Buy': 3.25,
            'Sell': 3.22
          },
          {
            'Symbol': 'NVMI',
            'Buy': 27.85,
            'Sell': 27.57
          },
          {
            'Symbol': 'NVMM',
            'Buy': 1.7,
            'Sell': 1.68
          },
          {
            'Symbol': 'NVTR',
            'Buy': 17.2,
            'Sell': 17.03
          },
          {
            'Symbol': 'NVUS',
            'Buy': 5.4,
            'Sell': 5.35
          },
          {
            'Symbol': 'NWBI',
            'Buy': 17.77,
            'Sell': 17.59
          },
          {
            'Symbol': 'NWFL',
            'Buy': 37.39,
            'Sell': 37.02
          },
          {
            'Symbol': 'NWLI',
            'Buy': 328.61,
            'Sell': 325.32
          },
          {
            'Symbol': 'NWPX',
            'Buy': 18.81,
            'Sell': 18.62
          },
          {
            'Symbol': 'NWS',
            'Buy': 15.2,
            'Sell': 15.05
          },
          {
            'Symbol': 'NWSA',
            'Buy': 14.72,
            'Sell': 14.57
          },
          {
            'Symbol': 'NXEO',
            'Buy': 9.95,
            'Sell': 9.85
          },
          {
            'Symbol': 'NXEOU',
            'Buy': 9.97,
            'Sell': 9.87
          },
          {
            'Symbol': 'NXEOW',
            'Buy': 0.67,
            'Sell': 0.66
          },
          {
            'Symbol': 'NXPI',
            'Buy': 93.96,
            'Sell': 93.02
          },
          {
            'Symbol': 'NXST',
            'Buy': 76.15,
            'Sell': 75.39
          },
          {
            'Symbol': 'NXTD',
            'Buy': 1.63,
            'Sell': 1.61
          },
          {
            'Symbol': 'NXTM',
            'Buy': 28.22,
            'Sell': 27.94
          },
          {
            'Symbol': 'NYMT',
            'Buy': 6.12,
            'Sell': 6.06
          },
          {
            'Symbol': 'NYMTN',
            'Buy': 24,
            'Sell': 23.76
          },
          {
            'Symbol': 'NYMTO',
            'Buy': 24.34,
            'Sell': 24.1
          },
          {
            'Symbol': 'NYMTP',
            'Buy': 24.16,
            'Sell': 23.92
          },
          {
            'Symbol': 'NYMX',
            'Buy': 3.17,
            'Sell': 3.14
          },
          {
            'Symbol': 'NYNY',
            'Buy': 12.45,
            'Sell': 12.33
          },
          {
            'Symbol': 'OASM',
            'Buy': 1.71,
            'Sell': 1.69
          },
          {
            'Symbol': 'OBAS',
            'Buy': 8.7,
            'Sell': 8.61
          },
          {
            'Symbol': 'OBCI',
            'Buy': 3.7,
            'Sell': 3.66
          },
          {
            'Symbol': 'OBLN',
            'Buy': 1.78,
            'Sell': 1.76
          },
          {
            'Symbol': 'OBNK',
            'Buy': 39.62,
            'Sell': 39.22
          },
          {
            'Symbol': 'OBSV',
            'Buy': 15,
            'Sell': 14.85
          },
          {
            'Symbol': 'OCC',
            'Buy': 3.81,
            'Sell': 3.77
          },
          {
            'Symbol': 'OCFC',
            'Buy': 28.79,
            'Sell': 28.5
          },
          {
            'Symbol': 'OCLR',
            'Buy': 8.9,
            'Sell': 8.81
          },
          {
            'Symbol': 'OCSI',
            'Buy': 8.61,
            'Sell': 8.52
          },
          {
            'Symbol': 'OCSL',
            'Buy': 4.96,
            'Sell': 4.91
          },
          {
            'Symbol': 'OCSLL',
            'Buy': 24.7,
            'Sell': 24.45
          },
          {
            'Symbol': 'OCUL',
            'Buy': 6.23,
            'Sell': 6.17
          },
          {
            'Symbol': 'ODFL',
            'Buy': 146.17,
            'Sell': 144.71
          },
          {
            'Symbol': 'ODP',
            'Buy': 3.02,
            'Sell': 2.99
          },
          {
            'Symbol': 'ODT',
            'Buy': 19.16,
            'Sell': 18.97
          },
          {
            'Symbol': 'OESX',
            'Buy': 1,
            'Sell': 0.99
          },
          {
            'Symbol': 'OFED',
            'Buy': 26.765,
            'Sell': 26.5
          },
          {
            'Symbol': 'OFIX',
            'Buy': 54.37,
            'Sell': 53.83
          },
          {
            'Symbol': 'OFLX',
            'Buy': 87.72,
            'Sell': 86.84
          },
          {
            'Symbol': 'OFS',
            'Buy': 11.82,
            'Sell': 11.7
          },
          {
            'Symbol': 'OFSSL',
            'Buy': 24.76,
            'Sell': 24.51
          },
          {
            'Symbol': 'OHAI',
            'Buy': 1.45,
            'Sell': 1.44
          },
          {
            'Symbol': 'OHGI',
            'Buy': 0.25,
            'Sell': 0.25
          },
          {
            'Symbol': 'OHRP',
            'Buy': 0.16,
            'Sell': 0.16
          },
          {
            'Symbol': 'OIIM',
            'Buy': 2.12,
            'Sell': 2.1
          },
          {
            'Symbol': 'OKTA',
            'Buy': 56.27,
            'Sell': 55.71
          },
          {
            'Symbol': 'OLBK',
            'Buy': 34.59,
            'Sell': 34.24
          },
          {
            'Symbol': 'OLD',
            'Buy': 26.26,
            'Sell': 26
          },
          {
            'Symbol': 'OLED',
            'Buy': 111.25,
            'Sell': 110.14
          },
          {
            'Symbol': 'OLLI',
            'Buy': 71.2,
            'Sell': 70.49
          },
          {
            'Symbol': 'OMAB',
            'Buy': 50.52,
            'Sell': 50.01
          },
          {
            'Symbol': 'OMCL',
            'Buy': 64.4,
            'Sell': 63.76
          },
          {
            'Symbol': 'OMED',
            'Buy': 2.42,
            'Sell': 2.4
          },
          {
            'Symbol': 'OMER',
            'Buy': 22.28,
            'Sell': 22.06
          },
          {
            'Symbol': 'OMEX',
            'Buy': 7.44,
            'Sell': 7.37
          },
          {
            'Symbol': 'ON',
            'Buy': 21.46,
            'Sell': 21.25
          },
          {
            'Symbol': 'ONB',
            'Buy': 19.3,
            'Sell': 19.11
          },
          {
            'Symbol': 'ONCE',
            'Buy': 57.85,
            'Sell': 57.27
          },
          {
            'Symbol': 'ONCS',
            'Buy': 1.5,
            'Sell': 1.49
          },
          {
            'Symbol': 'ONCY',
            'Buy': 4.5,
            'Sell': 4.46
          },
          {
            'Symbol': 'ONEQ',
            'Buy': 307.64,
            'Sell': 304.56
          },
          {
            'Symbol': 'ONS',
            'Buy': 0.8,
            'Sell': 0.79
          },
          {
            'Symbol': 'ONTX',
            'Buy': 0.437,
            'Sell': 0.43
          },
          {
            'Symbol': 'ONVO',
            'Buy': 1.33,
            'Sell': 1.32
          },
          {
            'Symbol': 'OPB',
            'Buy': 28.3,
            'Sell': 28.02
          },
          {
            'Symbol': 'OPBK',
            'Buy': 12.64,
            'Sell': 12.51
          },
          {
            'Symbol': 'OPES',
            'Buy': 9.79,
            'Sell': 9.69
          },
          {
            'Symbol': 'OPESU',
            'Buy': 10.15,
            'Sell': 10.05
          },
          {
            'Symbol': 'OPGN',
            'Buy': 1.84,
            'Sell': 1.82
          },
          {
            'Symbol': 'OPHC',
            'Buy': 5.75,
            'Sell': 5.69
          },
          {
            'Symbol': 'OPHT',
            'Buy': 2.49,
            'Sell': 2.47
          },
          {
            'Symbol': 'OPK',
            'Buy': 5.41,
            'Sell': 5.36
          },
          {
            'Symbol': 'OPNT',
            'Buy': 16.25,
            'Sell': 16.09
          },
          {
            'Symbol': 'OPOF',
            'Buy': 29.1,
            'Sell': 28.81
          },
          {
            'Symbol': 'OPRA',
            'Buy': 13.3,
            'Sell': 13.17
          },
          {
            'Symbol': 'OPRX',
            'Buy': 12.1,
            'Sell': 11.98
          },
          {
            'Symbol': 'OPTN',
            'Buy': 21.03,
            'Sell': 20.82
          },
          {
            'Symbol': 'OPTT',
            'Buy': 0.71,
            'Sell': 0.7
          },
          {
            'Symbol': 'ORBC',
            'Buy': 10.64,
            'Sell': 10.53
          },
          {
            'Symbol': 'ORBK',
            'Buy': 63.67,
            'Sell': 63.03
          },
          {
            'Symbol': 'ORG',
            'Buy': 31.11,
            'Sell': 30.8
          },
          {
            'Symbol': 'ORGS',
            'Buy': 6.86,
            'Sell': 6.79
          },
          {
            'Symbol': 'ORIG',
            'Buy': 26.19,
            'Sell': 25.93
          },
          {
            'Symbol': 'ORIT',
            'Buy': 15.85,
            'Sell': 15.69
          },
          {
            'Symbol': 'ORLY',
            'Buy': 316.1,
            'Sell': 312.94
          },
          {
            'Symbol': 'ORMP',
            'Buy': 4.8,
            'Sell': 4.75
          },
          {
            'Symbol': 'ORPN',
            'Buy': 1.24,
            'Sell': 1.23
          },
          {
            'Symbol': 'ORRF',
            'Buy': 24.8,
            'Sell': 24.55
          },
          {
            'Symbol': 'OSBC',
            'Buy': 15.05,
            'Sell': 14.9
          },
          {
            'Symbol': 'OSBCP',
            'Buy': 10.505,
            'Sell': 10.4
          },
          {
            'Symbol': 'OSIR',
            'Buy': 9.1,
            'Sell': 9.01
          },
          {
            'Symbol': 'OSIS',
            'Buy': 80.26,
            'Sell': 79.46
          },
          {
            'Symbol': 'OSN',
            'Buy': 2.41,
            'Sell': 2.39
          },
          {
            'Symbol': 'OSPN',
            'Buy': 16.45,
            'Sell': 16.29
          },
          {
            'Symbol': 'OSPR',
            'Buy': 10.89,
            'Sell': 10.78
          },
          {
            'Symbol': 'OSPRW',
            'Buy': 1.97,
            'Sell': 1.95
          },
          {
            'Symbol': 'OSS',
            'Buy': 4.03,
            'Sell': 3.99
          },
          {
            'Symbol': 'OSTK',
            'Buy': 46.95,
            'Sell': 46.48
          },
          {
            'Symbol': 'OSUR',
            'Buy': 16.56,
            'Sell': 16.39
          },
          {
            'Symbol': 'OTEL',
            'Buy': 15.25,
            'Sell': 15.1
          },
          {
            'Symbol': 'OTEX',
            'Buy': 39.11,
            'Sell': 38.72
          },
          {
            'Symbol': 'OTIC',
            'Buy': 3.45,
            'Sell': 3.42
          },
          {
            'Symbol': 'OTIV',
            'Buy': 1.07,
            'Sell': 1.06
          },
          {
            'Symbol': 'OTTR',
            'Buy': 48.5,
            'Sell': 48.02
          },
          {
            'Symbol': 'OTTW',
            'Buy': 13.7,
            'Sell': 13.56
          },
          {
            'Symbol': 'OVAS',
            'Buy': 0.78,
            'Sell': 0.77
          },
          {
            'Symbol': 'OVBC',
            'Buy': 49.7,
            'Sell': 49.2
          },
          {
            'Symbol': 'OVID',
            'Buy': 6.25,
            'Sell': 6.19
          },
          {
            'Symbol': 'OVLY',
            'Buy': 19.9,
            'Sell': 19.7
          },
          {
            'Symbol': 'OXBR',
            'Buy': 2.1,
            'Sell': 2.08
          },
          {
            'Symbol': 'OXBRW',
            'Buy': 0.07,
            'Sell': 0.07
          },
          {
            'Symbol': 'OXFD',
            'Buy': 14.32,
            'Sell': 14.18
          },
          {
            'Symbol': 'OXLC',
            'Buy': 11.5,
            'Sell': 11.39
          },
          {
            'Symbol': 'OXLCM',
            'Buy': 25.42,
            'Sell': 25.17
          },
          {
            'Symbol': 'OXLCO',
            'Buy': 25.36,
            'Sell': 25.11
          },
          {
            'Symbol': 'OXSQ',
            'Buy': 7.21,
            'Sell': 7.14
          },
          {
            'Symbol': 'OXSQL',
            'Buy': 25.624,
            'Sell': 25.37
          },
          {
            'Symbol': 'OZK',
            'Buy': 40.33,
            'Sell': 39.93
          },
          {
            'Symbol': 'PAAS',
            'Buy': 17.37,
            'Sell': 17.2
          },
          {
            'Symbol': 'PACB',
            'Buy': 4.3,
            'Sell': 4.26
          },
          {
            'Symbol': 'PACQW',
            'Buy': 1.32,
            'Sell': 1.31
          },
          {
            'Symbol': 'PACW',
            'Buy': 50.7,
            'Sell': 50.19
          },
          {
            'Symbol': 'PAGG',
            'Buy': 27.08,
            'Sell': 26.81
          },
          {
            'Symbol': 'PAHC',
            'Buy': 49.05,
            'Sell': 48.56
          },
          {
            'Symbol': 'PANL',
            'Buy': 3.4,
            'Sell': 3.37
          },
          {
            'Symbol': 'PATI',
            'Buy': 21.9,
            'Sell': 21.68
          },
          {
            'Symbol': 'PATK',
            'Buy': 61.3,
            'Sell': 60.69
          },
          {
            'Symbol': 'PAVM',
            'Buy': 1.45,
            'Sell': 1.44
          },
          {
            'Symbol': 'PAVMZ',
            'Buy': 0.59,
            'Sell': 0.58
          },
          {
            'Symbol': 'PAYX',
            'Buy': 70.9,
            'Sell': 70.19
          },
          {
            'Symbol': 'PBCT',
            'Buy': 18.36,
            'Sell': 18.18
          },
          {
            'Symbol': 'PBCTP',
            'Buy': 25.97,
            'Sell': 25.71
          },
          {
            'Symbol': 'PBHC',
            'Buy': 15.46,
            'Sell': 15.31
          },
          {
            'Symbol': 'PBIP',
            'Buy': 18.38,
            'Sell': 18.2
          },
          {
            'Symbol': 'PBPB',
            'Buy': 12.75,
            'Sell': 12.62
          },
          {
            'Symbol': 'PBSK',
            'Buy': 26.3,
            'Sell': 26.04
          },
          {
            'Symbol': 'PBYI',
            'Buy': 54,
            'Sell': 53.46
          },
          {
            'Symbol': 'PCAR',
            'Buy': 63.88,
            'Sell': 63.24
          },
          {
            'Symbol': 'PCH',
            'Buy': 46,
            'Sell': 45.54
          },
          {
            'Symbol': 'PCMI',
            'Buy': 23.45,
            'Sell': 23.22
          },
          {
            'Symbol': 'PCOM',
            'Buy': 15.26,
            'Sell': 15.11
          },
          {
            'Symbol': 'PCRX',
            'Buy': 45.1,
            'Sell': 44.65
          },
          {
            'Symbol': 'PCSB',
            'Buy': 20.07,
            'Sell': 19.87
          },
          {
            'Symbol': 'PCTI',
            'Buy': 4.7,
            'Sell': 4.65
          },
          {
            'Symbol': 'PCTY',
            'Buy': 63.4,
            'Sell': 62.77
          },
          {
            'Symbol': 'PCYG',
            'Buy': 8.2,
            'Sell': 8.12
          },
          {
            'Symbol': 'PCYO',
            'Buy': 10.8,
            'Sell': 10.69
          },
          {
            'Symbol': 'PDBC',
            'Buy': 17.82,
            'Sell': 17.64
          },
          {
            'Symbol': 'PDCE',
            'Buy': 55.04,
            'Sell': 54.49
          },
          {
            'Symbol': 'PDCO',
            'Buy': 22.51,
            'Sell': 22.28
          },
          {
            'Symbol': 'PDD',
            'Buy': 19.85,
            'Sell': 19.65
          },
          {
            'Symbol': 'PDEX',
            'Buy': 6.7,
            'Sell': 6.63
          },
          {
            'Symbol': 'PDFS',
            'Buy': 9.82,
            'Sell': 9.72
          },
          {
            'Symbol': 'PDLB',
            'Buy': 14.86,
            'Sell': 14.71
          },
          {
            'Symbol': 'PDLI',
            'Buy': 2.43,
            'Sell': 2.41
          },
          {
            'Symbol': 'PDP',
            'Buy': 57.36,
            'Sell': 56.79
          },
          {
            'Symbol': 'PDVW',
            'Buy': 25.95,
            'Sell': 25.69
          },
          {
            'Symbol': 'PEBK',
            'Buy': 30.4,
            'Sell': 30.1
          },
          {
            'Symbol': 'PEBO',
            'Buy': 35.98,
            'Sell': 35.62
          },
          {
            'Symbol': 'PEGA',
            'Buy': 58.4,
            'Sell': 57.82
          },
          {
            'Symbol': 'PEGI',
            'Buy': 18.6,
            'Sell': 18.41
          },
          {
            'Symbol': 'PEIX',
            'Buy': 2.4,
            'Sell': 2.38
          },
          {
            'Symbol': 'PENN',
            'Buy': 30.98,
            'Sell': 30.67
          },
          {
            'Symbol': 'PEP',
            'Buy': 113.59,
            'Sell': 112.45
          },
          {
            'Symbol': 'PERI',
            'Buy': 1.07,
            'Sell': 1.06
          },
          {
            'Symbol': 'PERY',
            'Buy': 29.01,
            'Sell': 28.72
          },
          {
            'Symbol': 'PESI',
            'Buy': 4.75,
            'Sell': 4.7
          },
          {
            'Symbol': 'PETQ',
            'Buy': 27.2,
            'Sell': 26.93
          },
          {
            'Symbol': 'PETS',
            'Buy': 38.03,
            'Sell': 37.65
          },
          {
            'Symbol': 'PETX',
            'Buy': 5.04,
            'Sell': 4.99
          },
          {
            'Symbol': 'PETZ',
            'Buy': 2.65,
            'Sell': 2.62
          },
          {
            'Symbol': 'PEY',
            'Buy': 17.82,
            'Sell': 17.64
          },
          {
            'Symbol': 'PEZ',
            'Buy': 56.51,
            'Sell': 55.94
          },
          {
            'Symbol': 'PFBC',
            'Buy': 62.26,
            'Sell': 61.64
          },
          {
            'Symbol': 'PFBI',
            'Buy': 19.01,
            'Sell': 18.82
          },
          {
            'Symbol': 'PFF',
            'Buy': 37.37,
            'Sell': 37
          },
          {
            'Symbol': 'PFG',
            'Buy': 55.23,
            'Sell': 54.68
          },
          {
            'Symbol': 'PFI',
            'Buy': 35.1,
            'Sell': 34.75
          },
          {
            'Symbol': 'PFIE',
            'Buy': 3.25,
            'Sell': 3.22
          },
          {
            'Symbol': 'PFIS',
            'Buy': 45.86,
            'Sell': 45.4
          },
          {
            'Symbol': 'PFLT',
            'Buy': 13.79,
            'Sell': 13.65
          },
          {
            'Symbol': 'PFM',
            'Buy': 26.65,
            'Sell': 26.38
          },
          {
            'Symbol': 'PFMT',
            'Buy': 1.63,
            'Sell': 1.61
          },
          {
            'Symbol': 'PFPT',
            'Buy': 116.86,
            'Sell': 115.69
          },
          {
            'Symbol': 'PFSW',
            'Buy': 9.57,
            'Sell': 9.47
          },
          {
            'Symbol': 'PGC',
            'Buy': 33,
            'Sell': 32.67
          },
          {
            'Symbol': 'PGJ',
            'Buy': 41.5,
            'Sell': 41.09
          },
          {
            'Symbol': 'PGLC',
            'Buy': 1.23,
            'Sell': 1.22
          },
          {
            'Symbol': 'PGNX',
            'Buy': 8.06,
            'Sell': 7.98
          },
          {
            'Symbol': 'PHII',
            'Buy': 8.5,
            'Sell': 8.42
          },
          {
            'Symbol': 'PHIIK',
            'Buy': 8.11,
            'Sell': 8.03
          },
          {
            'Symbol': 'PHO',
            'Buy': 31.36,
            'Sell': 31.05
          },
          {
            'Symbol': 'PI',
            'Buy': 18.99,
            'Sell': 18.8
          },
          {
            'Symbol': 'PICO',
            'Buy': 12.4,
            'Sell': 12.28
          },
          {
            'Symbol': 'PID',
            'Buy': 15.86,
            'Sell': 15.7
          },
          {
            'Symbol': 'PIE',
            'Buy': 19.42,
            'Sell': 19.23
          },
          {
            'Symbol': 'PIH',
            'Buy': 7.2,
            'Sell': 7.13
          },
          {
            'Symbol': 'PINC',
            'Buy': 38.05,
            'Sell': 37.67
          },
          {
            'Symbol': 'PIO',
            'Buy': 25.44,
            'Sell': 25.19
          },
          {
            'Symbol': 'PIRS',
            'Buy': 5.37,
            'Sell': 5.32
          },
          {
            'Symbol': 'PIXY',
            'Buy': 3.02,
            'Sell': 2.99
          },
          {
            'Symbol': 'PIZ',
            'Buy': 27.58,
            'Sell': 27.3
          },
          {
            'Symbol': 'PKBK',
            'Buy': 23.5,
            'Sell': 23.27
          },
          {
            'Symbol': 'PKOH',
            'Buy': 40.15,
            'Sell': 39.75
          },
          {
            'Symbol': 'PKW',
            'Buy': 59.37,
            'Sell': 58.78
          },
          {
            'Symbol': 'PLAB',
            'Buy': 8.9,
            'Sell': 8.81
          },
          {
            'Symbol': 'PLAY',
            'Buy': 50.59,
            'Sell': 50.08
          },
          {
            'Symbol': 'PLBC',
            'Buy': 27.68,
            'Sell': 27.4
          },
          {
            'Symbol': 'PLCE',
            'Buy': 129.3,
            'Sell': 128.01
          },
          {
            'Symbol': 'PLLL',
            'Buy': 12.57,
            'Sell': 12.44
          },
          {
            'Symbol': 'PLPC',
            'Buy': 81.3,
            'Sell': 80.49
          },
          {
            'Symbol': 'PLSE',
            'Buy': 14.8,
            'Sell': 14.65
          },
          {
            'Symbol': 'PLUG',
            'Buy': 1.91,
            'Sell': 1.89
          },
          {
            'Symbol': 'PLUS',
            'Buy': 99.8,
            'Sell': 98.8
          },
          {
            'Symbol': 'PLW',
            'Buy': 31.54,
            'Sell': 31.22
          },
          {
            'Symbol': 'PLXP',
            'Buy': 3.37,
            'Sell': 3.34
          },
          {
            'Symbol': 'PLXS',
            'Buy': 61,
            'Sell': 60.39
          },
          {
            'Symbol': 'PLYA',
            'Buy': 10.33,
            'Sell': 10.23
          },
          {
            'Symbol': 'PMBC',
            'Buy': 9.9,
            'Sell': 9.8
          },
          {
            'Symbol': 'PMD',
            'Buy': 21.93,
            'Sell': 21.71
          },
          {
            'Symbol': 'PME',
            'Buy': 2.5,
            'Sell': 2.48
          },
          {
            'Symbol': 'PMTS',
            'Buy': 2.57,
            'Sell': 2.54
          },
          {
            'Symbol': 'PNFP',
            'Buy': 63.25,
            'Sell': 62.62
          },
          {
            'Symbol': 'PNK',
            'Buy': 32.74,
            'Sell': 32.41
          },
          {
            'Symbol': 'PNNT',
            'Buy': 7.54,
            'Sell': 7.46
          },
          {
            'Symbol': 'PNQI',
            'Buy': 136.72,
            'Sell': 135.35
          },
          {
            'Symbol': 'PNTR',
            'Buy': 11.15,
            'Sell': 11.04
          },
          {
            'Symbol': 'PODD',
            'Buy': 85.39,
            'Sell': 84.54
          },
          {
            'Symbol': 'POLA',
            'Buy': 6.43,
            'Sell': 6.37
          },
          {
            'Symbol': 'POOL',
            'Buy': 160.76,
            'Sell': 159.15
          },
          {
            'Symbol': 'POPE',
            'Buy': 71.5,
            'Sell': 70.79
          },
          {
            'Symbol': 'POWI',
            'Buy': 71.75,
            'Sell': 71.03
          },
          {
            'Symbol': 'POWL',
            'Buy': 40.49,
            'Sell': 40.09
          },
          {
            'Symbol': 'PPBI',
            'Buy': 36.85,
            'Sell': 36.48
          },
          {
            'Symbol': 'PPC',
            'Buy': 17.64,
            'Sell': 17.46
          },
          {
            'Symbol': 'PPH',
            'Buy': 61.97,
            'Sell': 61.35
          },
          {
            'Symbol': 'PPIH',
            'Buy': 9.165,
            'Sell': 9.07
          },
          {
            'Symbol': 'PPSI',
            'Buy': 4.75,
            'Sell': 4.7
          },
          {
            'Symbol': 'PRAA',
            'Buy': 36.8,
            'Sell': 36.43
          },
          {
            'Symbol': 'PRAH',
            'Buy': 101.07,
            'Sell': 100.06
          },
          {
            'Symbol': 'PRAN',
            'Buy': 2.1,
            'Sell': 2.08
          },
          {
            'Symbol': 'PRCP',
            'Buy': 9.73,
            'Sell': 9.63
          },
          {
            'Symbol': 'PRFT',
            'Buy': 27.13,
            'Sell': 26.86
          },
          {
            'Symbol': 'PRFZ',
            'Buy': 141.66,
            'Sell': 140.24
          },
          {
            'Symbol': 'PRGS',
            'Buy': 38.12,
            'Sell': 37.74
          },
          {
            'Symbol': 'PRGX',
            'Buy': 9.05,
            'Sell': 8.96
          },
          {
            'Symbol': 'PRIM',
            'Buy': 25.25,
            'Sell': 25
          },
          {
            'Symbol': 'PRKR',
            'Buy': 0.67,
            'Sell': 0.66
          },
          {
            'Symbol': 'PRMW',
            'Buy': 19.32,
            'Sell': 19.13
          },
          {
            'Symbol': 'PRN',
            'Buy': 62.65,
            'Sell': 62.02
          },
          {
            'Symbol': 'PROV',
            'Buy': 18.4,
            'Sell': 18.22
          },
          {
            'Symbol': 'PRPH',
            'Buy': 3.02,
            'Sell': 2.99
          },
          {
            'Symbol': 'PRPL',
            'Buy': 6.3,
            'Sell': 6.24
          },
          {
            'Symbol': 'PRPLW',
            'Buy': 0.32,
            'Sell': 0.32
          },
          {
            'Symbol': 'PRPO',
            'Buy': 0.45,
            'Sell': 0.45
          },
          {
            'Symbol': 'PRQR',
            'Buy': 7.5,
            'Sell': 7.43
          },
          {
            'Symbol': 'PRSC',
            'Buy': 63.82,
            'Sell': 63.18
          },
          {
            'Symbol': 'PRSS',
            'Buy': 1.4,
            'Sell': 1.39
          },
          {
            'Symbol': 'PRTA',
            'Buy': 14.25,
            'Sell': 14.11
          },
          {
            'Symbol': 'PRTH',
            'Buy': 10.95,
            'Sell': 10.84
          },
          {
            'Symbol': 'PRTHW',
            'Buy': 1.85,
            'Sell': 1.83
          },
          {
            'Symbol': 'PRTK',
            'Buy': 10.45,
            'Sell': 10.35
          },
          {
            'Symbol': 'PRTO',
            'Buy': 2.3,
            'Sell': 2.28
          },
          {
            'Symbol': 'PRTS',
            'Buy': 1.15,
            'Sell': 1.14
          },
          {
            'Symbol': 'PRVB',
            'Buy': 3.44,
            'Sell': 3.41
          },
          {
            'Symbol': 'PS',
            'Buy': 27.06,
            'Sell': 26.79
          },
          {
            'Symbol': 'PSAU',
            'Buy': 16.98,
            'Sell': 16.81
          },
          {
            'Symbol': 'PSC',
            'Buy': 34.68,
            'Sell': 34.33
          },
          {
            'Symbol': 'PSCC',
            'Buy': 83.23,
            'Sell': 82.4
          },
          {
            'Symbol': 'PSCD',
            'Buy': 68.765,
            'Sell': 68.08
          },
          {
            'Symbol': 'PSCE',
            'Buy': 16.42,
            'Sell': 16.26
          },
          {
            'Symbol': 'PSCF',
            'Buy': 58.58,
            'Sell': 57.99
          },
          {
            'Symbol': 'PSCH',
            'Buy': 134.48,
            'Sell': 133.14
          },
          {
            'Symbol': 'PSCI',
            'Buy': 72.97,
            'Sell': 72.24
          },
          {
            'Symbol': 'PSCM',
            'Buy': 54.43,
            'Sell': 53.89
          },
          {
            'Symbol': 'PSCT',
            'Buy': 85.38,
            'Sell': 84.53
          },
          {
            'Symbol': 'PSCU',
            'Buy': 54.8,
            'Sell': 54.25
          },
          {
            'Symbol': 'PSDO',
            'Buy': 14.41,
            'Sell': 14.27
          },
          {
            'Symbol': 'PSEC',
            'Buy': 6.86,
            'Sell': 6.79
          },
          {
            'Symbol': 'PSL',
            'Buy': 70.98,
            'Sell': 70.27
          },
          {
            'Symbol': 'PSMT',
            'Buy': 82.7,
            'Sell': 81.87
          },
          {
            'Symbol': 'PSTI',
            'Buy': 1.25,
            'Sell': 1.24
          },
          {
            'Symbol': 'PTC',
            'Buy': 93.43,
            'Sell': 92.5
          },
          {
            'Symbol': 'PTCT',
            'Buy': 42.57,
            'Sell': 42.14
          },
          {
            'Symbol': 'PTEN',
            'Buy': 16.71,
            'Sell': 16.54
          },
          {
            'Symbol': 'PTF',
            'Buy': 62.85,
            'Sell': 62.22
          },
          {
            'Symbol': 'PTGX',
            'Buy': 11.43,
            'Sell': 11.32
          },
          {
            'Symbol': 'PTH',
            'Buy': 89.44,
            'Sell': 88.55
          },
          {
            'Symbol': 'PTI',
            'Buy': 2.43,
            'Sell': 2.41
          },
          {
            'Symbol': 'PTIE',
            'Buy': 1.21,
            'Sell': 1.2
          },
          {
            'Symbol': 'PTLA',
            'Buy': 29.85,
            'Sell': 29.55
          },
          {
            'Symbol': 'PTNR',
            'Buy': 3.9,
            'Sell': 3.86
          },
          {
            'Symbol': 'PTSI',
            'Buy': 62.01,
            'Sell': 61.39
          },
          {
            'Symbol': 'PTVCA',
            'Buy': 23.15,
            'Sell': 22.92
          },
          {
            'Symbol': 'PTVCB',
            'Buy': 23.3,
            'Sell': 23.07
          },
          {
            'Symbol': 'PTX',
            'Buy': 1.41,
            'Sell': 1.4
          },
          {
            'Symbol': 'PUB',
            'Buy': 35.55,
            'Sell': 35.19
          },
          {
            'Symbol': 'PUI',
            'Buy': 28.47,
            'Sell': 28.19
          },
          {
            'Symbol': 'PULM',
            'Buy': 0.31,
            'Sell': 0.31
          },
          {
            'Symbol': 'PVAC',
            'Buy': 77.83,
            'Sell': 77.05
          },
          {
            'Symbol': 'PVBC',
            'Buy': 28.3,
            'Sell': 28.02
          },
          {
            'Symbol': 'PWOD',
            'Buy': 45.86,
            'Sell': 45.4
          },
          {
            'Symbol': 'PXI',
            'Buy': 41.94,
            'Sell': 41.52
          },
          {
            'Symbol': 'PXLW',
            'Buy': 4.25,
            'Sell': 4.21
          },
          {
            'Symbol': 'PXS',
            'Buy': 0.94,
            'Sell': 0.93
          },
          {
            'Symbol': 'PYDS',
            'Buy': 1.95,
            'Sell': 1.93
          },
          {
            'Symbol': 'PYPL',
            'Buy': 86.55,
            'Sell': 85.68
          },
          {
            'Symbol': 'PYZ',
            'Buy': 69.177,
            'Sell': 68.49
          },
          {
            'Symbol': 'PZZA',
            'Buy': 41.03,
            'Sell': 40.62
          },
          {
            'Symbol': 'QABA',
            'Buy': 55.4,
            'Sell': 54.85
          },
          {
            'Symbol': 'QADA',
            'Buy': 51.7,
            'Sell': 51.18
          },
          {
            'Symbol': 'QADB',
            'Buy': 41,
            'Sell': 40.59
          },
          {
            'Symbol': 'QAT',
            'Buy': 17.98,
            'Sell': 17.8
          },
          {
            'Symbol': 'QBAK',
            'Buy': 8.39,
            'Sell': 8.31
          },
          {
            'Symbol': 'QCLN',
            'Buy': 20.39,
            'Sell': 20.19
          },
          {
            'Symbol': 'QCOM',
            'Buy': 64.65,
            'Sell': 64
          },
          {
            'Symbol': 'QCRH',
            'Buy': 43.2,
            'Sell': 42.77
          },
          {
            'Symbol': 'QDEL',
            'Buy': 68.48,
            'Sell': 67.8
          },
          {
            'Symbol': 'QINC',
            'Buy': 25.22,
            'Sell': 24.97
          },
          {
            'Symbol': 'QIWI',
            'Buy': 15.1,
            'Sell': 14.95
          },
          {
            'Symbol': 'QLC',
            'Buy': 35.06,
            'Sell': 34.71
          },
          {
            'Symbol': 'QLYS',
            'Buy': 88,
            'Sell': 87.12
          },
          {
            'Symbol': 'QNST',
            'Buy': 13.24,
            'Sell': 13.11
          },
          {
            'Symbol': 'QQEW',
            'Buy': 62.55,
            'Sell': 61.92
          },
          {
            'Symbol': 'QQQ',
            'Buy': 180.68,
            'Sell': 178.87
          },
          {
            'Symbol': 'QQQC',
            'Buy': 27.51,
            'Sell': 27.23
          },
          {
            'Symbol': 'QQQX',
            'Buy': 24.76,
            'Sell': 24.51
          },
          {
            'Symbol': 'QQXT',
            'Buy': 51.89,
            'Sell': 51.37
          },
          {
            'Symbol': 'QRHC',
            'Buy': 1.8,
            'Sell': 1.78
          },
          {
            'Symbol': 'QRTEA',
            'Buy': 23.09,
            'Sell': 22.86
          },
          {
            'Symbol': 'QRVO',
            'Buy': 84.69,
            'Sell': 83.84
          },
          {
            'Symbol': 'QSII',
            'Buy': 21.54,
            'Sell': 21.32
          },
          {
            'Symbol': 'QTEC',
            'Buy': 79.15,
            'Sell': 78.36
          },
          {
            'Symbol': 'QTNA',
            'Buy': 15.99,
            'Sell': 15.83
          },
          {
            'Symbol': 'QTNT',
            'Buy': 7.65,
            'Sell': 7.57
          },
          {
            'Symbol': 'QTRH',
            'Buy': 1.31,
            'Sell': 1.3
          },
          {
            'Symbol': 'QTRX',
            'Buy': 14.39,
            'Sell': 14.25
          },
          {
            'Symbol': 'QUIK',
            'Buy': 1.14,
            'Sell': 1.13
          },
          {
            'Symbol': 'QUMU',
            'Buy': 2.65,
            'Sell': 2.62
          },
          {
            'Symbol': 'QURE',
            'Buy': 34.34,
            'Sell': 34
          },
          {
            'Symbol': 'QYLD',
            'Buy': 25,
            'Sell': 24.75
          },
          {
            'Symbol': 'RADA',
            'Buy': 2.64,
            'Sell': 2.61
          },
          {
            'Symbol': 'RAIL',
            'Buy': 18.75,
            'Sell': 18.56
          },
          {
            'Symbol': 'RAND',
            'Buy': 2.25,
            'Sell': 2.23
          },
          {
            'Symbol': 'RARE',
            'Buy': 75.78,
            'Sell': 75.02
          },
          {
            'Symbol': 'RARX',
            'Buy': 10.92,
            'Sell': 10.81
          },
          {
            'Symbol': 'RAVE',
            'Buy': 1.32,
            'Sell': 1.31
          },
          {
            'Symbol': 'RAVN',
            'Buy': 38.5,
            'Sell': 38.12
          },
          {
            'Symbol': 'RBB',
            'Buy': 30.14,
            'Sell': 29.84
          },
          {
            'Symbol': 'RBBN',
            'Buy': 6.97,
            'Sell': 6.9
          },
          {
            'Symbol': 'RBCAA',
            'Buy': 48.93,
            'Sell': 48.44
          },
          {
            'Symbol': 'RBCN',
            'Buy': 8.25,
            'Sell': 8.17
          },
          {
            'Symbol': 'RBNC',
            'Buy': 27.44,
            'Sell': 27.17
          },
          {
            'Symbol': 'RCII',
            'Buy': 14.71,
            'Sell': 14.56
          },
          {
            'Symbol': 'RCKT',
            'Buy': 22.15,
            'Sell': 21.93
          },
          {
            'Symbol': 'RCKY',
            'Buy': 30.25,
            'Sell': 29.95
          },
          {
            'Symbol': 'RCM',
            'Buy': 9.28,
            'Sell': 9.19
          },
          {
            'Symbol': 'RCMT',
            'Buy': 4.83,
            'Sell': 4.78
          },
          {
            'Symbol': 'RCON',
            'Buy': 1.38,
            'Sell': 1.37
          },
          {
            'Symbol': 'RDCM',
            'Buy': 19.55,
            'Sell': 19.35
          },
          {
            'Symbol': 'RDFN',
            'Buy': 19.24,
            'Sell': 19.05
          },
          {
            'Symbol': 'RDHL',
            'Buy': 6.55,
            'Sell': 6.48
          },
          {
            'Symbol': 'RDI',
            'Buy': 16.02,
            'Sell': 15.86
          },
          {
            'Symbol': 'RDIB',
            'Buy': 27.295,
            'Sell': 27.02
          },
          {
            'Symbol': 'RDNT',
            'Buy': 13.8,
            'Sell': 13.66
          },
          {
            'Symbol': 'RDUS',
            'Buy': 22.16,
            'Sell': 21.94
          },
          {
            'Symbol': 'RDVT',
            'Buy': 7.68,
            'Sell': 7.6
          },
          {
            'Symbol': 'RDVY',
            'Buy': 30.77,
            'Sell': 30.46
          },
          {
            'Symbol': 'RDWR',
            'Buy': 27.39,
            'Sell': 27.12
          },
          {
            'Symbol': 'RECN',
            'Buy': 16.05,
            'Sell': 15.89
          },
          {
            'Symbol': 'REDU',
            'Buy': 12.84,
            'Sell': 12.71
          },
          {
            'Symbol': 'REFR',
            'Buy': 0.88,
            'Sell': 0.87
          },
          {
            'Symbol': 'REGI',
            'Buy': 20.05,
            'Sell': 19.85
          },
          {
            'Symbol': 'REGN',
            'Buy': 371.22,
            'Sell': 367.51
          },
          {
            'Symbol': 'REIS',
            'Buy': 19.625,
            'Sell': 19.43
          },
          {
            'Symbol': 'RELL',
            'Buy': 9.47,
            'Sell': 9.38
          },
          {
            'Symbol': 'RELV',
            'Buy': 5,
            'Sell': 4.95
          },
          {
            'Symbol': 'REPH',
            'Buy': 5.97,
            'Sell': 5.91
          },
          {
            'Symbol': 'REPL',
            'Buy': 16.24,
            'Sell': 16.08
          },
          {
            'Symbol': 'RESN',
            'Buy': 4.6,
            'Sell': 4.55
          },
          {
            'Symbol': 'RETA',
            'Buy': 65.46,
            'Sell': 64.81
          },
          {
            'Symbol': 'RETO',
            'Buy': 3.84,
            'Sell': 3.8
          },
          {
            'Symbol': 'RFAP',
            'Buy': 56.38,
            'Sell': 55.82
          },
          {
            'Symbol': 'RFDI',
            'Buy': 60.99,
            'Sell': 60.38
          },
          {
            'Symbol': 'RFEM',
            'Buy': 64.34,
            'Sell': 63.7
          },
          {
            'Symbol': 'RFEU',
            'Buy': 62.76,
            'Sell': 62.13
          },
          {
            'Symbol': 'RFIL',
            'Buy': 10.5,
            'Sell': 10.4
          },
          {
            'Symbol': 'RGCO',
            'Buy': 28.45,
            'Sell': 28.17
          },
          {
            'Symbol': 'RGEN',
            'Buy': 49.18,
            'Sell': 48.69
          },
          {
            'Symbol': 'RGLD',
            'Buy': 82.51,
            'Sell': 81.68
          },
          {
            'Symbol': 'RGLS',
            'Buy': 0.244,
            'Sell': 0.24
          },
          {
            'Symbol': 'RGNX',
            'Buy': 66.5,
            'Sell': 65.84
          },
          {
            'Symbol': 'RGSE',
            'Buy': 0.34,
            'Sell': 0.34
          },
          {
            'Symbol': 'RIBT',
            'Buy': 2.69,
            'Sell': 2.66
          },
          {
            'Symbol': 'RIBTW',
            'Buy': 0.18,
            'Sell': 0.18
          },
          {
            'Symbol': 'RICK',
            'Buy': 33.79,
            'Sell': 33.45
          },
          {
            'Symbol': 'RIGL',
            'Buy': 3.02,
            'Sell': 2.99
          },
          {
            'Symbol': 'RILY',
            'Buy': 22.8,
            'Sell': 22.57
          },
          {
            'Symbol': 'RILYG',
            'Buy': 24.82,
            'Sell': 24.57
          },
          {
            'Symbol': 'RILYH',
            'Buy': 25.38,
            'Sell': 25.13
          },
          {
            'Symbol': 'RILYL',
            'Buy': 25.27,
            'Sell': 25.02
          },
          {
            'Symbol': 'RILYZ',
            'Buy': 24.99,
            'Sell': 24.74
          },
          {
            'Symbol': 'RING',
            'Buy': 16.08,
            'Sell': 15.92
          },
          {
            'Symbol': 'RIOT',
            'Buy': 6.87,
            'Sell': 6.8
          },
          {
            'Symbol': 'RKDA',
            'Buy': 6.29,
            'Sell': 6.23
          },
          {
            'Symbol': 'RLJE',
            'Buy': 6.15,
            'Sell': 6.09
          },
          {
            'Symbol': 'RMBL',
            'Buy': 5.86,
            'Sell': 5.8
          },
          {
            'Symbol': 'RMBS',
            'Buy': 12.62,
            'Sell': 12.49
          },
          {
            'Symbol': 'RMCF',
            'Buy': 10.65,
            'Sell': 10.54
          },
          {
            'Symbol': 'RMGN',
            'Buy': 1.11,
            'Sell': 1.1
          },
          {
            'Symbol': 'RMNI',
            'Buy': 7.75,
            'Sell': 7.67
          },
          {
            'Symbol': 'RMR',
            'Buy': 87,
            'Sell': 86.13
          },
          {
            'Symbol': 'RMTI',
            'Buy': 4.94,
            'Sell': 4.89
          },
          {
            'Symbol': 'RNDB',
            'Buy': 17,
            'Sell': 16.83
          },
          {
            'Symbol': 'RNDM',
            'Buy': 50.09,
            'Sell': 49.59
          },
          {
            'Symbol': 'RNEM',
            'Buy': 50.8,
            'Sell': 50.29
          },
          {
            'Symbol': 'RNET',
            'Buy': 13.3,
            'Sell': 13.17
          },
          {
            'Symbol': 'RNLC',
            'Buy': 22.49,
            'Sell': 22.27
          },
          {
            'Symbol': 'RNMC',
            'Buy': 22.5,
            'Sell': 22.28
          },
          {
            'Symbol': 'RNSC',
            'Buy': 22.71,
            'Sell': 22.48
          },
          {
            'Symbol': 'RNST',
            'Buy': 45.18,
            'Sell': 44.73
          },
          {
            'Symbol': 'RNWK',
            'Buy': 3.49,
            'Sell': 3.46
          },
          {
            'Symbol': 'ROAD',
            'Buy': 12.2,
            'Sell': 12.08
          },
          {
            'Symbol': 'ROBT',
            'Buy': 30.83,
            'Sell': 30.52
          },
          {
            'Symbol': 'ROCK',
            'Buy': 43.35,
            'Sell': 42.92
          },
          {
            'Symbol': 'ROIC',
            'Buy': 18.91,
            'Sell': 18.72
          },
          {
            'Symbol': 'ROKU',
            'Buy': 56.32,
            'Sell': 55.76
          },
          {
            'Symbol': 'ROLL',
            'Buy': 139,
            'Sell': 137.61
          },
          {
            'Symbol': 'ROSE',
            'Buy': 8.51,
            'Sell': 8.42
          },
          {
            'Symbol': 'ROSEW',
            'Buy': 1.31,
            'Sell': 1.3
          },
          {
            'Symbol': 'ROST',
            'Buy': 91.32,
            'Sell': 90.41
          },
          {
            'Symbol': 'RP',
            'Buy': 58.95,
            'Sell': 58.36
          },
          {
            'Symbol': 'RPD',
            'Buy': 32.6,
            'Sell': 32.27
          },
          {
            'Symbol': 'RRGB',
            'Buy': 38.5,
            'Sell': 38.12
          },
          {
            'Symbol': 'RRR',
            'Buy': 31.83,
            'Sell': 31.51
          },
          {
            'Symbol': 'RSLS',
            'Buy': 0.072,
            'Sell': 0.07
          },
          {
            'Symbol': 'RSYS',
            'Buy': 1.38,
            'Sell': 1.37
          },
          {
            'Symbol': 'RTIX',
            'Buy': 4.5,
            'Sell': 4.46
          },
          {
            'Symbol': 'RTRX',
            'Buy': 26.43,
            'Sell': 26.17
          },
          {
            'Symbol': 'RTTR',
            'Buy': 2.24,
            'Sell': 2.22
          },
          {
            'Symbol': 'RUBY',
            'Buy': 21.99,
            'Sell': 21.77
          },
          {
            'Symbol': 'RUN',
            'Buy': 12.69,
            'Sell': 12.56
          },
          {
            'Symbol': 'RUSHA',
            'Buy': 42.46,
            'Sell': 42.04
          },
          {
            'Symbol': 'RUSHB',
            'Buy': 42.71,
            'Sell': 42.28
          },
          {
            'Symbol': 'RUTH',
            'Buy': 29.7,
            'Sell': 29.4
          },
          {
            'Symbol': 'RVEN',
            'Buy': 3.98,
            'Sell': 3.94
          },
          {
            'Symbol': 'RVLT',
            'Buy': 2.66,
            'Sell': 2.63
          },
          {
            'Symbol': 'RVNC',
            'Buy': 26.35,
            'Sell': 26.09
          },
          {
            'Symbol': 'RVSB',
            'Buy': 9.17,
            'Sell': 9.08
          },
          {
            'Symbol': 'RWLK',
            'Buy': 0.9,
            'Sell': 0.89
          },
          {
            'Symbol': 'RXII',
            'Buy': 1.56,
            'Sell': 1.54
          },
          {
            'Symbol': 'RYAAY',
            'Buy': 95.96,
            'Sell': 95
          },
          {
            'Symbol': 'RYTM',
            'Buy': 31.97,
            'Sell': 31.65
          },
          {
            'Symbol': 'SABR',
            'Buy': 24.94,
            'Sell': 24.69
          },
          {
            'Symbol': 'SAEX',
            'Buy': 0.62,
            'Sell': 0.61
          },
          {
            'Symbol': 'SAFM',
            'Buy': 101.77,
            'Sell': 100.75
          },
          {
            'Symbol': 'SAFT',
            'Buy': 93.5,
            'Sell': 92.57
          },
          {
            'Symbol': 'SAGE',
            'Buy': 147.1,
            'Sell': 145.63
          },
          {
            'Symbol': 'SAIA',
            'Buy': 76.25,
            'Sell': 75.49
          },
          {
            'Symbol': 'SAL',
            'Buy': 42.65,
            'Sell': 42.22
          },
          {
            'Symbol': 'SALM',
            'Buy': 4.1,
            'Sell': 4.06
          },
          {
            'Symbol': 'SAMG',
            'Buy': 16.5,
            'Sell': 16.34
          },
          {
            'Symbol': 'SANM',
            'Buy': 30.25,
            'Sell': 29.95
          },
          {
            'Symbol': 'SANW',
            'Buy': 3.34,
            'Sell': 3.31
          },
          {
            'Symbol': 'SASR',
            'Buy': 39.05,
            'Sell': 38.66
          },
          {
            'Symbol': 'SATS',
            'Buy': 48.72,
            'Sell': 48.23
          },
          {
            'Symbol': 'SAUC',
            'Buy': 0.99,
            'Sell': 0.98
          },
          {
            'Symbol': 'SBAC',
            'Buy': 156.68,
            'Sell': 155.11
          },
          {
            'Symbol': 'SBBP',
            'Buy': 5.9,
            'Sell': 5.84
          },
          {
            'Symbol': 'SBBX',
            'Buy': 28.05,
            'Sell': 27.77
          },
          {
            'Symbol': 'SBCF',
            'Buy': 29.7,
            'Sell': 29.4
          },
          {
            'Symbol': 'SBFG',
            'Buy': 19.91,
            'Sell': 19.71
          },
          {
            'Symbol': 'SBGI',
            'Buy': 27.6,
            'Sell': 27.32
          },
          {
            'Symbol': 'SBLK',
            'Buy': 13.23,
            'Sell': 13.1
          },
          {
            'Symbol': 'SBLKZ',
            'Buy': 25.4,
            'Sell': 25.15
          },
          {
            'Symbol': 'SBNY',
            'Buy': 110.79,
            'Sell': 109.68
          },
          {
            'Symbol': 'SBOT',
            'Buy': 1.46,
            'Sell': 1.45
          },
          {
            'Symbol': 'SBPH',
            'Buy': 12.32,
            'Sell': 12.2
          },
          {
            'Symbol': 'SBRA',
            'Buy': 22.26,
            'Sell': 22.04
          },
          {
            'Symbol': 'SBSI',
            'Buy': 34.88,
            'Sell': 34.53
          },
          {
            'Symbol': 'SBT',
            'Buy': 11.67,
            'Sell': 11.55
          },
          {
            'Symbol': 'SBUX',
            'Buy': 51.59,
            'Sell': 51.07
          },
          {
            'Symbol': 'SCAC',
            'Buy': 10.15,
            'Sell': 10.05
          },
          {
            'Symbol': 'SCACW',
            'Buy': 1.23,
            'Sell': 1.22
          },
          {
            'Symbol': 'SCHL',
            'Buy': 41.12,
            'Sell': 40.71
          },
          {
            'Symbol': 'SCHN',
            'Buy': 30,
            'Sell': 29.7
          },
          {
            'Symbol': 'SCKT',
            'Buy': 2.35,
            'Sell': 2.33
          },
          {
            'Symbol': 'SCON',
            'Buy': 2.7,
            'Sell': 2.67
          },
          {
            'Symbol': 'SCOR',
            'Buy': 18.83,
            'Sell': 18.64
          },
          {
            'Symbol': 'SCPH',
            'Buy': 4.52,
            'Sell': 4.47
          },
          {
            'Symbol': 'SCSC',
            'Buy': 40.85,
            'Sell': 40.44
          },
          {
            'Symbol': 'SCVL',
            'Buy': 31.87,
            'Sell': 31.55
          },
          {
            'Symbol': 'SCWX',
            'Buy': 13.16,
            'Sell': 13.03
          },
          {
            'Symbol': 'SCYX',
            'Buy': 1.42,
            'Sell': 1.41
          },
          {
            'Symbol': 'SCZ',
            'Buy': 61.73,
            'Sell': 61.11
          },
          {
            'Symbol': 'SDVY',
            'Buy': 21.46,
            'Sell': 21.25
          },
          {
            'Symbol': 'SEAC',
            'Buy': 3.04,
            'Sell': 3.01
          },
          {
            'Symbol': 'SECO',
            'Buy': 14.55,
            'Sell': 14.4
          },
          {
            'Symbol': 'SEDG',
            'Buy': 46.35,
            'Sell': 45.89
          },
          {
            'Symbol': 'SEED',
            'Buy': 6.66,
            'Sell': 6.59
          },
          {
            'Symbol': 'SEIC',
            'Buy': 59.13,
            'Sell': 58.54
          },
          {
            'Symbol': 'SEII',
            'Buy': 3.32,
            'Sell': 3.29
          },
          {
            'Symbol': 'SELB',
            'Buy': 13.53,
            'Sell': 13.39
          },
          {
            'Symbol': 'SELF',
            'Buy': 4.08,
            'Sell': 4.04
          },
          {
            'Symbol': 'SENEA',
            'Buy': 28.6,
            'Sell': 28.31
          },
          {
            'Symbol': 'SES',
            'Buy': 2.72,
            'Sell': 2.69
          },
          {
            'Symbol': 'SESN',
            'Buy': 1.7,
            'Sell': 1.68
          },
          {
            'Symbol': 'SFBC',
            'Buy': 40.785,
            'Sell': 40.38
          },
          {
            'Symbol': 'SFBS',
            'Buy': 42.15,
            'Sell': 41.73
          },
          {
            'Symbol': 'SFIX',
            'Buy': 32.29,
            'Sell': 31.97
          },
          {
            'Symbol': 'SFLY',
            'Buy': 77.05,
            'Sell': 76.28
          },
          {
            'Symbol': 'SFM',
            'Buy': 22.94,
            'Sell': 22.71
          },
          {
            'Symbol': 'SFNC',
            'Buy': 30.1,
            'Sell': 29.8
          },
          {
            'Symbol': 'SFST',
            'Buy': 43.95,
            'Sell': 43.51
          },
          {
            'Symbol': 'SGBX',
            'Buy': 4.86,
            'Sell': 4.81
          },
          {
            'Symbol': 'SGC',
            'Buy': 19.85,
            'Sell': 19.65
          },
          {
            'Symbol': 'SGEN',
            'Buy': 71.54,
            'Sell': 70.82
          },
          {
            'Symbol': 'SGH',
            'Buy': 31.85,
            'Sell': 31.53
          },
          {
            'Symbol': 'SGLB',
            'Buy': 0.87,
            'Sell': 0.86
          },
          {
            'Symbol': 'SGMA',
            'Buy': 7,
            'Sell': 6.93
          },
          {
            'Symbol': 'SGMO',
            'Buy': 15.35,
            'Sell': 15.2
          },
          {
            'Symbol': 'SGMS',
            'Buy': 34.25,
            'Sell': 33.91
          },
          {
            'Symbol': 'SGOC',
            'Buy': 1,
            'Sell': 0.99
          },
          {
            'Symbol': 'SGRP',
            'Buy': 1.25,
            'Sell': 1.24
          },
          {
            'Symbol': 'SGRY',
            'Buy': 16.95,
            'Sell': 16.78
          },
          {
            'Symbol': 'SGYP',
            'Buy': 1.8,
            'Sell': 1.78
          },
          {
            'Symbol': 'SHBI',
            'Buy': 18.86,
            'Sell': 18.67
          },
          {
            'Symbol': 'SHEN',
            'Buy': 36.75,
            'Sell': 36.38
          },
          {
            'Symbol': 'SHIP',
            'Buy': 1,
            'Sell': 0.99
          },
          {
            'Symbol': 'SHIPW',
            'Buy': 0.141,
            'Sell': 0.14
          },
          {
            'Symbol': 'SHLD',
            'Buy': 1.9,
            'Sell': 1.88
          },
          {
            'Symbol': 'SHLDW',
            'Buy': 0.14,
            'Sell': 0.14
          },
          {
            'Symbol': 'SHLM',
            'Buy': 43.8,
            'Sell': 43.36
          },
          {
            'Symbol': 'SHLO',
            'Buy': 8.7,
            'Sell': 8.61
          },
          {
            'Symbol': 'SHOO',
            'Buy': 56.15,
            'Sell': 55.59
          },
          {
            'Symbol': 'SHOS',
            'Buy': 2.5,
            'Sell': 2.48
          },
          {
            'Symbol': 'SHPG',
            'Buy': 170.75,
            'Sell': 169.04
          },
          {
            'Symbol': 'SHSP',
            'Buy': 12.23,
            'Sell': 12.11
          },
          {
            'Symbol': 'SHV',
            'Buy': 110.35,
            'Sell': 109.25
          },
          {
            'Symbol': 'SHY',
            'Buy': 83.22,
            'Sell': 82.39
          },
          {
            'Symbol': 'SIEB',
            'Buy': 12.78,
            'Sell': 12.65
          },
          {
            'Symbol': 'SIEN',
            'Buy': 20.84,
            'Sell': 20.63
          },
          {
            'Symbol': 'SIFI',
            'Buy': 13.75,
            'Sell': 13.61
          },
          {
            'Symbol': 'SIFY',
            'Buy': 1.74,
            'Sell': 1.72
          },
          {
            'Symbol': 'SIGA',
            'Buy': 7.85,
            'Sell': 7.77
          },
          {
            'Symbol': 'SIGI',
            'Buy': 61.25,
            'Sell': 60.64
          },
          {
            'Symbol': 'SIGM',
            'Buy': 6.1,
            'Sell': 6.04
          },
          {
            'Symbol': 'SILC',
            'Buy': 37.92,
            'Sell': 37.54
          },
          {
            'Symbol': 'SIMO',
            'Buy': 56.9,
            'Sell': 56.33
          },
          {
            'Symbol': 'SINA',
            'Buy': 74.5,
            'Sell': 73.76
          },
          {
            'Symbol': 'SINO',
            'Buy': 1.17,
            'Sell': 1.16
          },
          {
            'Symbol': 'SIR',
            'Buy': 20.52,
            'Sell': 20.31
          },
          {
            'Symbol': 'SIRI',
            'Buy': 6.96,
            'Sell': 6.89
          },
          {
            'Symbol': 'SITO',
            'Buy': 2.15,
            'Sell': 2.13
          },
          {
            'Symbol': 'SIVB',
            'Buy': 315.59,
            'Sell': 312.43
          },
          {
            'Symbol': 'SKIS',
            'Buy': 5,
            'Sell': 4.95
          },
          {
            'Symbol': 'SKOR',
            'Buy': 49.1,
            'Sell': 48.61
          },
          {
            'Symbol': 'SKYS',
            'Buy': 0.95,
            'Sell': 0.94
          },
          {
            'Symbol': 'SKYW',
            'Buy': 59.95,
            'Sell': 59.35
          },
          {
            'Symbol': 'SKYY',
            'Buy': 54.87,
            'Sell': 54.32
          },
          {
            'Symbol': 'SLAB',
            'Buy': 97.1,
            'Sell': 96.13
          },
          {
            'Symbol': 'SLCT',
            'Buy': 12.7,
            'Sell': 12.57
          },
          {
            'Symbol': 'SLDB',
            'Buy': 38.85,
            'Sell': 38.46
          },
          {
            'Symbol': 'SLGL',
            'Buy': 6.6,
            'Sell': 6.53
          },
          {
            'Symbol': 'SLGN',
            'Buy': 27.47,
            'Sell': 27.2
          },
          {
            'Symbol': 'SLIM',
            'Buy': 36.51,
            'Sell': 36.14
          },
          {
            'Symbol': 'SLM',
            'Buy': 11.37,
            'Sell': 11.26
          },
          {
            'Symbol': 'SLMBP',
            'Buy': 71,
            'Sell': 70.29
          },
          {
            'Symbol': 'SLNO',
            'Buy': 2.58,
            'Sell': 2.55
          },
          {
            'Symbol': 'SLP',
            'Buy': 18.2,
            'Sell': 18.02
          },
          {
            'Symbol': 'SLQD',
            'Buy': 49.66,
            'Sell': 49.16
          },
          {
            'Symbol': 'SLRC',
            'Buy': 21.68,
            'Sell': 21.46
          },
          {
            'Symbol': 'SLS',
            'Buy': 1.06,
            'Sell': 1.05
          },
          {
            'Symbol': 'SLVO',
            'Buy': 7.075,
            'Sell': 7
          },
          {
            'Symbol': 'SMBC',
            'Buy': 39.17,
            'Sell': 38.78
          },
          {
            'Symbol': 'SMBK',
            'Buy': 24.76,
            'Sell': 24.51
          },
          {
            'Symbol': 'SMCI',
            'Buy': 20,
            'Sell': 19.8
          },
          {
            'Symbol': 'SMCP',
            'Buy': 27,
            'Sell': 26.73
          },
          {
            'Symbol': 'SMED',
            'Buy': 3.5,
            'Sell': 3.47
          },
          {
            'Symbol': 'SMIT',
            'Buy': 2.58,
            'Sell': 2.55
          },
          {
            'Symbol': 'SMMF',
            'Buy': 25.13,
            'Sell': 24.88
          },
          {
            'Symbol': 'SMMT',
            'Buy': 2.25,
            'Sell': 2.23
          },
          {
            'Symbol': 'SMPL',
            'Buy': 17.26,
            'Sell': 17.09
          },
          {
            'Symbol': 'SMPLW',
            'Buy': 6.19,
            'Sell': 6.13
          },
          {
            'Symbol': 'SMRT',
            'Buy': 2.44,
            'Sell': 2.42
          },
          {
            'Symbol': 'SMSI',
            'Buy': 2.53,
            'Sell': 2.5
          },
          {
            'Symbol': 'SMTC',
            'Buy': 50.65,
            'Sell': 50.14
          },
          {
            'Symbol': 'SMTX',
            'Buy': 2.58,
            'Sell': 2.55
          },
          {
            'Symbol': 'SNBR',
            'Buy': 30.61,
            'Sell': 30.3
          },
          {
            'Symbol': 'SND',
            'Buy': 5.77,
            'Sell': 5.71
          },
          {
            'Symbol': 'SNDE',
            'Buy': 5.5,
            'Sell': 5.45
          },
          {
            'Symbol': 'SNDX',
            'Buy': 7.33,
            'Sell': 7.26
          },
          {
            'Symbol': 'SNES',
            'Buy': 0.98,
            'Sell': 0.97
          },
          {
            'Symbol': 'SNFCA',
            'Buy': 5.05,
            'Sell': 5
          },
          {
            'Symbol': 'SNGX',
            'Buy': 1.38,
            'Sell': 1.37
          },
          {
            'Symbol': 'SNGXW',
            'Buy': 0.4,
            'Sell': 0.4
          },
          {
            'Symbol': 'SNH',
            'Buy': 18.36,
            'Sell': 18.18
          },
          {
            'Symbol': 'SNHNI',
            'Buy': 24.51,
            'Sell': 24.26
          },
          {
            'Symbol': 'SNHNL',
            'Buy': 26.2,
            'Sell': 25.94
          },
          {
            'Symbol': 'SNHY',
            'Buy': 47.03,
            'Sell': 46.56
          },
          {
            'Symbol': 'SNLN',
            'Buy': 18.22,
            'Sell': 18.04
          },
          {
            'Symbol': 'SNMX',
            'Buy': 1.01,
            'Sell': 1
          },
          {
            'Symbol': 'SNNA',
            'Buy': 14.7,
            'Sell': 14.55
          },
          {
            'Symbol': 'SNOA',
            'Buy': 1.76,
            'Sell': 1.74
          },
          {
            'Symbol': 'SNOAW',
            'Buy': 0.1,
            'Sell': 0.1
          },
          {
            'Symbol': 'SNPS',
            'Buy': 92.77,
            'Sell': 91.84
          },
          {
            'Symbol': 'SNSR',
            'Buy': 20.26,
            'Sell': 20.06
          },
          {
            'Symbol': 'SNSS',
            'Buy': 2.31,
            'Sell': 2.29
          },
          {
            'Symbol': 'SOCL',
            'Buy': 33.06,
            'Sell': 32.73
          },
          {
            'Symbol': 'SODA',
            'Buy': 125,
            'Sell': 123.75
          },
          {
            'Symbol': 'SOFO',
            'Buy': 2.15,
            'Sell': 2.13
          },
          {
            'Symbol': 'SOHO',
            'Buy': 7.11,
            'Sell': 7.04
          },
          {
            'Symbol': 'SOHOO',
            'Buy': 25.11,
            'Sell': 24.86
          },
          {
            'Symbol': 'SOHU',
            'Buy': 23.83,
            'Sell': 23.59
          },
          {
            'Symbol': 'SOLO',
            'Buy': 2.49,
            'Sell': 2.47
          },
          {
            'Symbol': 'SONA',
            'Buy': 17.32,
            'Sell': 17.15
          },
          {
            'Symbol': 'SONC',
            'Buy': 34.08,
            'Sell': 33.74
          },
          {
            'Symbol': 'SONO',
            'Buy': 16.8,
            'Sell': 16.63
          },
          {
            'Symbol': 'SORL',
            'Buy': 5.06,
            'Sell': 5.01
          },
          {
            'Symbol': 'SOXX',
            'Buy': 184.72,
            'Sell': 182.87
          },
          {
            'Symbol': 'SP',
            'Buy': 39.4,
            'Sell': 39.01
          },
          {
            'Symbol': 'SPAR',
            'Buy': 14.95,
            'Sell': 14.8
          },
          {
            'Symbol': 'SPCB',
            'Buy': 1.9,
            'Sell': 1.88
          },
          {
            'Symbol': 'SPEX',
            'Buy': 1.1,
            'Sell': 1.09
          },
          {
            'Symbol': 'SPHS',
            'Buy': 2.74,
            'Sell': 2.71
          },
          {
            'Symbol': 'SPI',
            'Buy': 0.38,
            'Sell': 0.38
          },
          {
            'Symbol': 'SPKE',
            'Buy': 8.8,
            'Sell': 8.71
          },
          {
            'Symbol': 'SPKEP',
            'Buy': 23.38,
            'Sell': 23.15
          },
          {
            'Symbol': 'SPLK',
            'Buy': 104.99,
            'Sell': 103.94
          },
          {
            'Symbol': 'SPNE',
            'Buy': 14,
            'Sell': 13.86
          },
          {
            'Symbol': 'SPNS',
            'Buy': 10.75,
            'Sell': 10.64
          },
          {
            'Symbol': 'SPOK',
            'Buy': 15.35,
            'Sell': 15.2
          },
          {
            'Symbol': 'SPPI',
            'Buy': 20.5,
            'Sell': 20.3
          },
          {
            'Symbol': 'SPRO',
            'Buy': 11.37,
            'Sell': 11.26
          },
          {
            'Symbol': 'SPRT',
            'Buy': 2.85,
            'Sell': 2.82
          },
          {
            'Symbol': 'SPSC',
            'Buy': 91.48,
            'Sell': 90.57
          },
          {
            'Symbol': 'SPTN',
            'Buy': 23.46,
            'Sell': 23.23
          },
          {
            'Symbol': 'SPWH',
            'Buy': 4.85,
            'Sell': 4.8
          },
          {
            'Symbol': 'SPWR',
            'Buy': 7.2,
            'Sell': 7.13
          },
          {
            'Symbol': 'SQBG',
            'Buy': 2.08,
            'Sell': 2.06
          },
          {
            'Symbol': 'SQLV',
            'Buy': 30.31,
            'Sell': 30.01
          },
          {
            'Symbol': 'SQQQ',
            'Buy': 12.23,
            'Sell': 12.11
          },
          {
            'Symbol': 'SRAX',
            'Buy': 4.55,
            'Sell': 4.5
          },
          {
            'Symbol': 'SRCE',
            'Buy': 56.16,
            'Sell': 55.6
          },
          {
            'Symbol': 'SRCL',
            'Buy': 61.39,
            'Sell': 60.78
          },
          {
            'Symbol': 'SRCLP',
            'Buy': 46.68,
            'Sell': 46.21
          },
          {
            'Symbol': 'SRDX',
            'Buy': 68.1,
            'Sell': 67.42
          },
          {
            'Symbol': 'SRET',
            'Buy': 15.39,
            'Sell': 15.24
          },
          {
            'Symbol': 'SREV',
            'Buy': 3.35,
            'Sell': 3.32
          },
          {
            'Symbol': 'SRNE',
            'Buy': 5.4,
            'Sell': 5.35
          },
          {
            'Symbol': 'SRPT',
            'Buy': 130.13,
            'Sell': 128.83
          },
          {
            'Symbol': 'SRRA',
            'Buy': 2.1,
            'Sell': 2.08
          },
          {
            'Symbol': 'SRRK',
            'Buy': 13.86,
            'Sell': 13.72
          },
          {
            'Symbol': 'SRTS',
            'Buy': 7.32,
            'Sell': 7.25
          },
          {
            'Symbol': 'SRTSW',
            'Buy': 1.24,
            'Sell': 1.23
          },
          {
            'Symbol': 'SSB',
            'Buy': 82.75,
            'Sell': 81.92
          },
          {
            'Symbol': 'SSBI',
            'Buy': 15.6,
            'Sell': 15.44
          },
          {
            'Symbol': 'SSC',
            'Buy': 2.38,
            'Sell': 2.36
          },
          {
            'Symbol': 'SSFN',
            'Buy': 10.86,
            'Sell': 10.75
          },
          {
            'Symbol': 'SSKN',
            'Buy': 2.04,
            'Sell': 2.02
          },
          {
            'Symbol': 'SSLJ',
            'Buy': 1.15,
            'Sell': 1.14
          },
          {
            'Symbol': 'SSNC',
            'Buy': 55.2,
            'Sell': 54.65
          },
          {
            'Symbol': 'SSNT',
            'Buy': 3.89,
            'Sell': 3.85
          },
          {
            'Symbol': 'SSP',
            'Buy': 14.3,
            'Sell': 14.16
          },
          {
            'Symbol': 'SSRM',
            'Buy': 10.3,
            'Sell': 10.2
          },
          {
            'Symbol': 'SSTI',
            'Buy': 45.88,
            'Sell': 45.42
          },
          {
            'Symbol': 'SSYS',
            'Buy': 23.19,
            'Sell': 22.96
          },
          {
            'Symbol': 'STAA',
            'Buy': 39.5,
            'Sell': 39.11
          },
          {
            'Symbol': 'STAF',
            'Buy': 2.24,
            'Sell': 2.22
          },
          {
            'Symbol': 'STAY',
            'Buy': 21.14,
            'Sell': 20.93
          },
          {
            'Symbol': 'STBA',
            'Buy': 44.5,
            'Sell': 44.06
          },
          {
            'Symbol': 'STBZ',
            'Buy': 32.34,
            'Sell': 32.02
          },
          {
            'Symbol': 'STCN',
            'Buy': 2.08,
            'Sell': 2.06
          },
          {
            'Symbol': 'STDY',
            'Buy': 4.7,
            'Sell': 4.65
          },
          {
            'Symbol': 'STFC',
            'Buy': 30.44,
            'Sell': 30.14
          },
          {
            'Symbol': 'STIM',
            'Buy': 30.79,
            'Sell': 30.48
          },
          {
            'Symbol': 'STKL',
            'Buy': 8.05,
            'Sell': 7.97
          },
          {
            'Symbol': 'STKS',
            'Buy': 2.85,
            'Sell': 2.82
          },
          {
            'Symbol': 'STLD',
            'Buy': 43.65,
            'Sell': 43.21
          },
          {
            'Symbol': 'STLRW',
            'Buy': 0.15,
            'Sell': 0.15
          },
          {
            'Symbol': 'STML',
            'Buy': 15,
            'Sell': 14.85
          },
          {
            'Symbol': 'STMP',
            'Buy': 247.25,
            'Sell': 244.78
          },
          {
            'Symbol': 'STND',
            'Buy': 30,
            'Sell': 29.7
          },
          {
            'Symbol': 'STRA',
            'Buy': 126.26,
            'Sell': 125
          },
          {
            'Symbol': 'STRL',
            'Buy': 15.31,
            'Sell': 15.16
          },
          {
            'Symbol': 'STRM',
            'Buy': 1.23,
            'Sell': 1.22
          },
          {
            'Symbol': 'STRS',
            'Buy': 30.2,
            'Sell': 29.9
          },
          {
            'Symbol': 'STRT',
            'Buy': 33.55,
            'Sell': 33.21
          },
          {
            'Symbol': 'STX',
            'Buy': 50.56,
            'Sell': 50.05
          },
          {
            'Symbol': 'STXB',
            'Buy': 21.49,
            'Sell': 21.28
          },
          {
            'Symbol': 'SUMR',
            'Buy': 1.7,
            'Sell': 1.68
          },
          {
            'Symbol': 'SUNS',
            'Buy': 16.94,
            'Sell': 16.77
          },
          {
            'Symbol': 'SUNW',
            'Buy': 0.92,
            'Sell': 0.91
          },
          {
            'Symbol': 'SUPN',
            'Buy': 48,
            'Sell': 47.52
          },
          {
            'Symbol': 'SURF',
            'Buy': 10.97,
            'Sell': 10.86
          },
          {
            'Symbol': 'SUSB',
            'Buy': 24.55,
            'Sell': 24.3
          },
          {
            'Symbol': 'SUSC',
            'Buy': 24.34,
            'Sell': 24.1
          },
          {
            'Symbol': 'SVA',
            'Buy': 6.8,
            'Sell': 6.73
          },
          {
            'Symbol': 'SVBI',
            'Buy': 8.8,
            'Sell': 8.71
          },
          {
            'Symbol': 'SVRA',
            'Buy': 10.91,
            'Sell': 10.8
          },
          {
            'Symbol': 'SVVC',
            'Buy': 13.43,
            'Sell': 13.3
          },
          {
            'Symbol': 'SWIN',
            'Buy': 31.795,
            'Sell': 31.48
          },
          {
            'Symbol': 'SWIR',
            'Buy': 19.7,
            'Sell': 19.5
          },
          {
            'Symbol': 'SWKS',
            'Buy': 93.47,
            'Sell': 92.54
          },
          {
            'Symbol': 'SYBT',
            'Buy': 37.95,
            'Sell': 37.57
          },
          {
            'Symbol': 'SYBX',
            'Buy': 7.65,
            'Sell': 7.57
          },
          {
            'Symbol': 'SYKE',
            'Buy': 28.8,
            'Sell': 28.51
          },
          {
            'Symbol': 'SYMC',
            'Buy': 19.17,
            'Sell': 18.98
          },
          {
            'Symbol': 'SYNA',
            'Buy': 48.62,
            'Sell': 48.13
          },
          {
            'Symbol': 'SYNC',
            'Buy': 2.45,
            'Sell': 2.43
          },
          {
            'Symbol': 'SYNH',
            'Buy': 49.15,
            'Sell': 48.66
          },
          {
            'Symbol': 'SYNL',
            'Buy': 24.05,
            'Sell': 23.81
          },
          {
            'Symbol': 'SYNT',
            'Buy': 40.6,
            'Sell': 40.19
          },
          {
            'Symbol': 'SYPR',
            'Buy': 1.61,
            'Sell': 1.59
          },
          {
            'Symbol': 'SYRS',
            'Buy': 11.95,
            'Sell': 11.83
          },
          {
            'Symbol': 'TA',
            'Buy': 4,
            'Sell': 3.96
          },
          {
            'Symbol': 'TACO',
            'Buy': 12.55,
            'Sell': 12.42
          },
          {
            'Symbol': 'TACOW',
            'Buy': 2.85,
            'Sell': 2.82
          },
          {
            'Symbol': 'TACT',
            'Buy': 14.35,
            'Sell': 14.21
          },
          {
            'Symbol': 'TAIT',
            'Buy': 2.13,
            'Sell': 2.11
          },
          {
            'Symbol': 'TANH',
            'Buy': 1.57,
            'Sell': 1.55
          },
          {
            'Symbol': 'TANNI',
            'Buy': 24.45,
            'Sell': 24.21
          },
          {
            'Symbol': 'TANNL',
            'Buy': 23.8,
            'Sell': 23.56
          },
          {
            'Symbol': 'TANNZ',
            'Buy': 23.62,
            'Sell': 23.38
          },
          {
            'Symbol': 'TAOP',
            'Buy': 1.2,
            'Sell': 1.19
          },
          {
            'Symbol': 'TAPR',
            'Buy': 26.83,
            'Sell': 26.56
          },
          {
            'Symbol': 'TAST',
            'Buy': 15.5,
            'Sell': 15.35
          },
          {
            'Symbol': 'TATT',
            'Buy': 7.288,
            'Sell': 7.22
          },
          {
            'Symbol': 'TAYD',
            'Buy': 11.77,
            'Sell': 11.65
          },
          {
            'Symbol': 'TBBK',
            'Buy': 9.95,
            'Sell': 9.85
          },
          {
            'Symbol': 'TBIO',
            'Buy': 10.3,
            'Sell': 10.2
          },
          {
            'Symbol': 'TBK',
            'Buy': 41.95,
            'Sell': 41.53
          },
          {
            'Symbol': 'TBNK',
            'Buy': 30.05,
            'Sell': 29.75
          },
          {
            'Symbol': 'TBPH',
            'Buy': 27.73,
            'Sell': 27.45
          },
          {
            'Symbol': 'TBRG',
            'Buy': 9.65,
            'Sell': 9.55
          },
          {
            'Symbol': 'TBRGW',
            'Buy': 0.5,
            'Sell': 0.5
          },
          {
            'Symbol': 'TCBI',
            'Buy': 91.5,
            'Sell': 90.59
          },
          {
            'Symbol': 'TCBIL',
            'Buy': 25.59,
            'Sell': 25.33
          },
          {
            'Symbol': 'TCBIP',
            'Buy': 25.41,
            'Sell': 25.16
          },
          {
            'Symbol': 'TCBK',
            'Buy': 38.98,
            'Sell': 38.59
          },
          {
            'Symbol': 'TCCO',
            'Buy': 4.9,
            'Sell': 4.85
          },
          {
            'Symbol': 'TCDA',
            'Buy': 23.58,
            'Sell': 23.34
          },
          {
            'Symbol': 'TCFC',
            'Buy': 34.11,
            'Sell': 33.77
          },
          {
            'Symbol': 'TCGP',
            'Buy': 24.08,
            'Sell': 23.84
          },
          {
            'Symbol': 'TCMD',
            'Buy': 56.55,
            'Sell': 55.98
          },
          {
            'Symbol': 'TCON',
            'Buy': 2.15,
            'Sell': 2.13
          },
          {
            'Symbol': 'TCPC',
            'Buy': 14.82,
            'Sell': 14.67
          },
          {
            'Symbol': 'TCRD',
            'Buy': 8.08,
            'Sell': 8
          },
          {
            'Symbol': 'TCX',
            'Buy': 54.6,
            'Sell': 54.05
          },
          {
            'Symbol': 'TDIV',
            'Buy': 37.09,
            'Sell': 36.72
          },
          {
            'Symbol': 'TEAM',
            'Buy': 77.52,
            'Sell': 76.74
          },
          {
            'Symbol': 'TECD',
            'Buy': 84.93,
            'Sell': 84.08
          },
          {
            'Symbol': 'TECH',
            'Buy': 178.06,
            'Sell': 176.28
          },
          {
            'Symbol': 'TEDU',
            'Buy': 8.89,
            'Sell': 8.8
          },
          {
            'Symbol': 'TELL',
            'Buy': 7.17,
            'Sell': 7.1
          },
          {
            'Symbol': 'TENB',
            'Buy': 34.07,
            'Sell': 33.73
          },
          {
            'Symbol': 'TENX',
            'Buy': 6.15,
            'Sell': 6.09
          },
          {
            'Symbol': 'TERP',
            'Buy': 10.68,
            'Sell': 10.57
          },
          {
            'Symbol': 'TESS',
            'Buy': 16.95,
            'Sell': 16.78
          },
          {
            'Symbol': 'TFSL',
            'Buy': 15.3,
            'Sell': 15.15
          },
          {
            'Symbol': 'TGA',
            'Buy': 3.2,
            'Sell': 3.17
          },
          {
            'Symbol': 'TGEN',
            'Buy': 3.25,
            'Sell': 3.22
          },
          {
            'Symbol': 'TGLS',
            'Buy': 8.79,
            'Sell': 8.7
          },
          {
            'Symbol': 'TGTX',
            'Buy': 11.85,
            'Sell': 11.73
          },
          {
            'Symbol': 'THFF',
            'Buy': 50.25,
            'Sell': 49.75
          },
          {
            'Symbol': 'THRM',
            'Buy': 46.1,
            'Sell': 45.64
          },
          {
            'Symbol': 'TILE',
            'Buy': 22.6,
            'Sell': 22.37
          },
          {
            'Symbol': 'TIPT',
            'Buy': 6.75,
            'Sell': 6.68
          },
          {
            'Symbol': 'TISA',
            'Buy': 0.98,
            'Sell': 0.97
          },
          {
            'Symbol': 'TITN',
            'Buy': 15.15,
            'Sell': 15
          },
          {
            'Symbol': 'TIVO',
            'Buy': 12.1,
            'Sell': 11.98
          },
          {
            'Symbol': 'TLF',
            'Buy': 7.75,
            'Sell': 7.67
          },
          {
            'Symbol': 'TLGT',
            'Buy': 4,
            'Sell': 3.96
          },
          {
            'Symbol': 'TLND',
            'Buy': 58.44,
            'Sell': 57.86
          },
          {
            'Symbol': 'TLRY',
            'Buy': 25.5,
            'Sell': 25.25
          },
          {
            'Symbol': 'TLT',
            'Buy': 120.37,
            'Sell': 119.17
          },
          {
            'Symbol': 'TMCX',
            'Buy': 9.83,
            'Sell': 9.73
          },
          {
            'Symbol': 'TMCXW',
            'Buy': 0.44,
            'Sell': 0.44
          },
          {
            'Symbol': 'TMDI',
            'Buy': 2.18,
            'Sell': 2.16
          },
          {
            'Symbol': 'TMSR',
            'Buy': 10,
            'Sell': 9.9
          },
          {
            'Symbol': 'TMUS',
            'Buy': 64.64,
            'Sell': 63.99
          },
          {
            'Symbol': 'TNAV',
            'Buy': 5.75,
            'Sell': 5.69
          },
          {
            'Symbol': 'TNDM',
            'Buy': 31.12,
            'Sell': 30.81
          },
          {
            'Symbol': 'TNXP',
            'Buy': 1.15,
            'Sell': 1.14
          },
          {
            'Symbol': 'TOCA',
            'Buy': 9.07,
            'Sell': 8.98
          },
          {
            'Symbol': 'TOPS',
            'Buy': 1,
            'Sell': 0.99
          },
          {
            'Symbol': 'TORC',
            'Buy': 11.29,
            'Sell': 11.18
          },
          {
            'Symbol': 'TOTAU',
            'Buy': 10.18,
            'Sell': 10.08
          },
          {
            'Symbol': 'TOUR',
            'Buy': 7.44,
            'Sell': 7.37
          },
          {
            'Symbol': 'TOWN',
            'Buy': 32,
            'Sell': 31.68
          },
          {
            'Symbol': 'TPIC',
            'Buy': 28.72,
            'Sell': 28.43
          },
          {
            'Symbol': 'TPIV',
            'Buy': 8.5,
            'Sell': 8.42
          },
          {
            'Symbol': 'TQQQ',
            'Buy': 65.96,
            'Sell': 65.3
          },
          {
            'Symbol': 'TRCB',
            'Buy': 17.99,
            'Sell': 17.81
          },
          {
            'Symbol': 'TRCH',
            'Buy': 1.22,
            'Sell': 1.21
          },
          {
            'Symbol': 'TREE',
            'Buy': 244.5,
            'Sell': 242.06
          },
          {
            'Symbol': 'TRHC',
            'Buy': 65.17,
            'Sell': 64.52
          },
          {
            'Symbol': 'TRIB',
            'Buy': 4.52,
            'Sell': 4.47
          },
          {
            'Symbol': 'TRIL',
            'Buy': 5.47,
            'Sell': 5.42
          },
          {
            'Symbol': 'TRIP',
            'Buy': 54.44,
            'Sell': 53.9
          },
          {
            'Symbol': 'TRMB',
            'Buy': 39.23,
            'Sell': 38.84
          },
          {
            'Symbol': 'TRMD',
            'Buy': 7.08,
            'Sell': 7.01
          },
          {
            'Symbol': 'TRMK',
            'Buy': 34.91,
            'Sell': 34.56
          },
          {
            'Symbol': 'TRMT',
            'Buy': 12.7,
            'Sell': 12.57
          },
          {
            'Symbol': 'TRNC',
            'Buy': 16.93,
            'Sell': 16.76
          },
          {
            'Symbol': 'TRNS',
            'Buy': 23.7,
            'Sell': 23.46
          },
          {
            'Symbol': 'TROV',
            'Buy': 0.81,
            'Sell': 0.8
          },
          {
            'Symbol': 'TROW',
            'Buy': 117.96,
            'Sell': 116.78
          },
          {
            'Symbol': 'TRPX',
            'Buy': 3.72,
            'Sell': 3.68
          },
          {
            'Symbol': 'TRS',
            'Buy': 30.65,
            'Sell': 30.34
          },
          {
            'Symbol': 'TRST',
            'Buy': 9.2,
            'Sell': 9.11
          },
          {
            'Symbol': 'TRUE',
            'Buy': 11.78,
            'Sell': 11.66
          },
          {
            'Symbol': 'TRUP',
            'Buy': 37.85,
            'Sell': 37.47
          },
          {
            'Symbol': 'TRVG',
            'Buy': 4.36,
            'Sell': 4.32
          },
          {
            'Symbol': 'TRVN',
            'Buy': 1.6,
            'Sell': 1.58
          },
          {
            'Symbol': 'TSBK',
            'Buy': 36.36,
            'Sell': 36
          },
          {
            'Symbol': 'TSC',
            'Buy': 29.3,
            'Sell': 29.01
          },
          {
            'Symbol': 'TSCAP',
            'Buy': 26.35,
            'Sell': 26.09
          },
          {
            'Symbol': 'TSCO',
            'Buy': 80.51,
            'Sell': 79.7
          },
          {
            'Symbol': 'TSEM',
            'Buy': 20.39,
            'Sell': 20.19
          },
          {
            'Symbol': 'TSG',
            'Buy': 31.75,
            'Sell': 31.43
          },
          {
            'Symbol': 'TSLA',
            'Buy': 354,
            'Sell': 350.46
          },
          {
            'Symbol': 'TSRI',
            'Buy': 7.1,
            'Sell': 7.03
          },
          {
            'Symbol': 'TSRO',
            'Buy': 27.06,
            'Sell': 26.79
          },
          {
            'Symbol': 'TST',
            'Buy': 2.16,
            'Sell': 2.14
          },
          {
            'Symbol': 'TTD',
            'Buy': 115.5,
            'Sell': 114.35
          },
          {
            'Symbol': 'TTEC',
            'Buy': 27.3,
            'Sell': 27.03
          },
          {
            'Symbol': 'TTEK',
            'Buy': 67.7,
            'Sell': 67.02
          },
          {
            'Symbol': 'TTGT',
            'Buy': 23.88,
            'Sell': 23.64
          },
          {
            'Symbol': 'TTMI',
            'Buy': 18.62,
            'Sell': 18.43
          },
          {
            'Symbol': 'TTNP',
            'Buy': 0.89,
            'Sell': 0.88
          },
          {
            'Symbol': 'TTOO',
            'Buy': 6.5,
            'Sell': 6.44
          },
          {
            'Symbol': 'TTPH',
            'Buy': 3.36,
            'Sell': 3.33
          },
          {
            'Symbol': 'TTS',
            'Buy': 8.1,
            'Sell': 8.02
          },
          {
            'Symbol': 'TTWO',
            'Buy': 124.4,
            'Sell': 123.16
          },
          {
            'Symbol': 'TUES',
            'Buy': 2.9,
            'Sell': 2.87
          },
          {
            'Symbol': 'TUR',
            'Buy': 19.83,
            'Sell': 19.63
          },
          {
            'Symbol': 'TURN',
            'Buy': 2.4,
            'Sell': 2.38
          },
          {
            'Symbol': 'TUSA',
            'Buy': 35.4,
            'Sell': 35.05
          },
          {
            'Symbol': 'TUSK',
            'Buy': 37.61,
            'Sell': 37.23
          },
          {
            'Symbol': 'TVIX',
            'Buy': 33.7,
            'Sell': 33.36
          },
          {
            'Symbol': 'TVIZ',
            'Buy': 8.27,
            'Sell': 8.19
          },
          {
            'Symbol': 'TVTY',
            'Buy': 33.7,
            'Sell': 33.36
          },
          {
            'Symbol': 'TWIN',
            'Buy': 26.15,
            'Sell': 25.89
          },
          {
            'Symbol': 'TWLVU',
            'Buy': 10.18,
            'Sell': 10.08
          },
          {
            'Symbol': 'TWLVW',
            'Buy': 0.35,
            'Sell': 0.35
          },
          {
            'Symbol': 'TWMC',
            'Buy': 1.02,
            'Sell': 1.01
          },
          {
            'Symbol': 'TWNK',
            'Buy': 12.08,
            'Sell': 11.96
          },
          {
            'Symbol': 'TWNKW',
            'Buy': 1.35,
            'Sell': 1.34
          },
          {
            'Symbol': 'TWOU',
            'Buy': 71.78,
            'Sell': 71.06
          },
          {
            'Symbol': 'TXMD',
            'Buy': 5.19,
            'Sell': 5.14
          },
          {
            'Symbol': 'TXN',
            'Buy': 111.74,
            'Sell': 110.62
          },
          {
            'Symbol': 'TXRH',
            'Buy': 63.76,
            'Sell': 63.12
          },
          {
            'Symbol': 'TYHT',
            'Buy': 1.28,
            'Sell': 1.27
          },
          {
            'Symbol': 'TYME',
            'Buy': 2.45,
            'Sell': 2.43
          },
          {
            'Symbol': 'TYPE',
            'Buy': 20.85,
            'Sell': 20.64
          },
          {
            'Symbol': 'TZOO',
            'Buy': 13.15,
            'Sell': 13.02
          },
          {
            'Symbol': 'UAE',
            'Buy': 15.74,
            'Sell': 15.58
          },
          {
            'Symbol': 'UBFO',
            'Buy': 10.8,
            'Sell': 10.69
          },
          {
            'Symbol': 'UBIO',
            'Buy': 39.65,
            'Sell': 39.25
          },
          {
            'Symbol': 'UBNK',
            'Buy': 17.67,
            'Sell': 17.49
          },
          {
            'Symbol': 'UBNT',
            'Buy': 81.22,
            'Sell': 80.41
          },
          {
            'Symbol': 'UBOH',
            'Buy': 23.32,
            'Sell': 23.09
          },
          {
            'Symbol': 'UBSH',
            'Buy': 40.57,
            'Sell': 40.16
          },
          {
            'Symbol': 'UBSI',
            'Buy': 37.95,
            'Sell': 37.57
          },
          {
            'Symbol': 'UBX',
            'Buy': 15.8,
            'Sell': 15.64
          },
          {
            'Symbol': 'UCBA',
            'Buy': 27.6,
            'Sell': 27.32
          },
          {
            'Symbol': 'UCBI',
            'Buy': 30.57,
            'Sell': 30.26
          },
          {
            'Symbol': 'UCFC',
            'Buy': 10.54,
            'Sell': 10.43
          },
          {
            'Symbol': 'UCTT',
            'Buy': 14.07,
            'Sell': 13.93
          },
          {
            'Symbol': 'UDBI',
            'Buy': 32.93,
            'Sell': 32.6
          },
          {
            'Symbol': 'UEIC',
            'Buy': 42,
            'Sell': 41.58
          },
          {
            'Symbol': 'UEPS',
            'Buy': 9.4,
            'Sell': 9.31
          },
          {
            'Symbol': 'UFCS',
            'Buy': 49.18,
            'Sell': 48.69
          },
          {
            'Symbol': 'UFPI',
            'Buy': 36.59,
            'Sell': 36.22
          },
          {
            'Symbol': 'UFPT',
            'Buy': 34.9,
            'Sell': 34.55
          },
          {
            'Symbol': 'UG',
            'Buy': 18.764,
            'Sell': 18.58
          },
          {
            'Symbol': 'UGLD',
            'Buy': 8.39,
            'Sell': 8.31
          },
          {
            'Symbol': 'UHAL',
            'Buy': 362.18,
            'Sell': 358.56
          },
          {
            'Symbol': 'UIHC',
            'Buy': 20.82,
            'Sell': 20.61
          },
          {
            'Symbol': 'ULBI',
            'Buy': 10,
            'Sell': 9.9
          },
          {
            'Symbol': 'ULH',
            'Buy': 35.55,
            'Sell': 35.19
          },
          {
            'Symbol': 'ULTA',
            'Buy': 234.25,
            'Sell': 231.91
          },
          {
            'Symbol': 'ULTI',
            'Buy': 281.24,
            'Sell': 278.43
          },
          {
            'Symbol': 'UMBF',
            'Buy': 74.27,
            'Sell': 73.53
          },
          {
            'Symbol': 'UMPQ',
            'Buy': 21.15,
            'Sell': 20.94
          },
          {
            'Symbol': 'UMRX',
            'Buy': 14.95,
            'Sell': 14.8
          },
          {
            'Symbol': 'UNB',
            'Buy': 52.58,
            'Sell': 52.05
          },
          {
            'Symbol': 'UNFI',
            'Buy': 34.2,
            'Sell': 33.86
          },
          {
            'Symbol': 'UNIT',
            'Buy': 18.24,
            'Sell': 18.06
          },
          {
            'Symbol': 'UNTY',
            'Buy': 23.75,
            'Sell': 23.51
          },
          {
            'Symbol': 'UONE',
            'Buy': 2.65,
            'Sell': 2.62
          },
          {
            'Symbol': 'UONEK',
            'Buy': 2.2,
            'Sell': 2.18
          },
          {
            'Symbol': 'UPL',
            'Buy': 1.25,
            'Sell': 1.24
          },
          {
            'Symbol': 'UPLD',
            'Buy': 34.23,
            'Sell': 33.89
          },
          {
            'Symbol': 'URBN',
            'Buy': 47.61,
            'Sell': 47.13
          },
          {
            'Symbol': 'URGN',
            'Buy': 45.16,
            'Sell': 44.71
          },
          {
            'Symbol': 'USAK',
            'Buy': 22.25,
            'Sell': 22.03
          },
          {
            'Symbol': 'USAP',
            'Buy': 29.12,
            'Sell': 28.83
          },
          {
            'Symbol': 'USAT',
            'Buy': 15.5,
            'Sell': 15.35
          },
          {
            'Symbol': 'USAU',
            'Buy': 1.23,
            'Sell': 1.22
          },
          {
            'Symbol': 'USCR',
            'Buy': 53.2,
            'Sell': 52.67
          },
          {
            'Symbol': 'USEG',
            'Buy': 1.11,
            'Sell': 1.1
          },
          {
            'Symbol': 'USIG',
            'Buy': 53.56,
            'Sell': 53.02
          },
          {
            'Symbol': 'USLM',
            'Buy': 77.33,
            'Sell': 76.56
          },
          {
            'Symbol': 'USLV',
            'Buy': 7.74,
            'Sell': 7.66
          },
          {
            'Symbol': 'USMC',
            'Buy': 27.32,
            'Sell': 27.05
          },
          {
            'Symbol': 'USOI',
            'Buy': 28.55,
            'Sell': 28.26
          },
          {
            'Symbol': 'UTHR',
            'Buy': 126.53,
            'Sell': 125.26
          },
          {
            'Symbol': 'UTMD',
            'Buy': 93,
            'Sell': 92.07
          },
          {
            'Symbol': 'UTSI',
            'Buy': 3.68,
            'Sell': 3.64
          },
          {
            'Symbol': 'UVSP',
            'Buy': 27.5,
            'Sell': 27.23
          },
          {
            'Symbol': 'UXIN',
            'Buy': 6.68,
            'Sell': 6.61
          },
          {
            'Symbol': 'VALU',
            'Buy': 22.64,
            'Sell': 22.41
          },
          {
            'Symbol': 'VALX',
            'Buy': 29.4,
            'Sell': 29.11
          },
          {
            'Symbol': 'VBIV',
            'Buy': 2.03,
            'Sell': 2.01
          },
          {
            'Symbol': 'VBLT',
            'Buy': 1.75,
            'Sell': 1.73
          },
          {
            'Symbol': 'VBTX',
            'Buy': 30.6,
            'Sell': 30.29
          },
          {
            'Symbol': 'VC',
            'Buy': 120.18,
            'Sell': 118.98
          },
          {
            'Symbol': 'VCEL',
            'Buy': 12.6,
            'Sell': 12.47
          },
          {
            'Symbol': 'VCIT',
            'Buy': 83.78,
            'Sell': 82.94
          },
          {
            'Symbol': 'VCLT',
            'Buy': 88.17,
            'Sell': 87.29
          },
          {
            'Symbol': 'VCSH',
            'Buy': 78.16,
            'Sell': 77.38
          },
          {
            'Symbol': 'VCTR',
            'Buy': 9.85,
            'Sell': 9.75
          },
          {
            'Symbol': 'VCYT',
            'Buy': 11.82,
            'Sell': 11.7
          },
          {
            'Symbol': 'VEACW',
            'Buy': 1.5,
            'Sell': 1.49
          },
          {
            'Symbol': 'VECO',
            'Buy': 11.75,
            'Sell': 11.63
          },
          {
            'Symbol': 'VEON',
            'Buy': 2.86,
            'Sell': 2.83
          },
          {
            'Symbol': 'VERI',
            'Buy': 14.74,
            'Sell': 14.59
          },
          {
            'Symbol': 'VERU',
            'Buy': 2.16,
            'Sell': 2.14
          },
          {
            'Symbol': 'VGIT',
            'Buy': 62.52,
            'Sell': 61.89
          },
          {
            'Symbol': 'VGLT',
            'Buy': 73.99,
            'Sell': 73.25
          },
          {
            'Symbol': 'VGSH',
            'Buy': 59.83,
            'Sell': 59.23
          },
          {
            'Symbol': 'VIA',
            'Buy': 35.9,
            'Sell': 35.54
          },
          {
            'Symbol': 'VIAB',
            'Buy': 30.39,
            'Sell': 30.09
          },
          {
            'Symbol': 'VIAV',
            'Buy': 10.44,
            'Sell': 10.34
          },
          {
            'Symbol': 'VICL',
            'Buy': 1.35,
            'Sell': 1.34
          },
          {
            'Symbol': 'VICR',
            'Buy': 63.05,
            'Sell': 62.42
          },
          {
            'Symbol': 'VIGI',
            'Buy': 65.02,
            'Sell': 64.37
          },
          {
            'Symbol': 'VIIX',
            'Buy': 12.59,
            'Sell': 12.46
          },
          {
            'Symbol': 'VIRC',
            'Buy': 4.85,
            'Sell': 4.8
          },
          {
            'Symbol': 'VIRT',
            'Buy': 20.8,
            'Sell': 20.59
          },
          {
            'Symbol': 'VIVE',
            'Buy': 2.56,
            'Sell': 2.53
          },
          {
            'Symbol': 'VIVO',
            'Buy': 14.9,
            'Sell': 14.75
          },
          {
            'Symbol': 'VKTX',
            'Buy': 9.72,
            'Sell': 9.62
          },
          {
            'Symbol': 'VKTXW',
            'Buy': 8.33,
            'Sell': 8.25
          },
          {
            'Symbol': 'VLGEA',
            'Buy': 27.4,
            'Sell': 27.13
          },
          {
            'Symbol': 'VLRX',
            'Buy': 1.61,
            'Sell': 1.59
          },
          {
            'Symbol': 'VMBS',
            'Buy': 51.34,
            'Sell': 50.83
          },
          {
            'Symbol': 'VNDA',
            'Buy': 21.55,
            'Sell': 21.33
          },
          {
            'Symbol': 'VNET',
            'Buy': 8.82,
            'Sell': 8.73
          },
          {
            'Symbol': 'VNOM',
            'Buy': 37.63,
            'Sell': 37.25
          },
          {
            'Symbol': 'VNQI',
            'Buy': 57.44,
            'Sell': 56.87
          },
          {
            'Symbol': 'VOD',
            'Buy': 23.59,
            'Sell': 23.35
          },
          {
            'Symbol': 'VONE',
            'Buy': 130.32,
            'Sell': 129.02
          },
          {
            'Symbol': 'VONG',
            'Buy': 154.92,
            'Sell': 153.37
          },
          {
            'Symbol': 'VONV',
            'Buy': 109.41,
            'Sell': 108.32
          },
          {
            'Symbol': 'VOXX',
            'Buy': 5.35,
            'Sell': 5.3
          },
          {
            'Symbol': 'VRA',
            'Buy': 14.01,
            'Sell': 13.87
          },
          {
            'Symbol': 'VRAY',
            'Buy': 9.76,
            'Sell': 9.66
          },
          {
            'Symbol': 'VRCA',
            'Buy': 15.29,
            'Sell': 15.14
          },
          {
            'Symbol': 'VREX',
            'Buy': 28.51,
            'Sell': 28.22
          },
          {
            'Symbol': 'VRIG',
            'Buy': 25.15,
            'Sell': 24.9
          },
          {
            'Symbol': 'VRML',
            'Buy': 0.6,
            'Sell': 0.59
          },
          {
            'Symbol': 'VRNA',
            'Buy': 13.04,
            'Sell': 12.91
          },
          {
            'Symbol': 'VRNS',
            'Buy': 67.6,
            'Sell': 66.92
          },
          {
            'Symbol': 'VRNT',
            'Buy': 46.4,
            'Sell': 45.94
          },
          {
            'Symbol': 'VRSK',
            'Buy': 116.25,
            'Sell': 115.09
          },
          {
            'Symbol': 'VRSN',
            'Buy': 152.31,
            'Sell': 150.79
          },
          {
            'Symbol': 'VRTS',
            'Buy': 130.7,
            'Sell': 129.39
          },
          {
            'Symbol': 'VRTSP',
            'Buy': 109.33,
            'Sell': 108.24
          },
          {
            'Symbol': 'VRTU',
            'Buy': 52.27,
            'Sell': 51.75
          },
          {
            'Symbol': 'VRTX',
            'Buy': 174.41,
            'Sell': 172.67
          },
          {
            'Symbol': 'VSAR',
            'Buy': 1.7,
            'Sell': 1.68
          },
          {
            'Symbol': 'VSAT',
            'Buy': 66.52,
            'Sell': 65.85
          },
          {
            'Symbol': 'VSEC',
            'Buy': 40.38,
            'Sell': 39.98
          },
          {
            'Symbol': 'VSMV',
            'Buy': 29.19,
            'Sell': 28.9
          },
          {
            'Symbol': 'VSTM',
            'Buy': 8.75,
            'Sell': 8.66
          },
          {
            'Symbol': 'VTC',
            'Buy': 81.57,
            'Sell': 80.75
          },
          {
            'Symbol': 'VTGN',
            'Buy': 1.28,
            'Sell': 1.27
          },
          {
            'Symbol': 'VTHR',
            'Buy': 130.69,
            'Sell': 129.38
          },
          {
            'Symbol': 'VTIP',
            'Buy': 48.87,
            'Sell': 48.38
          },
          {
            'Symbol': 'VTL',
            'Buy': 9.2,
            'Sell': 9.11
          },
          {
            'Symbol': 'VTNR',
            'Buy': 1.21,
            'Sell': 1.2
          },
          {
            'Symbol': 'VTSI',
            'Buy': 4.8,
            'Sell': 4.75
          },
          {
            'Symbol': 'VTVT',
            'Buy': 1.22,
            'Sell': 1.21
          },
          {
            'Symbol': 'VTWG',
            'Buy': 152.73,
            'Sell': 151.2
          },
          {
            'Symbol': 'VTWO',
            'Buy': 134.28,
            'Sell': 132.94
          },
          {
            'Symbol': 'VTWV',
            'Buy': 116.61,
            'Sell': 115.44
          },
          {
            'Symbol': 'VUZI',
            'Buy': 7.25,
            'Sell': 7.18
          },
          {
            'Symbol': 'VVPR',
            'Buy': 1.5,
            'Sell': 1.49
          },
          {
            'Symbol': 'VVUS',
            'Buy': 0.65,
            'Sell': 0.64
          },
          {
            'Symbol': 'VWOB',
            'Buy': 75.52,
            'Sell': 74.76
          },
          {
            'Symbol': 'VXRT',
            'Buy': 3.05,
            'Sell': 3.02
          },
          {
            'Symbol': 'VXUS',
            'Buy': 53.88,
            'Sell': 53.34
          },
          {
            'Symbol': 'VYGR',
            'Buy': 18.24,
            'Sell': 18.06
          },
          {
            'Symbol': 'VYMI',
            'Buy': 62.65,
            'Sell': 62.02
          },
          {
            'Symbol': 'WABC',
            'Buy': 62.84,
            'Sell': 62.21
          },
          {
            'Symbol': 'WAFD',
            'Buy': 33.5,
            'Sell': 33.17
          },
          {
            'Symbol': 'WASH',
            'Buy': 58.35,
            'Sell': 57.77
          },
          {
            'Symbol': 'WATT',
            'Buy': 13.47,
            'Sell': 13.34
          },
          {
            'Symbol': 'WB',
            'Buy': 78.73,
            'Sell': 77.94
          },
          {
            'Symbol': 'WBA',
            'Buy': 65.78,
            'Sell': 65.12
          },
          {
            'Symbol': 'WDAY',
            'Buy': 135.93,
            'Sell': 134.57
          },
          {
            'Symbol': 'WDC',
            'Buy': 65.51,
            'Sell': 64.85
          },
          {
            'Symbol': 'WDFC',
            'Buy': 164.25,
            'Sell': 162.61
          },
          {
            'Symbol': 'WEB',
            'Buy': 28.05,
            'Sell': 27.77
          },
          {
            'Symbol': 'WEBK',
            'Buy': 33.61,
            'Sell': 33.27
          },
          {
            'Symbol': 'WEN',
            'Buy': 17.73,
            'Sell': 17.55
          },
          {
            'Symbol': 'WERN',
            'Buy': 36.9,
            'Sell': 36.53
          },
          {
            'Symbol': 'WETF',
            'Buy': 8.07,
            'Sell': 7.99
          },
          {
            'Symbol': 'WEYS',
            'Buy': 36.5,
            'Sell': 36.14
          },
          {
            'Symbol': 'WHF',
            'Buy': 14.76,
            'Sell': 14.61
          },
          {
            'Symbol': 'WHLR',
            'Buy': 4.97,
            'Sell': 4.92
          },
          {
            'Symbol': 'WHLRD',
            'Buy': 19.92,
            'Sell': 19.72
          },
          {
            'Symbol': 'WHLRP',
            'Buy': 18.79,
            'Sell': 18.6
          },
          {
            'Symbol': 'WIFI',
            'Buy': 30.17,
            'Sell': 29.87
          },
          {
            'Symbol': 'WILC',
            'Buy': 6.95,
            'Sell': 6.88
          },
          {
            'Symbol': 'WIN',
            'Buy': 4.82,
            'Sell': 4.77
          },
          {
            'Symbol': 'WINA',
            'Buy': 146.05,
            'Sell': 144.59
          },
          {
            'Symbol': 'WING',
            'Buy': 61.2,
            'Sell': 60.59
          },
          {
            'Symbol': 'WIRE',
            'Buy': 51.1,
            'Sell': 50.59
          },
          {
            'Symbol': 'WISA',
            'Buy': 4.77,
            'Sell': 4.72
          },
          {
            'Symbol': 'WIX',
            'Buy': 105.95,
            'Sell': 104.89
          },
          {
            'Symbol': 'WKHS',
            'Buy': 1.11,
            'Sell': 1.1
          },
          {
            'Symbol': 'WLDN',
            'Buy': 31.57,
            'Sell': 31.25
          },
          {
            'Symbol': 'WLFC',
            'Buy': 33.67,
            'Sell': 33.33
          },
          {
            'Symbol': 'WLTW',
            'Buy': 150.71,
            'Sell': 149.2
          },
          {
            'Symbol': 'WMGI',
            'Buy': 27.86,
            'Sell': 27.58
          },
          {
            'Symbol': 'WMIH',
            'Buy': 1.37,
            'Sell': 1.36
          },
          {
            'Symbol': 'WNEB',
            'Buy': 10.7,
            'Sell': 10.59
          },
          {
            'Symbol': 'WOOD',
            'Buy': 74.98,
            'Sell': 74.23
          },
          {
            'Symbol': 'WPRT',
            'Buy': 2.6,
            'Sell': 2.57
          },
          {
            'Symbol': 'WRLD',
            'Buy': 110.36,
            'Sell': 109.26
          },
          {
            'Symbol': 'WRLS',
            'Buy': 9.9,
            'Sell': 9.8
          },
          {
            'Symbol': 'WRLSR',
            'Buy': 0.5,
            'Sell': 0.5
          },
          {
            'Symbol': 'WSBC',
            'Buy': 48.82,
            'Sell': 48.33
          },
          {
            'Symbol': 'WSBF',
            'Buy': 16.9,
            'Sell': 16.73
          },
          {
            'Symbol': 'WSC',
            'Buy': 16.3,
            'Sell': 16.14
          },
          {
            'Symbol': 'WSCI',
            'Buy': 5.44,
            'Sell': 5.39
          },
          {
            'Symbol': 'WSFS',
            'Buy': 50.4,
            'Sell': 49.9
          },
          {
            'Symbol': 'WSTG',
            'Buy': 12.45,
            'Sell': 12.33
          },
          {
            'Symbol': 'WSTL',
            'Buy': 2.54,
            'Sell': 2.51
          },
          {
            'Symbol': 'WTBA',
            'Buy': 24.27,
            'Sell': 24.03
          },
          {
            'Symbol': 'WTFC',
            'Buy': 88.26,
            'Sell': 87.38
          },
          {
            'Symbol': 'WTFCM',
            'Buy': 26.76,
            'Sell': 26.49
          },
          {
            'Symbol': 'WVE',
            'Buy': 38.65,
            'Sell': 38.26
          },
          {
            'Symbol': 'WVFC',
            'Buy': 16.28,
            'Sell': 16.12
          },
          {
            'Symbol': 'WVVI',
            'Buy': 8.06,
            'Sell': 7.98
          },
          {
            'Symbol': 'WWD',
            'Buy': 79.85,
            'Sell': 79.05
          },
          {
            'Symbol': 'WWR',
            'Buy': 0.31,
            'Sell': 0.31
          },
          {
            'Symbol': 'WYNN',
            'Buy': 150.98,
            'Sell': 149.47
          },
          {
            'Symbol': 'XBIO',
            'Buy': 3.05,
            'Sell': 3.02
          },
          {
            'Symbol': 'XBIT',
            'Buy': 3.9,
            'Sell': 3.86
          },
          {
            'Symbol': 'XCRA',
            'Buy': 14.39,
            'Sell': 14.25
          },
          {
            'Symbol': 'XEL',
            'Buy': 47.73,
            'Sell': 47.25
          },
          {
            'Symbol': 'XELA',
            'Buy': 4.98,
            'Sell': 4.93
          },
          {
            'Symbol': 'XELB',
            'Buy': 2.6,
            'Sell': 2.57
          },
          {
            'Symbol': 'XENE',
            'Buy': 12.05,
            'Sell': 11.93
          },
          {
            'Symbol': 'XENT',
            'Buy': 26.25,
            'Sell': 25.99
          },
          {
            'Symbol': 'XERS',
            'Buy': 19.6,
            'Sell': 19.4
          },
          {
            'Symbol': 'XGTI',
            'Buy': 0.59,
            'Sell': 0.58
          },
          {
            'Symbol': 'XLNX',
            'Buy': 72.19,
            'Sell': 71.47
          },
          {
            'Symbol': 'XLRN',
            'Buy': 46.27,
            'Sell': 45.81
          },
          {
            'Symbol': 'XNCR',
            'Buy': 39.55,
            'Sell': 39.15
          },
          {
            'Symbol': 'XNET',
            'Buy': 10.27,
            'Sell': 10.17
          },
          {
            'Symbol': 'XOG',
            'Buy': 12.6,
            'Sell': 12.47
          },
          {
            'Symbol': 'XOMA',
            'Buy': 19.3,
            'Sell': 19.11
          },
          {
            'Symbol': 'XONE',
            'Buy': 6.7,
            'Sell': 6.63
          },
          {
            'Symbol': 'XPER',
            'Buy': 16.75,
            'Sell': 16.58
          },
          {
            'Symbol': 'XPLR',
            'Buy': 5.99,
            'Sell': 5.93
          },
          {
            'Symbol': 'XRAY',
            'Buy': 40,
            'Sell': 39.6
          },
          {
            'Symbol': 'XSPA',
            'Buy': 0.25,
            'Sell': 0.25
          },
          {
            'Symbol': 'XSPL',
            'Buy': 5.81,
            'Sell': 5.75
          },
          {
            'Symbol': 'XT',
            'Buy': 37.96,
            'Sell': 37.58
          },
          {
            'Symbol': 'YDIV',
            'Buy': 17.27,
            'Sell': 17.1
          },
          {
            'Symbol': 'YECO',
            'Buy': 1.51,
            'Sell': 1.49
          },
          {
            'Symbol': 'YGYI',
            'Buy': 4.31,
            'Sell': 4.27
          },
          {
            'Symbol': 'YIN',
            'Buy': 7.71,
            'Sell': 7.63
          },
          {
            'Symbol': 'YNDX',
            'Buy': 31.25,
            'Sell': 30.94
          },
          {
            'Symbol': 'YOGA',
            'Buy': 2.02,
            'Sell': 2
          },
          {
            'Symbol': 'YORW',
            'Buy': 29.75,
            'Sell': 29.45
          },
          {
            'Symbol': 'YRCW',
            'Buy': 8.95,
            'Sell': 8.86
          },
          {
            'Symbol': 'YRIV',
            'Buy': 11.71,
            'Sell': 11.59
          },
          {
            'Symbol': 'YTEN',
            'Buy': 1.45,
            'Sell': 1.44
          },
          {
            'Symbol': 'YTRA',
            'Buy': 5.86,
            'Sell': 5.8
          },
          {
            'Symbol': 'YY',
            'Buy': 87.93,
            'Sell': 87.05
          },
          {
            'Symbol': 'Z',
            'Buy': 50.16,
            'Sell': 49.66
          },
          {
            'Symbol': 'ZAGG',
            'Buy': 15.35,
            'Sell': 15.2
          },
          {
            'Symbol': 'ZBIO',
            'Buy': 14.55,
            'Sell': 14.4
          },
          {
            'Symbol': 'ZBRA',
            'Buy': 161.19,
            'Sell': 159.58
          },
          {
            'Symbol': 'ZEAL',
            'Buy': 14.09,
            'Sell': 13.95
          },
          {
            'Symbol': 'ZEUS',
            'Buy': 23.41,
            'Sell': 23.18
          },
          {
            'Symbol': 'ZFGN',
            'Buy': 9.68,
            'Sell': 9.58
          },
          {
            'Symbol': 'ZG',
            'Buy': 50.76,
            'Sell': 50.25
          },
          {
            'Symbol': 'ZGNX',
            'Buy': 50,
            'Sell': 49.5
          },
          {
            'Symbol': 'ZION',
            'Buy': 52.58,
            'Sell': 52.05
          },
          {
            'Symbol': 'ZIONW',
            'Buy': 20.6,
            'Sell': 20.39
          },
          {
            'Symbol': 'ZIOP',
            'Buy': 2.75,
            'Sell': 2.72
          },
          {
            'Symbol': 'ZIV',
            'Buy': 78.19,
            'Sell': 77.41
          },
          {
            'Symbol': 'ZIXI',
            'Buy': 5.56,
            'Sell': 5.5
          },
          {
            'Symbol': 'ZKIN',
            'Buy': 4.14,
            'Sell': 4.1
          },
          {
            'Symbol': 'ZLAB',
            'Buy': 21.75,
            'Sell': 21.53
          },
          {
            'Symbol': 'ZN',
            'Buy': 2.78,
            'Sell': 2.75
          },
          {
            'Symbol': 'ZNGA',
            'Buy': 3.84,
            'Sell': 3.8
          },
          {
            'Symbol': 'ZS',
            'Buy': 38.21,
            'Sell': 37.83
          },
          {
            'Symbol': 'ZSAN',
            'Buy': 4.32,
            'Sell': 4.28
          },
          {
            'Symbol': 'ZUMZ',
            'Buy': 27,
            'Sell': 26.73
          },
          {
            'Symbol': 'ZYNE',
            'Buy': 6.43,
            'Sell': 6.37
          }
        ]
                ";
    }
}
