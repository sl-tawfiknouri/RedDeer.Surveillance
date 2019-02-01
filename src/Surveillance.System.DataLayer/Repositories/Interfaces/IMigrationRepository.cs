using System.Threading.Tasks;

namespace Surveillance.Systems.DataLayer.Repositories.Interfaces
{
    public interface IMigrationRepository
    {
        int LatestMigrationAvailable();
        Task<int> LatestMigrationVersion();
        Task UpdateMigrations();
    }
}