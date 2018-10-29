using System.Threading.Tasks;

namespace Surveillance.Services.Interfaces
{
    public interface IEnrichmentService
    {
        Task Initialise();
        Task Terminate();
    }
}