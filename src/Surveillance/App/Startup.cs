using Surveillance.Auditing.Context;
using Surveillance.Auditing.DataLayer.Processes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RedDeer.Surveillance.App
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment()) 
                app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => 
            { 
                endpoints.MapRazorPages(); 
            });

            app.Run(async context => { await context.Response.WriteAsync("Red Deer Surveillance Service App"); });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            SystemProcessContext.ProcessType = SystemProcessType.SurveillanceService;
        }
    }
}