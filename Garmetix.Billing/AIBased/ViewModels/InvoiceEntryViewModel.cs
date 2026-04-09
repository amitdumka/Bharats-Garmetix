using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SQLite;
using Garmetix.AI.Billing.Models;
using Microsoft.Maui.Controls;
using Garmetix.Billing.AIBased.Services;
using Garmetix.Billing.AIBased.Helpers;

namespace Garmetix.AI.Billing.ViewModels
{
    public partial class InvoiceEntryViewModel : ObservableObject
    {
        [ObservableProperty]
        private InvoiceItem selectedInvoiceItem;
        // ADD THIS PROPERTY at the top of your ViewModel with your other properties:
        [ObservableProperty]
        private bool isNewCustomer = false;

        private SQLiteAsyncConnection _database;
        private readonly IPrintService _printService;

        [ObservableProperty]
        private Invoice currentInvoice;

        public ObservableCollection<InvoiceItem> InvoiceItems { get; set; } = new();
        public ObservableCollection<Product> AvailableProducts { get; set; } = new();
        public ObservableCollection<PaymentDetail> Payments { get; set; } = new();

        [ObservableProperty]
        private Product selectedProduct;

        // Payment Sub-form properties
        [ObservableProperty]
        private string paymentModeInput = "Cash";

        [ObservableProperty]
        private decimal paymentAmountInput;

        public InvoiceEntryViewModel(IPrintService printService)
        {
            _printService = printService;
            CurrentInvoice = new Invoice();

            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "aadwika_billing.db3");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Invoice>().Wait();
            _database.CreateTableAsync<InvoiceItem>().Wait();
            _database.CreateTableAsync<Product>().Wait();
            _database.CreateTableAsync<Customer>().Wait(); // Added Customer Table

            // Load Dummy Data for testing
            AvailableProducts.Add(new Product { Name = "Cotton Kurta", Barcode = "1001", BaseRate = 1500, Category = GarmentCategory.ReadyMade });
            AvailableProducts.Add(new Product { Name = "Silk Saree Fabric", Barcode = "1002", BaseRate = 3000, Category = GarmentCategory.Fabric });
        }

        // --- FEATURE: Customer Lookup & Save ---
        // UPDATE your SearchCustomerAsync method:
        [RelayCommand]
        public async Task SearchCustomerAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentInvoice.MobileNo)) return;

            var customer = await _database.Table<Customer>().FirstOrDefaultAsync(c => c.MobileNo == CurrentInvoice.MobileNo);
            if (customer != null)
            {
                CurrentInvoice.CustomerName = customer.Name;
                CurrentInvoice.Gstin = customer.Gstin;
                IsNewCustomer = false; // Hide the save button
                OnPropertyChanged(nameof(CurrentInvoice));
            }
            else
            {
                // Customer not found, show the save button
                IsNewCustomer = true;
            }
        }

        // UPDATE your SaveCustomerAsync method to hide the button after saving:
        [RelayCommand]
        public async Task SaveCustomerAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentInvoice.MobileNo) || string.IsNullOrWhiteSpace(CurrentInvoice.CustomerName))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please enter Mobile No and Name to save.", "OK");
                return;
            }

            var existing = await _database.Table<Customer>().FirstOrDefaultAsync(c => c.MobileNo == CurrentInvoice.MobileNo);
            if (existing == null)
            {
                await _database.InsertAsync(new Customer
                {
                    MobileNo = CurrentInvoice.MobileNo,
                    Name = CurrentInvoice.CustomerName,
                    Gstin = CurrentInvoice.Gstin
                });
                await Application.Current.MainPage.DisplayAlert("Success", "Customer saved to database.", "OK");
                IsNewCustomer = false; // Hide the button now that they are saved
            }
        }

        // --- FEATURE: Add/Delete Items & Auto-Calculate ---
        [RelayCommand]
        public void AddProductToInvoice()
        {
            if (SelectedProduct == null) return;

            var newItem = new InvoiceItem
            {
                ProductName = SelectedProduct.Name,
                Category = SelectedProduct.Category,
                Rate = SelectedProduct.BaseRate,
                Quantity = 1,
                DiscountAmount = 0
            };

            // Subscribe to changes so editing the grid updates totals
            newItem.PropertyChanged += InvoiceItem_PropertyChanged;

            InvoiceItems.Add(newItem);
            SelectedProduct = null;
            CalculateInvoiceTotals();
        }

        // UPDATE your RemoveInvoiceItem method to prevent the crash:
        // 1. UPDATE THE DELETE COMMAND
        [RelayCommand]
        public async Task RemoveInvoiceItemAsync(InvoiceItem item) // Added Async to the name
        {
            if (item != null && InvoiceItems.Contains(item))
            {
                // CRITICAL FIX: Yield the thread for a fraction of a second.
                // This stops Syncfusion from crashing when deleting the row you just clicked.
                await Task.Delay(50);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    item.PropertyChanged -= InvoiceItem_PropertyChanged;
                    InvoiceItems.Remove(item);
                    CalculateInvoiceTotals();
                });
            }
        }

        private void InvoiceItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Triggers when user edits Rate, Qty, or Discount in the SfDataGrid
            if (e.PropertyName is nameof(InvoiceItem.Rate) or nameof(InvoiceItem.Quantity) or nameof(InvoiceItem.DiscountAmount))
            {
                CalculateInvoiceTotals();
            }
        }

        // --- FEATURE: Multiple Payment Modes ---
        [RelayCommand]
        public void AddPayment()
        {
            if (PaymentAmountInput <= 0) return;

            Payments.Add(new PaymentDetail { Mode = PaymentModeInput, Amount = PaymentAmountInput });
            PaymentAmountInput = 0; // Reset input field
            CalculateInvoiceTotals();
        }

        [RelayCommand]
        public void RemovePayment(PaymentDetail payment)
        {
            if (Payments.Contains(payment))
            {
                Payments.Remove(payment);
                CalculateInvoiceTotals();
            }
        }

        // 2. UPDATE THE CALCULATIONS
        public void CalculateInvoiceTotals()
        {
            CurrentInvoice.SubTotal = InvoiceItems.Sum(i => (i.Rate * i.Quantity));
            CurrentInvoice.TotalDiscount = InvoiceItems.Sum(i => i.DiscountAmount);
            CurrentInvoice.TotalTax = InvoiceItems.Sum(i => i.TaxAmount);

            // Calculate exact total first
            decimal exactGrandTotal = (CurrentInvoice.SubTotal - CurrentInvoice.TotalDiscount) + CurrentInvoice.TotalTax;

            // Round to nearest whole number
            CurrentInvoice.GrandTotal = Math.Round(exactGrandTotal, 0, MidpointRounding.AwayFromZero);

            // Calculate the Round Off difference (e.g., if Exact is 100.40, Grand is 100. RoundOff is -0.40)
            CurrentInvoice.RoundOffAmount = CurrentInvoice.GrandTotal - exactGrandTotal;

            CurrentInvoice.PaidAmount = Payments.Sum(p => p.Amount);

            // REMOVED: OnPropertyChanged(nameof(CurrentInvoice)); 
            // Do not put that line back! Your properties are Observable, they update the UI automatically.
        }

        [RelayCommand]
        public async Task SaveInvoiceAsync()
        {
            if (InvoiceItems.Count == 0) return;

            CurrentInvoice.InvoiceNo = "INV-" + DateTime.Now.Ticks.ToString();
            CalculateInvoiceTotals();

            if (CurrentInvoice.PaidAmount < CurrentInvoice.GrandTotal)
            {
                var page = Application.Current?.Windows[0]?.Page;
                if (page == null) return;

                bool proceed = await page.DisplayAlert("Part Payment", $"Balance of Rs. {CurrentInvoice.BalanceAmount} remaining. Proceed?", "Yes", "No");
                if (!proceed) return;
            }

            // Save Invoice
            await _database.InsertAsync(CurrentInvoice);

            // Save Items
            foreach (var item in InvoiceItems)
            {
                item.InvoiceId = CurrentInvoice.Id;
                await _database.InsertAsync(item);
            }

            // Generate and Print (Pass Payments list if your ReceiptBuilder supports it)
            byte[] rawReceiptBytes = ReceiptBuilder.GenerateInvoiceBytes(CurrentInvoice, InvoiceItems);
            await _printService.PrintReceiptAsync(rawReceiptBytes);

            // Reset Form
            foreach (var item in InvoiceItems) item.PropertyChanged -= InvoiceItem_PropertyChanged;
            InvoiceItems.Clear();
            Payments.Clear();
            CurrentInvoice = new Invoice();
            OnPropertyChanged(nameof(CurrentInvoice));
        }
    }
}