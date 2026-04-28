using Garmetix.Base.Views.Customs.Forms;
using Garmetix.CoreBase.Stores.PageModels;
using Syncfusion.Maui.DataForm;

namespace Garmetix.CoreBase.Stores.Pages
{
    public class EntryStorePage : ContentPage
    {
        public EntryStorePage(StoreFormModel fvm)
        {
            var efv = new EntryFormView { Title = "Store New", ColumnCount = 2 };

            Content = new VerticalStackLayout
        {
            efv
        };

            fvm.InitFormViewModel();
            this.Title = "Store [New]";
            efv.BindingContext = fvm;
            efv.DataForm.ItemsSourceProvider = fvm;
            efv.DataForm.RegisterEditor("Company", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("StoreGroup", DataFormEditorType.ComboBox);
            efv.DataForm.RegisterEditor("Store", DataFormEditorType.ComboBox);

            efv.DataForm.GenerateDataFormItem += fvm.OnGenerateDataFormItem;
        }
    }

    public class EntryStoreGroupPage : ContentPage
    {
        public EntryStoreGroupPage(StoreGroupFormModel fvm)
        {
            var efv = new EntryFormView { Title = "Store Group New", ColumnCount = 2 };

            Content = new VerticalStackLayout
        {
            efv
        };

            fvm.InitFormViewModel();
            this.Title = "Store Group [New]";
            efv.BindingContext = fvm;
            efv.DataForm.ItemsSourceProvider = fvm;
            efv.DataForm.RegisterEditor("Company", DataFormEditorType.ComboBox);


            efv.DataForm.GenerateDataFormItem += fvm.OnGenerateDataFormItem;
        }
    }

    public class EntryCompanyPage : ContentPage
    {
        public EntryCompanyPage(CompanyFormModel fvm)
        {
            var efv = new EntryFormView { Title = "Company New", ColumnCount = 4 };

            Content = new VerticalStackLayout
        {
            efv
        };

            fvm.InitFormViewModel();
            this.Title = "Company [New]";
            efv.BindingContext = fvm;
            efv.DataForm.ItemsSourceProvider = fvm;

            efv.DataForm.GenerateDataFormItem += fvm.OnGenerateDataFormItem;
        }
    }
}