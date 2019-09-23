namespace Surveillance.DataLayer.Aurora.Analytics.Interfaces
{
    using System.Threading.Tasks;

    public interface IRuleAnalyticsAlertsRepository
    {
        Task Create(AlertAnalytics analytics);
    }
}