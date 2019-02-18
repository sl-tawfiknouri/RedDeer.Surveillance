using System.Collections.Generic;
using System.Threading.Tasks;

namespace Surveillance.DataLayer.Aurora.Files.Interfaces
{
    public interface IFileUploadOrderAllocationRepository
    {
        Task Create(IReadOnlyCollection<string> orderAllocationIds, int uploadId);
    }
}