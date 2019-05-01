using SAHB.GraphQLClient.Builder;
using SAHB.GraphQLClient.QueryGenerator;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Surveillance.Api.Client.Infrastructure
{
    public interface IRequest
    {
        Task<dynamic> QueryAsync(Action<IGraphQLBuilder> builder, GraphQLQueryArgument[] arguments = null);
    }
}
