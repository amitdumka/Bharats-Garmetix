using Garmetix.Databases;
using Garmetix.ImportExports.ViewModels;

namespace Garmetix.ImportExport.Import;

public partial class PurchaseImportPage : ContentPage
{
	PurchaseImportViewModel viewModel;
	public PurchaseImportPage(DatabaseContext db)
	{
		InitializeComponent();
        viewModel = new PurchaseImportViewModel(db);
        BindingContext = viewModel;
    }
}