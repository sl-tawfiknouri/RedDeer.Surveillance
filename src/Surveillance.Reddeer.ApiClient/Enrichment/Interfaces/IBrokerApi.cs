namespace Surveillance.Reddeer.ApiClient.Enrichment.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    using RedDeer.Contracts.SurveillanceService.Api.BrokerEnrichment;

    public interface IBrokerApi
    {
        Task<bool> HeartBeating(CancellationToken token);

        Task<BrokerEnrichmentMessage> Post(BrokerEnrichmentMessage message);
    }
}