using System.Linq;
using System.Reflection;

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
            Columns = typeof (T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.Name.ToUpper() != "ID")
                .Select(p => p.Name)
                .ToArray();

            Data = new T[0];
            Options = new GridOptions
            {
                Filter = new GridFilter<T>(),
                Sorting = new GridSorting<T>(),
                Paging = new GridPaging()
            };
        }

        public string[] Columns { get; set; }
        public T[] Data { get; set; }

        public GridOptions Options { get; set; }

        public class GridOptions
        {
            public GridFilter<T> Filter { get; set; }
            public GridSorting<T> Sorting { get; set; }
            public GridPaging Paging { get; set; }
        }
    }
}