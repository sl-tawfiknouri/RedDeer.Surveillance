using System.Threading.Tasks;

namespace Surveillance.DataLayer.Aurora.Analytics
{
    public interface IRuleAnalyticsAlertsRepository
    {
        Task Create(AlertAnalytics analytics);
    }
}