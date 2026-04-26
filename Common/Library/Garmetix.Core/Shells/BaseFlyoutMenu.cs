using Garmetix.Models.Accounting;
using Garmetix.Views.Controls;
using Syncfusion.Maui.Toolkit.SegmentedControl;
using System;
using System.Collections.Generic;
using System.Text;

using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices; // Needed for DeviceInfo

namespace Garmetix.CoreBase.View
{
    public abstract class BaseFlyoutMenu : FlyoutItem
    {
        protected BaseFlyoutMenu(string menuTitle, string menuIcon = null)
        {
            Title = menuTitle;
            if (menuIcon != null) Icon = menuIcon;
        }

        // 1. Your existing standard method
        protected void AddPageTab(string tabTitle, string iconFile, string routeName, Type pageType, bool isVisible = true)
        {
            var tab = new Tab { Title = tabTitle, Icon = iconFile, IsVisible = isVisible };
            tab.Items.Add(new ShellContent { Title = tabTitle, Route = routeName, ContentTemplate = new DataTemplate(pageType) });
            this.Items.Add(tab);
        }

        // 2. ADD THIS NEW METHOD for Platform-Specific Pages
        protected void AddPlatformSpecificPageTab(string tabTitle, string iconFile, string routeName, Type mobilePageType, Type desktopPageType, bool isVisible = true)
        {
            // Determine the correct page type based on the device
            Type targetPageType = (DeviceInfo.Idiom == DeviceIdiom.Desktop) ? desktopPageType : mobilePageType;

            var tab = new Tab { Title = tabTitle, Icon = iconFile, IsVisible = isVisible };
            tab.Items.Add(new ShellContent { Title = tabTitle, Route = routeName, ContentTemplate = new DataTemplate(targetPageType) });
            this.Items.Add(tab);
        }
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

