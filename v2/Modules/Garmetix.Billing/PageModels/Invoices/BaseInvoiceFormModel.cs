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

namespace Garmetix.Billing.PageModels.Invoices
{
    public abstract partial class BaseInvoiceFormModel : ObservableObject
    {
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

        // --- PAYMENTS & DISCOUNTS ---
        protected CardPaymentDto _capturedCardDetails; 

        [ObservableProperty] protected PaymentMode paymentModeInput = PaymentMode.Cash;
        [ObservableProperty] protected string paymentNarration = string.Empty;
        [ObservableProperty] protected bool _isNarrationVisible = false;
        [ObservableProperty] protected string _narrationPlaceholder;
        [ObservableProperty] protected decimal paymentAmountInput = 0m;

        [ObservableProperty] protected decimal globalDiscountInput;
        [ObservableProperty] protected string globalDiscountTypeInput = "Amount";

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

        protected BaseInvoiceFormModel(InvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        // --- ABSTRACT METHODS (To be implemented by derived classes) ---
        public abstract void CalculateTotals();
        public abstract Task ClearFormAsync();


        // 1. The MVVM Toolkit looks for this partial method in the same class the property is declared
        partial void OnSelectedProductChanged(Product? value)
        {
            // 2. Route it to a virtual method that derived classes can override
            HandleProductSelected(value);
        }

        // 3. Define the virtual method
        protected virtual void HandleProductSelected(Product? value)
        {
            // You can leave this empty, or put shared logic here (like clearing the search box)
        }


        // --- SHARED SEARCH LOGIC ---
        partial void OnSearchTextChanged(string value)
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
        partial void OnPaymentModeInputChanged(PaymentMode value)
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

        partial void OnGlobalDiscountInputChanged(decimal value) => CalculateTotals();
        partial void OnGlobalDiscountTypeInputChanged(string value) => CalculateTotals();

        [RelayCommand]
        public virtual async Task GoBackAsync()
        {
            if (!IsBusy) await Shell.Current.GoToAsync("..");
        }


        //TODO  Handling Invoice Type in the base class might not be ideal if different invoice types have different behaviors.

        // ---   Shared Login For Invoice Type


        protected  virtual async   Task HandledInvoiceForTypes(Invoice inv)
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