using System.Collections.Generic;

namespace BigBank.WebApi.Models
{
    public class ErrorResult
    {
        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Error details. Available in development mode
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// Validation errors
        /// </summary>
        public IReadOnlyList<ValidationError> ValidationErrors { get; set; }
    }
}