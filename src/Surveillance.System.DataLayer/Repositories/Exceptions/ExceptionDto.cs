namespace Surveillance.Auditing.DataLayer.Repositories.Exceptions
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
        public int? SystemProcessOperationUploadFileRuleId { get; set; }
        public int? SystemProcessOperationThirdPartyDataRequestId { get; set; }

        public override string ToString()
        {
            return $"Exception| ID {Id} | ExceptionMessage {ExceptionMessage} | InnerExceptionMessage {InnerExceptionMessage} | StackTrace {StackTrace} | SystemProcessId {SystemProcessId} | SystemProcessOperationId {SystemProcessOperationId} | SystemProcessOperationRuleRunId {SystemProcessOperationRuleRunId} | SystemProcessOperationDistributeRuleId {SystemProcessOperationDistributeRuleId} | SystemProcessOperationUploadFileRuleId {SystemProcessOperationUploadFileRuleId} | SystemProcessOperationThirdPartyDataRequestId {SystemProcessOperationThirdPartyDataRequestId}";
        }
    }
}