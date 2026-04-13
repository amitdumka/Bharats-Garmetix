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
    [QueryProperty(nameof(PurchaseId), "PurchaseId")]
    public partial class PurchaseEntryViewModel : ObservableObject
    {
        [ObservableProperty] private bool isBusy;
        [ObservableProperty] private string purchaseId;
        [ObservableProperty] private string pageTitle = "New Purchase Entry";

        [ObservableProperty] private PurchaseInvoice currentPurchase;
        [ObservableProperty] private ObservableCollection<PurchaseItem> purchaseItems = new();

        // We hold onto the old items during an Edit so we can reverse their stock impact
        private List<PurchaseItem> _originalItemsBeforeEdit = new();

        [ObservableProperty] private decimal subTotal;
        [ObservableProperty] private decimal totalTax;
        [ObservableProperty] private decimal grandTotal;

        [ObservableProperty] private string searchText;
        [ObservableProperty] private ObservableCollection<Product> filteredProducts = new();
        [ObservableProperty] private Product selectedProduct;
        private List<Product> _productCache = new();

        public PurchaseEntryViewModel()
        {
            CurrentPurchase = new PurchaseInvoice
            {
                InwardNo = "INW-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                InwardDate = DateTime.Now,
                VendorInvoiceDate = DateTime.Now
            };
            _ = LoadCacheAsync();
        }

        partial void OnPurchaseIdChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                PageTitle = "Edit Purchase Entry";
                _ = LoadExistingPurchaseAsync(Guid.Parse(value));
            }
        }

        private async Task LoadCacheAsync()
        {
            var db = await DatabaseHelper.GetDatabaseAsync();
            _productCache = await db.Table<Product>().ToListAsync();
        }

        private async Task LoadExistingPurchaseAsync(Guid id)
        {
            IsBusy = true;
            try
            {
                var db = await DatabaseHelper.GetDatabaseAsync();
                CurrentPurchase = await db.Table<PurchaseInvoice>().Where(p => p.Id == id).FirstOrDefaultAsync();

                var items = await db.Table<PurchaseItem>().Where(i => i.PurchaseInvoiceId == id).ToListAsync();

                // Save a deep copy of the original items for stock reversal later
                _originalItemsBeforeEdit = items.Select(i => new PurchaseItem { Barcode = i.Barcode, Quantity = i.Quantity }).ToList();

                PurchaseItems = new ObservableCollection<PurchaseItem>(items);
                foreach (var item in PurchaseItems) item.PropertyChanged += (s, e) => CalculateTotals();
                CalculateTotals();
            }
            catch (Exception ex) { await ShowErrorAsync("Error", ex.Message); }
            finally { IsBusy = false; }
        }

        // --- ITEM MANAGEMENT ---
        partial void OnSearchTextChanged(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) { FilteredProducts.Clear(); return; }
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
                var newItem = new PurchaseItem
                {
                    PurchaseInvoiceId = CurrentPurchase.Id,
                    Barcode = value.Barcode,          // CRITICAL: We need Barcode to track stock
                    ProductName = value.Name,
                    Rate = value.BaseRate,
                    Quantity = 1m,
                    TaxPercentage = value.TaxRate
                };
                newItem.PropertyChanged += (s, e) => CalculateTotals();
                PurchaseItems.Add(newItem);

                SearchText = string.Empty;
                CalculateTotals();
            }
        }

        [RelayCommand]
        public void RemoveItem(PurchaseItem item)
        {
            if (item != null && PurchaseItems.Contains(item))
            {
                PurchaseItems.Remove(item);
                CalculateTotals();
            }
        }

        private void CalculateTotals()
        {
            SubTotal = PurchaseItems.Sum(i => i.TaxableValue);
            TotalTax = PurchaseItems.Sum(i => i.TaxAmount);
            GrandTotal = PurchaseItems.Sum(i => i.TotalAmount);

            CurrentPurchase.SubTotal = SubTotal;
            CurrentPurchase.TotalTax = TotalTax;
            CurrentPurchase.GrandTotal = GrandTotal;
        }

        // --- ENTERPRISE SAVING & INVENTORY TRACKING ---
        [RelayCommand]
        public async Task SavePurchaseAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentPurchase.VendorName))
            {
                await ShowErrorAsync("Validation", "Vendor Name is required.");
                return;
            }

            IsBusy = true;
            try
            {
                var db = await DatabaseHelper.GetDatabaseAsync();

                // 1. Compile all barcodes involved in this transaction (old and new)
                var allBarcodes = PurchaseItems.Select(i => i.Barcode)
                                    .Concat(_originalItemsBeforeEdit.Select(o => o.Barcode))
                                    .Distinct().ToList();

                // 2. Fetch existing stock from DB
                var existingStocks = await db.Table<Stock>().Where(s => allBarcodes.Contains(s.Barcode)).ToListAsync();
                var stockDictionary = existingStocks.ToDictionary(s => s.Barcode, s => s);

                // 3. Process Reversals (If Editing)
                foreach (var oldItem in _originalItemsBeforeEdit)
                {
                    if (stockDictionary.TryGetValue(oldItem.Barcode, out var stock))
                    {
                        stock.PurchasedQty -= oldItem.Quantity;
                        //stock.CurrentQty = stock.PurchasedQty - stock.SoldQty;
                    }
                }

                // 4. Process New Additions
                foreach (var newItem in PurchaseItems)
                {
                    if (stockDictionary.TryGetValue(newItem.Barcode, out var stock))
                    {
                        stock.PurchasedQty += newItem.Quantity;
                        stock.BasicCostRate = newItem.Rate; // Update cost price to latest purchase price
                        //stock.CurrentQty = stock.PurchasedQty - stock.SoldQty;
                    }
                    else
                    {
                        // If this product has never been in stock before, create a new row!
                        var newStock = new Stock
                        {
                            Barcode = newItem.Barcode,
                            ProductId = Guid.NewGuid(), // Or fetch actual ProductId from cache
                            PurchasedQty = newItem.Quantity,
                            SoldQty = 0,
                           // CurrentQty = newItem.Quantity,
                            BasicCostRate = newItem.Rate,
                            TaxRate = newItem.TaxPercentage
                        };
                        stockDictionary[newItem.Barcode] = newStock;
                    }
                }

                // 5. ATOMIC COMMIT (Saves Invoice, Items, and ALL Stock updates in one flawless burst)
                await db.RunInTransactionAsync(tran =>
                {
                    // Master Record
                    var existing = tran.Table<PurchaseInvoice>().Where(p => p.Id == CurrentPurchase.Id).FirstOrDefault();
                    if (existing == null) tran.Insert(CurrentPurchase);
                    else tran.Update(CurrentPurchase);

                    // Wipe and replace items safely
                    tran.Table<PurchaseItem>().Delete(i => i.PurchaseInvoiceId == CurrentPurchase.Id);
                    foreach (var item in PurchaseItems) tran.Insert(item);

                    // Update Stock tables
                    foreach (var stock in stockDictionary.Values)
                    {
                        var stockExists = tran.Table<Stock>().Where(s => s.Barcode == stock.Barcode).FirstOrDefault();
                        if (stockExists == null) tran.Insert(stock);
                        else tran.Update(stock);
                    }
                });
                // Notify the dashboard that the database has changed!
                Garmetix.AI.Billing.Services.DashboardDataService.Instance.InvalidateCache();
                await Application.Current.MainPage.DisplayAlert("Success", "Purchase saved and Inventory updated.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex) { await ShowErrorAsync("Save Error", ex.Message); }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        public async Task GoBackAsync() => await Shell.Current.GoToAsync("..");

        private async Task ShowErrorAsync(string title, string msg)
        {
            MainThread.BeginInvokeOnMainThread(async () => await Application.Current.MainPage.DisplayAlert(title, msg, "OK"));
        }
    }
}