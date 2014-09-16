using System.Linq;
using System.Reflection;

namespace MvcAjaxGridSample.Models
{
    public class GridSorting<T>
    {
        public GridSorting()
        {
            Fields = typeof(T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.Name.ToUpper() != "ID")
                .Select(p => new SortingField
                {
                    Name = p.Name
                })
                .ToArray();
        }

        public SortingField[] Fields { get; set; }

        public class SortingField
        {
            /// <summary>
            /// Property name
            /// </summary>
            public string Name { get; set; }

            // True - accesding, False - descending, NUll - no sorting by the field
            public bool? Acsending { get; set; }
        }
    }
}