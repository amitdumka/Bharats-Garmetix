using Bharat.ToolKits.Notifications;

namespace Garmetix.Reports.Views
{
    public partial class ReportMenu
    {
        public ReportMenu()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                _ = Notify.DisplaySnackbarAsync(e.Message);
            }
        }
    }
}