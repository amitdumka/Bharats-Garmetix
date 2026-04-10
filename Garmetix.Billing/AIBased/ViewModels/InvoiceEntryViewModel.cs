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
        private SQLiteAsyncConnection _database;
        private readonly IPrintService _printService;

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

        [ObservableProperty]
        private bool isNewCustomer = false;

        // --- GLOBAL DISCOUNT INPUTS ---
        [ObservableProperty]
        private decimal globalDiscountInput;

        partial void OnGlobalDiscountInputChanged(decimal value) => CalculateInvoiceTotals();

        [ObservableProperty]
        private string globalDiscountTypeInput = "Amount";

        partial void OnGlobalDiscountTypeInputChanged(string value) => CalculateInvoiceTotals();

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
                string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "aadwikabilling.db3");
                _database = new SQLiteAsyncConnection(dbPath);

                await _database.CreateTableAsync<Invoice>();
                await _database.CreateTableAsync<InvoiceItem>();
                await _database.CreateTableAsync<Product>();
                await _database.CreateTableAsync<Customer>();

                if (await _database.Table<Product>().CountAsync() == 0)
                {
                    AvailableProducts.Add(new Product { Name = "Cotton Kurta", Barcode = "1001", BaseRate = 1500, Category = GarmentCategory.ReadyMade });
                    AvailableProducts.Add(new Product { Name = "Silk Saree Fabric", Barcode = "1002", BaseRate = 3000, Category = GarmentCategory.Fabric });
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Database Error", ex);
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
                await Application.Current.MainPage.DisplayAlert("Validation", "Please enter Mobile No and Name.", "OK");
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
                    await Application.Current.MainPage.DisplayAlert("Success", "Customer saved.", "OK");
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
                    DiscountPercentage = 0
                };

                newItem.PropertyChanged += InvoiceItem_PropertyChanged;
                InvoiceItems.Add(newItem);
                SelectedProduct = null;
                CalculateInvoiceTotals();
            }
            catch (Exception ex)
            {
                _ = ShowErrorAsync("Add Product Error", ex);
            }
        }

        [RelayCommand]
        public void RemoveInvoiceItem(InvoiceItem item)
        {
            if (item == null || !InvoiceItems.Contains(item)) return;

            item.PropertyChanged -= InvoiceItem_PropertyChanged;

            if (SelectedInvoiceItem == item) SelectedInvoiceItem = null;

            Application.Current.Dispatcher.Dispatch(() =>
            {
                try
                {
                    InvoiceItems.Remove(item);
                    CalculateInvoiceTotals();
                }
                catch (Exception ex)
                {
                    _ = ShowErrorAsync("Remove Item Error", ex);
                }
            });
        }

        private void InvoiceItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // CHANGED DiscountAmount to DiscountPercentage
            if (e.PropertyName is nameof(InvoiceItem.Rate) or nameof(InvoiceItem.Quantity) or nameof(InvoiceItem.DiscountPercentage) or nameof(InvoiceItem.DiscountAmount))
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

                decimal preDiscountTotal = (CurrentInvoice.SubTotal - CurrentInvoice.TotalDiscount) + CurrentInvoice.TotalTax;

                if (GlobalDiscountTypeInput == "%")
                {
                    CurrentInvoice.GlobalDiscountAmount = preDiscountTotal * (GlobalDiscountInput / 100m);
                }
                else
                {
                    CurrentInvoice.GlobalDiscountAmount = GlobalDiscountInput;
                }

                decimal exactGrandTotal = preDiscountTotal - CurrentInvoice.GlobalDiscountAmount;
                if (exactGrandTotal < 0) exactGrandTotal = 0;

                CurrentInvoice.GrandTotal = Math.Round(exactGrandTotal, 0, MidpointRounding.AwayFromZero);
                CurrentInvoice.RoundOffAmount = CurrentInvoice.GrandTotal - exactGrandTotal;

                CurrentInvoice.PaidAmount = Payments.Sum(p => p.Amount);
            }
            catch (Exception ex)
            {
                _ = ShowErrorAsync("Calculation Error", ex);
            }
        }
        // --- NAVIGATION & UTILITY COMMANDS ---
        // ==========================================
        // DATABASE SAVING ENGINE (CENTRALIZED)
        // ==========================================
        private async Task<bool> SaveInvoiceToDatabaseAsync()
        {
            if (InvoiceItems.Count == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Validation", "Cannot save empty invoice.", "OK");
                return false;
            }
            if (IsBusy) return false;

            try
            {
                IsBusy = true;
                CurrentInvoice.InvoiceNo = "INV-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
                CalculateInvoiceTotals();

                if (CurrentInvoice.PaidAmount < CurrentInvoice.GrandTotal)
                {
                    bool proceed = await Application.Current.MainPage.DisplayAlert(
                        "Part Payment",
                        $"Balance of ₹ {CurrentInvoice.BalanceAmount} is unpaid. Proceed?",
                        "Yes", "No");

                    if (!proceed) return false;
                }

                await _database.RunInTransactionAsync(tran =>
                {
                    tran.Insert(CurrentInvoice);
                    foreach (var item in InvoiceItems)
                    {
                        item.InvoiceId = CurrentInvoice.Id;
                        tran.Insert(item);
                    }
                });

                return true;
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Save Invoice Error", ex);
                return false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        // ==========================================
        // THE 3 ACTION COMMANDS
        // ==========================================

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

        //[RelayCommand]
        //public async Task SaveAndPrintA5Async()
        //{
        //    if (await SaveInvoiceToDatabaseAsync())
        //    {
        //        string html = ReceiptBuilder.GenerateA5HtmlInvoice(CurrentInvoice, InvoiceItems);
        //        //TODO: from where to send print?
        //        // 2. Send it directly to the OS Native Printer
        //        await _printService.PrintHtmlAsync(html, $"Invoice_{CurrentInvoice.InvoiceNo}");

        //        // Pushes the preview page so the user can see it and print it
        //        await Application.Current.MainPage.Navigation.PushModalAsync(new Views.InvoicePreviewPage(html, CurrentInvoice.InvoiceNo));
        //        ResetFormWithoutPrompt();
        //    }
        //}
        [RelayCommand]
        public async Task SaveAndPrintA5Async()
        {
            if (await SaveInvoiceToDatabaseAsync())
            {
                // 1. Generate the PDF
                string pdfPath = PdfReceiptBuilder.GenerateA5Pdf(CurrentInvoice, InvoiceItems, Payments);

                // 2. Open the PDF natively so the user can immediately hit Print
                await Microsoft.Maui.ApplicationModel.Launcher.Default.OpenAsync(new OpenFileRequest
                {
                    Title = "Print Invoice",
                    File = new ReadOnlyFile(pdfPath)
                });

                // 3. Clear the screen for the next customer
                ResetFormWithoutPrompt();
            }
        }

        [RelayCommand]
        public async Task SaveAndShareAsync()
        {
            if (await SaveInvoiceToDatabaseAsync())
            {
                // Pass the Payments collection into the PDF generator
                string pdfPath = PdfReceiptBuilder.GenerateA5Pdf(CurrentInvoice, InvoiceItems, Payments);

                // Share the file directly to WhatsApp (or email)
                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = $"Share Invoice {CurrentInvoice.InvoiceNo}",
                    File = new ShareFile(pdfPath)
                });

                ResetFormWithoutPrompt();
            }
        }

        //[RelayCommand]
        //public async Task SaveAndShareAsync()
        //{
        //    if (await SaveInvoiceToDatabaseAsync())
        //    {
        //        string html = ReceiptBuilder.GenerateA5HtmlInvoice(CurrentInvoice, InvoiceItems);

        //        // Pushes the preview page so the user can review before sharing
        //        await Application.Current.MainPage.Navigation.PushModalAsync(new Views.InvoicePreviewPage(html, CurrentInvoice.InvoiceNo));
        //        ResetFormWithoutPrompt();
        //    }
        //}

        // ==========================================
        // UTILITIES
        // ==========================================
        [RelayCommand]
        public async Task GoBackAsync()
        {
            if (IsBusy) return;
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        public async Task ClearInvoiceAsync()
        {
            if (IsBusy) return;
            bool confirm = await Application.Current.MainPage.DisplayAlert("Clear Form", "Clear the entire invoice?", "Yes", "Cancel");
            if (confirm) ResetFormWithoutPrompt();
        }

        private void ResetFormWithoutPrompt()
        {
            foreach (var item in InvoiceItems) item.PropertyChanged -= InvoiceItem_PropertyChanged;
            InvoiceItems.Clear();
            Payments.Clear();
            GlobalDiscountInput = 0;
            GlobalDiscountTypeInput = "Amount";
            PaymentAmountInput = 0;
            PaymentModeInput = "Cash";
            IsNewCustomer = false;
            SelectedProduct = null;
            SelectedInvoiceItem = null;
            CurrentInvoice = new Invoice();
            OnPropertyChanged(nameof(CurrentInvoice));
        }
         

         
        [RelayCommand]
        public async Task SaveInvoiceAsync()
        {
            if (InvoiceItems.Count == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Validation", "Cannot save empty invoice.", "OK");
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
                        $"Balance of Rs. {CurrentInvoice.BalanceAmount} is unpaid. Proceed?",
                        "Yes", "No");

                    if (!proceed) return;
                }

                await _database.RunInTransactionAsync(tran =>
                {
                    tran.Insert(CurrentInvoice);
                    foreach (var item in InvoiceItems)
                    {
                        item.InvoiceId = CurrentInvoice.Id;
                        tran.Insert(item);
                    }
                });

                byte[] rawReceiptBytes = ReceiptBuilder.GenerateThermalReceiptBytes(CurrentInvoice, InvoiceItems);
                await _printService.PrintReceiptAsync(rawReceiptBytes);

                foreach (var item in InvoiceItems) item.PropertyChanged -= InvoiceItem_PropertyChanged;
                InvoiceItems.Clear();
                Payments.Clear();
                GlobalDiscountInput = 0;
                CurrentInvoice = new Invoice();
                OnPropertyChanged(nameof(CurrentInvoice));

                await Application.Current.MainPage.DisplayAlert("Success", "Invoice saved successfully.", "OK");
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Save Invoice Error", ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ShowErrorAsync(string title, Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert(title, $"Error: {ex.Message}", "OK");
        }
    }
}