using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcAjaxGridSample.Models
{
    public class BookEditViewModel : BookViewModel, IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(Title))
                result.Add(new ValidationResult("Value is required.", new[] {"Title"}));
            else if (Title.Length < 5)
                result.Add(new ValidationResult("Length must be greater than 5.", new[] {"Title"}));

            if (!IssueYear.HasValue)
                result.Add(new ValidationResult("Value is required.", new[] {"IssueYear"}));
            else if (IssueYear < 2000)
                result.Add(new ValidationResult("Value must be equal or greater than 2000.", new[] {"IssueYear"}));

            return result;
        }
    }
}