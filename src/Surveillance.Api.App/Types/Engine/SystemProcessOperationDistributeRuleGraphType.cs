namespace Surveillance.Api.App.Types.Engine
{
    using GraphQL.Authorization;
    using GraphQL.DataLoader;
    using GraphQL.Types;

    using Surveillance.Api.App.Authorization;
    using Surveillance.Api.DataAccess.Abstractions.Entities;
    using Surveillance.Api.DataAccess.Abstractions.Repositories;

    public class SystemProcessOperationDistributeRuleGraphType : ObjectGraphType<ISystemProcessOperationDistributeRule>
    {
        public SystemProcessOperationDistributeRuleGraphType(
            ISystemProcessOperationRepository operationRepository,
            IDataLoaderContextAccessor dataLoaderAccessor)
        {
            this.AuthorizeWith(PolicyManifest.AdminPolicy);

            this.Field(i => i.Id).Description("Identifier for the system process operation distribute rule");
            this.Field<SystemProcessOperationGraphType>(
                "processOperation",
                resolve: context =>
                    {
                        var loader = dataLoaderAccessor.Context.GetOrAddLoader(
                            $"GetSystemProcessOperationById-{context.Source.Id}",
                            () => operationRepository.GetForId(context.Source.Id));

                        return loader.LoadAsync();
                    });

            this.Field(i => i.ScheduleRuleInitialStart).Type(new DateTimeGraphType())
                .Description("Scheduled rule start before distribution");
            this.Field(i => i.ScheduleRuleInitialEnd, true).Type(new DateTimeGraphType())
                .Description("Scheduled rule end before distribution");
            this.Field(i => i.RulesDistributed).Description("Rules distributed by the disassemble operation");
        }
    }
}