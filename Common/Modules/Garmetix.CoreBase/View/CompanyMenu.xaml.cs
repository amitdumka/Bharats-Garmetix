using Bharat.ToolKits.Notifications;

namespace Garmetix.CoreBase.View
{
    public partial class CompanyMenu
    {
        public CompanyMenu()
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