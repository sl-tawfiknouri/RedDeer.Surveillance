namespace Surveillance.Api.App.Types.Engine
{
    using System.Linq;

    using GraphQL.Authorization;
    using GraphQL.DataLoader;
    using GraphQL.Types;

    using Surveillance.Api.App.Authorization;
    using Surveillance.Api.DataAccess.Abstractions.Entities;
    using Surveillance.Api.DataAccess.Abstractions.Repositories;

    public class
        SystemProcessOperationDataSynchroniserRequestGraphType : ObjectGraphType<ISystemProcessOperationDataSynchroniser
        >
    {
        public SystemProcessOperationDataSynchroniserRequestGraphType(
            ISystemProcessOperationRepository operationRepository,
            ISystemProcessOperationRuleRunRepository ruleRunRepository,
            IDataLoaderContextAccessor dataLoaderAccessor)
        {
            this.AuthorizeWith(PolicyManifest.AdminPolicy);

            this.Field(i => i.Id).Description("Identifier for the system process operation data synchroniser request");
            this.Field(i => i.QueueMessageId).Description("Queue message id");

            this.Field(i => i.RuleRunId).Description("Rule run identifier for the data synchroniser");
            this.Field<ListGraphType<SystemProcessOperationRuleRunGraphType>>(
                "ruleRun",
                resolve: context =>
                    {
                        IQueryable<ISystemProcessOperationRuleRun> IdQuery(IQueryable<ISystemProcessOperationRuleRun> i)
                        {
                            return i.Where(x => x.SystemProcessOperationId == context.Source.RuleRunId);
                        }

                        var loader = dataLoaderAccessor.Context.GetOrAddLoader(
                            $"GetSystemProcessOperationRuleRunById-{context.Source.RuleRunId}",
                            () => ruleRunRepository.Query(IdQuery));

                        return loader.LoadAsync();
                    });

            this.Field<SystemProcessOperationGraphType>(
                "processOperation",
                resolve: context =>
                    {
                        var loader = dataLoaderAccessor.Context.GetOrAddLoader(
                            $"GetSystemProcessOperationById-{context.Source.Id}",
                            () => operationRepository.GetForId(context.Source.Id));

                        return loader.LoadAsync();
                    });
        }
    }
}