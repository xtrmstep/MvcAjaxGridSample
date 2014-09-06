namespace MvcAjaxGridSample.Models
{
    /// <summary>
    ///     Provides a generic interface to grid data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GridViewModel<T>
    {
        public GridViewModel()
        {
            Data = new T[0];
            Options = new GridOptions();
        }

        /// <summary>
        ///     Array of items to be shown in the grid
        /// </summary>
        public T[] Data { get; set; }

        public GridOptions Options { get; set; }
        public int? DeletedId { get; set; }

        public class GridOptions
        {
            public GridOptions()
            {

                Filter = new GridFilterViewModel();
                Paging = new GridPagingViewModel();
            }
            /// <summary>
            ///     Contains information about column filters
            /// </summary>
            public GridFilterViewModel Filter { get; set; }

            /// <summary>
            ///     Contains information about pagination of the grid
            /// </summary>
            public GridPagingViewModel Paging { get; set; }

            public GridCommand? Command { get; set; }   
        }
    }
}