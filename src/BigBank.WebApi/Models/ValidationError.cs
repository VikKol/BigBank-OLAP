using System.Collections.Generic;

namespace BigBank.WebApi.Models
{
    public class ValidationError
    {
        /// <summary>
        /// Field/parameter name
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Validation messages
        /// </summary>
        public IReadOnlyList<string> Errors { get; set; }
    }
}