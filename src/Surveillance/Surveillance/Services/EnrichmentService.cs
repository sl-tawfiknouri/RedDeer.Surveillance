using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.SecurityEnrichment;
using Surveillance.DataLayer.Api.Enrichment.Interfaces;
using Surveillance.DataLayer.Aurora.Market.Interfaces;
using Surveillance.Services.Interfaces;
using Timer = System.Timers.Timer;

namespace Surveillance.Services
{
    public class EnrichmentService : IEnrichmentService
    {
        private const int ScanFrequencyInSeconds = 60;

        private readonly IReddeerMarketRepository _marketRepository;
        private readonly IEnrichmentApiRepository _apiRepository;
        private readonly ILogger<EnrichmentService> _logger;

        private Timer _timer;

        public EnrichmentService(
            IReddeerMarketRepository marketRepository,
            IEnrichmentApiRepository apiRepository,
            ILogger<EnrichmentService> logger)
        {
            _marketRepository = marketRepository ?? throw new ArgumentNullException(nameof(marketRepository));
            _apiRepository = apiRepository ?? throw new ArgumentNullException(nameof(apiRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Initialise()
        {
            var tokenSource = new CancellationTokenSource(60000);
            var heartBeating = await _apiRepository.HeartBeating(tokenSource.Token);

            if (!heartBeating)
            {
                _logger.LogError("Enrichment Service could not reach the heartbeat service, beginning retry polling process");
            }

            while (!heartBeating)
            {
                Thread.Sleep(15000);
                var loopTokenSource = new CancellationTokenSource(10000);
                heartBeating = await _apiRepository.HeartBeating(loopTokenSource.Token);
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

        private async Task<bool> Scan()
        {
            var securities = await _marketRepository.GetUnEnrichedSecurities();

            if (securities == null
                || !securities.Any())
            {
                return false;
            }

            var scanTokenSource = new CancellationTokenSource(10000);
            var apiCheck = await _apiRepository.HeartBeating(scanTokenSource.Token);
            if (!apiCheck)
            {
                _logger.LogError("Enrichment Service was about to enrich a scan but found the enrichment api to be unresponsive.");
                return false;
            }

            var message = new SecurityEnrichmentMessage
            {
                Securities = securities?.ToArray()
            };

            var enrichmentResponse = await _apiRepository.Get(message);
            await _marketRepository.UpdateUnEnrichedSecurities(enrichmentResponse?.Securities);

            return enrichmentResponse?.Securities?.Any() ?? false;
        }
    }
}
