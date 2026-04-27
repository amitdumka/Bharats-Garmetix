using System.Runtime.InteropServices;


namespace Bharat.ToolKits.Helpers
{
    public class InternetStatus
    {
        //Creating the extern function...
        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int Description, int ReservedValue);

        //Creating a function that uses the API function...
        public static bool IsConnectedToInternet()
        {
            int Desc;
            return InternetGetConnectedState(out Desc, 0);
        }
    }
}