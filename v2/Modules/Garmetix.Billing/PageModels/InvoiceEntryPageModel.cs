using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Billing.Helpers;
using Garmetix.Core.Models.Inventory;
using Garmetix.Databases;
using Garmetix.Databases.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Garmetix.Core.Enums;
using Garmetix.Billing.Models;

namespace Garmetix.Billing.PageModels
{
    internal partial class InvoiceEntryPageModel : ObservableObject
    {
        private readonly IPrintService _printService;

        // --- NEW: Search Text Binding ---
        [ObservableProperty]
        private string searchText;

        // This automatically fires the moment you type a letter in the UI
        partial void OnSearchTextChanged(string value)
        {
            UpdateFilteredProducts(value);
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

        // --- HIGH PERFORMANCE CACHING ---
        private Dictionary<string, Product> _productBarcodeCache = new();

        private List<Product> _productNameCache = new();

        public ObservableCollection<Product> FilteredProducts { get; set; } = new();

        // --- STATE MANAGEMENT ---
        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsNotBusy))] private bool isBusy;

        public bool IsNotBusy => !IsBusy;

        [ObservableProperty] private Invoice currentInvoice;
        public ObservableCollection<InvoiceItem> InvoiceItems { get; set; } = new();
        public ObservableCollection<InvoicePayment> Payments { get; set; } = new();
        // The UI only binds to this small filtered list to prevent lagging

        [ObservableProperty] private Product? selectedProduct;
        [ObservableProperty] private InvoiceItem? selectedInvoiceItem;
        [ObservableProperty] private PaymentMode paymentModeInput = PaymentMode.Cash;
        [ObservableProperty] private decimal paymentAmountInput = 0m;
        [ObservableProperty] private bool isNewCustomer = false;

        // --- GLOBAL DISCOUNT INPUTS ---
        [ObservableProperty] private decimal globalDiscountInput;

        partial void OnGlobalDiscountInputChanged(decimal value) => CalculateInvoiceTotals();

        [ObservableProperty] private string globalDiscountTypeInput = "Amount";

        partial void OnGlobalDiscountTypeInputChanged(string value) => CalculateInvoiceTotals();

        private DatabaseContext _localDb = DatabaseService.Instance.LocalDB;
        public DatabaseContext GetContext() => _localDb;
        public InvoiceEntryPageModel(IPrintService printService)
        {
            _printService = printService;
            CurrentInvoice = new Invoice() { InvoiceNumber = "NeedtobeGenerate" };
            searchText = ""; selectedProduct = null;
            selectedInvoiceItem = null;

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
            catch (Exception ex) { await ShowErrorAsync("Customer Search Error", ex); }
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
            catch (Exception ex) { await ShowErrorAsync("Save Customer Error", ex); }
            finally { IsBusy = false; }
        }

        // --- CART LOGIC ---
        //[RelayCommand]
        //public void AddProductToInvoice()
        //{
        //    if (SelectedProduct == null) return;
        //    try
        //    {
        //        var newItem = new InvoiceItem
        //        {
        //            ProductName = SelectedProduct.Name,
        //            Category = SelectedProduct.Category,
        //            Rate = SelectedProduct.BaseRate,
        //            Quantity = 1,
        //            DiscountPercentage = 0
        //        };

        //        newItem.PropertyChanged += InvoiceItem_PropertyChanged;
        //        InvoiceItems.Add(newItem);
        //        SelectedProduct = null;
        //        CalculateInvoiceTotals();
        //    }
        //    catch (Exception ex) { _ = ShowErrorAsync("Add Product Error", ex); }
        //}
        [RelayCommand]
        public void AddProductToInvoice()
        {
            if (SelectedProduct == null) return;
            try
            {
                var newItem = new InvoiceItemDTO
                {
                    Barcode = SelectedProduct.Barcode,
                    Category = SelectedProduct.ProductType,
                    BasePrice = SelectedProduct.BasicPrice,

                    // CHANGED: Use 1m to signify 1 as a decimal
                    ActualQuantity = 1m,
                    BilledQuantity = 1m,

                    DiscountAmount = 0
                };

                newItem.PropertyChanged += InvoiceItem_PropertyChanged;
                InvoiceItems.Add(newItem);
                CalculateInvoiceTotals();
            }
            catch (Exception ex) { _ = ShowErrorAsync("Add Product Error", ex); }
        }

        [RelayCommand]
        public void RemoveInvoiceItem(InvoiceItem item)
        {
            if (item == null || !InvoiceItems.Contains(item)) return;
            item.PropertyChanged -= InvoiceItem_PropertyChanged;
            if (SelectedInvoiceItem == item) SelectedInvoiceItem = null;

            Application.Current.Dispatcher.Dispatch(() =>
            {
                try { InvoiceItems.Remove(item); CalculateInvoiceTotals(); }
                catch (Exception ex) { _ = ShowErrorAsync("Remove Item Error", ex); }
            });
        }

        private void InvoiceItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(InvoiceItem.BasePrice) or nameof(InvoiceItem.BilledQuantity) or nameof(InvoiceItem.DiscountAmount) or nameof(InvoiceItem.DiscountAmount))
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
            if (Payments.Contains(payment)) { Payments.Remove(payment); CalculateInvoiceTotals(); }
        }

        [Obsolete]
        public void CalculateInvoiceTotals()
        {
            //TODO: need to reclaibrated for actual result, it has bug and it not proper
            try
            {
                CurrentInvoice.NetAmount = InvoiceItems.Sum(i => (i.BasePrice * i.BilledQuantity));
                CurrentInvoice.DiscountAmount = InvoiceItems.Sum(i => i.DiscountAmount);
                CurrentInvoice.TaxAmount = InvoiceItems.Sum(i => i.TaxAmount);

                decimal preDiscountTotal = (CurrentInvoice.NetAmount - CurrentInvoice.DiscountAmount) + CurrentInvoice.TaxAmount;
                CurrentInvoice.BillDiscountAmount = GlobalDiscountTypeInput == "%" ? preDiscountTotal * (GlobalDiscountInput / 100m) : GlobalDiscountInput;

                decimal exactGrandTotal = preDiscountTotal - CurrentInvoice.BillDiscountAmount;
                if (exactGrandTotal < 0) exactGrandTotal = 0;

                CurrentInvoice.BillAmount = Math.Round(exactGrandTotal, 0, MidpointRounding.AwayFromZero);
                CurrentInvoice.RoundOff = CurrentInvoice.BillAmount - exactGrandTotal;
                CurrentInvoice.PaidAmount = Payments.Sum(p => p.Amount);
            }
            catch (Exception ex) { _ = ShowErrorAsync("Calculation Error", ex); }
        }

        // --- DATABASE SAVE ENGINE ---
        private async Task<bool> SaveInvoiceToDatabaseAsync()
        {
            if (InvoiceItems.Count == 0) { await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Validation", "Cannot save empty invoice.", "OK"); return false; }
            if (IsBusy) return false;

            try
            {
                IsBusy = true;
                //CurrentInvoice.InvoiceNo = "INV-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
                CurrentInvoice.InvoiceNumber = await GenerateNextInvoiceNumberAsync();
                CalculateInvoiceTotals();

                if (CurrentInvoice.PaidAmount < CurrentInvoice.BillAmount)
                {
                    bool proceed = await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Part Payment", $"Balance of ₹ {CurrentInvoice.BalanceAmount} is unpaid. Proceed?", "Yes", "No");
                    if (!proceed) return false;
                }

                //await _database.RunInTransactionAsync(tran =>
                //{
                //    tran.Insert(CurrentInvoice);
                //    foreach (var item in InvoiceItems) { item.InvoiceId = CurrentInvoice.Id; tran.Insert(item); }
                //});

                try
                {
                    await GetContext().Database.BeginTransactionAsync();
                    await GetContext().Invoices.AddAsync(CurrentInvoice);
                    foreach (var item in InvoiceItems) { item.InvoiceId = CurrentInvoice.Id; GetContext().InvoiceItems.Add(item); }

                    // check if this required
                    await GetContext().SaveChangesAsync();
                    await GetContext().Database.CommitTransactionAsync();

                }
                catch (Exception)
                {

                    await GetContext().Database.RollbackTransactionAsync();
                    throw;
                }

                // Notify the dashboard that the database has changed!
                // DashboardDataService.Instance.InvalidateCache();
                DatabaseService.Instance.InvalidateCache();
                return true;
            }
            catch (Exception ex) { await ShowErrorAsync("Save Invoice Error", ex); return false; }
            finally { IsBusy = false; }
        }

        /// <summary>
        /// Generates the next unique invoice number for the current store and month in the format
        /// 'STORECODE-YYYYMM-IN-XXXX'.
        /// </summary>
        /// <remarks>The invoice number is incremented based on the most recent invoice for the current
        /// month and store. If no invoices exist for the current month, the sequence starts at 0001. In case of a
        /// database error, a random four-digit sequence is used as a fallback. The method uses the store code from
        /// application preferences, defaulting to 'AFA' if not set.</remarks>
        /// <returns>A string containing the next invoice number, formatted with the store code, current year and month, and a
        /// four-digit sequence number.</returns>
        private async Task<string> GenerateNextInvoiceNumberAsync()
        {
            //TODO: move to Invoice Service  even save and delete also . 
            // 1. Get Store Code from MAUI Preferences (Defaults to "AFA" if not set yet)
            string storeCode = Microsoft.Maui.Storage.Preferences.Default.Get("StoreCode", "AFA");

            // 2. Get Current Year and Month (e.g., "202604")
            string yearMonth = DateTime.Now.ToString("yyyyMM");

            // 3. Define the prefix (e.g., "AFA-202604-IN-")
            string prefix = $"{storeCode}-{yearMonth}-IN-";

            try
            {
                // 4. Find the most recent invoice in the database that matches THIS month's prefix
                var lastInvoice = await GetContext().Invoices
                    .Where(i => i.InvoiceNumber.StartsWith(prefix))
                    .OrderByDescending(i => i.InvoiceNumber)
                    .FirstOrDefaultAsync();

                int nextSequenceNumber = 1; // Default to 1 if it's the first bill of the month

                if (lastInvoice != null && !string.IsNullOrEmpty(lastInvoice.InvoiceNumber))
                {
                    // Extract the last 4 characters (the numbers) from the previous invoice
                    string lastSequenceStr = lastInvoice.InvoiceNumber.Substring(lastInvoice.InvoiceNumber.Length - 4);

                    if (int.TryParse(lastSequenceStr, out int lastSequence))
                    {
                        nextSequenceNumber = lastSequence + 1;
                    }
                }

                // 5. Format the number with leading zeros so it is always 4 digits (e.g., "0001")
                string sequenceString = nextSequenceNumber.ToString("D4");

                // 6. Return the perfectly formatted string
                return $"{prefix}{sequenceString}";
            }
            catch (Exception ex)
            {
                // Fallback in case of an unexpected database read error
                System.Diagnostics.Debug.WriteLine($"Error generating invoice number: {ex.Message}");
                string fallbackSequence = new Random().Next(1000, 9999).ToString();
                return $"{prefix}{fallbackSequence}";
            }
        }

        // --- FINAL ACTION COMMANDS ---
        [RelayCommand]
        public async Task SaveAndWhatsAppAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentInvoice.CustomerMobileNumber))
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Error", "Please enter a customer mobile number.", "OK");
                return;
            }

            if (await SaveInvoiceToDatabaseAsync())
            {
                string pdfPath = PdfReceiptBuilder.GenerateA5Pdf(CurrentInvoice, InvoiceItems, Payments);
                string whatsappNumber = CurrentInvoice.CustomerMobileNumber.Length == 10 ? $"91{CurrentInvoice.CustomerMobileNumber}" : CurrentInvoice.CustomerMobileNumber;
                string message = $"Hello {CurrentInvoice.CustomerName}, thank you for shopping at Aadwika Fashion! Your invoice amount is ₹{CurrentInvoice.BillAmount:F2}.";
                string url = $"https://api.whatsapp.com/send?phone={whatsappNumber}&text={Uri.EscapeDataString(message)}";

                try
                {
                    await Launcher.Default.OpenAsync(new Uri(url));
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync("WhatsApp", "Opening WhatsApp. Please tap 'Attach' to send the generated PDF.", "OK");
                }
                catch (Exception) { await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Error", "Could not open WhatsApp.", "OK"); }

                ResetFormWithoutPrompt();
            }
        }

        [RelayCommand]
        public async Task SaveAndPrintA5Async()
        {
            if (await SaveInvoiceToDatabaseAsync())
            {
                string pdfPath = PdfReceiptBuilder.GenerateA5Pdf(CurrentInvoice, InvoiceItems, Payments);
                await Launcher.Default.OpenAsync(new OpenFileRequest { Title = "Print Invoice", File = new ReadOnlyFile(pdfPath) });
                ResetFormWithoutPrompt();
            }
        }

        [RelayCommand]
        public async Task SaveAndPrintThermalAsync()
        {
            if (await SaveInvoiceToDatabaseAsync())
            {
                byte[] thermalBytes = ReceiptBuilder.GenerateThermalReceiptBytes(CurrentInvoice, InvoiceItems);
                await _printService.PrintReceiptAsync(thermalBytes);
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Success", "Thermal Receipt Printed.", "OK");
                ResetFormWithoutPrompt();
            }
        }

        // --- UTILITIES ---
        [RelayCommand]
        public async Task GoBackAsync()
        { if (!IsBusy) await Shell.Current.GoToAsync(".."); }

        [RelayCommand]
        public async Task ClearInvoiceAsync()
        {
            if (IsBusy) return;
            if (await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Clear Form", "Clear the entire invoice?", "Yes", "Cancel")) ResetFormWithoutPrompt();
        }

        private void ResetFormWithoutPrompt()
        {
            foreach (var item in InvoiceItems) item.PropertyChanged -= InvoiceItem_PropertyChanged;
            InvoiceItems.Clear(); Payments.Clear();
            GlobalDiscountInput = 0; GlobalDiscountTypeInput = "Amount";
            PaymentAmountInput = 0; PaymentModeInput = PaymentMode.Cash;
            IsNewCustomer = false; SelectedProduct = null; SelectedInvoiceItem = null;
            CurrentInvoice = new Invoice { InvoiceNumber = "NOTGENERATED" };
            OnPropertyChanged(nameof(CurrentInvoice));
        }

        private async Task ShowErrorAsync(string title, Exception ex) => await Application.Current!.Windows[0].Page!.DisplayAlertAsync(title, $"Error: {ex.Message}", "OK");
    }
}