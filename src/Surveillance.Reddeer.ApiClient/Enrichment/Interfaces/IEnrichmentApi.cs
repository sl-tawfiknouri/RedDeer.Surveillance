namespace Surveillance.Reddeer.ApiClient.Enrichment.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    using RedDeer.Contracts.SurveillanceService.Api.SecurityEnrichment;

    public interface IEnrichmentApi
    {
        Task<bool> HeartBeating(CancellationToken token);

        Task<SecurityEnrichmentMessage> Post(SecurityEnrichmentMessage message);
    }
}