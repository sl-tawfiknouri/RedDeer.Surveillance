namespace DataImport.Services
{
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

    public class EnrichmentService : IEnrichmentService
    {
        private const int ScanFrequencyInSeconds = 60;

        private readonly IEnrichmentApi _api;

        private readonly IBrokerApi _brokerApi;

        private readonly ILogger<EnrichmentService> _logger;

        private readonly IReddeerMarketRepository _marketRepository;

        private readonly IOrderBrokerRepository _orderBrokerRepository;

        private Timer _timer;

        public EnrichmentService(
            IReddeerMarketRepository marketRepository,
            IOrderBrokerRepository orderBrokerRepository,
            IEnrichmentApi api,
            IBrokerApi brokerApi,
            ILogger<EnrichmentService> logger)
        {
            this._marketRepository = marketRepository ?? throw new ArgumentNullException(nameof(marketRepository));
            this._orderBrokerRepository =
                orderBrokerRepository ?? throw new ArgumentNullException(nameof(orderBrokerRepository));
            this._api = api ?? throw new ArgumentNullException(nameof(api));
            this._brokerApi = brokerApi ?? throw new ArgumentNullException(nameof(brokerApi));
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
                var message = new SecurityEnrichmentMessage { Securities = securities?.ToArray() };

                this._logger.LogInformation("We need to add enrichment for brokers");

                var enrichmentResponse = await this._api.PostAsync(message);
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