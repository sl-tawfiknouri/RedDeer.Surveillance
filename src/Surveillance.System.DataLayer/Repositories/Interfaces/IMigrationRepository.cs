using System.Threading.Tasks;

namespace Surveillance.Auditing.DataLayer.Repositories.Interfaces
{
    public interface IMigrationRepository
    {
        int LatestMigrationAvailable();
        Task<int> LatestMigrationVersion();
        Task UpdateMigrations();
    }
}