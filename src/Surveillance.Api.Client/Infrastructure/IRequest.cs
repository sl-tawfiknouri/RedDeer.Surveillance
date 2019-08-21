namespace RedDeer.Surveillance.Api.Client.Infrastructure
{
    using System.Threading;
    using System.Threading.Tasks;

    using GraphQL.Common.Request;
    using GraphQL.Common.Response;

    public interface IRequest
    {
        Task<GraphQLResponse> PostAsync(GraphQLRequest request, CancellationToken ctx);
    }
}