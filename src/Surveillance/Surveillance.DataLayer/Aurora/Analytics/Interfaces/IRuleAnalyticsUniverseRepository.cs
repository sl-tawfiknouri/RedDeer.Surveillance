using System.Threading.Tasks;

namespace Surveillance.DataLayer.Aurora.Analytics.Interfaces
{
    public interface IRuleAnalyticsUniverseRepository
    {
        Task Create(UniverseAnalytics analytics);
    }
}