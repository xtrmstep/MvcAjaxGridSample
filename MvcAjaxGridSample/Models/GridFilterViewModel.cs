namespace MvcAjaxGridSample.Models
{
    /// <summary>
    ///     Provides information about simple column filters using information about grid data
    /// </summary>
    public class GridFilterViewModel
    {
        public GridFilterViewModel()
        {
            IssueYear = null;
        }
        public string Title { get; set; }
        public int? IssueYear { get; set; }
        public GridFilterViewModel Current { get; set; }
    }
}