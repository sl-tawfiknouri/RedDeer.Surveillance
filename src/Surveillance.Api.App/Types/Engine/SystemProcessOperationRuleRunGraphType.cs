namespace Surveillance.Api.App.Types.Engine
{
    using GraphQL.Authorization;
    using GraphQL.DataLoader;
    using GraphQL.Types;

    using Surveillance.Api.App.Authorization;
    using Surveillance.Api.DataAccess.Abstractions.Entities;
    using Surveillance.Api.DataAccess.Abstractions.Repositories;

    public class SystemProcessOperationRuleRunGraphType : ObjectGraphType<ISystemProcessOperationRuleRun>
    {
        public SystemProcessOperationRuleRunGraphType(
            ISystemProcessOperationRepository operationRepository,
            IDataLoaderContextAccessor dataLoaderAccessor)
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);

            this.Field(i => i.Id).Description("Identifier for the system process operation rule run");
            this.Field(i => i.CorrelationId, true).Description("Correlation id for the system process operation rule run");
            this.Field(i => i.IsBackTest, true).Description("Back test flag for the rule run");
            this.Field(i => i.IsForceRun, true).Description("Force run flag for the rule run");
            this.Field(i => i.RuleDescription).Description("Rule description for the rule run");
            this.Field(i => i.RuleParameterId, true).Description("Rule id");
            this.Field(i => i.RuleTypeId, true).Description("Rule category id");
            this.Field(i => i.RuleVersion).Description("The version of the rule ran");
            this.Field(i => i.ScheduleRuleStart).Type(new DateTimeGraphType()).Description(
                "The start date for the rule run. The actual data for the rule run is pushed out by the rule time window");
            this.Field(i => i.ScheduleRuleEnd).Type(new DateTimeGraphType()).Description(
                "The end date for the rule run. This is the date in the data, not in the real world");

            this.Field<SystemProcessOperationGraphType>(
                "processOperation",
                resolve: context =>
                    {
                        var loader = dataLoaderAccessor.Context.GetOrAddLoader(
                            $"GetSystemProcessOperationById-{context.Source.SystemProcessOperationId}",
                            () => operationRepository.GetForId(context.Source.SystemProcessOperationId));

                        return loader.LoadAsync();
                    });
        }
    }
}