using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Billing.Models;
using Garmetix.Billing.Pages.Popups;
using Garmetix.Billing.Services;
using Garmetix.Core.Enums;
using Garmetix.Core.Models.Inventory;
using Garmetix.Core.Session;
using System.Collections.ObjectModel;

namespace Garmetix.Billing.PageModels
{
    [QueryProperty(nameof(InvoiceId), "InvoiceId")]
    public partial class InvoiceEditPageModel : ObservableObject
    {
        //Invoice Service
        private readonly InvoiceService _invoiceService;

        private CardPaymentDto _capturedCardDetails;


        [ObservableProperty] private bool isBusy;
        [ObservableProperty] private bool isSaving;

        [ObservableProperty] private string invoiceId = string.Empty; //Seleted Invoice Id For Ref

        // --- CORE INVOICE DATA ---
        [ObservableProperty] private Invoice orginalInvoice;

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

        private List<Product> _productCache = new();  // Use this to cache products for faster research during autocomplete

        public InvoiceEditPageModel()//, bool isBusy, string invoiceId, InvoiceDTO currentInvoice, ObservableCollection<InvoiceItem> editItems, ObservableCollection<PaymentDetail> payments, string paymentModeInput, string paymentAmountInput, decimal globalDiscountInput, string globalDiscountTypeInput, decimal subTotal, decimal totalTax, decimal totalDiscount, decimal roundOffAmount, decimal grandTotal, decimal paidAmount, decimal balanceAmount, string searchText, ObservableCollection<Product> filteredProducts, Product selectedProduct, List<Product> productCache)
        {

            _invoiceService = InvoiceService.Instance;

            //_invoiceService = invoiceService;
        }

        // ---------------------------------------------------------
        // INITIALIZATION & LOADING
        // ---------------------------------------------------------
        partial void OnInvoiceIdChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
                _ = LoadInvoiceDataAsync(Guid.Parse(value));
        }
        // 2. Trigger the popup when "Card" is selected
         
        //TODO: need to update the logic
        private async Task LoadInvoiceDataAsync(Guid id)
        { //TODO: need to convert to DTO and use service instead of direct DB calls
            IsBusy = true;
            try
            {
                // 2. Load the Master Invoice

                //Fetching Original Invoice for reference and to get uneditable data like company id, created at, created by etc

                orginalInvoice = await _invoiceService.GetInvoiceByIdAsync(id);

                //Converting to DTO for editing and calculations
               

                CurrentInvoice = orginalInvoice.ToInvoiceDto();

                // Set the UI Discount Inputs
                if (CurrentInvoice.GlobalDiscountAmount > 0)
                {
                    GlobalDiscountInput = CurrentInvoice.GlobalDiscountAmount;
                    GlobalDiscountTypeInput = "Amount";
                }

                // 3. Load Items
                //var items = await db.Table<InvoiceItem>().Where(i => i.InvoiceId == id).ToListAsync();
               // var items = await _invoiceService.GetEntryItemListAsync(id);

                var items = new List<EntryItem>();
                foreach (var item in orginalInvoice!.InvoiceItems)
                {
                    items.Add(item.ToEntryItem() ?? new EntryItem());
                }
                
                EditItems = new ObservableCollection<EntryItem>(items);
                
                foreach (var item in EditItems)
                {
                    item.PropertyChanged += (s, e) => CalculateTotals();
                }

                // 4. Load Split Payments
                //var paymentList = await db.Table<PaymentDetail>().Where(p => p.InvoiceId == id).ToListAsync();
                //var paymentListw = await _invoiceService.GetPaymentDetailsAsync(id);

                var paymentList = new List<PaymentDetail>();
                foreach (var item in orginalInvoice.Payments)
                {
                    if (item.PaymentMode == PaymentMode.Card)
                    {
                        var card=orginalInvoice.CardPayments.Where(x=>x.Id == item.Id).FirstOrDefault();
                        paymentList.Add(item.ToPaymentDetail(card) ?? new PaymentDetail());
                    }
                    else
                    {
                        paymentList.Add(item.ToPaymentDetail() ?? new PaymentDetail());
                    }

                    
                }
                Payments = new ObservableCollection<PaymentDetail>(paymentList);

                CalculateTotals();
            }
            catch (Exception ex) { await InvoiceService.ShowErrorAsync("Load Error", ex); }
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
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    EditItems.Remove(item);
                    CalculateTotals();
                });
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
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Payments.Add(new PaymentDetail
                    {
                        InvoiceId = CurrentInvoice.Id,
                        PaymentMode = (PaymentMode)Enum.Parse(typeof(PaymentMode), PaymentModeInput),
                        Amount = amount,
                        PaymentDate = DateTime.Now
                    });

                    PaymentAmountInput = string.Empty;
                    CalculateTotals();
                });
            }
        }

        [RelayCommand]
        public void RemovePayment(PaymentDetail payment)
        {
            if (payment != null && Payments.Contains(payment))
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Payments.Remove(payment);
                    CalculateTotals();
                });
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

            decimal itemSubTotal = EditItems.Sum(i => i.BasePrice * i.BilledQuantity);
            decimal itemDiscounts = EditItems.Sum(i => i.DiscountAmount);
            decimal itemTaxes = EditItems.Sum(i => i.TaxAmount);

            decimal globalDiscountCalculated = GlobalDiscountTypeInput == "%"
                ? itemSubTotal * (GlobalDiscountInput / 100m)
                : GlobalDiscountInput;

            decimal rawGrandTotal = itemSubTotal - itemDiscounts - globalDiscountCalculated + itemTaxes;
            if (rawGrandTotal < 0) rawGrandTotal = 0;

            decimal roundedGrandTotal = Math.Round(rawGrandTotal, 0, MidpointRounding.AwayFromZero);
            decimal roundOffValue = roundedGrandTotal - rawGrandTotal;
            decimal totalPaid = Payments.Sum(p => p.Amount);
            decimal balance = roundedGrandTotal - totalPaid;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                SubTotal = itemSubTotal;
                TotalTax = itemTaxes;
                TotalDiscount = itemDiscounts + globalDiscountCalculated;
                RoundOffAmount = roundOffValue;
                GrandTotal = roundedGrandTotal;
                PaidAmount = totalPaid;
                BalanceAmount = balance;

                CurrentInvoice.SubTotal = SubTotal;
                CurrentInvoice.TotalTax = TotalTax;
                CurrentInvoice.GlobalDiscountAmount = globalDiscountCalculated;
                CurrentInvoice.RoundOffAmount = RoundOffAmount;
                CurrentInvoice.GrandTotal = GrandTotal;
                CurrentInvoice.PaidAmount = PaidAmount;
            });
        }

        // ---------------------------------------------------------
        // SAVING TRANSACTIONS
        // ---------------------------------------------------------
        [RelayCommand]
        public async Task SaveChangesAsync()
        {
            if (EditItems.Count == 0)
            {
                await InvoiceService.ShowErrorAsync("Validation", "An invoice must have at least one item.");
                return;
            }

            IsBusy = true;
            try
            {
                var result = await UpdateInvoicesAsync(CurrentInvoice, EditItems, Payments);
                if (result)
                {
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Success", "Invoice updated successfully.", "OK");
                    await Shell.Current.GoToAsync("..");
                }
            }
            catch (Exception ex) { await InvoiceService.ShowErrorAsync("Save Error", ex); }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        public async Task CancelEditAsync()
        {
            bool confirm = await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Cancel", "Discard your edits?", "Yes", "No");
            if (confirm) await Shell.Current.GoToAsync("..");
        }

        public async Task<bool> UpdateInvoicesAsync(InvoiceDTO invoicedto, IEnumerable<EntryItem> InvoiceItems, IEnumerable<PaymentDetail> paymentDetails)
        {
            if (InvoiceItems.Count() == 0)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Validation", "Cannot update empty invoice.", "OK");
                return false;
            }

            if (isSaving)
                return false;

            try
            {
                isSaving = true;

                var invoicePaymentList = new List<InvoicePayment>();
                var cardpaymentList = new List<CardPayment>();
                var editedInvoice = new Invoice
                {
                    Synced = false,
                    Deleted = false,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = "AutoAdmin",
                    CreatedAt = orginalInvoice.CreatedAt,

                    B2BSale = invoicedto.IsB2BSale,
                    InterState = invoicedto.IsInterStateSale,
                    ReturnInvoice = false,

                    Id = invoicedto.Id,

                    InvoiceNumber = invoicedto.InvoiceNumber,
                    OnDate = invoicedto.OnDate,

                    CustomerGSTIN = invoicedto.CustomerGSTIN,
                    CustomerMobileNumber = invoicedto.CustomerMobileNumber,
                    CustomerName = invoicedto.CustomerName,

                    ItemCount = InvoiceItems.Count(),
                    Quantity = invoicedto.BilledQuantity,

                    CreditSale = invoicedto.BalanceAmount > 0 ? true : false,

                    BasePrice = invoicedto.SubTotal,//TODO: need to handle base price changes during edit

                    DiscountAmount = invoicedto.TotalDiscount,

                    NetAmount = invoicedto.SubTotal, //TODO: need to handle net amount changes during edit

                    TaxAmount = invoicedto.TotalTax,
                    CGSTAmount = invoicedto.TotalTax / 2m,
                    SGSTAmount = invoicedto.TotalTax / 2m,
                    IGSTAmount = invoicedto.TotalTax,

                    BillDiscountAmount = invoicedto.GlobalDiscountAmount,

                    RoundOff = invoicedto.RoundOffAmount,
                    BillAmount = invoicedto.GrandTotal,

                    PaidAmount = invoicedto.PaidAmount,

                    CompanyId = orginalInvoice.CompanyId,

                    SalemanId = orginalInvoice.SalemanId, //TODO: need to handle salesman changes
                    CustomerId = orginalInvoice.CustomerId, //TODO: need to handle customer changes

                    MRP = InvoiceItems.Sum(i => i.BasePrice * i.BilledQuantity),//TODO: need to handle MRP changes during edit
                };

                if (editedInvoice.PaidAmount < editedInvoice.BillAmount)
                {
                    bool proceed = await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Part Payment", $"Balance of ₹ {invoicedto.BalanceAmount} is unpaid. Proceed?", "Yes", "No");
                    if (!proceed) return false;
                }

                var invoiceItemList = new List<InvoiceItem>();
                try
                {
                    foreach (var item in InvoiceItems)
                    {
                        invoiceItemList.Add(new InvoiceItem
                        {
                            CompanyId = editedInvoice.CompanyId,
                            Deleted = false,
                            Synced = false,
                            CreatedAt = editedInvoice.CreatedAt,
                            UpdatedAt = editedInvoice.UpdatedAt,
                            CreatedBy = editedInvoice.CreatedBy,
                            InvoiceId = editedInvoice.Id,

                            Id = item.Id,

                            Barcode = item.Barcode,
                            BilledQuantity = item.BilledQuantity,

                            BasePrice = item.BasePrice,
                            DiscountAmount = item.DiscountAmount,
                            Amount = item.TotalAmount,

                            Category = item.Category,

                            TaxAmount = item.TaxAmount,

                            //TaxId=item.TaxId,
                            //TaxPercentage=item.TaxPercentage,
                            //TaxType=item.TaxType,
                            //ProductId=item.ProductId,
                            //MRP=item.MRP,
                        });
                    }

                    // 1. Identify distinct payment modes in the current transaction
                    var distinctModes = paymentDetails.Select(p => p.PaymentMode).Distinct().ToList();

                    // 2. Set overall paymentMode based on the count of distinct modes
                    editedInvoice.PaymentMode = distinctModes.Count > 1
                         ? PaymentMode.MixPayments
                         : distinctModes.FirstOrDefault();

                    foreach (var item in paymentDetails)
                    {
                        if (item.PaymentMode == PaymentMode.Card)
                        {
                            cardpaymentList.Add(new CardPayment
                            {
                                InvoiceId = editedInvoice.Id,

                                Amount = item.Amount,
                                UpdatedAt = DateTime.UtcNow,
                                CreatedAt = DateTime.UtcNow,
                                Deleted = false,
                                CreatedBy = "AutoAdmin",
                                Id = item.Guid,
                                OnDate = item.PaymentDate,
                                Synced = false,
                                CardNumber = item.CardPaymentNumber.Value,
                                CardType = item.CardType.Value,
                                Card = item.Card.Value,
                                BankName = item.CardPaymentBank,
                                AuthCode = item.AuthCode.Value,
                                CompanyId = editedInvoice.CompanyId
                            });
                        }

                        invoicePaymentList.Add(new InvoicePayment
                        {
                            Amount = item.Amount,
                            CompanyId = editedInvoice.CompanyId,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            CreatedBy = "AutoAdmin",
                            Id = item.Guid,
                            Deleted = false,
                            OnDate = item.PaymentDate,
                            InvoiceId = currentInvoice.Id,
                            PaymentMode = item.PaymentMode,
                            Synced = false,
                            ReferenceNumber = item.PaymentNote
                        });
                    }

                    return await _invoiceService.UpdateInvoicesAsync(editedInvoice, invoiceItemList, invoicePaymentList, cardpaymentList);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    // Clear the lists to free memory
                    invoiceItemList.Clear();
                    invoicePaymentList.Clear();
                    cardpaymentList.Clear();
                }
                // Notify the dashboard that the database has changed!
                // DashboardDataService.Instance.InvalidateCache();
                //DatabaseService.Instance.InvalidateCache();
                //return true;
            }
            catch (Exception ex)
            {
                await InvoiceService.ShowErrorAsync("Save Invoice Error", ex);
                return false;

            }
            finally
            {
                isSaving = false;
            }
        }
    }
}


 