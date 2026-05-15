using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Billing.Models;
using Garmetix.Billing.Services;
using Garmetix.Core.Models.Inventory;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using EntryItem = Garmetix.Billing.Models.EntryItem;

namespace Garmetix.Billing.PageModels
{
    [QueryProperty(nameof(InvoiceId), "InvoiceId")]
    public partial class InvoiceEditPageModel : ObservableObject
    {

        //Invoice Service
        private readonly InvoiceService _invoiceService;

        [ObservableProperty] private bool isBusy;
        [ObservableProperty] private string invoiceId = string.Empty; //Seleted Invoice Id For Ref

        // --- CORE INVOICE DATA ---
        [ObservableProperty] private InvoiceDTO currentInvoice;  // DTO of Invoice to edit the object

        [ObservableProperty] private ObservableCollection<EntryItem> editItems = new(); // Invoice item 

        // --- PAYMENT SPLITTING ---
        [ObservableProperty] private ObservableCollection<Models.PaymentDetail> payments = new(); // Payment details

        [ObservableProperty] private string paymentModeInput = "Cash"; // Payment Mode
        [ObservableProperty] private string paymentAmountInput = string.Empty; // Payment Amount

        // --- GLOBAL DISCOUNTS ---
        [ObservableProperty] private decimal globalDiscountInput;  // Global Discount input

        [ObservableProperty] private string globalDiscountTypeInput = "Amount";

        // --- UI TOTALS BINDINGS ---
        [ObservableProperty] private decimal subTotal;  //Sub Total

        [ObservableProperty] private decimal totalTax; //Total Tax
        [ObservableProperty] private decimal totalDiscount; // Total Discount 
        [ObservableProperty] private decimal roundOffAmount; //roundofAmount
        [ObservableProperty] private decimal grandTotal; //Grand Total
        [ObservableProperty] private decimal paidAmount; //paid amt
        [ObservableProperty] private decimal balanceAmount; // Balance Amt

        // --- AUTOCOMPLETE SEARCH ---
        [ObservableProperty] private string searchText = string.Empty;

        [ObservableProperty] private ObservableCollection<Product> filteredProducts = new();
        [ObservableProperty] private Product selectedProduct;
        private List<Product> _productCache = new();



        public InvoiceEditPageModel(InvoiceService invoiceService)//, bool isBusy, string invoiceId, InvoiceDTO currentInvoice, ObservableCollection<InvoiceItem> editItems, ObservableCollection<PaymentDetail> payments, string paymentModeInput, string paymentAmountInput, decimal globalDiscountInput, string globalDiscountTypeInput, decimal subTotal, decimal totalTax, decimal totalDiscount, decimal roundOffAmount, decimal grandTotal, decimal paidAmount, decimal balanceAmount, string searchText, ObservableCollection<Product> filteredProducts, Product selectedProduct, List<Product> productCache)
        {
            _invoiceService = invoiceService;
            //this.isBusy = isBusy;
            //this.invoiceId = invoiceId;
            //this.currentInvoice = currentInvoice;
            //this.editItems = editItems;
            //this.payments = payments;
            //this.paymentModeInput = paymentModeInput;
            //this.paymentAmountInput = paymentAmountInput;
            //this.globalDiscountInput = globalDiscountInput;
            //this.globalDiscountTypeInput = globalDiscountTypeInput;
            //this.subTotal = subTotal;
            //this.totalTax = totalTax;
            //this.totalDiscount = totalDiscount;
            //this.roundOffAmount = roundOffAmount;
            //this.grandTotal = grandTotal;
            //this.paidAmount = paidAmount;
            //this.balanceAmount = balanceAmount;
            //this.searchText = searchText;
            //this.filteredProducts = filteredProducts;
            //this.selectedProduct = selectedProduct;
            //_productCache = productCache;
        }




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


                // 1. Load Caches
                //  _productCache = await db.Table<Product>().ToListAsync();
                _productCache = await _invoiceService.GetContext().Products.ToListAsync();

                // 2. Load the Master Invoice
                // CurrentInvoice = await db.Table<Invoice>().Where(i => i.Id == id).FirstOrDefaultAsync();
                CurrentInvoice = await _invoiceService.GetInvoiceDTOById(id);

                // Set the UI Discount Inputs
                if (CurrentInvoice.GlobalDiscountAmount > 0)
                {
                    GlobalDiscountInput = CurrentInvoice.GlobalDiscountAmount;
                    GlobalDiscountTypeInput = "Amount";
                }

                // 3. Load Items
                //var items = await db.Table<InvoiceItem>().Where(i => i.InvoiceId == id).ToListAsync();
                var items = await _invoiceService.GetEntryItemListAsync(id);


                EditItems = new ObservableCollection<EntryItem>(items);
                foreach (var item in EditItems)
                {
                    item.PropertyChanged += (s, e) => CalculateTotals();
                }

                // 4. Load Split Payments
                //var paymentList = await db.Table<PaymentDetail>().Where(p => p.InvoiceId == id).ToListAsync();
                var paymentList = await _invoiceService.GetPaymentDetailsAsync(id);
                Payments = new ObservableCollection<PaymentDetail>(paymentList);

                CalculateTotals();
            }
            catch (Exception ex) { await ShowErrorAsync("Load Error", ex.Message); }
            finally { IsBusy = false; }
        }

        // ---------------------------------------------------------
        // CART MANAGEMENT
        // ---------------------------------------------------------
        private partial void OnSearchTextChanged(string value)
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

        private partial void OnSelectedProductChanged(Product value)
        {
            if (value != null)
            {
                var newItem = new EntryItem
                {
                    InvoiceId = CurrentInvoice.Id,
                    ProductName = value.Name,
                    Category = value.ProductType,
                    BasePrice = value.BasicPrice,
                    BilledQuantity = 1m,
                    DiscountPercentage = 0
                };
                newItem.PropertyChanged += (s, e) => CalculateTotals();
                EditItems.Add(newItem);

                SearchText = string.Empty;
                CalculateTotals();
            }
        }

        [RelayCommand]
        public void RemoveItem(EntryItem item)
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
                    PaymentMode = PaymentModeInput,
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
        private partial void OnGlobalDiscountInputChanged(decimal value) => CalculateTotals();

        private partial void OnGlobalDiscountTypeInputChanged(string value) => CalculateTotals();

        private void CalculateTotals()
        {
            if (CurrentInvoice == null) return;

            // 1. Sum up item base values
            decimal itemSubTotal = EditItems.Sum(i => i.BasePrice * i.BilledQuantity);
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

              var result=    _invoiceService.UpdateInvoices(CurrentInvoice, EditItems, Payments);
                //var db = await DatabaseHelper.GetDatabaseAsync();

                // Run everything in an atomic transaction
                //await db.RunInTransactionAsync(tran =>
                //{
                //    tran.Update(CurrentInvoice);

                //    tran.Table<InvoiceItem>().Delete(i => i.InvoiceId == CurrentInvoice.Id);
                //    foreach (var item in EditItems) tran.Insert(item);

                //    tran.Table<PaymentDetail>().Delete(p => p.InvoiceId == CurrentInvoice.Id);
                //    foreach (var pay in Payments) tran.Insert(pay);
                //});
                // Notify the dashboard that the database has changed!
               // Garmetix.AI.Billing.Services.DashboardDataService.Instance.InvalidateCache();
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Success", "Invoice updated successfully.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex) { await ShowErrorAsync("Save Error", ex.Message); }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        public async Task CancelEditAsync()
        {
            bool confirm = await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Cancel", "Discard your edits?", "Yes", "No");
            if (confirm) await Shell.Current.GoToAsync("..");
        }

        private async Task ShowErrorAsync(string title, string message)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (Application.Current?.MainPage != null)
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync(title, message, "OK");
            });
        }
    }
}