using System;
using System.Linq;
using System.Reflection;

namespace MvcAjaxGridSample.Models
{
    public class GridFilter<T>
    {
        public GridFilter()
        {
            Fields = typeof(T)
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

        public class FilterField
        {
            /// <summary>
            ///     Property name
            /// </summary>
            public string Name { get; set; }

            public Type Type { get; set; }
            public string Value { get; set; }
        }
    }
}