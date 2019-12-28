using System.Net;
using System.Net.Http;

namespace RedDeer.Etl.SqlSriptExecutor.Extensions
{
    public static class HttpStatusCodeExtensions
    {
        public static HttpStatusCode EnsureSuccessStatusCode(this HttpStatusCode statusCode)
            => new HttpResponseMessage(statusCode).EnsureSuccessStatusCode().StatusCode;

        public static bool IsSuccessStatusCode(this HttpStatusCode statusCode)
           => new HttpResponseMessage(statusCode).IsSuccessStatusCode;
    }
}
