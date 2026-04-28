using Garmetix.CoreBase.DayOperations.PageModels;

namespace Garmetix.CoreBase.DayOperations.Pages;

public partial class DayBeginEntyPage : ContentPage
{
  private readonly  DayOperationPageModel vm = new();
    public DayBeginEntyPage()
	{
		InitializeComponent();
        dayBeginForm.ItemsSourceProvider = vm; 
        dayBeginForm.GenerateDataFormItem += vm.OnGenerateDataFormItem!;

        vm.EntryForm= dayBeginForm;
        vm.Title="Day Begins";
        vm.IsDayBeginEntryVisible=true;
        //vm.Initialize("Begin");
        BindingContext = vm;
        
	}
	protected override void OnAppearing()
    {
        base.OnAppearing();
        vm.Initialize("Begin");
    }
}