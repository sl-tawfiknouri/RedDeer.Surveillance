using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using DataImport.Services.Interfaces;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.BrokerEnrichment;
using RedDeer.Contracts.SurveillanceService.Api.SecurityEnrichment;
using Surveillance.DataLayer.Aurora.Market.Interfaces;
using Surveillance.DataLayer.Aurora.Orders.Interfaces;
using Surveillance.Reddeer.ApiClient.Enrichment.Interfaces;
using Timer = System.Timers.Timer;

namespace DataImport.Services
{
    public class EnrichmentService : IEnrichmentService
    {
        private const int ScanFrequencyInSeconds = 60;

        private readonly IReddeerMarketRepository _marketRepository;
        private readonly IOrderBrokerRepository _orderBrokerRepository;
        private readonly IEnrichmentApi _api;
        private readonly IBrokerApi _brokerApi;
        private readonly ILogger<EnrichmentService> _logger;

        private Timer _timer;

        public EnrichmentService(
            IReddeerMarketRepository marketRepository,
            IOrderBrokerRepository orderBrokerRepository,
            IEnrichmentApi api,
            IBrokerApi brokerApi,
            ILogger<EnrichmentService> logger)
        {
            _marketRepository = marketRepository ?? throw new ArgumentNullException(nameof(marketRepository));
            _orderBrokerRepository = orderBrokerRepository ?? throw new ArgumentNullException(nameof(orderBrokerRepository));
            _api = api ?? throw new ArgumentNullException(nameof(api));
            _brokerApi = brokerApi ?? throw new ArgumentNullException(nameof(brokerApi));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Initialise()
        {
            var tokenSource = new CancellationTokenSource(60000);
            var heartBeating = await _api.HeartBeating(tokenSource.Token);

            if (!heartBeating)
            {
                _logger.LogError("Enrichment Service could not reach the heartbeat service, beginning retry polling process");
            }

            while (!heartBeating)
            {
                Thread.Sleep(15000);
                var loopTokenSource = new CancellationTokenSource(10000);
                heartBeating = await _api.HeartBeating(loopTokenSource.Token);
            }

            var timer = new Timer(ScanFrequencyInSeconds * 1000)
            {
                AutoReset = true,
                Interval = ScanFrequencyInSeconds * 1000
            };

            timer.Elapsed += TimerOnElapsed;
            timer.Start();
            _timer = timer;
        }

        public async Task Terminate()
        {
            _timer?.Stop();
            await Task.CompletedTask;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var run = true;
                while (run)
                {
                    var task = Scan();
                    task.Wait();
                    run = task.Result;
                }
            }
            catch (Exception a)
            {
                _logger.LogError(a, "Enrichment Service encountered an error on scan");
            }
        }

        public async Task<bool> Scan()
        {
            var securities = await _marketRepository.GetUnEnrichedSecurities();
            var brokers = await _orderBrokerRepository.GetUnEnrichedBrokers();

            var scanTokenSource = new CancellationTokenSource(10000);
            var apiCheck = await _api.HeartBeating(scanTokenSource.Token);
            if (!apiCheck)
            {
                _logger.LogError("Enrichment Service was about to enrich a scan but found the enrichment api to be unresponsive.");
                return false;
            }

            var response = false;

            if ((securities != null
                 && securities.Any()))
            {
                var message = new SecurityEnrichmentMessage
                {
                    Securities = securities?.ToArray()
                };

                _logger.LogInformation($"We need to add enrichment for brokers");

                var enrichmentResponse = await _api.Post(message);
                await _marketRepository.UpdateUnEnrichedSecurities(enrichmentResponse?.Securities);

                response = enrichmentResponse?.Securities?.Any() ?? false;
            }

            if ((brokers != null
                 && brokers.Any()))
            {
                var message = new BrokerEnrichmentMessage
                {
                    Brokers = brokers?.Select(_ => new BrokerEnrichmentDto()
                    {
                        CreatedOn = _.CreatedOn,
                        ExternalId = _.ReddeerId,
                        Id = _.Id,
                        Live = _.Live,
                        Name = _.Name
                    }).ToArray()
                };

                _logger.LogInformation($"We need to add enrichment for brokers");

                var enrichmentResponse = await _brokerApi.Post(message);
                await _orderBrokerRepository.UpdateEnrichedBroker(enrichmentResponse.Brokers);
                response = true;
            }

            return response;
        }
    }
}
