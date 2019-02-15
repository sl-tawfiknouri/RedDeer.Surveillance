using System.Collections.Generic;
using System.Threading.Tasks;

namespace Surveillance.DataLayer.Aurora.Files.Interfaces
{
    public interface IFileUploadOrdersRepository
    {
        Task Create(IReadOnlyCollection<int> orderIds, int uploadId);
    }
}