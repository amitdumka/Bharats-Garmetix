using Garmetix.Reports.PageModels;

namespace Garmetix.Reports.Pages;

public partial class LedgerReportPage : ContentPage
{
	public LedgerReportPage()
	{
		InitializeComponent();
		BindingContext = new LedgerReportPageModel();
	}
}