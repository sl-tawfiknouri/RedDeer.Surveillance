using System.Threading;
using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService.Api.SecurityEnrichment;

namespace Surveillance.DataLayer.Api.Enrichment.Interfaces
{
    public interface IEnrichmentApiRepository
    {
        Task<SecurityEnrichmentMessage> Get(SecurityEnrichmentMessage message);
        Task<bool> HeartBeating(CancellationToken token);
    }
}