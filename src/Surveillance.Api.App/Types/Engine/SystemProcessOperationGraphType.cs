using GraphQL.Authorization;
using GraphQL.DataLoader;
using GraphQL.Types;
using Surveillance.Api.App.Authorization;
using Surveillance.Api.DataAccess.Abstractions.Entities;
using Surveillance.Api.DataAccess.Abstractions.Repositories;

namespace Surveillance.Api.App.Types.Engine
{
    public class SystemProcessOperationGraphType : ObjectGraphType<ISystemProcessOperation>
    {
        public SystemProcessOperationGraphType(
            ISystemProcessRepository operationRepository,
            IDataLoaderContextAccessor dataLoaderAccessor)
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);

            Field(i => i.Id).Description("Identifier for the system process operation");
            Field(i => i.SystemProcessId).Description("Identifier for the system process");

            Field<SystemProcessGraphType>("process", resolve: context =>
            {
                var loader =
                    dataLoaderAccessor.Context.GetOrAddLoader<ISystemProcess>(
                        $"GetSystemProcessById-{context.Source.SystemProcessId}",
                        () => operationRepository.GetForId(context.Source.SystemProcessId));

                return loader.LoadAsync();
            });

            Field(i => i.OperationState).Description("The last state of the operation");
            Field(i => i.OperationStart).Type(new DateTimeGraphType()).Description("The time the operation started at in the real world (UTC)");
            Field(i => i.OperationEnd, nullable: true).Type(new DateTimeGraphType()).Description("The time the operation ended at in the real world (UTC)");
        }
    }
}
