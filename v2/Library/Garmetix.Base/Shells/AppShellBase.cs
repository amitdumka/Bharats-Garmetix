using Garmetix.Core.Shells;
using Garmetix.Views.Controls;
using Syncfusion.Maui.Toolkit.SegmentedControl;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Garmetix.Base.Shells
{
    public partial class AppShellBase : StandardAppShell
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
        public AppShellBase() : base(new StandardShellConfig
        {
            AppName = "Garmetix",
            // Note: Replace these hex codes with your actual light/dark dictionary colors
            BackgroundColorLight = Color.FromArgb("#FFFFFF"),
            BackgroundColorDark = Color.FromArgb("#121212"),
            FlyoutBackgroundImage = "thearvindstore005.jpg",
            HeaderBehavior = FlyoutHeaderBehavior.CollapseOnScroll,

            // Injecting your existing XAML Header Control
            CustomHeaderView = new ShellHeader()
        })
        {
            // We set the Footer inside the constructor because we need to build the Grid
            _config.CustomFooterView = CreateCustomFooter();

            // Re-apply the footer now that we built it
            FlyoutFooter = _config.CustomFooterView;
        }

        protected override void BuildAppSpecificMenu()
        {
            // 1. Main Page
            //Items.Add(new ShellContent
            //{
            //    Title = "Home",
            //    Route = "MainPage",
            //    ContentTemplate = new DataTemplate(typeof(MainPage))
            //});

            //// 2. Inject your pre-built XAML Flyout Items!
            //Items.Add(new AccountingMenu());
            //Items.Add(new AccountsMenu());
            //Items.Add(new LedgerMenu());
            //Items.Add(new HRMMenu());
            //Items.Add(new BankingMenu());
            //Items.Add(new CompanyMenu());
        }

        protected override void RegisterAppRoutes()
        {
            // Register hidden routes here if needed
        }

        // --- Custom Footer with Syncfusion Logic ---
        private View CreateCustomFooter()
        {
            var grid = new Grid { Padding = new Thickness(15) };
            grid.SetAppThemeColor(Grid.BackgroundColorProperty, _config.BackgroundColorLight, _config.BackgroundColorDark);

            // Add your custom XAML Footer
            var shellFooter = new ShellFooter { BindingContext = this };
            grid.Children.Add(shellFooter);

            // Build the Syncfusion Segmented Control
            var segmentedControl = new SfSegmentedControl
            {
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.End,
                SegmentHeight = 30,
                SegmentWidth = 30,
                ItemsSource = new[]
                {
                    new SfSegmentItem { ImageSource = "icon_light.png" }, // Replace with your StaticResource image strings
                    new SfSegmentItem { ImageSource = "icon_dark.png" }
                }
            };

            segmentedControl.SelectionChanged += SfSegmentedControl_SelectionChanged;
            grid.Children.Add(segmentedControl);

            return grid  ;
        }

        private void SfSegmentedControl_SelectionChanged(object sender, Syncfusion.Maui.Toolkit.SegmentedControl.SelectionChangedEventArgs e)
        {
            // Your custom theme switching logic goes here
            if (e.NewIndex == 0)
                Application.Current?.UserAppTheme = AppTheme.Light;
            else
                Application.Current?.UserAppTheme = AppTheme.Dark;
        }

        // --- Logout & Quit Implementations ---
        protected override async Task HandleLogout()
        {
            // Implement your logout logic
            await Current.GoToAsync("//LoginPage");
        }

        protected override void HandleQuit()
        {
            Application.Current?.Quit();
        }
    }

    //Template for how to create a new menu. Just inherit from BaseFlyoutMenu and call AddPageTab for each page you want to add.
    //public class AccountingMenu : BaseFlyoutMenu
    //{
    //    public AccountingMenu() : base("Vouchers") // The main menu title
    //    {
    //        // Just call the helper method for each page!
    //        AddPageTab("Voucher", "rain_icon.png", "Voucher", typeof(VoucherPage));
    //        AddPageTab("Cash Voucher", "rain_icon.png", "CashVoucher", typeof(CashVoucherPage));
    //    }
    //}
}
