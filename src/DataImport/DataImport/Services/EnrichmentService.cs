﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using DataImport.Services.Interfaces;
using Domain.Core.Financial.Assets;
using Domain.Core.Financial.Cfis.Interfaces;
using Firefly.Service.Data.TickPriceHistory.Shared.Protos;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.BrokerEnrichment;
using RedDeer.Contracts.SurveillanceService.Api.SecurityEnrichment;
using Surveillance.Data.Universe.Refinitiv.Interfaces;
using Surveillance.DataLayer.Aurora.Market.Interfaces;
using Surveillance.DataLayer.Aurora.Orders.Interfaces;
using Surveillance.Reddeer.ApiClient.Enrichment.Interfaces;
using Timer = System.Timers.Timer;

namespace DataImport.Services
{
    public class EnrichmentService : IEnrichmentService
    {
        private const int ScanFrequencyInSeconds = 60;
        private readonly IEnrichmentApi _api;
        private readonly IBrokerApi _brokerApi;
        private readonly ILogger<EnrichmentService> _logger;
        private readonly IReddeerMarketRepository _marketRepository;
        private readonly IOrderBrokerRepository _orderBrokerRepository;
        private readonly ITickPriceHistoryServiceClientFactory _tickPriceHistoryServiceClientFactory;
        private readonly ICfiInstrumentTypeMapper _cfiInstrumentTypeMapper;
        private Timer _timer;
        private readonly SemaphoreSlim _semaphoreSlimScanLock = new SemaphoreSlim(1, 1);

        public EnrichmentService(
            IReddeerMarketRepository marketRepository,
            IOrderBrokerRepository orderBrokerRepository,
            IEnrichmentApi api,
            IBrokerApi brokerApi,
            ITickPriceHistoryServiceClientFactory tickPriceHistoryServiceClientFactory,
            ICfiInstrumentTypeMapper cfiInstrumentTypeMapper,
            ILogger<EnrichmentService> logger)
        {
            this._marketRepository = marketRepository ?? throw new ArgumentNullException(nameof(marketRepository));
            this._orderBrokerRepository = orderBrokerRepository ?? throw new ArgumentNullException(nameof(orderBrokerRepository));
            this._api = api ?? throw new ArgumentNullException(nameof(api));
            this._brokerApi = brokerApi ?? throw new ArgumentNullException(nameof(brokerApi));
            this._tickPriceHistoryServiceClientFactory = tickPriceHistoryServiceClientFactory ?? throw new ArgumentNullException(nameof(tickPriceHistoryServiceClientFactory));
            this._cfiInstrumentTypeMapper = cfiInstrumentTypeMapper ?? throw new ArgumentNullException(nameof(cfiInstrumentTypeMapper));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Initialise()
        {
            var tokenSource = new CancellationTokenSource(60000);
            var heartBeating = await this._api.HeartBeatingAsync(tokenSource.Token);

            if (!heartBeating)
                this._logger.LogError(
                    "Enrichment Service could not reach the heartbeat service, beginning retry polling process");

            while (!heartBeating)
            {
                Thread.Sleep(15000);
                var loopTokenSource = new CancellationTokenSource(10000);
                heartBeating = await this._api.HeartBeatingAsync(loopTokenSource.Token);
            }

            var timer = new Timer(ScanFrequencyInSeconds * 1000)
            {
                AutoReset = true, Interval = ScanFrequencyInSeconds * 1000
            };

            timer.Elapsed += this.TimerOnElapsed;
            timer.Start();
            this._timer = timer;
        }

        public async Task<bool> Scan()
        {
            await _semaphoreSlimScanLock.WaitAsync();
            {
                try
                {
                    var securities = await this._marketRepository.GetUnEnrichedSecurities();
                    var brokers = await this._orderBrokerRepository.GetUnEnrichedBrokers();

                    var scanTokenSource = new CancellationTokenSource(10000);
                    var apiCheck = await this._api.HeartBeatingAsync(scanTokenSource.Token);
                    if (!apiCheck)
                    {
                        this._logger.LogError(
                            "Enrichment Service was about to enrich a scan but found the enrichment api to be unresponsive.");
                        return false;
                    }

                    var response = false;

                    if (securities != null && securities.Any())
                    {
                        var enrichmentResponse = await Enrich(securities);
                        await this._marketRepository.UpdateUnEnrichedSecurities(enrichmentResponse?.Securities);
                        response = enrichmentResponse?.Securities?.Any() ?? false;
                    }

                    if (brokers != null && brokers.Any())
                    {
                        var message = new BrokerEnrichmentMessage
                        {
                            Brokers = brokers?.Select(
                                _ => new BrokerEnrichmentDto
                                {
                                    CreatedOn = _.CreatedOn,
                                    ExternalId = _.ReddeerId,
                                    Id = _.Id,
                                    Live = _.Live,
                                    Name = _.Name
                                }).ToArray()
                        };

                        this._logger.LogInformation("We need to add enrichment for brokers");

                        var enrichmentResponse = await this._brokerApi.PostAsync(message);
                        await this._orderBrokerRepository.UpdateEnrichedBroker(enrichmentResponse.Brokers);
                        response = true;
                    }

                    return response;
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, $"Error while Scan {nameof(Scan)} ");
                    return false;
                }
                finally
                {
                    _semaphoreSlimScanLock.Release();
                }
            }
        }

        private async Task<SecurityEnrichmentMessage> Enrich(IReadOnlyCollection<SecurityEnrichmentDto> securities)
        {
            var bondsWithoutRrpsRic = securities.Where(w => _cfiInstrumentTypeMapper.MapCfi(w.Cfi) == InstrumentTypes.Bond && string.IsNullOrEmpty(w.Ric)).ToList();
            if (bondsWithoutRrpsRic.Any())
            {
                await EnrichBondWithRicFromTr(bondsWithoutRrpsRic);
            }

            var clientServiceMessage = new SecurityEnrichmentMessage { Securities = securities.ToArray() };
            var clientServiceResponse = await this._api.PostAsync(clientServiceMessage);

            return clientServiceResponse;
        }

        private async Task EnrichBondWithRicFromTr(IEnumerable<SecurityEnrichmentDto> bondsWithoutRrpsRic)
        {
            try
            {
                if (!bondsWithoutRrpsRic.Any()) return;

                foreach (var bond in bondsWithoutRrpsRic)
                {
                    var request = new GetEnrichmentIdentifierRequest
                    {
                        Identifiers = new SecurityIdentifiers {Isin = bond.Isin}
                    };

                    this._logger.LogInformation($"EnrichBondWithRicFromTr requesting Ric for Isin {bond.Isin}.");

                    var tickPriceHistoryServiceClient = _tickPriceHistoryServiceClientFactory.Create();
                    var result = await tickPriceHistoryServiceClient.GetEnrichmentIdentifierAsync(request);
                    bond.Ric = result.Identifiers.Select(s => s.Ric).FirstOrDefault();

                    this._logger.LogInformation($"EnrichBondWithRicFromTr found Ric {bond.Ric} for Isin {bond.Isin}.");
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error while enriching RIC from TR");
            }
        }

        public async Task Terminate()
        {
            this._timer?.Stop();
            await Task.CompletedTask;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var run = true;
                while (run)
                {
                    var task = this.Scan();
                    task.Wait();
                    run = task.Result;
                }
            }
            catch (Exception a)
            {
                this._logger.LogError(a, "Enrichment Service encountered an error on scan");
            }
        }
    }
}