using Bharat.ToolKits.Helpers;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Garmetix.Authentication.Pages;
using Garmetix.Core.Sessions;
using Syncfusion.Maui.Toolkit.Themes;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Font = Microsoft.Maui.Font;

namespace Garmetix
{
    /// <summary>
    /// 
    /// </summary>
    [Obsolete("AppShell is marked as obsolete. Please use the new GarmetixAppShell class instead.")]
    public partial class AppShell : Shell, INotifyPropertyChanged
    {
        public new event PropertyChangedEventHandler? PropertyChanged;
        public ICommand ExitCommand { get; }
        protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private string _storeName = "Garmetix";
        public string StoreName
        {
            get => _storeName;
            set
            {
                if (_storeName != value)
                {
                    _storeName = value;
                    OnPropertyChanged(nameof(StoreName));
                }
            }
        }
        public AppShell()
        {
            InitializeComponent();
            var currentTheme = Application.Current!.RequestedTheme;
            //  ThemeSegmentedControl.SelectedIndex = currentTheme == AppTheme.Light ? 0 : 1;

            StoreName = $"{StorageOps.GetPref("CompanyName", "Garmetix")}, {StorageOps.GetPref("StoreCode", "AF")}";
            //"StorageOps.GetPref("CompanyName", "AF");
            ExitCommand = new Command(CloseApp);

            BindingContext = this;

        }
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
        public static async Task DisplaySnackbarAsync(string message)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var snackbarOptions = new SnackbarOptions
            {
                BackgroundColor = Color.FromArgb("#FF3300"),
                TextColor = Colors.White,
                ActionButtonTextColor = Colors.Yellow,
                CornerRadius = new CornerRadius(0),
                Font = Font.SystemFontOfSize(18),
                ActionButtonFont = Font.SystemFontOfSize(14)
            };

            var snackbar = Snackbar.Make(message, visualOptions: snackbarOptions);

            await snackbar.Show(cancellationTokenSource.Token);
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
        public static async Task DisplayToastAsync(string message)
        {
            // Toast is currently not working in MCT on Windows
            if (OperatingSystem.IsWindows())
                return;

            var toast = Toast.Make(message, textSize: 18);

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await toast.Show(cts.Token);
        }

        private void SfSegmentedControl_SelectionChanged(object? sender, Syncfusion.Maui.Toolkit.SegmentedControl.SelectionChangedEventArgs e)
        {
            Application.Current!.UserAppTheme = e.NewIndex == 0 ? AppTheme.Light : AppTheme.Dark;
        }
    }
}
