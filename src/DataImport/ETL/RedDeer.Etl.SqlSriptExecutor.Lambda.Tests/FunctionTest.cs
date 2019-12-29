using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using RedDeer.Etl.SqlSriptExecutor.Lambda;
using RedDeer.Etl.SqlSriptExecutor.Services.Models;
using Newtonsoft.Json;
using RedDeer.Etl.SqlSriptExecutor.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace RedDeer.Etl.SqlSriptExecutor.Lambda.Tests
{
    public class FunctionTest
    {
        [Fact]
        public async Task TestToUpperFunction()
        {

            var request = new SqlSriptExecutorRequest();
            request.Scripts = new SqlSriptData[]
            {
                new SqlSriptData
                {
                    CsvOutputLocation = "s3://rd-dev-client-mantasmasidlauskas-eu-west-1/surveillance-etl/spike/results/orders",
                    Database = "mm-spike",
                    SqlScriptS3Location = "s3://rd-dev-client-mantasmasidlauskas-eu-west-1/surveillance-etl/spike/scripts/Orders.sql"
                }
            };

            var requestJson = JsonConvert.SerializeObject(request);

            // Invoke the lambda function and confirm the string was upper cased.
            var function = new Function((sc) => 
            {
                sc.RemoveAll<IEC2InstanceMetadataProvider>();
                sc.AddTransient<IEC2InstanceMetadataProvider, UnitTestEC2InstanceMetadataProvider>();
                return sc; 
            });
            var context = new TestLambdaContext();
            var upperCase = await function.FunctionHandler(request, context);

            //Assert.Equal("HELLO WORLD", upperCase);
        }
    }
}
