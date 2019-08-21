namespace Surveillance.Auditing.DataLayer.Repositories.Interfaces
{
    using System.Threading.Tasks;

    public interface IMigrationRepository
    {
        int LatestMigrationAvailable();

        Task<int> LatestMigrationVersion();

        Task UpdateMigrations();
    }
}