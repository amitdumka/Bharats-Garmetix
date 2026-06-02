using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Billing.Models;
using Garmetix.Billing.Pages.Popups;
using Garmetix.Billing.Services;
using Garmetix.Core.Enums;
using Garmetix.Core.Models.Inventory;
using Garmetix.Databases;
using Garmetix.Databases.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Garmetix.Billing.PageModels.Invoices
{
    public abstract partial class BaseInvoiceFormModel : ObservableObject
    {
        // --- HIGH PERFORMANCE CACHING --- See if requried
        // protected Dictionary<string, Product> _productBarcodeCache = new();
        //protected List<Product> _productNameCache = new();



        protected readonly InvoiceService _invoiceService;

        protected DatabaseContext GetContext() => DatabaseService.Instance.LocalDB;

        // --- STATE MANAGEMENT ---
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        protected bool isBusy;

        public bool IsNotBusy => !IsBusy;

        [ObservableProperty] protected bool isSaving;

        // --- CORE DATA ---
        [ObservableProperty] protected InvoiceDTO currentInvoice = new();

        public ObservableCollection<PaymentDetail> Payments { get; set; } = new();
        public ObservableCollection<EntryItem> InvoiceItems { get; set; } = new();
        [ObservableProperty] protected EntryItem? selectedInvoiceItem;

        // --- PAYMENTS & DISCOUNTS ---
        protected CardPaymentDto _capturedCardDetails;

        [ObservableProperty] protected PaymentMode paymentModeInput = PaymentMode.Cash;
        [ObservableProperty] protected string paymentNarration = string.Empty;
        [ObservableProperty] protected bool _isNarrationVisible = false;
        [ObservableProperty] protected string _narrationPlaceholder;
        [ObservableProperty] protected decimal paymentAmountInput = 0m;

        [ObservableProperty] protected decimal globalDiscountInput;
        [ObservableProperty] protected string globalDiscountTypeInput = "Amount";

        [ObservableProperty] protected bool _isCardPaymentSet = false;

        public IList<PaymentMode> PaymentModes { get; } = Enum.GetValues(typeof(PaymentMode)).Cast<PaymentMode>().ToList();

        // --- UI TOTALS ---
        [ObservableProperty] protected decimal subTotal;

        [ObservableProperty] protected decimal totalTax;
        [ObservableProperty] protected decimal totalDiscount;
        [ObservableProperty] protected decimal roundOffAmount;
        [ObservableProperty] protected decimal grandTotal;
        [ObservableProperty] protected decimal paidAmount;
        [ObservableProperty] protected decimal balanceAmount;

        // --- SEARCH ENGINE ---
        [ObservableProperty] protected string searchText = string.Empty;

        [ObservableProperty] protected Product? selectedProduct;
        public ObservableCollection<Product> FilteredProducts { get; set; } = new();
        protected CancellationTokenSource? _searchCts;
        protected List<Product> _productCache = new(); // Used for local caching if needed

        // --- CUSTOMER INFO (Shared across invoice types, but can be overridden if needed) ---
        [ObservableProperty] protected bool isNewCustomer = false;

        [ObservableProperty] protected bool activeCustomer = false;
        [ObservableProperty] protected string customerMobile = "";
        [ObservableProperty] protected decimal customerBalance = 0;

        protected BaseInvoiceFormModel(InvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
            searchText = "";
            selectedProduct = null;
            selectedInvoiceItem = null;

            CurrentInvoice = new InvoiceDTO();
        }

        // --- ABSTRACT METHODS (To be implemented by derived classes) ---
        public virtual void CalculateTotals()
        {
            try
            {
                SubTotal = InvoiceItems.Sum(i => i.BasePrice * i.BilledQuantity);
                TotalDiscount = InvoiceItems.Sum(i => i.DiscountAmount);
                TotalTax = InvoiceItems.Sum(i => i.TaxAmount);

                decimal preDiscountTotal = (SubTotal - TotalDiscount) + TotalTax;
                decimal globalDiscountCalculated = GlobalDiscountTypeInput == "%" ? preDiscountTotal * (GlobalDiscountInput / 100m) : GlobalDiscountInput;

                decimal rawGrandTotal = preDiscountTotal - globalDiscountCalculated;
                if (rawGrandTotal < 0) rawGrandTotal = 0;

                GrandTotal = Math.Round(rawGrandTotal, 0, MidpointRounding.AwayFromZero);
                RoundOffAmount = GrandTotal - rawGrandTotal;
                PaidAmount = Payments.Sum(p => p.Amount);
                BalanceAmount = GrandTotal - PaidAmount;

                // Sync with DTO
                CurrentInvoice.SubTotal = SubTotal;
                CurrentInvoice.TotalTax = TotalTax;
                CurrentInvoice.GlobalDiscountAmount = globalDiscountCalculated;
                CurrentInvoice.RoundOffAmount = RoundOffAmount;
                CurrentInvoice.GrandTotal = GrandTotal;
                CurrentInvoice.PaidAmount = PaidAmount;
            }
            catch (Exception ex) { _ = InvoiceService.ShowErrorAsync("Calculation Error", ex); }
        }
        public virtual async Task ClearFormAsync()
        {
            // Implementation can be overridden by derived classes
            foreach (var item in InvoiceItems) item.PropertyChanged -= InvoiceItem_PropertyChanged;
            InvoiceItems.Clear();
            Payments.Clear();
            GlobalDiscountInput = 0;
            GlobalDiscountTypeInput = "Amount";
            PaymentModeInput = PaymentMode.Cash;

            PaymentAmountInput = 0; SelectedInvoiceItem = null;
            IsNewCustomer = false;
            SelectedProduct = null;
            CurrentInvoice = new InvoiceDTO();
            OnPropertyChanged(nameof(CurrentInvoice));
            await Task.CompletedTask;
        }

        // 1. The MVVM Toolkit looks for this partial method in the same class the property is declared
        protected partial void OnSelectedProductChanged(Product? value)
        {
            // 2. Route it to a virtual method that derived classes can override
            HandleProductSelected(value);
        }

        // 3. Define the virtual method
        protected virtual void HandleProductSelected(Product? value)
        {
            // You can leave this empty, or put shared logic here (like clearing the search box)

            if (value != null)
            {
                AddProductToInvoice(value);     // Instantly adds to the cart
                SearchText = string.Empty;      // Clears the search box for the next item
            }
        }
        [RelayCommand]
        protected async Task AddProductToInvoiceItem()
        {
            if (SelectedProduct != null)
                AddProductToInvoice(SelectedProduct);
            else
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("No Product Selected", "Please select a product to add.", "OK");
            }
        }
        protected void AddProductToInvoice(Product product)
        {
            //TODO: [RelayCommand]
            if  ( product == null) return;
            try
            {
                var newItem = new EntryItem
                {
                    Barcode = product.Barcode,
                    Category = product.ProductType,
                    BasePrice = product.BasicPrice,
                    ProductName = product.Name,
                    ProductId = product.Id,
                    MRP = product.MRP,
                    Unit = product.Unit,
                    BilledQuantity = 1m,
                    DiscountPercentage = 0,
                    Id = product.Id,
                    InvoiceId = CurrentInvoice.Id,
                };

                newItem.PropertyChanged += InvoiceItem_PropertyChanged;
                InvoiceItems.Add(newItem);
                CalculateTotals();
            }
            catch (Exception ex) { _ = InvoiceService.ShowErrorAsync("Add Product Error", ex); }
        }

        [RelayCommand]
        public void RemoveInvoiceItem(EntryItem item)
        {
            if (item == null || !InvoiceItems.Contains(item)) return;
            item.PropertyChanged -= InvoiceItem_PropertyChanged;
            if (SelectedInvoiceItem == item) SelectedInvoiceItem = null;

            // InvoiceItems.Remove(item);
            // CalculateTotals();
            Application.Current!.Dispatcher.Dispatch(() =>
            {
                try { InvoiceItems.Remove(item); CalculateTotals(); }
                catch (Exception ex) { _ = InvoiceService.ShowErrorAsync("Remove Item Error", ex); }
            });
        }

        protected void InvoiceItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(EntryItem.BasePrice) or nameof(EntryItem.BilledQuantity) or nameof(EntryItem.DiscountAmount))
            {
                CalculateTotals();
            }
        }

        // --- SHARED SEARCH LOGIC ---
        protected partial void OnSearchTextChanged(string value)
        {
            if (SelectedProduct != null) return;
            _ = UpdateFilteredProductsAsync(value);
        }

        protected virtual async Task UpdateFilteredProductsAsync(string text)
        {
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            if (string.IsNullOrWhiteSpace(text) || text.Length < 3)
            {
                MainThread.BeginInvokeOnMainThread(() => FilteredProducts.Clear());
                return;
            }

            try
            {
                await Task.Delay(300, token); // Debounce
                var results = await GetContext().Products
                    .Where(p => EF.Functions.Like(p.Name, $"%{text}%") || EF.Functions.Like(p.Barcode, $"%{text}%"))
                    .Take(20)
                    .ToListAsync(token);

                if (token.IsCancellationRequested) return;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    FilteredProducts.Clear();
                    foreach (var p in results) FilteredProducts.Add(p);
                });
            }
            catch (TaskCanceledException) { /* Harmless, user kept typing */ }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }
        }

        // --- SHARED PAYMENT LOGIC ---
        [RelayCommand]
        public virtual void AddPayment()
        {
            //if (PaymentAmountInput <= 0) return;
            //Payments.Add(new PaymentDetail { PaymentMode = PaymentModeInput, Amount = PaymentAmountInput, PaymentDate = DateTime.Now });
            //PaymentAmountInput = 0;
            //CalculateTotals();
            if (PaymentAmountInput <= 0) return;

            if (PaymentModeInput == PaymentMode.Card && _capturedCardDetails == null)
            {
                _ = InvoiceService.ShowErrorAsync("Payment Error", new Exception("Card details not captured."));
                return;
            }
            else if (PaymentModeInput == PaymentMode.Card && _capturedCardDetails != null)
            {
                Payments.Add(new PaymentDetail
                {
                    PaymentMode = PaymentModeInput,
                    Amount = _capturedCardDetails.Amount,//PaymentAmountInput,
                    PaymentNote = "Card: " + _capturedCardDetails.BankName + " " + _capturedCardDetails.CardType.ToString(),
                    AuthCode = _capturedCardDetails.AuthCode,
                    CardPaymentNumber = _capturedCardDetails.CardNumber,
                    CardPaymentBank = _capturedCardDetails.BankName,
                    Card = _capturedCardDetails.Card,
                    CardType = _capturedCardDetails.CardType
                });
                _capturedCardDetails = null; // Clear after use
            }
            else if (PaymentModeInput == PaymentMode.Cash)
                Payments.Add(new PaymentDetail { PaymentMode = PaymentModeInput, Amount = PaymentAmountInput });
            else

                Payments.Add(new PaymentDetail { PaymentMode = PaymentModeInput, Amount = PaymentAmountInput, PaymentNote = PaymentNarration });

            PaymentAmountInput = 0;
            CalculateTotals();
            PaymentModeInput = PaymentMode.Cash; // Reset to default after adding payment
        }

        [RelayCommand]
        public virtual void RemovePayment(PaymentDetail payment)
        {
            if (payment != null && Payments.Contains(payment))
            {
                Payments.Remove(payment);
                CalculateTotals();
            }
        }

        // 2. Trigger the popup when "Card" is selected
        protected partial void OnPaymentModeInputChanged(PaymentMode value)
        {
            _capturedCardDetails = null;

            if (value == PaymentMode.Card)
            {
                IsNarrationVisible = false;
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    var popup = new CardPaymentPopup();
                    var result = await Shell.Current.CurrentPage.ShowPopupAsync<CardPaymentDto>(popup);// as CardPaymentDto;

                    if (result.Result != null)
                    {
                        _capturedCardDetails = result.Result;
                        PaymentAmountInput = _capturedCardDetails.Amount;
                        AddPayment();
                    }
                    else
                        PaymentModeInput = PaymentMode.Cash; // Revert if cancelled
                });
            }
            else if (value == PaymentMode.Cash)
            {
                IsNarrationVisible = false;
            }
            else
            {
                // Handle UPI/Cheque Narration
                IsNarrationVisible = true;
                // ... set placeholder logic ...
                // UPI, Cheque, Wallet
                IsNarrationVisible = true;
                PaymentNarration = string.Empty;

                // Dynamically change the placeholder based on the mode
                NarrationPlaceholder = value switch
                {
                    PaymentMode.UPI => "Enter UTR / Transaction No.",
                    PaymentMode.Cheque => "Enter Cheque Number & Bank",
                    PaymentMode.Wallets => "Enter Wallet Txn ID",
                    PaymentMode.SaleReturn => "Enter Original Invoice No.",
                    PaymentMode.CreditNote => "Enter Credit Note Details",
                    PaymentMode.RTGS or PaymentMode.NEFT or PaymentMode.IMPS => "Enter Transaction(UTR) Reference",
                    _ => "Enter Reference Details"
                };
            }
        }

        protected partial void OnGlobalDiscountInputChanged(decimal value) => CalculateTotals();

        protected partial void OnGlobalDiscountTypeInputChanged(string value) => CalculateTotals();

        [RelayCommand]
        public virtual async Task GoBackAsync()
        {
            if (!IsBusy) await Shell.Current.GoToAsync("..");
        }

        // --- UTILITIES ---

        /// <summary>
        /// Clear the invoice
        /// </summary>
        /// <returns></returns>
        [RelayCommand] //Working
        public async Task ClearInvoiceAsync()
        {
            if (IsBusy) return;
            if (await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Clear Form", "Clear the entire invoice?", "Yes", "Cancel"))
                await ClearFormAsync();
        }

        // --- Shared Logic for Customer Handling ---
        protected partial void OnCustomerMobileChanged(string value) => SearchCustomerAsync();

        [RelayCommand]
        public async Task SearchCustomerAsync()
        {
            //TODO: Check  Customer Mobile Number  Validation  and Format before searching
            if (string.IsNullOrWhiteSpace(CurrentInvoice.CustomerMobileNumber) || IsBusy) return;
            try
            {
                IsBusy = true;
                var customer = await GetContext().Customers.FirstOrDefaultAsync(c => c.MobileNumber == CurrentInvoice.CustomerMobileNumber);
                if (customer != null)
                {
                    CurrentInvoice.CustomerName = customer.Name;
                    CurrentInvoice.CustomerGSTIN = customer.GSTIN;

                    CustomerBalance = customer.CreditBalance;
                    ActiveCustomer = true;
                    IsNewCustomer = false;
                    OnPropertyChanged(nameof(CurrentInvoice));
                }
                else
                {
                    ActiveCustomer = false;
                    IsNewCustomer = true;
                }
            }
            catch (Exception ex) { await InvoiceService.ShowErrorAsync("Customer Search Error", ex); }
            finally { IsBusy = false; }
        }

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

        //TODO  Handling Invoice Type in the base class might not be ideal if different invoice types have different behaviors.

        // ---   Shared Login For Invoice Type

        protected virtual async Task HandledInvoiceForTypes(Invoice inv)
        {
            // For InvoiceType  Regylar
            // For InvoiceType  Sale Return
            // For  InvoiceType  CashMemo
            // For InvoiceType  Service Invoice

            // For SaleInvoiceType
            // For SaleInvoiceType  B2B
            // For SaleInvoiceType  B2c
            // For SaleInvoiceType  Cashmeno

            // Handling Invoice Status ,
            //Like Paid, Due, Partially Paid, Cancelled etc
        }
    }
}