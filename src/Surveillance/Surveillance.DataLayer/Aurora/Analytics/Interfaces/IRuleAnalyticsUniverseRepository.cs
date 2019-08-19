namespace Surveillance.DataLayer.Aurora.Analytics.Interfaces
{
    using System.Threading.Tasks;

    public interface IRuleAnalyticsUniverseRepository
    {
        Task Create(UniverseAnalytics analytics);
    }
}