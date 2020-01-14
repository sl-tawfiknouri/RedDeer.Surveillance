using System;

namespace RedDeer.Etl.SqlSriptExecutor.Services.Models
{
    public class S3ObjectModel
    {
        public string BucketName { get; set; }

        public string Key { get; set; }
        
        public DateTime LastModified { get; set; }
        
        public long Size { get; set; }
    }
}
