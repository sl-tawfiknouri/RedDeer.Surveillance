namespace RedDeer.Etl.SqlSriptExecutor.Services.Models
{
    public class SqlSriptData
    {
        public string Database { get; set; }
        public string SqlScriptS3Location { get; set; }
        public string CsvOutputLocation { get; set; }
    }
}
