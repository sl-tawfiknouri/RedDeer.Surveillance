using System.ComponentModel;

namespace Surveillance.Auditing.DataLayer.Processes
{
    public enum OperationState
    {
        [Description("In Process")]
        InProcess = 0,
        [Description("Failed")]
        Failed = 1,
        [Description("Complete")]
        Completed = 2,
        [Description("Complete With Errors")]
        CompletedWithErrors = 3,
        [Description("Block Client Service Down")]
        BlockedClientServiceDown = 4,
        [Description("Incomplete Missing Data")]
        IncompleteMissingData = 5
    }
}
