﻿using GraphQL.Authorization;
using GraphQL.DataLoader;
using GraphQL.Types;
using Surveillance.Api.App.Authorization;
using Surveillance.Api.DataAccess.Abstractions.Entities;
using Surveillance.Api.DataAccess.Abstractions.Repositories;

namespace Surveillance.Api.App.Types.Engine
{
    public class SystemProcessOperationRuleRunGraphType : ObjectGraphType<ISystemProcessOperationRuleRun>
    {
        public SystemProcessOperationRuleRunGraphType(ISystemProcessOperationRepository operationRepository, IDataLoaderContextAccessor dataLoaderAccessor)
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);

            Field(i => i.Id).Description("Identifier for the system process operation rule run");
            Field(i => i.CorrelationId).Description("Correlation id for the system process operation rule run");
            Field(i => i.IsBackTest).Description("Back test flag for the rule run");
            Field(i => i.IsForceRun).Description("Force run flag for the rule run");
            Field(i => i.RuleDescription).Description("Rule description for the rule run");
            Field(i => i.RuleParameterId).Description("Rule id");
            Field(i => i.RuleTypeId).Description("Rule category id");
            Field(i => i.RuleVersion).Description("The version of the rule ran");
            Field(i => i.ScheduleRuleStart).Type(new DateTimeGraphType()).Description("The start date for the rule run. The actual data for the rule run is pushed out by the rule time window");
            Field(i => i.ScheduleRuleEnd).Type(new DateTimeGraphType()).Description("The end date for the rule run. This is the date in the data, not in the real world");

            Field<SystemProcessOperationGraphType>("processOperation", resolve: context =>
            {
                var loader =
                    dataLoaderAccessor.Context.GetOrAddLoader<ISystemProcessOperation>(
                        $"GetSystemProcessOperationById-{context.Source.SystemProcessOperationId}",
                        () => operationRepository.GetForId(context.Source.SystemProcessOperationId));

                return loader.LoadAsync();
            });
        }
    }
}
