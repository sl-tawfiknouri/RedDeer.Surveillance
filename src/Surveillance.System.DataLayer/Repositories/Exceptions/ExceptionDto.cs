namespace Surveillance.System.DataLayer.Repositories.Exceptions
{
    public class ExceptionDto
    {
        public int Id { get; set; }
        public string ExceptionMessage { get; set; }
        public string InnerExceptionMessage { get; set; }
        public string StackTrace { get; set; }
        public string SystemProcessId { get; set; }
        public int? SystemProcessOperationId { get; set; }
        public int? SystemProcessOperationRuleRunId { get; set; }
        public int? SystemProcessOperationDistributeRuleId { get; set; }
    }
}