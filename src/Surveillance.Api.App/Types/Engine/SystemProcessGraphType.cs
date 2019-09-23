namespace Surveillance.Api.App.Types.Engine
{
    using GraphQL.Authorization;
    using GraphQL.Types;

    using Surveillance.Api.App.Authorization;
    using Surveillance.Api.DataAccess.Abstractions.Entities;

    public class SystemProcessGraphType : ObjectGraphType<ISystemProcess>
    {
        public SystemProcessGraphType()
        {
            this.AuthorizeWith(PolicyManifest.AdminPolicy);

            this.Field(i => i.Id).Description("Identifier for the system process");
            this.Field(i => i.Heartbeat, true).Type(new DateTimeGraphType()).Description(
                "Heartbeat (UTC) in UK time format. The last time the process wrote to the database via polling. This is a substitute for process end time");
            this.Field(i => i.InstanceInitiated, true).Type(new DateTimeGraphType()).Description(
                "Instance of the process was initiated at (UTC) in UK time formats");
            this.Field(i => i.MachineId).Description("An identifier for the machine running the process");
            this.Field(i => i.ProcessId)
                .Description("The process id for the OS process running the surveillance process");
            this.Field(i => i.SystemProcessTypeId).Description("The system process type id (enum)");
        }
    }
}