namespace Surveillance.DataLayer.Aurora.Files.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IFileUploadOrdersRepository
    {
        Task Create(IReadOnlyCollection<string> orderIds, int uploadId);
    }
}