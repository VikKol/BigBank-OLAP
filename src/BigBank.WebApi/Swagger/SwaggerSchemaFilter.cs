using System;
using System.Globalization;
using BigBank.OLAP.Constants;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BigBank.WebApi
{
    internal class SwaggerSchemaFilter : ISchemaFilter
    {
        public void Apply(Schema model, SchemaFilterContext context)
        {
            if (context.SystemType == typeof(Guid) || context.SystemType == typeof(Guid?))
            {
                model.Example = Guid.NewGuid();
            }

            if (context.SystemType == typeof(DateTime))
            {
                model.Format = BigBankFormats.DateTime;
                model.Example = DateTime.UtcNow.ToString(BigBankFormats.DateTime, CultureInfo.InvariantCulture);
            }
        }
    }
}