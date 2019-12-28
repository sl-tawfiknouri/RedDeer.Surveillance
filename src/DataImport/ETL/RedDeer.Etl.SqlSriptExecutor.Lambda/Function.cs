using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RedDeer.Etl.SqlSriptExecutor.Services.Interfaces;
using RedDeer.Etl.SqlSriptExecutor.Services.Models;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace RedDeer.Etl.SqlSriptExecutor.Lambda
{
    public class Function
    {
        private readonly IServiceProvider serviceProvider;

        public Function()
            : this(null)
        { }

        public Function(Func<IServiceCollection, IServiceCollection> configureServices = null)
        {
            var serviceCollection = new ServiceCollection()
                .AddLogging(loggingBuilder => loggingBuilder.AddConsole().AddDebug().SetMinimumLevel(LogLevel.Debug))
                .AddSqlSriptExecutorServices();

            if (configureServices != null)
            {
                serviceCollection = configureServices(serviceCollection);
            }

            serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public async Task<bool> FunctionHandler(SqlSriptExecutorRequest request, ILambdaContext context)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var sqlSriptExecutor = scope.ServiceProvider.GetRequiredService<ISqlSriptExecutorService>();
                var result = await sqlSriptExecutor.ExecuteAsync(request);
                return result;
            }
        }
    }
}
