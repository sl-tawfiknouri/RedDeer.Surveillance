namespace RedDeer.Etl.SqlSriptExecutor.Services.Models
{
    public class FunctionRequest
    {
        public FilePreProcessorData FilePreProcessorData { get; set; }

        public SqlSriptData[] Scripts { get; set; }
    }
}
