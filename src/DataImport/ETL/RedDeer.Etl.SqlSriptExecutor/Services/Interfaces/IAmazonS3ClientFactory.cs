using Amazon.S3;

namespace RedDeer.Etl.SqlSriptExecutor.Services.Interfaces
{
    public interface IAmazonS3ClientFactory
    {
        IAmazonS3 Create();
    }
}
