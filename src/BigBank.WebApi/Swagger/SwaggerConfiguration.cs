using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace BigBank.WebApi
{
    internal static class SwaggerConfiguration
    {
        private const string ServiceName = "BigBank API";
        private const string DocumentName = "big-bank-api";

        public static void AddSwagger(this IServiceCollection services, IHostingEnvironment environment)
        {
            services.AddSwaggerGen(options => 
            {
                options.SwaggerDoc(DocumentName, new Info { Title = ServiceName });

                var xmlDocPath = Path.Combine(environment.ContentRootPath, "BigBank.WebApi.xml");
                if (File.Exists(xmlDocPath))
                {
                    options.IncludeXmlComments(xmlDocPath);
                }

                options.SchemaFilter<SwaggerSchemaFilter>();
                options.OperationFilter<SwaggerOperationFilter>();

                options.DescribeAllEnumsAsStrings();
            });
        }

        public static void UseSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger(options => 
            {
                options.RouteTemplate = "api/swagger/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = "api/swagger";
                options.SwaggerEndpoint(DocumentName + "/swagger.json", ServiceName);
            });
        }
    }
}