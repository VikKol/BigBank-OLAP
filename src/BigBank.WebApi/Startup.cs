using System;
using BigBank.OLAP;
using BigBank.WebApi;
using BigBank.WebApi.Helpers;
using BigBank.WebApi.Middlewares;
using BigBank.WebApi.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BigBank
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            _configuration = configuration;
            _hostingEnvironment = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddOptions();

            services
                .AddBigBankOlapServices(new Uri(_configuration["DruidBrokerUrl"]));

            services
                .AddSingleton(sp => new BigBankDataSourceHealthChecker(
                    new Uri(_configuration["DruidCoordinatorUrl"]),
                    sp.GetRequiredService<ILoggerFactory>()));

            services
                .AddSwagger(_hostingEnvironment);

            services
                .AddMvc(c => c.Filters.Add<ValidationFilterAttribute>())
                .AddJsonOptions(c => JsonConfigurator.ApplySerializerSettings(c.SerializerSettings));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            appLifetime.ApplicationStarted.Register(() => OnAppStarted(app));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseHealthCheck();
            app.UseMiddleware<ExceptionHandlingMiddleware>(env);
            app.UseMvcWithDefaultRoute();
        }

        private void OnAppStarted(IApplicationBuilder app)
        {
            var druidHealthChecker = app.ApplicationServices.GetRequiredService<BigBankDataSourceHealthChecker>();
            druidHealthChecker.WaitUntilDataSourceIsAvailableAsync().Wait();
        }
    }
}