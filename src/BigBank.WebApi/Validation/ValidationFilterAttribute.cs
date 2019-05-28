using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BigBank.WebApi.Validation
{
    public class ValidationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var action = context.ActionDescriptor as ControllerActionDescriptor;
            if (action != null)
            {
                var parameters = action.MethodInfo.GetParameters();

                foreach (var parameter in parameters)
                {
                    if (context.ActionArguments.TryGetValue(parameter.Name, out var argument))
                    {
                        EvaluateValidationAttributes(parameter, argument, context.ModelState);
                    }
                    else
                    {
                        var requiredAttribute = parameter.GetCustomAttribute<RequiredAttribute>();
                        if (requiredAttribute != null)
                        {
                            context.ModelState.AddModelError(parameter.Name, requiredAttribute.FormatErrorMessage(parameter.Name));
                        }
                    }
                }
            }

            if (!context.ModelState.IsValid)
            {
                throw new ModelStateValidationException(context.ModelState);
            }

            base.OnActionExecuting(context);
        }

        private static void EvaluateValidationAttributes(ParameterInfo parameter, object argument, ModelStateDictionary modelState)
        {
            foreach (var attributeData in parameter.CustomAttributes)
            {
                var validationAttribute = parameter.GetCustomAttribute(attributeData.AttributeType) as ValidationAttribute;
                if (validationAttribute != null)
                {
                    var isValid = validationAttribute.IsValid(argument);
                    if (!isValid)
                    {
                        modelState.AddModelError(parameter.Name, validationAttribute.FormatErrorMessage(parameter.Name));
                    }
                }
            }
        }
    }
}