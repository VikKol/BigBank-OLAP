using System.Net;
using System.Threading.Tasks;
using BigBank.WebApi.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BigBank.WebApi.Middlewares
{
    public class HealthCheckMiddleware
    {
        public HealthCheckMiddleware(RequestDelegate next)
        {
        }

        public Task InvokeAsync(HttpContext context, BigBankDataSourceHealthChecker healthChecker)
        {
            context.Response.StatusCode = healthChecker.IsHealthy()
                ? (int)HttpStatusCode.OK
                : (int)HttpStatusCode.ServiceUnavailable;

            return Task.CompletedTask;
        }
    }

    public static class AppBuilderHealthCheckExtensions
    {
        public static void UseHealthCheck(this IApplicationBuilder app)
        {
            app.MapWhen(c => c.Request.Path == "/healthcheck", b => b.UseMiddleware<HealthCheckMiddleware>());
        }
    }
}