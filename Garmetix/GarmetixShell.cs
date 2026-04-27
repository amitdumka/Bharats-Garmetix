using Bharat.ToolKits.Helpers;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Garmetix.Authentication.Pages;
using Garmetix.Core.Sessions;
using Garmetix.Core.Shells;
using Garmetix.CoreBase.View;
using Syncfusion.Maui.Toolkit.Themes;

namespace Garmetix
{
    public class GarmetixShell : AppShellBase
    {

        public GarmetixShell()
        {
            var currentTheme = Application.Current!.RequestedTheme;
            ThemeSegmentedControl.SelectedIndex = currentTheme == AppTheme.Light ? 0 : 1;
            StoreName = $"{StorageOps.GetPref("CompanyName", "Garmetix")}, {StorageOps.GetPref("StoreCode", "AF")}";
            // ExitCommand = new Command(CloseApp);
            BindingContext = this;
        }

        protected override void BuildAppSpecificMenu()
        {
            // 1. Dashboard Page
            Items.Add(new ShellContent
            {
                Title = "Home",
                Route = "Dashboard",
                ContentTemplate = new DataTemplate(typeof(MainPage))
            });

            //// 2. Inject your pre-built XAML Flyout Items!
            Items.Add(new Accounting_Menu());  //Accouting Menu
            Items.Add(new Accounts_Menu());    // Accounts Menu
            Items.Add(new Ledger_Menu());      //Ledger Menu
            Items.Add(new HRM_Menu());         //HRM Menu
            Items.Add(new Banking_Menu());     //Banking Menu
            Items.Add(new Company_Menu());  
        }

        /// <summary>
        /// Update the Theme of the app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateTheme(object sender, System.EventArgs e)
        {
            ICollection<ResourceDictionary> mergedDictionaries = Application.Current!.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
            {
                var theme = mergedDictionaries.OfType<SyncfusionThemeResourceDictionary>().FirstOrDefault();
                if (theme != null)
                {
                    if (theme.VisualTheme is SfVisuals.MaterialDark)
                    {
                        theme.VisualTheme = SfVisuals.MaterialLight;
                        Application.Current.UserAppTheme = AppTheme.Light;
                    }
                    else
                    {
                        theme.VisualTheme = SfVisuals.MaterialDark;
                        Application.Current.UserAppTheme = AppTheme.Dark;
                    }
                }
            }
        }
        void CloseApp()
        {
            LogoutAndClose(false);
        }
        private void Logout(object sender, EventArgs e)
        {
            LogoutAndClose(false);
        }
        //Adding from older version
        private void LogoutAndClose(bool forceClose = false)
        {
            Dispatcher.Dispatch(async () =>
            {
                var exit = await DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");

                if (exit)
                {
                    if (forceClose)
                    {
                        Application.Current?.Quit();
                    }
                    var currentWindow = Application.Current?.Windows.FirstOrDefault();
                    if (currentWindow != null)
                    {
                        _ = SessionService.EndSessionAsync();
                        SessionService.ClearSession();
                        //TODO: check if this work , or need to send new shell
                        currentWindow.Page = new Login(this);
                    }

                    var cancellationTokenSource = new CancellationTokenSource();
                    await Toast.Make("Logged out Successfully", ToastDuration.Short, 14).Show(cancellationTokenSource.Token);
                }
            });
        }
        private void Quit(object sender, EventArgs e)
        {
            LogoutAndClose(true);
        }
    }
}
