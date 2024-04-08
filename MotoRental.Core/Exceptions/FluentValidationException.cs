using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoRental.Core.Exceptions
{
    public class FluentValidationException : Exception
    {
        public Dictionary<string, string[]> Errors { get; }

        public FluentValidationException(Dictionary<string, string[]> errors) : base("Validation errors have occurred.")
        {
            Errors = errors;
        }

    }
}
