using FluentValidation.Results;
using System.Collections.Generic;
using System.Linq;

namespace SharedKernel.Files.Tests
{
    public static class ValidationFailureExtensions
    {
        public static IList<ValidationFailure> WhereErrorMessage(this IList<ValidationFailure> errors, string errorMessage)
            => errors.Where(w => $"{errorMessage}".Equals(w.ErrorMessage)).ToList();
        
        public static IList<ValidationFailure> WhereRuleValidator(this IList<ValidationFailure> errors, string ruleValidator)
            => errors.Where(w => ruleValidator.Equals(w.ErrorCode)).ToList();

        public static IList<ValidationFailure> WherePropertyName(this IList<ValidationFailure> errors, string propertyName)
            => errors.Where(w => w.PropertyName == propertyName).ToList();
    }
}
