namespace Surveillance.Auditing.DataLayer.Repositories.Exceptions
{
    public class ExceptionDto
    {
        public string ExceptionMessage { get; set; }

        public int Id { get; set; }

        public string InnerExceptionMessage { get; set; }

        public string StackTrace { get; set; }

        public string SystemProcessId { get; set; }

        public int? SystemProcessOperationDistributeRuleId { get; set; }

        public int? SystemProcessOperationId { get; set; }

        public int? SystemProcessOperationRuleRunId { get; set; }

        public int? SystemProcessOperationThirdPartyDataRequestId { get; set; }

        public int? SystemProcessOperationUploadFileRuleId { get; set; }

        public override string ToString()
        {
            return
                $"Exception| ID {this.Id} | ExceptionMessage {this.ExceptionMessage} | InnerExceptionMessage {this.InnerExceptionMessage} | StackTrace {this.StackTrace} | SystemProcessId {this.SystemProcessId} | SystemProcessOperationId {this.SystemProcessOperationId} | SystemProcessOperationRuleRunId {this.SystemProcessOperationRuleRunId} | SystemProcessOperationDistributeRuleId {this.SystemProcessOperationDistributeRuleId} | SystemProcessOperationUploadFileRuleId {this.SystemProcessOperationUploadFileRuleId} | SystemProcessOperationThirdPartyDataRequestId {this.SystemProcessOperationThirdPartyDataRequestId}";
        }
    }
}