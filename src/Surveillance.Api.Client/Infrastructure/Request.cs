using SAHB.GraphQLClient;
using SAHB.GraphQLClient.Builder;
using SAHB.GraphQLClient.Executor;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Surveillance.Api.Client.Infrastructure
{
    public class Request : IRequest
    {
        private readonly string _url;
        private readonly string _bearer;
        private readonly IGraphQLHttpClient _client;

        public Request(string url, string bearer, HttpMessageHandler httpMessageHandler)
        {
            _url = url;
            _bearer = bearer;

            var executor = new CustomGraphQLHttpExecutor(httpMessageHandler);
            var fieldBuilder = new GraphQLFieldBuilder();
            var queryGenerator = new GraphQLQueryGeneratorFromFields();
            _client = new GraphQLHttpClient(executor, fieldBuilder, queryGenerator);
        }

        public async Task<dynamic> QueryAsync(Action<IGraphQLBuilder> builder, GraphQLQueryArgument[] arguments = null)
        {
            var query = _client.CreateQuery(builder, _url, _bearer, arguments: arguments);
            return await query.Execute();
        }

    }
}
