using GraphQL.Types;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.App.Types.Engine
{
    public class SystemProcessGraphType : ObjectGraphType<ISystemProcess>
    {
        public SystemProcessGraphType()
        {
            Field(i => i.Id).Description("Identifier for the system process");
            Field(i => i.Heartbeats).Description("Heartbeat (UTC) in UK time format. The last time the process wrote to the database via polling. This is a substitute for process end time");
            Field(i => i.Initiated).Description("Instance of the process was initiated at (UTC) in UK time formats");
            Field(i => i.MachineId).Description("An identifier for the machine running the process");
            Field(i => i.ProcessId).Description("The process id for the OS process running the surveillance process");
            Field(i => i.SystemProcessTypeId).Description("The system process type id (enum)");
        }
    }
}
