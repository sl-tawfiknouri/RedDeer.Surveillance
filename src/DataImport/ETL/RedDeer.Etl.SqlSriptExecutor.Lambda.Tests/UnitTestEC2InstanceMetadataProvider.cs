using RedDeer.Etl.SqlSriptExecutor.Services.Interfaces;

namespace RedDeer.Etl.SqlSriptExecutor.Lambda.Tests
{
    public class UnitTestEC2InstanceMetadataProvider
        : IEC2InstanceMetadataProvider
    {
        public string InstanceId() 
            => null;
    }
}
