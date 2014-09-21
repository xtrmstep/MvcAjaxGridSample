using System;
using System.Web.Script.Serialization;

namespace MvcAjaxGridSample.Models
{
    public class FilterField
    {
        /// <summary>
        ///     Property name
        /// </summary>
        public string Name { get; set; }

        [ScriptIgnore]
        public Type Type { get; set; }

        public string Value { get; set; }
    }
}