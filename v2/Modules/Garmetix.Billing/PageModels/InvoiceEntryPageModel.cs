using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Core.Models.Inventory;
using Garmetix.Databases;
using Garmetix.Databases.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Garmetix.Core.Enums;
using Garmetix.Billing.Models;
using Garmetix.Billing.Services;

namespace Garmetix.Billing.PageModels
{
    internal partial class InvoiceEntryPageModel : ObservableObject
    {
        private InvoiceService _invoiceService;

        // --- NEW: Search Text Binding ---
        [ObservableProperty]
        private string searchText;

        // This automatically fires the moment you type a letter in the UI
        private partial void OnSearchTextChanged(string value)
        {
            UpdateFilteredProducts(value);
        }

        // NEW: This automatically fires the moment you click an item in the search list
        private partial void OnSelectedProductChanged(Product? value)
        {
            if (value != null)
            {
                AddProductToInvoice();     // Instantly adds to the cart
                SearchText = string.Empty; // Clears the search box for the next item
            }
        }

        // --- HIGH PERFORMANCE CACHING ---
        private Dictionary<string, Product> _productBarcodeCache = new();

        private List<Product> _productNameCache = new();

        public ObservableCollection<Product> FilteredProducts { get; set; } = new();

        // --- STATE MANAGEMENT ---
        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsNotBusy))] private bool isBusy;

        public bool IsNotBusy => !IsBusy;

        [ObservableProperty] private InvoiceDTO currentInvoice;
        public ObservableCollection<EntryItem> InvoiceItems { get; set; } = new();
        public ObservableCollection<InvoicePayment> Payments { get; set; } = new();
        // The UI only binds to this small filtered list to prevent lagging

        [ObservableProperty] private Product? selectedProduct;
        [ObservableProperty] private EntryItem? selectedInvoiceItem;
        [ObservableProperty] private PaymentMode paymentModeInput = PaymentMode.Cash;
        [ObservableProperty] private decimal paymentAmountInput = 0m;
        [ObservableProperty] private bool isNewCustomer = false;

        // --- GLOBAL DISCOUNT INPUTS ---
        [ObservableProperty] private decimal globalDiscountInput;

        private partial void OnGlobalDiscountInputChanged(decimal value) => CalculateInvoiceTotals();

        [ObservableProperty] private string globalDiscountTypeInput = "Amount";

        private partial void OnGlobalDiscountTypeInputChanged(string value) => CalculateInvoiceTotals();

        private DatabaseContext _localDb = DatabaseService.Instance.LocalDB;

        public DatabaseContext GetContext() => _localDb;

        public InvoiceEntryPageModel(InvoiceService invoiceService)
        {
            _invoiceService = invoiceService;

            searchText = ""; selectedProduct = null;
            selectedInvoiceItem = null;

            CurrentInvoice = new InvoiceDTO();
        }

        // Called when the user types in the search box
        public void UpdateFilteredProducts_old(string searchText)
        {
            FilteredProducts.Clear();
            var results = string.IsNullOrWhiteSpace(searchText)
                ? _productNameCache.Take(50)
                : _productNameCache.Where(p => p.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) || p.Barcode.Contains(searchText))
                                   .Take(50);

            foreach (var p in results) FilteredProducts.Add(p);
        }

        public void UpdateFilteredProducts(string searchText)
        {
            // Run the LINQ query in memory
            var results = string.IsNullOrWhiteSpace(searchText)
                ? _productNameCache.Take(50).ToList()
                : _productNameCache.Where(p =>
                    (p.Name != null && p.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                    (p.Barcode != null && p.Barcode.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                ).Take(50).ToList();

            // Safely push the results to the UI thread
            Application.Current?.Dispatcher.Dispatch(() =>
            {
                FilteredProducts.Clear();
                foreach (var p in results)
                {
                    FilteredProducts.Add(p);
                }
            });
        }

        // --- CUSTOMER LOGIC ---
        [RelayCommand]
        public async Task SearchCustomerAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentInvoice.CustomerMobileNumber) || IsBusy) return;
            try
            {
                IsBusy = true;
                var customer = await GetContext().Customers.FirstOrDefaultAsync(c => c.MobileNumber == CurrentInvoice.CustomerMobileNumber);
                if (customer != null)
                {
                    CurrentInvoice.CustomerName = customer.Name;
                    CurrentInvoice.CustomerGSTIN = customer.GSTIN;
                    IsNewCustomer = false;
                    OnPropertyChanged(nameof(CurrentInvoice));
                }
                else IsNewCustomer = true;
            }
            catch (Exception ex) { await InvoiceService.ShowErrorAsync("Customer Search Error", ex); }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        public async Task SaveCustomerAsync()
        {
            if (IsBusy || string.IsNullOrWhiteSpace(CurrentInvoice.CustomerMobileNumber) || string.IsNullOrWhiteSpace(CurrentInvoice.CustomerName)) return;
            try
            {
                IsBusy = true;
                var existing = await GetContext().Customers.FirstOrDefaultAsync(c => c.MobileNumber == CurrentInvoice.CustomerMobileNumber);
                if (existing == null)
                {
                    await GetContext().Customers.AddAsync(new Customer { MobileNumber = CurrentInvoice.CustomerMobileNumber, Name = CurrentInvoice.CustomerName, GSTIN = CurrentInvoice.CustomerGSTIN });
                    IsNewCustomer = false;
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Success", "Customer saved.", "OK");
                }
            }
            catch (Exception ex) { await InvoiceService.ShowErrorAsync("Save Customer Error", ex); }
            finally { IsBusy = false; }
        }

        // --- CART LOGIC ---

        [RelayCommand]
        public void AddProductToInvoice()
        {
            if (SelectedProduct == null) return;
            try
            {
                var newItem = new EntryItem
                {
                    Barcode = SelectedProduct.Barcode,
                    Category = SelectedProduct.ProductType,
                    BasePrice = SelectedProduct.BasicPrice,

                    // CHANGED: Use 1m to signify 1 as a decimal

                    BilledQuantity = 1m,

                    DiscountPercentage = 0
                };

                newItem.PropertyChanged += InvoiceItem_PropertyChanged;
                InvoiceItems.Add(newItem);
                CalculateInvoiceTotals();
            }
            catch (Exception ex) { _ =InvoiceService.ShowErrorAsync("Add Product Error", ex); }
        }

        [RelayCommand]
        public void RemoveInvoiceItem(EntryItem item)
        {
            if (item == null || !InvoiceItems.Contains(item)) return;
            item.PropertyChanged -= InvoiceItem_PropertyChanged;
            if (SelectedInvoiceItem == item) SelectedInvoiceItem = null;

            Application.Current.Dispatcher.Dispatch(() =>
            {
                try { InvoiceItems.Remove(item); CalculateInvoiceTotals(); }
                catch (Exception ex) { _ = InvoiceService.ShowErrorAsync("Remove Item Error", ex); }
            });
        }

        private void InvoiceItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(EntryItem.BasePrice) or nameof(EntryItem.BilledQuantity) or nameof(EntryItem.DiscountAmount) or nameof(EntryItem.DiscountAmount))
            {
                CalculateInvoiceTotals();
            }
        }


        // --- PAYMENT LOGIC ---
        [RelayCommand]
        public void AddPayment()
        {
            if (PaymentAmountInput <= 0) return;
            Payments.Add(new InvoicePayment { PaymentMode = PaymentModeInput, Amount = PaymentAmountInput });
            PaymentAmountInput = 0;
            CalculateInvoiceTotals();
        }

        [RelayCommand]
        public void RemovePayment(InvoicePayment payment)
        {
            if (Payments.Contains(payment))
            {
                Payments.Remove(payment);
                CalculateInvoiceTotals();
            }
        }

        [Obsolete]
        public void CalculateInvoiceTotals()
        {
            //TODO: need to reclaibrated for actual result, it has bug and it not proper
            try
            {
                CurrentInvoice.SubTotal = InvoiceItems.Sum(i => (i.BasePrice * i.BilledQuantity));
                CurrentInvoice.TotalDiscount = InvoiceItems.Sum(i => i.DiscountAmount);
                CurrentInvoice.TotalTax = InvoiceItems.Sum(i => i.TaxAmount);

                decimal preDiscountTotal = (CurrentInvoice.SubTotal - CurrentInvoice.TotalDiscount) + CurrentInvoice.TotalTax;
                CurrentInvoice.GlobalDiscountAmount = GlobalDiscountTypeInput == "%" ? preDiscountTotal * (GlobalDiscountInput / 100m) : GlobalDiscountInput;

                decimal exactGrandTotal = preDiscountTotal - CurrentInvoice.GlobalDiscountAmount;
                if (exactGrandTotal < 0) exactGrandTotal = 0;

                CurrentInvoice.GrandTotal = Math.Round(exactGrandTotal, 0, MidpointRounding.AwayFromZero);
                CurrentInvoice.RoundOffAmount = CurrentInvoice.GrandTotal - exactGrandTotal;
                CurrentInvoice.PaidAmount = Payments.Sum(p => p.Amount);
            }
            catch (Exception ex) { _ = InvoiceService.ShowErrorAsync("Calculation Error", ex); }
        }




        // --- DATABASE SAVE ENGINE and FINAL ACTION COMMANDS ---
        [RelayCommand]
        public async Task SaveAndWhatsAppAsync()
        {
            if (_invoiceService.SaveAndPrint(CurrentInvoice, InvoiceItems, Payments, print: false, sendOverMsg: true))
            {
                ResetFormWithoutPrompt();
            }
        }

        [RelayCommand]
        public async Task SaveAndPrintA5Async()
        {
            if (_invoiceService.SaveAndPrint(CurrentInvoice, InvoiceItems, Payments, print: true, thermal: false sendOverMsg: false))
            {
                ResetFormWithoutPrompt();
            }
        }

        [RelayCommand]
        public async Task SaveAndPrintThermalAsync()
        {
            if (_invoiceService.SaveAndPrint(CurrentInvoice, InvoiceItems, Payments, print: true, thermal: true sendOverMsg: false))
            {
                ResetFormWithoutPrompt();
            }
        }

        // --- UTILITIES ---
        /// <summary>
        /// Go back to main or list page
        /// </summary>
        /// <returns></returns>

        [RelayCommand]
        public async Task GoBackAsync()
        { if (!IsBusy) await Shell.Current.GoToAsync(".."); }

        /// <summary>
        /// Clear the invoice
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task ClearInvoiceAsync()
        {
            if (IsBusy) return;
            if (await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Clear Form", "Clear the entire invoice?", "Yes", "Cancel"))
                ResetFormWithoutPrompt();
        }

        /// <summary>
        /// Reset the form without prompt
        /// </summary>
        private void ResetFormWithoutPrompt()
        {
            foreach (var item in InvoiceItems) item.PropertyChanged -= InvoiceItem_PropertyChanged;
            InvoiceItems.Clear(); Payments.Clear();
            GlobalDiscountInput = 0; GlobalDiscountTypeInput = "Amount";
            PaymentAmountInput = 0; PaymentModeInput = PaymentMode.Cash;
            IsNewCustomer = false; SelectedProduct = null; SelectedInvoiceItem = null;
            CurrentInvoice = new InvoiceDTO();
            OnPropertyChanged(nameof(CurrentInvoice));
        }
    }
}