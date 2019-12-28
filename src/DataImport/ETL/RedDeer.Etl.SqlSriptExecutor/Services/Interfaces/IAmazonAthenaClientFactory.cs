using Amazon.Athena;

namespace RedDeer.Etl.SqlSriptExecutor.Services.Interfaces
{
    public interface IAmazonAthenaClientFactory
    {
        IAmazonAthena Create();
    }
}
