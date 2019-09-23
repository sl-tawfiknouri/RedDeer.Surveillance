namespace Infrastructure.Network.Aws.Interfaces
{
    using System.Threading.Tasks;

    public interface IAwsS3Client
    {
        Task<bool> RetrieveFile(string bucketName, string key, string versionId, string targetFile, bool retry = true);
    }
}