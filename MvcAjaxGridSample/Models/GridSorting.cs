using System.Linq;
using System.Reflection;
using WebGrease.Css.Extensions;

namespace MvcAjaxGridSample.Models
{
    public class GridSorting<T>
    {
        public GridSorting()
        {
            Fields = typeof (T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.Name.ToUpper() != "ID")
                .Select(p => new SortingField
                {
                    Name = p.Name
                })
                .ToArray();
        }

        public SortingField[] Fields { get; set; }

        public void Set(SortingField sortingField)
        {
            var field = Fields.FirstOrDefault(f => f.Name == sortingField.Name);
            if (field != null)
                field.Ascending = sortingField.Ascending;
        }

        public void Clear()
        {
            Fields.ForEach(f => f.Ascending = null);
        }
    }
}