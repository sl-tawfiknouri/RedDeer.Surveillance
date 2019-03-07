using System.Threading.Tasks;

namespace Infrastructure.Network.Aws_IO.Interfaces
{
    public interface IAwsS3Client
    {
        Task<bool> RetrieveFile(string bucketName, string key, string versionId, string targetFile, bool retry = true);
    }
}