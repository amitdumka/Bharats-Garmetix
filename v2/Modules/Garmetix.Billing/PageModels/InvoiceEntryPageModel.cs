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

namespace Garmetix.Billing.PageModels
{
    public partial class InvoiceEntryPageModel : ObservableObject
    {
        // 1. Temporary holding variable for the DTO
        private CardPaymentDto _capturedCardDetails;

        [ObservableProperty] private bool _isCardPaymentSet = false;

        protected InvoiceService _invoiceService;
        protected DatabaseContext _localDb = DatabaseService.Instance.LocalDB;
        public DatabaseContext GetContext() => _localDb;

        protected CancellationTokenSource _searchCts;

        // --- NEW: Search Text Binding ---
        [ObservableProperty] protected string searchText;

        //---------- Payment Mode and Narration Inputs ----------
        //[ObservableProperty] protected PaymentMode paymentModeInput = PaymentMode.Cash;

        [ObservableProperty] protected decimal paymentAmountInput = 0m;

        [ObservableProperty] protected PaymentMode _selectedPaymentMode = PaymentMode.Cash;
        [ObservableProperty] protected bool _isNarrationVisible = false;
        [ObservableProperty] protected string _paymentNarration = string.Empty;
        [ObservableProperty] protected string _narrationPlaceholder;

        // NEW: Expose the enum values as a list for the ComboBox ItemsSource
        public IList<PaymentMode> PaymentModes { get; } = Enum.GetValues(typeof(PaymentMode)).Cast<PaymentMode>().ToList();

        // --- STATE MANAGEMENT ---
        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsNotBusy))] protected bool isBusy;

        public bool IsNotBusy => !IsBusy;
        [ObservableProperty] protected bool isNewCustomer = false;  //TODO: Handle this
        [ObservableProperty] protected bool activeCustomer = false;  //TODO: Handle this

        // --- GLOBAL DISCOUNT INPUTS ---
        [ObservableProperty] protected decimal globalDiscountInput;
        [ObservableProperty] protected string globalDiscountTypeInput = "Amount";

        [ObservableProperty] protected string customerMobile = "";
        [ObservableProperty] protected decimal customerBalance = 0;
        // --- HIGH PERFORMANCE CACHING ---
        protected Dictionary<string, Product> _productBarcodeCache = new();

        protected List<Product> _productNameCache = new();

        [ObservableProperty] protected InvoiceDTO currentInvoice;
        public ObservableCollection<EntryItem> InvoiceItems { get; set; } = new();
        public ObservableCollection<PaymentDetail> Payments { get; set; } = new();
        public ObservableCollection<Product> FilteredProducts { get; set; } = new();
        [ObservableProperty] protected Product? selectedProduct;

        [ObservableProperty] protected EntryItem? selectedInvoiceItem;


        partial void OnSearchTextChanged(string value)
        {
            // If a product was just selected, DO NOT run another search
            if (SelectedProduct != null) return;

            _ = UpdateFilteredProductsAsync(value);
        }

        // NEW: This automatically fires the moment you click an item in the search list
        partial void OnSelectedProductChanged(Product? value)
        {
            if (value != null)
            {
                AddProductToInvoice();     // Instantly adds to the cart
                SearchText = string.Empty; // Clears the search box for the next item
            }
        }
        partial void OnCustomerMobileChanged(string value) => SearchCustomerAsync();

        // --- CUSTOMER LOGIC ---
        [RelayCommand]
        public async Task SearchCustomerAsync()
        {
            //TODO: Mobile number is empty check for error 
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
        // Add this field to your ViewModel to track ongoing searches

        protected async Task UpdateFilteredProductsAsync(string text)
        {
            // 1. Cancel any ongoing database queries because the user kept typing
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            // 2. You MUST clear the UI if the text is invalid
            if (string.IsNullOrWhiteSpace(text) || text.Length < 5)
            {
                MainThread.BeginInvokeOnMainThread(() => FilteredProducts.Clear());
                return;
            }

            try
            {
                // 3. Debounce: Wait 300ms to see if the user is still typing before hitting the DB
                await Task.Delay(300, token);

                // Pass the token to ToListAsync so it cancels safely
                // Inside UpdateFilteredProductsAsync...

                var results = await GetContext().Products
                    .Where(p => EF.Functions.Like(p.Name, $"%{text}%") || EF.Functions.Like(p.Barcode, $"%{text}%"))
                    .Take(20)
                    .ToListAsync(token);

                if (token.IsCancellationRequested) return;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // 1. Clear the EXISTING collection that the UI is already bound to
                    FilteredProducts.Clear();

                    // 2. Add the new items one by one
                    foreach (var p in results)
                    {
                        FilteredProducts.Add(p);
                    }
                });
            }
            catch (TaskCanceledException)
            {
                // This is expected and harmless — it means the user kept typing
            }
            catch (Exception ex)
            {
                // Handle unexpected DB errors
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
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
                    ProductName = SelectedProduct.Name,
                    ProductId = SelectedProduct.Id, //Setting Product Id
                    MRP = SelectedProduct.MRP,
                    Unit = SelectedProduct.Unit,
                    //TODO: need to check for avialble QTY  in stock, 
                    //TOOD: instead of product need to call data from stock table , and make a stock service 
                    //TODO: for handling purchase , sale  , stock out and stock in logic, currently it is not handled and it is just taking product qty as 1 for billing which is not correct

                    // CHANGED: Use 1m to signify 1 as a decimal

                    BilledQuantity = 1m, Id = SelectedProduct.Id,
                    InvoiceId=CurrentInvoice.Id,
                    DiscountPercentage = 0
                };

                newItem.PropertyChanged += InvoiceItem_PropertyChanged;
                InvoiceItems.Add(newItem);
                CalculateInvoiceTotals();
            }
            catch (Exception ex) { _ = InvoiceService.ShowErrorAsync("Add Product Error", ex); }
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
        partial void OnGlobalDiscountInputChanged(decimal value) => CalculateInvoiceTotals();


        partial void OnGlobalDiscountTypeInputChanged(string value) => CalculateInvoiceTotals();

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

        // --- PAYMENT LOGIC ---
        [RelayCommand]
        public void AddPayment()
        {
            if (PaymentAmountInput <= 0) return;

            if (SelectedPaymentMode == PaymentMode.Card && _capturedCardDetails == null)
            {
                _ = InvoiceService.ShowErrorAsync("Payment Error", new Exception("Card details not captured."));
                return;
            }
            else if (SelectedPaymentMode == PaymentMode.Card && _capturedCardDetails != null)
            {
                Payments.Add(new PaymentDetail
                {
                    PaymentMode = SelectedPaymentMode,
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
            else if (SelectedPaymentMode == PaymentMode.Cash)
                Payments.Add(new PaymentDetail { PaymentMode = SelectedPaymentMode, Amount = PaymentAmountInput });
            else

                Payments.Add(new PaymentDetail { PaymentMode = SelectedPaymentMode, Amount = PaymentAmountInput, PaymentNote = PaymentNarration });

            PaymentAmountInput = 0;
            CalculateInvoiceTotals();
            SelectedPaymentMode = PaymentMode.Cash; // Reset to default after adding payment
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

        /// <summary>
        /// Go back to main or list page
        /// </summary>
        /// <returns></returns>

        [RelayCommand]
        public async Task GoBackAsync()
        { if (!IsBusy) await Shell.Current.GoToAsync(".."); }







        public InvoiceEntryPageModel(InvoiceService invoiceService)
        {
            _invoiceService = invoiceService;

            searchText = ""; selectedProduct = null;
            selectedInvoiceItem = null;

            CurrentInvoice = new InvoiceDTO();
        }



        protected void InvoiceItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(EntryItem.BasePrice) or nameof(EntryItem.BilledQuantity) or nameof(EntryItem.DiscountAmount) or nameof(EntryItem.DiscountAmount))
            {
                CalculateInvoiceTotals();
            }
        }


        // 2. Trigger the popup when "Card" is selected
        partial void OnSelectedPaymentModeChanged(PaymentMode value)
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
                        SelectedPaymentMode = PaymentMode.Cash; // Revert if cancelled
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

         


        // --- DATABASE SAVE ENGINE and FINAL ACTION COMMANDS ---
        [RelayCommand]
        public async Task SaveAndWhatsAppAsync()
        {
            if (await _invoiceService.SaveAndPrint(CurrentInvoice, InvoiceItems, Payments, print: false, sendOverMsg: true))
            {
                ResetFormWithoutPrompt();
            }
        }

        [RelayCommand]
        public async Task SaveAndPrintA5Async()
        {
            if (await _invoiceService.SaveAndPrint(CurrentInvoice, InvoiceItems, Payments, print: true, thermal: false, sendOverMsg: false))
            {
                ResetFormWithoutPrompt();
            }
        }

        [RelayCommand]
        public async Task SaveAndPrintThermalAsync()
        {
            if (await _invoiceService.SaveAndPrint(CurrentInvoice, InvoiceItems, Payments, print: true, thermal: true, sendOverMsg: false))
            {
                ResetFormWithoutPrompt();
            }
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
                ResetFormWithoutPrompt();
        }

        /// <summary>
        /// Reset the form without prompt
        /// </summary>
        protected void ResetFormWithoutPrompt()
        {
            foreach (var item in InvoiceItems) item.PropertyChanged -= InvoiceItem_PropertyChanged;
            InvoiceItems.Clear(); Payments.Clear();
            GlobalDiscountInput = 0; GlobalDiscountTypeInput = "Amount";
            PaymentAmountInput = 0; SelectedPaymentMode = PaymentMode.Cash;
            IsNewCustomer = false; SelectedProduct = null; SelectedInvoiceItem = null;
            CurrentInvoice = new InvoiceDTO();
            OnPropertyChanged(nameof(CurrentInvoice));
        }

    }
}