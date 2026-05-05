namespace Bharat.ToolKits.Helpers
{
    /// <summary>
    /// Router helper class 
    /// </summary>
    public static class RouterHelper
    {
        public static void AddRoute(Type type)
        {
            Routing.RegisterRoute(type.Name, type);
        }
        public static void AddRoute(string name,Type type)
        {
            Routing.RegisterRoute(name, type);
        }
    }
}