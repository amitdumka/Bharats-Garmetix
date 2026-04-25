using Garmetix.Authentication.PageModels;	
namespace Garmetix.Authentication.Pages;

public partial class PinUnlockPage : ContentPage
{
	public PinUnlockPage( Shell Appshell)
	{
		InitializeComponent();
		BindingContext = new PinUnlockViewModel(Appshell);
    }
    //(You will need a simple IValueConverter named BoolToColorConverter in your resources to turn the Ellipse dots #38BDF8 when True and #334155 when False).

}