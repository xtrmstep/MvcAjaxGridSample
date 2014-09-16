namespace MvcAjaxGridSample.Models
{
    /// <summary>
    /// Represents a block of data to control page information for a grid
    /// </summary>
    public class GridPaging
    {
        /// <summary>
        ///     Total items in requested data
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        ///     Total pages for current page size
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        ///     Number of item on a single page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        ///     Current page index
        /// </summary>
        public int PageIndex { get; set; }
    }
}