using Garmetix.CoreBase.DayOperations.PageModels;

namespace Garmetix.CoreBase.DayOperations.Pages;

public partial class DayEndEntryPage : ContentPage
{
	private readonly DayOperationPageModel _vm= new();
	public DayEndEntryPage()
	{
		InitializeComponent();
        _vm ??= new DayOperationPageModel();
		_vm.Title = "Day Closing";
		_vm.EntryForm=this.dayEndForm;
        dayEndForm.ItemsSourceProvider = _vm;
        dayEndForm.GenerateDataFormItem += _vm.OnGenerateDataFormItem!;
       _vm.IsDayEndEntryVisible = true;
        BindingContext = _vm;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		_vm.Initialize("End");
    }
}