using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcAjaxGridSample.Types.DataModel
{
    public class Book : Entity, IValidatableObject
    {
        public Book()
        {
            Id = -1;
        }

        public string Title { get; set; }

        public int IssueYear { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();

            if (Title.Length < 5)
                result.Add(new ValidationResult("Length must be greater than 5.", new[] {"Title"}));

            if (IssueYear < 2000)
                result.Add(new ValidationResult("Value must be equal or greater than 2000.", new[] { "IssueYear" }));

            return result;
        }
    }
}