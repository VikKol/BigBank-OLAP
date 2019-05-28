using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BigBank.WebApi.Validation
{
    public class ModelStateValidationException : Exception
    {
        public ModelStateDictionary ModelState { get; }

        public ModelStateValidationException(ModelStateDictionary modelState)
            : base("Model is not valid.")
        {
            ModelState = modelState ?? throw new ArgumentNullException(nameof(modelState));
        }
    }
}