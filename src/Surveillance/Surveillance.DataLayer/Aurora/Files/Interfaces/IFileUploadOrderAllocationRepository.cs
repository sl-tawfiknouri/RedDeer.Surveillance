using System.Collections.Generic;
using System.Threading.Tasks;

namespace Surveillance.DataLayer.Aurora.Files.Interfaces
{
    public interface IFileUploadOrderAllocationRepository
    {
        Task Create(IReadOnlyCollection<int> orderAllocationIds, int uploadId);
    }
}