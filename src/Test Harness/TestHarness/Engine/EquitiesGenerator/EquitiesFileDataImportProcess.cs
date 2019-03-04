using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using Domain.Equity.Streams.Interfaces;
using Domain.Equity.TimeBars;
using Domain.Equity.TimeBars.Interfaces;
using Microsoft.Extensions.Logging;
using TestHarness.Engine.EquitiesGenerator.Interfaces;

namespace TestHarness.Engine.EquitiesGenerator
{
    public class EquitiesFileDataImportProcess : IEquityDataGenerator
    {
        private readonly string _filePath;
        private readonly ILogger _logger;
        private readonly ISecurityCsvToDtoMapper _securityMapper;

        public EquitiesFileDataImportProcess(
            string filePath,
            ILogger logger,
            ISecurityCsvToDtoMapper securityMapper)
        {
            _filePath = filePath ?? string.Empty;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _securityMapper = securityMapper ?? throw new ArgumentNullException(nameof(securityMapper));
        }

        public void InitiateWalk(IStockExchangeStream stream)
        {
            if (string.IsNullOrWhiteSpace(_filePath))
            {
                _logger.LogError("Equities File Data Import Process did not find file because the path was empty or null");
                return;
            }

            if (!File.Exists(_filePath))
            {
                _logger.LogError($"Equities File Data Import Process did not find file {_filePath}");
                return;
            }

            var securities = new List<EquityInstrumentIntraDayTimeBar>();


            using (var reader = File.OpenText(_filePath))
            {
                var csv = new CsvReader(reader);
                csv.Configuration.HasHeaderRecord = true;

                var csvRecords = csv.GetRecords<FinancialInstrumentTimeBarCsv>().ToList();

                foreach (var record in csvRecords)
                {
                    var mappedRecord = _securityMapper.Map(record);
                    if (mappedRecord != null)
                    {
                        securities.Add(mappedRecord);
                    }
                }
            }

            if (_securityMapper.FailedParseTotal > 0)
            {
                _logger.LogError($"EquitiesFileDataImportProcess had {_securityMapper.FailedParseTotal} errors parsing the input CSV file {_filePath}");
            }

            if (!securities.Any())
            {
                return;
            }

            var frames = ExtractDateBasedExchangeFramesOverMarkets(securities);

            foreach (var frame in frames)
            {
                stream.Add(frame);
            }
        }

        private IList<EquityIntraDayTimeBarCollection> ExtractDateBasedExchangeFramesOverMarkets(IReadOnlyCollection<EquityInstrumentIntraDayTimeBar> securities)
        {
            if (securities == null
                || !securities.Any())
            {
                return new List<EquityIntraDayTimeBarCollection>();
            }

            return securities
                .GroupBy(sec => sec.Market.Id)
                .Select(groupedExchange => groupedExchange.GroupBy(gb => gb.TimeStamp))
                .SelectMany(io =>
                    io.Select(iio =>
                        new EquityIntraDayTimeBarCollection(
                            new Market(
                                null,
                                iio.FirstOrDefault()?.Market.Id,
                                iio.FirstOrDefault()?.Market.Name,
                                MarketTypes.STOCKEXCHANGE),
                        iio.Key,
                        iio.ToList())))
                .OrderBy(io => io.Epoch)
                .ToList();
        }

        public void TerminateWalk()
        {
            // no op
        }
    }
}
