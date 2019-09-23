namespace DataImport.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface IEnrichmentService
    {
        Task Initialise();

        Task<bool> Scan();

        Task Terminate();
    }
}