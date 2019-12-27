namespace Surveillance.Api.App.Types.Engine
{
    using GraphQL.Authorization;
    using GraphQL.DataLoader;
    using GraphQL.Types;

    using Surveillance.Api.App.Authorization;
    using Surveillance.Api.DataAccess.Abstractions.Entities;
    using Surveillance.Api.DataAccess.Abstractions.Repositories;

    public class SystemProcessOperationUploadFileGraphType : ObjectGraphType<ISystemProcessOperationUploadFile>
    {
        public SystemProcessOperationUploadFileGraphType(
            ISystemProcessOperationRepository operationRepository,
            IDataLoaderContextAccessor dataLoaderAccessor)
        {
            this.AuthorizeWith(PolicyManifest.AdminPolicy);

            this.Field(i => i.Id).Description("Identifier for the system process operation upload file");
            this.Field<SystemProcessOperationGraphType>(
                "processOperation",
                resolve: context =>
                    {
                        var loader = dataLoaderAccessor.Context.GetOrAddLoader(
                            $"GetSystemProcessOperationById-{context.Source.Id}",
                            () => operationRepository.GetForId(context.Source.Id));

                        return loader.LoadAsync();
                    });

            this.Field(i => i.FilePath, true).Description("The path of the file uploaded from disk");
            this.Field(i => i.FileType, true).Description("The type of the file being uploaded");
            
        }
    }
}