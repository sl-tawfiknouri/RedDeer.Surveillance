using System.Threading.Tasks;

namespace DataImport.Services.Interfaces
{
    public interface IEnrichmentService
    {
        Task Initialise();
        Task Terminate();
        Task<bool> Scan();
    }
}
