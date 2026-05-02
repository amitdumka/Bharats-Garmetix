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
         
        public ICommand ExitCommand { get; set; }
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

        public SfSegmentedControl ThemeSegmentedControl { get; set; }
        public AppTheme CurrentTheme => Application.Current?.RequestedTheme ?? AppTheme.Light;

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
            //read from Preferences and set the theme accordingly
            var setTheme= Preferences.Get("AppTheme", "Light");
            Application.Current!.UserAppTheme = setTheme == "Light" ? AppTheme.Light : AppTheme.Dark;
            // We set the Footer inside the constructor because we need to build the Grid
            _config.CustomFooterView = CreateCustomFooter();
            // Re-apply the footer now that we built it
            FlyoutFooter = _config.CustomFooterView;

            ThemeSegmentedControl?.SelectedIndex = CurrentTheme == AppTheme.Light ? 0 : 1;
            
            //TODO: test with the actual logout and quit logic
            ExitCommand = new Command(HandleQuit);
        }

        protected override void BuildAppSpecificMenu()
        {
            throw new NotImplementedException();
        }
        protected override void UpdateTheme(object? sender, System.EventArgs e)
        {
             throw new NotImplementedException();
        }

        protected override void RegisterAppRoutes()
        {
            // Register hidden routes here if needed
            //throw new NotImplementedException();
        }

        // --- Custom Footer with Syncfusion Logic ---
        private View CreateCustomFooter()
        {
            var grid = new Grid { Padding = new Thickness(15) };
            grid.SetAppThemeColor(Grid.BackgroundColorProperty, _config.BackgroundColorLight, _config.BackgroundColorDark);

            // Add your custom XAML Footer
            //Bind the StoreName property to the ShellFooter's Label
            var shellFooter = new ShellFooter(this.StoreName);

            grid.Children.Add(shellFooter);

            //icon 
            var lightIcon = Application.Current.Resources.TryGetValue("IconLight", out var lightRes)
                        ? (ImageSource)lightRes
                        : null;

            var darkIcon = Application.Current.Resources.TryGetValue("IconDark", out var darkRes)
                           ? (ImageSource)darkRes
                           : null;

            // Build the Syncfusion Segmented Control
            ThemeSegmentedControl = new SfSegmentedControl
            {
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.End,
                SegmentHeight = 30,
                SegmentWidth = 30,
                ItemsSource = new[]
                {
                    new SfSegmentItem { ImageSource = lightIcon}, // Replace with your StaticResource image strings
                    new SfSegmentItem { ImageSource = darkIcon }
                }
            };

            ThemeSegmentedControl.SelectionChanged += SfSegmentedControl_SelectionChanged;
            grid.Children.Add(ThemeSegmentedControl);

            return grid  ;
        }

        private void SfSegmentedControl_SelectionChanged(object? sender, Syncfusion.Maui.Toolkit.SegmentedControl.SelectionChangedEventArgs e)
        {
            // Your custom theme switching logic goes here
            if (e.NewIndex == 0)
                Application.Current?.UserAppTheme = AppTheme.Light;
            else
                Application.Current?.UserAppTheme = AppTheme.Dark;

            UpdateTheme(sender, e);
        }

        // --- Logout & Quit Implementations ---
        protected override async Task HandleLogout()
        {
            // Implement your logout logic
            await Current.GoToAsync("LoginPage");
        }

        protected override void HandleQuit()
        {
            Application.Current?.Quit();
        }
    }

   
}
