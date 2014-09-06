using MvcAjaxGridSample.Types.DataModel;
using MvcAjaxGridSample.Types.DataStorage.Implementation;

namespace MvcAjaxGridSampleTests.Mocks
{
    internal class TestRepository : Repository<Book>
    {
        public TestRepository()
        {
            _internalStorage.Add(1, new Book {Id = 1, Title = "Book 1", IssueYear = 2001});
            _internalStorage.Add(2, new Book { Id = 2, Title = "Book 2", IssueYear = 2002 });
            _internalStorage.Add(3, new Book { Id = 3, Title = "Book 3", IssueYear = 2003 });
            _internalStorage.Add(4, new Book { Id = 4, Title = "Book 4", IssueYear = 2004 });
            _internalStorage.Add(5, new Book { Id = 5, Title = "Book 5", IssueYear = 2005 });
        }
    }
}