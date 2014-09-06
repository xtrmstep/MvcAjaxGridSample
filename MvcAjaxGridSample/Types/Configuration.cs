namespace MvcAjaxGridSample.Types
{
    public static class Configuration
    {
        static Configuration()
        {
            Grid = new GridSettings {PageSize = 2};
        }

        public static GridSettings Grid { get; private set; }

        public class GridSettings
        {
            public int PageSize { get; set; }
        }
    }
}