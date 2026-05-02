namespace Garmetix.Views.Controls;

public partial class ShellFooter : ContentView
{


    public static readonly BindableProperty FooterTextProperty =
        BindableProperty.Create(nameof(FooterText), typeof(string), typeof(ShellFooter), defaultValue: "© 2024 Garmetix. All rights reserved.");
    
    public string FooterText
    {
        get => (string)GetValue(FooterTextProperty);
        set => SetValue(FooterTextProperty, value);
    }
    // StoreName is a bindable property that we can set from the Shell to update the footer text dynamically
    public static readonly BindableProperty StoreNameProperty =
        BindableProperty.Create(nameof(StoreName), typeof(string), typeof(ShellFooter), defaultValue: "Garmetix Store");
    public string StoreName
    {
        get => (string)GetValue(StoreNameProperty);
        set => SetValue(StoreNameProperty, value);
    }
    public ShellFooter(string storeName)
    
    {
        InitializeComponent();
        StoreName = storeName;
        BindingContext = this;
    }
    
}

