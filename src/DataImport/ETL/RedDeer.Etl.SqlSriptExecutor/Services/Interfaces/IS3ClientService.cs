using RedDeer.Etl.SqlSriptExecutor.Services.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RedDeer.Etl.SqlSriptExecutor.Services.Interfaces
{
    public interface IS3ClientService
    {
        Task<string> ReadAllText(string uri);

        Task<MemoryStream> GetObjectStream(string bucket, string key, string versionId = null);

        Task<bool> PutObjectStream(string bucket, string key, Stream stream);

        Task<List<S3ObjectModel>> ListObjectsAsync(string bucket, string prefix);

        Task<List<S3ObjectModel>> ListObjectsAsync(string uriString);
    }
}
