using MvcAjaxGridSample.Types.DataModel;

namespace MvcAjaxGridSample.Models
{
    public class BookViewModel
    {
        public BookViewModel()
        {
            
        }
        public BookViewModel(Book book)
        {
            Id = book.Id;
            Title = book.Title;
            IssueYear = book.IssueYear;
        }

        public int? Id { get; set; }

        public string Title { get; set; }

        public int? IssueYear { get; set; }
    }
}