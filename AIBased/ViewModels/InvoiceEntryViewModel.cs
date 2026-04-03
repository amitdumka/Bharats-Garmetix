using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SQLite;
using AadwikaBilling.Models;
using AadwikaBilling.Services;
using AadwikaBilling.Helpers;

namespace AadwikaBilling.ViewModels
{
    public partial class InvoiceEntryViewModel : ObservableObject
    {
        private SQLiteAsyncConnection _database;
        private readonly IPrintService _printService;

        [ObservableProperty]
        private Invoice currentInvoice;

        public ObservableCollection<InvoiceItem> InvoiceItems { get; set; } = new();
        public ObservableCollection<Product> AvailableProducts { get; set; } = new();

        [ObservableProperty]
        private Product selectedProduct;

        public InvoiceEntryViewModel(IPrintService printService)
        {
            _printService = printService;
            CurrentInvoice = new Invoice();
            
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "aadwika_billing.db3");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Invoice>().Wait();
            _database.CreateTableAsync<InvoiceItem>().Wait();
            _database.CreateTableAsync<Product>().Wait();

            // Load Dummy Data for testing
            AvailableProducts.Add(new Product { Name = "Cotton Kurta", Barcode = "1001", BaseRate = 1500, Category = GarmentCategory.ReadyMade });
            AvailableProducts.Add(new Product { Name = "Silk Saree Fabric", Barcode = "1002", BaseRate = 3000, Category = GarmentCategory.Fabric });
        }

        [RelayCommand]
        public void AddProductToInvoice()
        {
            if (SelectedProduct == null) return;

            var newItem = new InvoiceItem
            {
                ProductName = SelectedProduct.Name,
                Category = SelectedProduct.Category,
                Rate = SelectedProduct.BaseRate,
                Quantity = 1,
                DiscountAmount = 0
            };

            InvoiceItems.Add(newItem);
            SelectedProduct = null;
            CalculateInvoiceTotals();
        }

        public void CalculateInvoiceTotals()
        {
            CurrentInvoice.SubTotal = InvoiceItems.Sum(i => i.Rate * i.Quantity);
            CurrentInvoice.TotalDiscount = InvoiceItems.Sum(i => i.DiscountAmount);
            CurrentInvoice.TotalTax = InvoiceItems.Sum(i => i.TaxAmount);
            CurrentInvoice.GrandTotal = InvoiceItems.Sum(i => i.TotalAmount);
            OnPropertyChanged(nameof(CurrentInvoice));
        }

        [RelayCommand]
        public async Task SaveInvoiceAsync()
        {
            if (InvoiceItems.Count == 0) return;

            CurrentInvoice.InvoiceNo = "INV-" + DateTime.Now.Ticks.ToString();
            CalculateInvoiceTotals();

            // Check for part payment logic (simplified alert check)
            if (CurrentInvoice.PaidAmount < CurrentInvoice.GrandTotal)
            {
                bool proceed = await App.Current.MainPage.DisplayAlert("Part Payment", $"Balance of Rs. {CurrentInvoice.BalanceAmount} remaining. Proceed?", "Yes", "No");
                if (!proceed) return;
            }

            await _database.InsertAsync(CurrentInvoice);

            foreach (var item in InvoiceItems)
            {
                item.InvoiceId = CurrentInvoice.Id;
                await _database.InsertAsync(item);
            }

            // Generate and Print
            byte[] rawReceiptBytes = ReceiptBuilder.GenerateInvoiceBytes(CurrentInvoice, InvoiceItems);
            await _printService.PrintReceiptAsync(rawReceiptBytes);

            // Reset
            InvoiceItems.Clear();
            CurrentInvoice = new Invoice();
            OnPropertyChanged(nameof(CurrentInvoice));
        }
    }
}