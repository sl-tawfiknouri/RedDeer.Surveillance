using RedDeer.Etl.SqlSriptExecutor.Services.Interfaces;

namespace RedDeer.Etl.SqlSriptExecutor.Services
{
    public class EC2InstanceMetadataProvider
        : IEC2InstanceMetadataProvider
    {
        public string InstanceId() 
            => Amazon.Util.EC2InstanceMetadata.InstanceId;
    }
}
