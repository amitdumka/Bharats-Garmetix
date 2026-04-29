using Garmetix.Commons.Dashboard.PageModels;

namespace Garmetix.CoreBase.Dashboard.Pages;

public partial class DashboardPage : ContentPage
{
	public DashboardPage(DefaultDashboardPageModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}