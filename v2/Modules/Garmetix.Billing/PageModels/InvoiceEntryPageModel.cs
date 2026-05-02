using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Billing.Helpers;
using Garmetix.Core.Models.Inventory;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace Garmetix.Billing.PageModels
{
    internal partial class InvoiceEntryPageModel : ObservableObject
    {
        private readonly IPrintService _printService;

        // --- NEW: Search Text Binding ---
        [ObservableProperty]
        private string searchText;

        // This automatically fires the moment you type a letter in the UI
        private partial void OnSearchTextChanged(string value)
        {
            UpdateFilteredProducts(value);
        }

        // NEW: This automatically fires the moment you click an item in the search list
        private partial void OnSelectedProductChanged(Product value)
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
        public ObservableCollection<PaymentDetail> Payments { get; set; } = new();
        // The UI only binds to this small filtered list to prevent lagging

        [ObservableProperty] private Product selectedProduct;
        [ObservableProperty] private InvoiceItem selectedInvoiceItem;
        [ObservableProperty] private string paymentModeInput = "Cash";
        [ObservableProperty] private decimal paymentAmountInput;
        [ObservableProperty] private bool isNewCustomer = false;

        // --- GLOBAL DISCOUNT INPUTS ---
        [ObservableProperty] private decimal globalDiscountInput;

        private partial void OnGlobalDiscountInputChanged(decimal value) => CalculateInvoiceTotals();

        [ObservableProperty] private string globalDiscountTypeInput = "Amount";

        private partial void OnGlobalDiscountTypeInputChanged(string value) => CalculateInvoiceTotals();

        public InvoiceEntryViewModel(IPrintService printService)
        {
            _printService = printService;
            CurrentInvoice = new Invoice();
            InitializeDatabaseAsync();
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
            Application.Current.Dispatcher.Dispatch(() =>
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
                else IsNewCustomer = true;
            }
            catch (Exception ex) { await ShowErrorAsync("Customer Search Error", ex); }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        public async Task SaveCustomerAsync()
        {
            if (IsBusy || string.IsNullOrWhiteSpace(CurrentInvoice.MobileNo) || string.IsNullOrWhiteSpace(CurrentInvoice.CustomerName)) return;
            try
            {
                IsBusy = true;
                var existing = await _database.Table<Customer>().FirstOrDefaultAsync(c => c.MobileNo == CurrentInvoice.MobileNo);
                if (existing == null)
                {
                    await _database.InsertAsync(new Customer { MobileNo = CurrentInvoice.MobileNo, Name = CurrentInvoice.CustomerName, Gstin = CurrentInvoice.Gstin });
                    IsNewCustomer = false;
                    await Application.Current.MainPage.DisplayAlert("Success", "Customer saved.", "OK");
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
                var newItem = new InvoiceItem
                {
                    ProductName = SelectedProduct.Name,
                    Category = SelectedProduct.Category,
                    Rate = SelectedProduct.BaseRate,

                    // CHANGED: Use 1m to signify 1 as a decimal
                    Quantity = 1m,

                    DiscountPercentage = 0
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
            if (e.PropertyName is nameof(InvoiceItem.Rate) or nameof(InvoiceItem.Quantity) or nameof(InvoiceItem.DiscountPercentage) or nameof(InvoiceItem.DiscountAmount))
            {
                CalculateInvoiceTotals();
            }
        }

        // --- PAYMENT LOGIC ---
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
            if (Payments.Contains(payment)) { Payments.Remove(payment); CalculateInvoiceTotals(); }
        }

        public void CalculateInvoiceTotals()
        {
            try
            {
                CurrentInvoice.SubTotal = InvoiceItems.Sum(i => (i.Rate * i.Quantity));
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
            catch (Exception ex) { _ = ShowErrorAsync("Calculation Error", ex); }
        }

        // --- DATABASE SAVE ENGINE ---
        private async Task<bool> SaveInvoiceToDatabaseAsync()
        {
            if (InvoiceItems.Count == 0) { await Application.Current.MainPage.DisplayAlert("Validation", "Cannot save empty invoice.", "OK"); return false; }
            if (IsBusy) return false;

            try
            {
                IsBusy = true;
                //CurrentInvoice.InvoiceNo = "INV-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
                CurrentInvoice.InvoiceNo = await GenerateNextInvoiceNumberAsync();
                CalculateInvoiceTotals();

                if (CurrentInvoice.PaidAmount < CurrentInvoice.GrandTotal)
                {
                    bool proceed = await Application.Current.MainPage.DisplayAlert("Part Payment", $"Balance of ₹ {CurrentInvoice.BalanceAmount} is unpaid. Proceed?", "Yes", "No");
                    if (!proceed) return false;
                }

                await _database.RunInTransactionAsync(tran =>
                {
                    tran.Insert(CurrentInvoice);
                    foreach (var item in InvoiceItems) { item.InvoiceId = CurrentInvoice.Id; tran.Insert(item); }
                });
                // Notify the dashboard that the database has changed!
                Garmetix.AI.Billing.Services.DashboardDataService.Instance.InvalidateCache();
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
            // 1. Get Store Code from MAUI Preferences (Defaults to "AFA" if not set yet)
            string storeCode = Microsoft.Maui.Storage.Preferences.Default.Get("StoreCode", "AFA");

            // 2. Get Current Year and Month (e.g., "202604")
            string yearMonth = DateTime.Now.ToString("yyyyMM");

            // 3. Define the prefix (e.g., "AFA-202604-IN-")
            string prefix = $"{storeCode}-{yearMonth}-IN-";

            try
            {
                // 4. Find the most recent invoice in the database that matches THIS month's prefix
                var lastInvoice = await _database.Table<Invoice>()
                    .Where(i => i.InvoiceNo.StartsWith(prefix))
                    .OrderByDescending(i => i.InvoiceNo)
                    .FirstOrDefaultAsync();

                int nextSequenceNumber = 1; // Default to 1 if it's the first bill of the month

                if (lastInvoice != null && !string.IsNullOrEmpty(lastInvoice.InvoiceNo))
                {
                    // Extract the last 4 characters (the numbers) from the previous invoice
                    string lastSequenceStr = lastInvoice.InvoiceNo.Substring(lastInvoice.InvoiceNo.Length - 4);

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
            if (string.IsNullOrWhiteSpace(CurrentInvoice.MobileNo))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please enter a customer mobile number.", "OK");
                return;
            }

            if (await SaveInvoiceToDatabaseAsync())
            {
                string pdfPath = PdfReceiptBuilder.GenerateA5Pdf(CurrentInvoice, InvoiceItems, Payments);
                string whatsappNumber = CurrentInvoice.MobileNo.Length == 10 ? $"91{CurrentInvoice.MobileNo}" : CurrentInvoice.MobileNo;
                string message = $"Hello {CurrentInvoice.CustomerName}, thank you for shopping at Aadwika Fashion! Your invoice amount is ₹{CurrentInvoice.GrandTotal:F2}.";
                string url = $"https://api.whatsapp.com/send?phone={whatsappNumber}&text={Uri.EscapeDataString(message)}";

                try
                {
                    await Launcher.Default.OpenAsync(new Uri(url));
                    await Application.Current.MainPage.DisplayAlert("WhatsApp", "Opening WhatsApp. Please tap 'Attach' to send the generated PDF.", "OK");
                }
                catch (Exception) { await Application.Current.MainPage.DisplayAlert("Error", "Could not open WhatsApp.", "OK"); }

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
                await Application.Current.MainPage.DisplayAlert("Success", "Thermal Receipt Printed.", "OK");
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
            if (await Application.Current.MainPage.DisplayAlert("Clear Form", "Clear the entire invoice?", "Yes", "Cancel")) ResetFormWithoutPrompt();
        }

        private void ResetFormWithoutPrompt()
        {
            foreach (var item in InvoiceItems) item.PropertyChanged -= InvoiceItem_PropertyChanged;
            InvoiceItems.Clear(); Payments.Clear();
            GlobalDiscountInput = 0; GlobalDiscountTypeInput = "Amount";
            PaymentAmountInput = 0; PaymentModeInput = "Cash";
            IsNewCustomer = false; SelectedProduct = null; SelectedInvoiceItem = null;
            CurrentInvoice = new Invoice();
            OnPropertyChanged(nameof(CurrentInvoice));
        }

        private async Task ShowErrorAsync(string title, Exception ex) => await Application.Current.MainPage.DisplayAlert(title, $"Error: {ex.Message}", "OK");
    }
}