using System.Collections.Generic;
using System.Linq;
using GraphQL.Authorization;
using GraphQL.DataLoader;
using GraphQL.Types;
using Surveillance.Api.App.Authorization;
using Surveillance.Api.DataAccess.Abstractions.Entities;
using Surveillance.Api.DataAccess.Abstractions.Repositories;

namespace Surveillance.Api.App.Types.Engine
{
    public class SystemProcessOperationDataSynchroniserRequestGraphType : ObjectGraphType<ISystemProcessOperationDataSynchroniser>
    {
        public SystemProcessOperationDataSynchroniserRequestGraphType(
            ISystemProcessOperationRepository operationRepository,
            ISystemProcessOperationRuleRunRepository ruleRunRepository,
            IDataLoaderContextAccessor dataLoaderAccessor)
        {
            this.AuthorizeWith(PolicyManifest.AdminPolicy);

            Field(i => i.Id).Description("Identifier for the system process operation data synchroniser request");
            Field(i => i.QueueMessageId).Description("Queue message id");

            Field(i => i.RuleRunId).Description("Rule run identifier for the data synchroniser");
            Field<ListGraphType<SystemProcessOperationRuleRunGraphType>>("ruleRun", resolve: context =>
            {
                IQueryable<ISystemProcessOperationRuleRun> IdQuery(IQueryable<ISystemProcessOperationRuleRun> i) => i.Where(x => x.SystemProcessOperationId == context.Source.RuleRunId);

                var loader =
                    dataLoaderAccessor.Context.GetOrAddLoader<IEnumerable<ISystemProcessOperationRuleRun>>(
                        $"GetSystemProcessOperationRuleRunById-{context.Source.RuleRunId}",
                        () => ruleRunRepository.Query(IdQuery));

                return loader.LoadAsync();
            });

            Field<SystemProcessOperationGraphType>("processOperation", resolve: context =>
            {
                var loader =
                    dataLoaderAccessor.Context.GetOrAddLoader<ISystemProcessOperation>(
                        $"GetSystemProcessOperationById-{context.Source.Id}",
                        () => operationRepository.GetForId(context.Source.Id));

                return loader.LoadAsync();
            });
        }
    }
}