using System.Threading.Tasks;

namespace Surveillance.DataLayer.Aurora.Analytics.Interfaces
{
    public interface IRuleAnalyticsAlertsRepository
    {
        Task Create(AlertAnalytics analytics);
    }
}