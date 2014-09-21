using System.Linq;
using System.Reflection;
using WebGrease.Css.Extensions;

namespace MvcAjaxGridSample.Models
{
    public class GridFilter<T>
    {
        public GridFilter()
        {
            Fields = typeof (T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.Name.ToUpper() != "ID")
                .Select(p => new FilterField
                {
                    Name = p.Name,
                    Type = p.PropertyType
                })
                .ToArray();
        }

        public FilterField[] Fields { get; set; }

        public void Clear()
        {
            Fields.ForEach(f => f.Value = null);
        }
    }
}