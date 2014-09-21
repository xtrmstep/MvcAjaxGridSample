namespace MvcAjaxGridSample.Models
{
    public class SortingField
    {
        /// <summary>
        ///     Property name
        /// </summary>
        public string Name { get; set; }

        // True - accesding, False - descending, NUll - no sorting by the field
        public bool? Ascending { get; set; }
    }
}