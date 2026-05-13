//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;
//using Garmetix.AI.Billing.Models;
//using Garmetix.Billing.AIBased.Helpers;
//using System.Collections.ObjectModel;

//namespace Garmetix.AI.Billing.ViewModels
//{
//    public partial class PurchaseHistoryViewModel : ObservableObject
//    {
//        [ObservableProperty] private bool isBusy;
//        [ObservableProperty] private decimal totalPurchaseAmount;
//        [ObservableProperty] private decimal totalTaxPaid;

//        [ObservableProperty] private ObservableCollection<PurchaseInvoice> purchases = new();
//        [ObservableProperty] private PurchaseInvoice selectedPurchase;

//        public async Task LoadDataAsync()
//        {
//            if (IsBusy) return;
//            IsBusy = true;
//            try
//            {
//                var db = await DatabaseHelper.GetDatabaseAsync();

//                var rawPurchases = await db.Table<PurchaseInvoice>().ToListAsync();
//                var sorted = rawPurchases.OrderByDescending(p => p.InwardDate).ToList();

//                MainThread.BeginInvokeOnMainThread(() =>
//                {
//                    Purchases = new ObservableCollection<PurchaseInvoice>(sorted);
//                    TotalPurchaseAmount = sorted.Sum(p => p.GrandTotal);
//                    TotalTaxPaid = sorted.Sum(p => p.TotalTax);
//                });
//            }
//            catch (Exception ex) { await ShowErrorAsync("Load Error", ex.Message); }
//            finally { IsBusy = false; }
//        }

//        partial void OnSelectedPurchaseChanged(PurchaseInvoice value)
//        {
//            if (value != null) _ = HandleSelectionAsync(value);
//        }

//        private async Task HandleSelectionAsync(PurchaseInvoice purchase)
//        {
//            string action = await Application.Current.MainPage.DisplayActionSheet(
//                $"Inward {purchase.InwardNo}", "Cancel", "Delete Purchase", "Edit Purchase");

//            if (action == "Edit Purchase")
//            {
//                await Shell.Current.GoToAsync($"PurchaseEntryPage?PurchaseId={purchase.Id}");
//            }
//            else if (action == "Delete Purchase")
//            {
//                await DeletePurchaseAsync(purchase);
//            }

//            SelectedPurchase = null; // Deselect row
//        }

//        private async Task DeletePurchaseAsync(PurchaseInvoice purchase)
//        {
//            bool confirm = await Application.Current!.Windows[0].Page!.DisplayAlertAsync(
//                "Delete Inward",
//                $"Delete {purchase.InwardNo}? This will permanently REMOVE these items from your current inventory stock.",
//                "Yes, Delete", "Cancel");

//            if (!confirm) return;

//            IsBusy = true;
//            try
//            {
//                var db = await DatabaseHelper.GetDatabaseAsync();

//                // 1. Load the items we are about to delete
//                var itemsToDelete = await db.Table<PurchaseItem>().Where(i => i.PurchaseInvoiceId == purchase.Id).ToListAsync();

//                // 2. Fetch all related stock records to adjust them
//                var barcodes = itemsToDelete.Select(i => i.Barcode).Distinct().ToList();
//                var stocksToUpdate = await db.Table<Stock>().Where(s => barcodes.Contains(s.Barcode)).ToListAsync();

//                // 3. Reverse the stock
//                foreach (var item in itemsToDelete)
//                {
//                    var stock = stocksToUpdate.FirstOrDefault(s => s.Barcode == item.Barcode);
//                    if (stock != null)
//                    {
//                        stock.PurchasedQty -= item.Quantity;
//                       // stock.CurrentQty = stock.PurchasedQty - stock.SoldQty;
//                    }
//                }

//                // 4. ATOMIC DELETE & STOCK UPDATE
//                await db.RunInTransactionAsync(tran =>
//                {
//                    tran.Table<PurchaseItem>().Delete(i => i.PurchaseInvoiceId == purchase.Id);
//                    tran.Delete(purchase);
//                    foreach (var stock in stocksToUpdate) tran.Update(stock);
//                });

//                await LoadDataAsync();
//            }
//            catch (Exception ex) { await ShowErrorAsync("Delete Error", ex.Message); }
//            finally { IsBusy = false; }
//        }

//        [RelayCommand]
//        public async Task AddNewPurchaseAsync() => await Shell.Current.GoToAsync("PurchaseEntryPage");

//        private async Task ShowErrorAsync(string title, string msg)
//        {
//            MainThread.BeginInvokeOnMainThread(async () => await Application.Current!.Windows[0].Page!.DisplayAlertAsync(title, msg, "OK"));
//        }
//    }
//}