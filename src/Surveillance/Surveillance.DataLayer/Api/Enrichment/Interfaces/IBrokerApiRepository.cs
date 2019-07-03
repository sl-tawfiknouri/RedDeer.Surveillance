using System.Threading;
using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService.Api.BrokerEnrichment;

namespace Surveillance.DataLayer.Api.Enrichment
{
    public interface IBrokerApiRepository
    {
        Task<BrokerEnrichmentMessage> Get(BrokerEnrichmentMessage message);
        Task<bool> HeartBeating(CancellationToken token);
    }
}