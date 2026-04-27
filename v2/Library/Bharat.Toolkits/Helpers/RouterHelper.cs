using System.Runtime.InteropServices;


namespace Bharat.ToolKits.Helpers
{
    public static class RouterHelper
    {
        public static void AddRoute(Type type)
        {
            Routing.RegisterRoute(type.Name, type);
        }
    }
}