using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.AI.Billing.Models;
using Garmetix.Billing.AIBased.Helpers;
using Garmetix.Billing.AIBased.Services;
using SQLite;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Garmetix.AI.Billing.ViewModels
{
    public partial class InvoiceEntryViewModel : ObservableObject
    {
        private SQLiteAsyncConnection _database;
        private readonly IPrintService _printService;

        // --- STATE MANAGEMENT ---
        // --- STATE MANAGEMENT ---
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool isBusy;

        public bool IsNotBusy => !IsBusy;

        [ObservableProperty]
        private Invoice currentInvoice;

        public ObservableCollection<InvoiceItem> InvoiceItems { get; set; } = new();
        public ObservableCollection<Product> AvailableProducts { get; set; } = new();
        public ObservableCollection<PaymentDetail> Payments { get; set; } = new();

        [ObservableProperty]
        private Product selectedProduct;

        [ObservableProperty]
        private InvoiceItem selectedInvoiceItem;

        [ObservableProperty]
        private string paymentModeInput = "Cash";

        [ObservableProperty]
        private decimal paymentAmountInput;
        // --- GLOBAL DISCOUNT INPUTS ---
        [ObservableProperty]
        private decimal globalDiscountInput;

        partial void OnGlobalDiscountInputChanged(decimal value) => CalculateInvoiceTotals();

        [ObservableProperty]
        private string globalDiscountTypeInput = "Amount";

        partial void OnGlobalDiscountTypeInputChanged(string value) => CalculateInvoiceTotals();

        [ObservableProperty]
        private bool isNewCustomer = false;

        public InvoiceEntryViewModel(IPrintService printService)
        {
            _printService = printService;
            CurrentInvoice = new Invoice();
            InitializeDatabaseAsync();
        }

        private async void InitializeDatabaseAsync()
        {
            try
            {
                IsBusy = true;
                string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "aadwika_billing.db3");
                _database = new SQLiteAsyncConnection(dbPath);

                await _database.CreateTableAsync<Invoice>();
                await _database.CreateTableAsync<InvoiceItem>();
                await _database.CreateTableAsync<Product>();
                await _database.CreateTableAsync<Customer>();

                // Load Dummy Data if empty
                if (await _database.Table<Product>().CountAsync() == 0)
                {
                    AvailableProducts.Add(new Product { Name = "Cotton Kurta", Barcode = "1001", BaseRate = 1500, Category = GarmentCategory.ReadyMade });
                    AvailableProducts.Add(new Product { Name = "Silk Saree Fabric", Barcode = "1002", BaseRate = 3000, Category = GarmentCategory.Fabric });
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Database Initialization Failed", ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task SearchCustomerAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentInvoice.MobileNo) || IsBusy) return;

            try
            {
                IsBusy = true;
                var customer = await _database.Table<Customer>().FirstOrDefaultAsync(c => c.MobileNo == CurrentInvoice.MobileNo);
                if (customer != null)
                {
                    CurrentInvoice.CustomerName = customer.Name;
                    CurrentInvoice.Gstin = customer.Gstin;
                    IsNewCustomer = false;
                    OnPropertyChanged(nameof(CurrentInvoice));
                }
                else
                {
                    IsNewCustomer = true;
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Customer Search Error", ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task SaveCustomerAsync()
        {
            if (IsBusy) return;
            if (string.IsNullOrWhiteSpace(CurrentInvoice.MobileNo) || string.IsNullOrWhiteSpace(CurrentInvoice.CustomerName))
            {
                await Application.Current.MainPage.DisplayAlert("Validation", "Please enter a valid Mobile No and Name.", "OK");
                return;
            }

            try
            {
                IsBusy = true;
                var existing = await _database.Table<Customer>().FirstOrDefaultAsync(c => c.MobileNo == CurrentInvoice.MobileNo);
                if (existing == null)
                {
                    await _database.InsertAsync(new Customer
                    {
                        MobileNo = CurrentInvoice.MobileNo,
                        Name = CurrentInvoice.CustomerName,
                        Gstin = CurrentInvoice.Gstin
                    });
                    IsNewCustomer = false;
                    await Application.Current.MainPage.DisplayAlert("Success", "Customer details saved successfully.", "OK");
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Save Customer Error", ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public void AddProductToInvoice()
        {
            if (SelectedProduct == null) return;

            try
            {
                var newItem = new InvoiceItem
                {
                    ProductName = SelectedProduct.Name,
                    Category = SelectedProduct.Category,
                    Rate = SelectedProduct.BaseRate,
                    Quantity = 1,
                    DiscountAmount = 0
                };

                newItem.PropertyChanged += InvoiceItem_PropertyChanged;
                InvoiceItems.Add(newItem);
                SelectedProduct = null;
                CalculateInvoiceTotals();
            }
            catch (Exception ex)
            {
                _ = ShowErrorAsync("Failed to add product", ex);
            }
        }

        [RelayCommand]
        public void RemoveInvoiceItem(InvoiceItem item)
        {
            if (item == null || !InvoiceItems.Contains(item)) return;

            item.PropertyChanged -= InvoiceItem_PropertyChanged;

            if (SelectedInvoiceItem == item)
                SelectedInvoiceItem = null;

            Application.Current.Dispatcher.Dispatch(() =>
            {
                try
                {
                    InvoiceItems.Remove(item);
                    CalculateInvoiceTotals();
                }
                catch (Exception ex)
                {
                    _ = ShowErrorAsync("Failed to remove item", ex);
                }
            });
        }
        public void CalculateInvoiceTotals()
        {
            try
            {
                // 1. Calculate Item-Level Totals
                CurrentInvoice.SubTotal = InvoiceItems.Sum(i => (i.Rate * i.Quantity));
                CurrentInvoice.TotalDiscount = InvoiceItems.Sum(i => i.DiscountAmount); // Item discounts
                CurrentInvoice.TotalTax = InvoiceItems.Sum(i => i.TaxAmount);

                // 2. Pre-Global Discount Total
                decimal preDiscountTotal = (CurrentInvoice.SubTotal - CurrentInvoice.TotalDiscount) + CurrentInvoice.TotalTax;

                // 3. Apply Global Bill Discount (Amount or Percentage)
                if (GlobalDiscountTypeInput == "%")
                {
                    CurrentInvoice.GlobalDiscountAmount = preDiscountTotal * (GlobalDiscountInput / 100m);
                }
                else // "Amount"
                {
                    CurrentInvoice.GlobalDiscountAmount = GlobalDiscountInput;
                }

                // 4. Exact Grand Total after Global Discount
                decimal exactGrandTotal = preDiscountTotal - CurrentInvoice.GlobalDiscountAmount;

                // Prevent negative totals if the user enters a discount larger than the bill
                if (exactGrandTotal < 0) exactGrandTotal = 0;

                // 5. Apply Round Off
                CurrentInvoice.GrandTotal = Math.Round(exactGrandTotal, 0, MidpointRounding.AwayFromZero);
                CurrentInvoice.RoundOffAmount = CurrentInvoice.GrandTotal - exactGrandTotal;

                // 6. Calculate Balances
                CurrentInvoice.PaidAmount = Payments.Sum(p => p.Amount);
            }
            catch (Exception ex)
            {
                _ = ShowErrorAsync("Calculation Error", ex);
            }
        }
        private void InvoiceItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(InvoiceItem.Rate) or nameof(InvoiceItem.Quantity) or nameof(InvoiceItem.DiscountAmount))
            {
                CalculateInvoiceTotals();
            }
        }

        [RelayCommand]
        public void AddPayment()
        {
            if (PaymentAmountInput <= 0) return;

            Payments.Add(new PaymentDetail { Mode = PaymentModeInput, Amount = PaymentAmountInput });
            PaymentAmountInput = 0;
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

        public void CalculateInvoiceTotals()
        {
            try
            {
                CurrentInvoice.SubTotal = InvoiceItems.Sum(i => (i.Rate * i.Quantity));
                CurrentInvoice.TotalDiscount = InvoiceItems.Sum(i => i.DiscountAmount);
                CurrentInvoice.TotalTax = InvoiceItems.Sum(i => i.TaxAmount);

                decimal exactGrandTotal = (CurrentInvoice.SubTotal - CurrentInvoice.TotalDiscount) + CurrentInvoice.TotalTax;
                CurrentInvoice.GrandTotal = Math.Round(exactGrandTotal, 0, MidpointRounding.AwayFromZero);
                CurrentInvoice.RoundOffAmount = CurrentInvoice.GrandTotal - exactGrandTotal;
                CurrentInvoice.PaidAmount = Payments.Sum(p => p.Amount);
            }
            catch (Exception ex)
            {
                _ = ShowErrorAsync("Calculation Error", ex);
            }
        }

        [RelayCommand]
        public async Task SaveInvoiceAsync()
        {
            if (InvoiceItems.Count == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Validation", "Cannot save an empty invoice. Please add items.", "OK");
                return;
            }
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                CurrentInvoice.InvoiceNo = "INV-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
                CalculateInvoiceTotals();

                if (CurrentInvoice.PaidAmount < CurrentInvoice.GrandTotal)
                {
                    bool proceed = await Application.Current.MainPage.DisplayAlert(
                        "Part Payment",
                        $"Balance of Rs. {CurrentInvoice.BalanceAmount} is unpaid. Proceed with saving?",
                        "Yes", "No");

                    if (!proceed) return;
                }

                // Database Transaction
                await _database.RunInTransactionAsync(tran =>
                {
                    tran.Insert(CurrentInvoice);
                    foreach (var item in InvoiceItems)
                    {
                        item.InvoiceId = CurrentInvoice.Id;
                        tran.Insert(item);
                    }
                });

                // Generate and Print
                byte[] rawReceiptBytes = ReceiptBuilder.GenerateInvoiceBytes(CurrentInvoice, InvoiceItems);
                await _printService.PrintReceiptAsync(rawReceiptBytes);

                // Reset Form
                foreach (var item in InvoiceItems) item.PropertyChanged -= InvoiceItem_PropertyChanged;
                InvoiceItems.Clear();
                Payments.Clear();
                CurrentInvoice = new Invoice();
                OnPropertyChanged(nameof(CurrentInvoice));

                await Application.Current.MainPage.DisplayAlert("Success", "Invoice saved and printed successfully.", "OK");
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Failed to Save Invoice", ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Standardized Error Helper
        private async Task ShowErrorAsync(string title, Exception ex)
        {
            // In a production app, log 'ex.Message' to AppCenter or Crashlytics here.
            await Application.Current.MainPage.DisplayAlert(title, $"An unexpected error occurred: {ex.Message}", "OK");
        }
    }
}