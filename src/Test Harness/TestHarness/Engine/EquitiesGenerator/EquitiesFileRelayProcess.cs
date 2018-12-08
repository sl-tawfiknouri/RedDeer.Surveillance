﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using DomainV2.Equity.Frames;
using DomainV2.Equity.Frames.Interfaces;
using DomainV2.Equity.Streams.Interfaces;
using DomainV2.Financial;
using Microsoft.Extensions.Logging;
using TestHarness.Engine.EquitiesGenerator.Interfaces;

namespace TestHarness.Engine.EquitiesGenerator
{
    public class EquitiesFileRelayProcess : IEquityDataGenerator
    {
        private readonly string _filePath;
        private readonly ILogger<EquitiesFileRelayProcess> _logger;
        private readonly ISecurityCsvToDtoMapper _securityMapper;

        public EquitiesFileRelayProcess(
            string filePath,
            ILogger<EquitiesFileRelayProcess> logger,
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
                _logger.LogError("Equities File Relay Process did not find file because the path was empty or null");
                return;
            }

            if (!File.Exists(_filePath))
            {
                _logger.LogError($"Equities File Relay Process did not find file {_filePath}");
                return;
            }

            var securities = new List<SecurityTick>();


            using (var reader = File.OpenText(_filePath))
            {
                var csv = new CsvReader(reader);
                csv.Configuration.HasHeaderRecord = true;

                var csvRecords = csv.GetRecords<SecurityTickCsv>().ToList();

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
                _logger.LogError($"EquitiesFileRelayProcess had {_securityMapper.FailedParseTotal} errors parsing the input CSV file {_filePath}");
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

        private IList<ExchangeFrame> ExtractDateBasedExchangeFramesOverMarkets(IReadOnlyCollection<SecurityTick> securities)
        {
            if (securities == null
                || !securities.Any())
            {
                return new List<ExchangeFrame>();
            }

            return securities
                .GroupBy(sec => sec.Market.Id)
                .Select(groupedExchange => groupedExchange.GroupBy(gb => gb.TimeStamp))
                .SelectMany(io =>
                    io.Select(iio =>
                        new ExchangeFrame(
                            new Market(
                                null,
                                iio.FirstOrDefault()?.Market.Id,
                                iio.FirstOrDefault()?.Market.Name,
                                MarketTypes.STOCKEXCHANGE),
                        iio.Key,
                        iio.ToList())))
                .OrderBy(io => io.TimeStamp)
                .ToList();
        }

        public void TerminateWalk()
        {
            // no op
        }
    }
}
