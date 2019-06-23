using GraphQL.Authorization;
using GraphQL.DataLoader;
using GraphQL.Types;
using Surveillance.Api.App.Authorization;
using Surveillance.Api.DataAccess.Abstractions.Entities;
using Surveillance.Api.DataAccess.Abstractions.Repositories;

namespace Surveillance.Api.App.Types.Engine
{
    public class SystemProcessOperationDistributeRuleGraphType : ObjectGraphType<ISystemProcessOperationDistributeRule>
    {
        public SystemProcessOperationDistributeRuleGraphType(
            ISystemProcessOperationRepository operationRepository,
            IDataLoaderContextAccessor dataLoaderAccessor)
        {
            this.AuthorizeWith(PolicyManifest.AdminPolicy);

            Field(i => i.Id).Description("Identifier for the system process operation distribute rule");
            Field<SystemProcessOperationGraphType>("processOperation", resolve: context =>
            {
                var loader =
                    dataLoaderAccessor.Context.GetOrAddLoader(
                        $"GetSystemProcessOperationById-{context.Source.Id}",
                        () => operationRepository.GetForId(context.Source.Id));

                return loader.LoadAsync();
            });

            Field(i => i.ScheduleRuleInitialStart).Type(new DateTimeGraphType()).Description("Scheduled rule start before distribution");
            Field(i => i.ScheduleRuleInitialEnd, nullable: true).Type(new DateTimeGraphType()).Description("Scheduled rule end before distribution");
            Field(i => i.RulesDistributed).Description("Rules distributed by the disassemble operation");
        }
    }
}
