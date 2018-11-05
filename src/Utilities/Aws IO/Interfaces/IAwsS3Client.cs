using System.Threading.Tasks;

namespace Utilities.Aws_IO.Interfaces
{
    public interface IAwsS3Client
    {
        Task<bool> RetrieveFile(string bucketName, string key, string targetFile, bool retry = true);
    }
}