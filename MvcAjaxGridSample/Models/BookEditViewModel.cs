using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MvcAjaxGridSample.Models
{
    public class BookEditViewModel : IValidatableObject
    {
        public int? Id { get; set; }

        public string Title { get; set; }

        public int? IssueYear { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(Title))
                result.Add(new ValidationResult("Value is required.", new[] { "Title" }));
            if (Title.Length < 5)
                result.Add(new ValidationResult("Length must be greater than 5.", new[] {"Title"}));

            if (!IssueYear.HasValue)
                result.Add(new ValidationResult("Value is required.", new[] { "IssueYear" }));
            else if (IssueYear < 2000)
                result.Add(new ValidationResult("Value must be equal or greater than 2000.", new[] { "IssueYear" }));

            return result;
        }
    }
}