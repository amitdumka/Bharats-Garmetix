using Garmetix.Billing.PageModels;
using Kotlin.Time;
using Syncfusion.Maui.Inputs;
using System.ComponentModel;

namespace Garmetix.Billing.Pages
{
    public partial class InvoiceEntryPage : ContentPage
    {
        public InvoiceEntryPage(InvoiceEntryPageModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private void MobileNoEntry_Unfocused(object sender, FocusEventArgs e)
        {
            var vm = BindingContext as InvoiceEntryPageModel;
            if (vm != null && vm.SearchCustomerCommand.CanExecute(null))
            {
                vm.SearchCustomerCommand.Execute(null);
            }
        }
        //TODO:  --- NEW: Wires the Search Box to the ViewModel Cache ---
        //private void ProductSearch_TextChanged(object sender, Syncfusion.Maui.Inputs.TextChangedEventArgs e)
        //{
        //    if (BindingContext is  InvoiceEntryPageModel vm)
        //    {
        //        // Passes the typed text (Barcode or Name) into the high-speed search engine
        //        vm.UpdateFilteredProducts(e.NewTextValue);
        //    }
        //}
    }
}


//using System;
//using System.ComponentModel;
//using System.Globalization;
//using System.Linq;
//using System.Reflection;
//using Microsoft.Maui.Controls.Xaml;

//public class EnumToCollectionExtension : IValueProvider
//{
//    public Type EnumType { get; set; }

//    public object ProvideValue(IServiceProvider serviceProvider)
//    {
//        if (EnumType == null) return null;

//        return Enum.GetValues(EnumType)
//            .Cast<Enum>()
//            .Select(e => new
//            {
//                Value = e,
//                Display = GetDescription(e)
//            }).ToList();
//    }

//    private string GetDescription(Enum value)
//    {
//        FieldInfo field = value.GetType().GetField(value.ToString());
//        DescriptionAttribute attribute = field?.GetCustomAttribute<DescriptionAttribute>();
//        return attribute != null ? attribute.Description : value.ToString();
//    }
//}

//< !--Syncfusion ComboBox bound dynamically to your Enum -->
//    <inputs:SfComboBox Grid.Column = "0"
//                       ItemsSource = "{local:EnumToCollection EnumType={x:Type local:PaymentMode}}"
//                       DisplayMemberPath = "Display"
//                       SelectedValuePath = "Value"
//                       SelectedValue = "{Binding PaymentModeInput}"
//                       HeightRequest = "40" />