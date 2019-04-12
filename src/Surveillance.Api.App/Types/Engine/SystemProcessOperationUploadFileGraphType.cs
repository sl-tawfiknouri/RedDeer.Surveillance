using GraphQL.Authorization;
using GraphQL.DataLoader;
using GraphQL.Types;
using Surveillance.Api.App.Authorization;
using Surveillance.Api.DataAccess.Abstractions.Entities;
using Surveillance.Api.DataAccess.Abstractions.Repositories;

namespace Surveillance.Api.App.Types.Engine
{
    public class SystemProcessOperationUploadFileGraphType : ObjectGraphType<ISystemProcessOperationUploadFile>
    {
        public SystemProcessOperationUploadFileGraphType(ISystemProcessOperationRepository operationRepository, IDataLoaderContextAccessor dataLoaderAccessor)
        {
            this.AuthorizeWith(PolicyManifest.AdminPolicy);

            Field(i => i.Id).Description("Identifier for the system process operation upload file");
            Field<SystemProcessOperationGraphType>("processOperation", resolve: context =>
            {
                var loader =
                    dataLoaderAccessor.Context.GetOrAddLoader<ISystemProcessOperation>(
                        $"GetSystemProcessOperationById-{context.Source.Id}",
                        () => operationRepository.GetForId(context.Source.Id));

                return loader.LoadAsync();
            });

            Field(i => i.FilePath).Description("The path of the file uploaded from disk");
            Field(i => i.FileType).Description("The type of the file being uploaded");
        }
    }
}
