using Garmetix.Reports.PageModels;

namespace Garmetix.Reports.Pages;

public partial class ReprintReportPage : ContentPage
{
	public ReprintReportPage()
	{
		InitializeComponent();
		BindingContext = new ReprintPageModel();
	}
}