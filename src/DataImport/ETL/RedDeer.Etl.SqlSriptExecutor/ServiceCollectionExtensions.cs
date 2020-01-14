using Microsoft.Extensions.DependencyInjection;
using RedDeer.Etl.SqlSriptExecutor.Services;
using RedDeer.Etl.SqlSriptExecutor.Services.Interfaces;

namespace RedDeer.Etl.SqlSriptExecutor
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlSriptExecutorServices(this IServiceCollection services)
        {
            services.AddTransient<IFunctionService, FunctionService>();

            services.AddTransient<IS3ClientService, S3ClientService>();
            services.AddTransient<IAmazonS3ClientFactory, AmazonS3ClientFactory>();

            services.AddTransient<IAthenaClientService, AthenaClientService>();
            services.AddTransient<IAmazonAthenaClientFactory, AmazonAthenaClientFactory>();

            services.AddTransient<ISqlSriptExecutorService, SqlSriptExecutorService>();
            services.AddTransient<IEC2InstanceMetadataProvider, EC2InstanceMetadataProvider>();

            services.AddTransient<ICSVService, CSVService>();
            services.AddTransient<IFilePreProcessorService, FilePreProcessorService>();

            return services;
        }
    }
}
