using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Billing.Models;
using Garmetix.Billing.PageModels.Invoices;
using Garmetix.Billing.Pages.Popups;
using Garmetix.Billing.Services;
using Garmetix.Core.Enums;
using Garmetix.Core.Models.Inventory;
using Garmetix.Databases.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Garmetix.Billing.PageModels
{
    [Obsolete("This page model is being refactored, Used Invoices.InvoiceEntryPageModel instead")]
    public partial class InvoiceEntryPageModel : BaseInvoiceFormModel
    {
        // --- HIGH PERFORMANCE CACHING ---
        protected Dictionary<string, Product> _productBarcodeCache = new();

        protected List<Product> _productNameCache = new();

        [ObservableProperty] protected Product? selectedProduct;

        private partial void OnCustomerMobileChanged(string value) => SearchCustomerAsync();

        

        [RelayCommand]
        public async Task SaveCustomerAsync()
        {
            //TODO: move the save logic to service and also add update logic for existing customer, currently it only adds new customer, it does not update existing customer details
            //TODO: Mobile number is empty check for error and also check for existing customer with same mobile number
            if (IsBusy || string.IsNullOrWhiteSpace(CurrentInvoice.CustomerMobileNumber) || string.IsNullOrWhiteSpace(CurrentInvoice.CustomerName)) return;
            try
            {
                IsBusy = true;
                var existing = await GetContext().Customers.FirstOrDefaultAsync(c => c.MobileNumber == CurrentInvoice.CustomerMobileNumber);
                if (existing == null)
                {
                    await GetContext().Customers.AddAsync(new Customer { MobileNumber = CurrentInvoice.CustomerMobileNumber, Name = CurrentInvoice.CustomerName, GSTIN = CurrentInvoice.CustomerGSTIN, CompanyId = DatabaseService.CompanyId });
                    IsNewCustomer = (await GetContext().SaveChangesAsync()) > 0;
                    if (IsNewCustomer)
                    {
                        await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Success", "Customer saved.", "OK");
                        IsNewCustomer = false;
                    }
                    else
                    {
                        await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Success", "Failed to Customer save.", "OK");
                    }
                }
            }
            catch (Exception ex) { await InvoiceService.ShowErrorAsync("Save Customer Error", ex); }
            finally { IsBusy = false; }
        }

       

         

        

        
        

        

        
        

        public override void CalculateTotals()
        {
            throw new NotImplementedException();
        }

        public override Task ClearFormAsync()
        {
            throw new NotImplementedException();
        }
    }
}