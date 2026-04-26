namespace Garmetix.Core.Shells
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class StandardAppShell : Shell
    {
        protected readonly StandardShellConfig _config;

        protected StandardAppShell(StandardShellConfig config)
        {
            _config = config;

            ApplyStandardStyling();
            ApplyCustomHeadersAndFooters();

            BuildAppSpecificMenu();
            BuildStandardFooterMenus();
            RegisterAppRoutes();
        }

        private void ApplyStandardStyling()
        {
            FlyoutBehavior = FlyoutBehavior.Flyout;

            // Dynamic Light/Dark Mode Binding
            this.SetAppThemeColor(Shell.FlyoutBackgroundColorProperty, _config.BackgroundColorLight, _config.BackgroundColorDark);

            // Background Image
            if (!string.IsNullOrEmpty(_config.FlyoutBackgroundImage))
            {
                FlyoutBackgroundImage = _config.FlyoutBackgroundImage;
                FlyoutBackgroundImageAspect = _config.FlyoutBackgroundImageAspect;
            }

            FlyoutHeaderBehavior = _config.HeaderBehavior;
        }

        private void ApplyCustomHeadersAndFooters()
        {
            if (_config.CustomHeaderView != null)
                FlyoutHeader = _config.CustomHeaderView;

            if (_config.CustomFooterView != null)
                FlyoutFooter = _config.CustomFooterView;
        }

        private void BuildStandardFooterMenus()
        {
            if (_config.IncludeStandardLogoutMenu)
            {
                var logoutItem = new MenuItem { Text = "Logout", StyleClass = ["MenuItemLayoutStyle"] };
                logoutItem.Clicked += async (s, e) => await HandleLogout();
                Items.Add(logoutItem);

                var quitItem = new MenuItem { Text = "Quit", StyleClass = ["MenuItemLayoutStyle"] };
                quitItem.Clicked += (s, e) => HandleQuit();
                Items.Add(quitItem);
            }
        }

        // --- ABSTRACT HOOKS ---
        protected abstract void BuildAppSpecificMenu();
        protected abstract void RegisterAppRoutes();

        // Virtual methods allow the specific app to define *how* logout and quit work
        protected virtual Task HandleLogout() => Task.CompletedTask;
        protected virtual void HandleQuit() { }
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
