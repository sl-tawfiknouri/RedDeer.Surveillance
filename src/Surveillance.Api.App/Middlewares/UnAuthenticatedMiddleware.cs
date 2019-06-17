using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

namespace Surveillance.Api.App.Middlewares
{
    public class UnAuthenticatedMiddleware
    {
        private readonly RequestDelegate _next;

        public UnAuthenticatedMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }

            if (context.Response.StatusCode == 401)
            {
                return;
            }

            await _next.Invoke(context);
        }
    }
}
