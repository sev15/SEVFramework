using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SEV.Common
{
    public class DomainValidationException : Exception
    {
        public Dictionary<string, List<string>> Errors { get; set; }

        public DomainValidationException()
        {
            Errors = new Dictionary<string, List<string>>();
        }

        public DomainValidationException(string message) : base(message)
        {
            Errors = new Dictionary<string, List<string>> { { "model", new List<string> { message } } };
        }

        public DomainValidationException(string message, Exception innerException) : base(message, innerException)
        {
            Errors = new Dictionary<string, List<string>> { { "model", new List<string> { message } } };
        }

        public DomainValidationException(IEnumerable<ValidationResult> results) : this()
        {
            AddErrors(results);
        }

        private void AddErrors(IEnumerable<ValidationResult> results)
        {
            foreach (var result in results)
            {
                if (result.MemberNames.Any())
                {
                    foreach (var memberName in result.MemberNames)
                    {
                        AddError(memberName, result.ErrorMessage);
                    }
                }
                else
                {
                    AddError("model", result.ErrorMessage);
                }
            }
        }

        private void AddError(string propertyName, string errorMessage)
        {
            if (!Errors.ContainsKey(propertyName))
            {
                Errors[propertyName] = new List<string>();
            }
            Errors[propertyName].Add(errorMessage);
        }
    }
}
