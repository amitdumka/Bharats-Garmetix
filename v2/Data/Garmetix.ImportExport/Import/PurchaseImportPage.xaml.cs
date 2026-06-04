using Garmetix.Databases;
using Garmetix.ImportExports.Services;
using Garmetix.ImportExports.ViewModels;

namespace Garmetix.ImportExport.Import;

public partial class PurchaseImportPage : ContentPage
{
	PurchaseImportViewModel viewModel;
	public PurchaseImportPage(DatabaseContext db, CategoryMappingService categoryMappingService)
	{
		InitializeComponent();
        viewModel = new PurchaseImportViewModel(db,categoryMappingService);
        BindingContext = viewModel;
    }
}