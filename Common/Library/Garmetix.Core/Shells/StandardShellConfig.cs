namespace Garmetix.Core.Shells
{
    public class StandardShellConfig
    {
        public string AppName { get; set; } = "Garmetix";

        // Theme Colors
        public Color BackgroundColorLight { get; set; } = Colors.White;
        public Color BackgroundColorDark { get; set; } = Color.FromArgb("#121212");

        // Flyout Styling
        public string FlyoutBackgroundImage { get; set; } = string.Empty;
        public Aspect FlyoutBackgroundImageAspect { get; set; } = Aspect.Fill;
        public FlyoutHeaderBehavior HeaderBehavior { get; set; } = FlyoutHeaderBehavior.CollapseOnScroll;

        // Custom Views for Header and Footer
        public View? CustomHeaderView { get; set; }
        public View? CustomFooterView { get; set; }

        public bool IncludeStandardLogoutMenu { get; set; } = true;
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
