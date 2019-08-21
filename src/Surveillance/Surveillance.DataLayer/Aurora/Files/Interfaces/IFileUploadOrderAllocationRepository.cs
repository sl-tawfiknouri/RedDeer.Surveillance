namespace Surveillance.DataLayer.Aurora.Files.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IFileUploadOrderAllocationRepository
    {
        Task Create(IReadOnlyCollection<string> orderAllocationIds, int uploadId);
    }
}