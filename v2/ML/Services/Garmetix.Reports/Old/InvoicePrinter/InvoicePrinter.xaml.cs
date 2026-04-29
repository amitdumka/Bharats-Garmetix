// MainPage.xaml.cs
using Garmetix.Reports.InvoicePrinter.ViewModels;

namespace Garmetix.Reports.InvoicePrinter.Pages;

public partial class InvoicePrinterPage : ContentPage
{
	public InvoicePrinterPage(InvoicePageModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
