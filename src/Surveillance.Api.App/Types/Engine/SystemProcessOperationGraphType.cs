namespace Surveillance.Api.App.Types.Engine
{
    using GraphQL.Authorization;
    using GraphQL.DataLoader;
    using GraphQL.Types;

    using Surveillance.Api.App.Authorization;
    using Surveillance.Api.DataAccess.Abstractions.Entities;
    using Surveillance.Api.DataAccess.Abstractions.Repositories;

    public class SystemProcessOperationGraphType : ObjectGraphType<ISystemProcessOperation>
    {
        public SystemProcessOperationGraphType(
            ISystemProcessRepository operationRepository,
            IDataLoaderContextAccessor dataLoaderAccessor)
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);

            this.Field(i => i.Id).Description("Identifier for the system process operation");
            this.Field(i => i.SystemProcessId).Description("Identifier for the system process");

            this.Field<SystemProcessGraphType>(
                "process",
                resolve: context =>
                    {
                        var loader = dataLoaderAccessor.Context.GetOrAddLoader(
                            $"GetSystemProcessById-{context.Source.SystemProcessId}",
                            () => operationRepository.GetForId(context.Source.SystemProcessId));

                        return loader.LoadAsync();
                    });

            this.Field(i => i.OperationState).Description("The last state of the operation");
            this.Field(i => i.OperationStart).Type(new DateTimeGraphType())
                .Description("The time the operation started at in the real world (UTC)");
            this.Field(i => i.OperationEnd, true).Type(new DateTimeGraphType())
                .Description("The time the operation ended at in the real world (UTC)");
        }
    }
}