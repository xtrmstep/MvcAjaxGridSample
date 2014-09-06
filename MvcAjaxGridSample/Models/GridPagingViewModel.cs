﻿namespace MvcAjaxGridSample.Models
{
    /// <summary>
    /// Represents a block of data to control page information for a grid
    /// </summary>
    public class GridPagingViewModel
    {
        public GridPagingViewModel(int totalItems, int pageSize)
        {
            PageSize = pageSize == 0 ? 1 : pageSize;
            PageIndex = 1;
            TotalItems = totalItems;
            TotalPages = totalItems == 0 ? 1 : totalItems/pageSize + 1;
            GoToIndex = null;
        }

        /// <summary>
        ///     Total items in requested data
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        ///     Total pages for current page size
        /// </summary>
        public int TotalPages { get; private set; }

        /// <summary>
        ///     Number of item on a single page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        ///     Current page index
        /// </summary>
        public int PageIndex { get; set; }
        public int? GoToIndex { get; set; }
    }
}