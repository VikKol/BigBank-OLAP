using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BigBank.OLAP.Exceptions;
using BigBank.WebApi.Models;
using BigBank.WebApi.Validation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BigBank.WebApi.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private static readonly IReadOnlyDictionary<Type, HttpStatusCode> _statusCodeMapping = new Dictionary<Type, HttpStatusCode>
        {
            [typeof(BigBankException)] = HttpStatusCode.BadRequest,
            [typeof(ModelStateValidationException)] = HttpStatusCode.BadRequest
        };

        private readonly RequestDelegate _next;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ExceptionHandlingMiddleware(RequestDelegate next, IHostingEnvironment hostingEnvironment)
        {
            _next = next;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task InvokeAsync(HttpContext context, IOptions<MvcJsonOptions> jsonOptions)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                HttpStatusCode statusCode;
                var foundMapping = _statusCodeMapping.TryGetValue(ex.GetType(), out statusCode);
                if (!foundMapping)
                {
                    statusCode = HttpStatusCode.InternalServerError;
                }

                await WriteErrorResponseAsync(context, statusCode, ex, jsonOptions.Value.SerializerSettings);
            }
        }

        private Task WriteErrorResponseAsync(HttpContext context, HttpStatusCode statusCode, Exception exception, JsonSerializerSettings serializerSettings)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var error = new ErrorResult { Message = exception.Message };

            if (exception is ModelStateValidationException modelStateValidationException)
            {
                var validationErrors = new List<ValidationError>();

                foreach (var item in modelStateValidationException.ModelState)
                {
                    validationErrors.Add(new ValidationError
                    {
                        Key = item.Key,
                        Errors = item.Value.Errors.Select(e => e.ErrorMessage).ToList()
                    });
                }

                error.ValidationErrors = validationErrors;
            }
            else if (_hostingEnvironment.IsDevelopment())
            {
                error.Details = exception.ToString();
            }

            return context.Response.WriteAsync(JsonConvert.SerializeObject(error, serializerSettings));
        }
    }
}