using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Garmetix.AI.Billing.Models;
using Garmetix.Billing.AIBased.Helpers;

namespace Garmetix.AI.Billing.ViewModels
{
    [QueryProperty(nameof(InvoiceId), "InvoiceId")]
    public partial class EditInvoiceViewModel : ObservableObject
    {
        [ObservableProperty] private bool isBusy;
        [ObservableProperty] private string invoiceId;

        // --- CORE INVOICE DATA ---
        [ObservableProperty] private Invoice currentInvoice;
        [ObservableProperty] private ObservableCollection<InvoiceItem> editItems = new();

        // --- PAYMENT SPLITTING ---
        [ObservableProperty] private ObservableCollection<PaymentDetail> payments = new();
        [ObservableProperty] private string paymentModeInput = "Cash";
        [ObservableProperty] private string paymentAmountInput;

        // --- GLOBAL DISCOUNTS ---
        [ObservableProperty] private decimal globalDiscountInput;
        [ObservableProperty] private string globalDiscountTypeInput = "Amount";

        // --- UI TOTALS BINDINGS ---
        [ObservableProperty] private decimal subTotal;
        [ObservableProperty] private decimal totalTax;
        [ObservableProperty] private decimal totalDiscount;
        [ObservableProperty] private decimal roundOffAmount;
        [ObservableProperty] private decimal grandTotal;
        [ObservableProperty] private decimal paidAmount;
        [ObservableProperty] private decimal balanceAmount;

        // --- AUTOCOMPLETE SEARCH ---
        [ObservableProperty] private string searchText;
        [ObservableProperty] private ObservableCollection<Product> filteredProducts = new();
        [ObservableProperty] private Product selectedProduct;
        private List<Product> _productCache = new();

        // ---------------------------------------------------------
        // INITIALIZATION & LOADING
        // ---------------------------------------------------------
        partial void OnInvoiceIdChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
                _ = LoadInvoiceDataAsync(Guid.Parse(value));
        }

        private async Task LoadInvoiceDataAsync(Guid id)
        {
            IsBusy = true;
            try
            {
                var db = await DatabaseHelper.GetDatabaseAsync();

                // 1. Load Caches
                _productCache = await db.Table<Product>().ToListAsync();

                // 2. Load the Master Invoice
                CurrentInvoice = await db.Table<Invoice>().Where(i => i.Id == id).FirstOrDefaultAsync();

                // Set the UI Discount Inputs
                if (CurrentInvoice.GlobalDiscountAmount > 0)
                {
                    GlobalDiscountInput = CurrentInvoice.GlobalDiscountAmount;
                    GlobalDiscountTypeInput = "Amount";
                }

                // 3. Load Items
                var items = await db.Table<InvoiceItem>().Where(i => i.InvoiceId == id).ToListAsync();
                EditItems = new ObservableCollection<InvoiceItem>(items);
                foreach (var item in EditItems)
                {
                    item.PropertyChanged += (s, e) => CalculateTotals();
                }

                // 4. Load Split Payments
                var paymentList = await db.Table<PaymentDetail>().Where(p => p.InvoiceId == id).ToListAsync();
                Payments = new ObservableCollection<PaymentDetail>(paymentList);

                CalculateTotals();
            }
            catch (Exception ex) { await ShowErrorAsync("Load Error", ex.Message); }
            finally { IsBusy = false; }
        }

        // ---------------------------------------------------------
        // CART MANAGEMENT
        // ---------------------------------------------------------
        partial void OnSearchTextChanged(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                FilteredProducts.Clear();
                return;
            }

            var results = _productCache.Where(p =>
                (p.Name != null && p.Name.Contains(value, StringComparison.OrdinalIgnoreCase)) ||
                (p.Barcode != null && p.Barcode.Contains(value, StringComparison.OrdinalIgnoreCase)))
                .Take(20).ToList();

            FilteredProducts = new ObservableCollection<Product>(results);
        }

        partial void OnSelectedProductChanged(Product value)
        {
            if (value != null)
            {
                var newItem = new InvoiceItem
                {
                    InvoiceId = CurrentInvoice.Id,
                    ProductName = value.Name,
                    Category = value.Category,
                    Rate = value.BaseRate,
                    Quantity = 1m,
                    DiscountPercentage = 0
                };

                newItem.PropertyChanged += (s, e) => CalculateTotals();
                EditItems.Add(newItem);

                SearchText = string.Empty;
                CalculateTotals();
            }
        }

        [RelayCommand]
        public void RemoveItem(InvoiceItem item)
        {
            if (item != null && EditItems.Contains(item))
            {
                EditItems.Remove(item);
                CalculateTotals();
            }
        }

        // ---------------------------------------------------------
        // PAYMENT SPLITTING LOGIC
        // ---------------------------------------------------------
        [RelayCommand]
        public void AddPayment()
        {
            if (decimal.TryParse(PaymentAmountInput, out decimal amount) && amount > 0)
            {
                Payments.Add(new PaymentDetail
                {
                    InvoiceId = CurrentInvoice.Id,
                    Mode = PaymentModeInput,
                    Amount = amount,
                    PaymentDate = DateTime.Now
                });

                PaymentAmountInput = string.Empty;
                CalculateTotals();
            }
        }

        [RelayCommand]
        public void RemovePayment(PaymentDetail payment)
        {
            if (payment != null && Payments.Contains(payment))
            {
                Payments.Remove(payment);
                CalculateTotals();
            }
        }

        // ---------------------------------------------------------
        // LIVE CALCULATIONS
        // ---------------------------------------------------------
        partial void OnGlobalDiscountInputChanged(decimal value) => CalculateTotals();
        partial void OnGlobalDiscountTypeInputChanged(string value) => CalculateTotals();

        private void CalculateTotals()
        {
            if (CurrentInvoice == null) return;

            // 1. Sum up item base values
            decimal itemSubTotal = EditItems.Sum(i => i.Rate * i.Quantity);
            decimal itemDiscounts = EditItems.Sum(i => i.DiscountAmount);
            decimal itemTaxes = EditItems.Sum(i => i.TaxAmount);

            // 2. Calculate Global Bill Discount
            decimal globalDiscountCalculated = 0;
            if (GlobalDiscountTypeInput == "%")
            {
                globalDiscountCalculated = itemSubTotal * (GlobalDiscountInput / 100m);
            }
            else
            {
                globalDiscountCalculated = GlobalDiscountInput;
            }

            // 3. Raw Grand Total Calculation
            decimal rawGrandTotal = itemSubTotal - itemDiscounts - globalDiscountCalculated + itemTaxes;

            // 4. Rounding Off to nearest Rupee
            decimal roundedGrandTotal = Math.Round(rawGrandTotal, 0, MidpointRounding.AwayFromZero);
            decimal roundOffValue = roundedGrandTotal - rawGrandTotal;

            // 5. Payments & Balances
            decimal totalPaid = Payments.Sum(p => p.Amount);
            decimal balance = roundedGrandTotal - totalPaid;

            // 6. UPDATE UI BINDINGS
            SubTotal = itemSubTotal;
            TotalTax = itemTaxes;
            TotalDiscount = itemDiscounts + globalDiscountCalculated;
            RoundOffAmount = roundOffValue;
            GrandTotal = roundedGrandTotal;
            PaidAmount = totalPaid;
            BalanceAmount = balance;

            // 7. Update the Master Invoice Object for Database Saving
            CurrentInvoice.SubTotal = SubTotal;
            CurrentInvoice.TotalTax = TotalTax;
            CurrentInvoice.GlobalDiscountAmount = globalDiscountCalculated;
            CurrentInvoice.RoundOffAmount = RoundOffAmount;
            CurrentInvoice.GrandTotal = GrandTotal;
            CurrentInvoice.PaidAmount = PaidAmount;
            //CurrentInvoice.BalanceAmount = BalanceAmount;
        }

        // ---------------------------------------------------------
        // SAVING TRANSACTIONS
        // ---------------------------------------------------------
        [RelayCommand]
        public async Task SaveChangesAsync()
        {
            if (EditItems.Count == 0)
            {
                await ShowErrorAsync("Validation", "An invoice must have at least one item.");
                return;
            }

            IsBusy = true;
            try
            {
                var db = await DatabaseHelper.GetDatabaseAsync();

                // Run everything in an atomic transaction
                await db.RunInTransactionAsync(tran =>
                {
                    tran.Update(CurrentInvoice);

                    tran.Table<InvoiceItem>().Delete(i => i.InvoiceId == CurrentInvoice.Id);
                    foreach (var item in EditItems) tran.Insert(item);

                    tran.Table<PaymentDetail>().Delete(p => p.InvoiceId == CurrentInvoice.Id);
                    foreach (var pay in Payments) tran.Insert(pay);
                });

                await Application.Current.MainPage.DisplayAlert("Success", "Invoice updated successfully.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex) { await ShowErrorAsync("Save Error", ex.Message); }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        public async Task CancelEditAsync()
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert("Cancel", "Discard your edits?", "Yes", "No");
            if (confirm) await Shell.Current.GoToAsync("..");
        }

        private async Task ShowErrorAsync(string title, string message)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (Application.Current?.MainPage != null)
                    await Application.Current.MainPage.DisplayAlert(title, message, "OK");
            });
        }
    }
}