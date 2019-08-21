namespace Surveillance.Api.App.Middlewares
{
    using System.Net;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    public class UnAuthenticatedMiddleware
    {
        private readonly RequestDelegate _next;

        public UnAuthenticatedMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated) context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            if (context.Response.StatusCode == 401) return;

            await this._next.Invoke(context);
        }
    }
}