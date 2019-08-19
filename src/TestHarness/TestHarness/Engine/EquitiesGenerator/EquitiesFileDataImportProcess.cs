namespace TestHarness.Engine.EquitiesGenerator
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using CsvHelper;

    using Domain.Core.Markets;
    using Domain.Core.Markets.Collections;
    using Domain.Core.Markets.Timebars;
    using Domain.Surveillance.Streams.Interfaces;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Files.Security;
    using SharedKernel.Files.Security.Interfaces;

    using TestHarness.Engine.EquitiesGenerator.Interfaces;

    public class EquitiesFileDataImportProcess : IEquityDataGenerator
    {
        private readonly string _filePath;

        private readonly ILogger _logger;

        private readonly ISecurityCsvToDtoMapper _securityMapper;

        public EquitiesFileDataImportProcess(string filePath, ILogger logger, ISecurityCsvToDtoMapper securityMapper)
        {
            this._filePath = filePath ?? string.Empty;
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._securityMapper = securityMapper ?? throw new ArgumentNullException(nameof(securityMapper));
        }

        public void InitiateWalk(IStockExchangeStream stream)
        {
            if (string.IsNullOrWhiteSpace(this._filePath))
            {
                this._logger.LogError(
                    "Equities File Data Import Process did not find file because the path was empty or null");
                return;
            }

            if (!File.Exists(this._filePath))
            {
                this._logger.LogError($"Equities File Data Import Process did not find file {this._filePath}");
                return;
            }

            var securities = new List<EquityInstrumentIntraDayTimeBar>();

            using (var reader = File.OpenText(this._filePath))
            {
                var csv = new CsvReader(reader);
                csv.Configuration.HasHeaderRecord = true;

                var csvRecords = csv.GetRecords<FinancialInstrumentTimeBarCsv>().ToList();

                foreach (var record in csvRecords)
                {
                    var mappedRecord = this._securityMapper.Map(record);
                    if (mappedRecord != null) securities.Add(mappedRecord);
                }
            }

            if (this._securityMapper.FailedParseTotal > 0)
                this._logger.LogError(
                    $"EquitiesFileDataImportProcess had {this._securityMapper.FailedParseTotal} errors parsing the input CSV file {this._filePath}");

            if (!securities.Any()) return;

            var frames = this.ExtractDateBasedExchangeFramesOverMarkets(securities);

            foreach (var frame in frames) stream.Add(frame);
        }

        public void TerminateWalk()
        {
            // no op
        }

        private IList<EquityIntraDayTimeBarCollection> ExtractDateBasedExchangeFramesOverMarkets(
            IReadOnlyCollection<EquityInstrumentIntraDayTimeBar> securities)
        {
            if (securities == null || !securities.Any()) return new List<EquityIntraDayTimeBarCollection>();

            return securities.GroupBy(sec => sec.Market.Id)
                .Select(groupedExchange => groupedExchange.GroupBy(gb => gb.TimeStamp)).SelectMany(
                    io => io.Select(
                        iio => new EquityIntraDayTimeBarCollection(
                            new Market(
                                null,
                                iio.FirstOrDefault()?.Market.Id,
                                iio.FirstOrDefault()?.Market.Name,
                                MarketTypes.STOCKEXCHANGE),
                            iio.Key,
                            iio.ToList()))).OrderBy(io => io.Epoch).ToList();
        }
    }
}