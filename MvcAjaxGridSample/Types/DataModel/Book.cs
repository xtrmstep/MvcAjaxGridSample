namespace MvcAjaxGridSample.Types.DataModel
{
    public class Book : Entity
    {
        public Book()
        {
            Id = -1;
        }

        public string Title { get; set; }
        public int IssueYear { get; set; }
    }
}