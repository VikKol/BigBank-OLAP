using System.Linq;
using BigBank.WebApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BigBank.WebApi
{
    internal class SwaggerOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters != null)
            {
                var dateParam = operation.Parameters.FirstOrDefault(p => p.Name == "date");
                if (dateParam != null)
                {
                    dateParam.Extensions.Add("default", "1/1/2018 00:00:00");
                }
            }

            var errorSchema = context.SchemaRegistry.GetOrRegister(typeof(ErrorResult));
            operation.Responses["404"] = new Response { Description = "Not found error" };
            operation.Responses["400"] = new Response { Description = "Validation error", Schema = errorSchema };
            operation.Responses["500"] = new Response { Description = "Unexpected error", Schema = errorSchema };
        }
    }
}