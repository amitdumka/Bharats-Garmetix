using System;
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
    public partial class VoucherRegistryViewModel : ObservableObject
    {
        [ObservableProperty] private bool isBusy;
        [ObservableProperty] private string searchText;

        private List<Voucher> _allVouchers = new();
        [ObservableProperty] private ObservableCollection<Voucher> filteredVouchers = new();

        public async Task LoadDataAsync()
        {
            IsBusy = true;
            try
            {
                var db = await DatabaseHelper.GetDatabaseAsync();
                _allVouchers = await db.Table<Voucher>().OrderByDescending(v => v.OnDate).ToListAsync();
                ApplyFilters();
            }
            catch (Exception ex) { await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK"); }
            finally { IsBusy = false; }
        }

        partial void OnSearchTextChanged(string value) => ApplyFilters();

        private void ApplyFilters()
        {
            var query = _allVouchers.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var search = SearchText.ToLower();
                query = query.Where(v => v.VoucherNumber.ToLower().Contains(search) || 
                                         v.PartyName.ToLower().Contains(search));
            }
            FilteredVouchers = new ObservableCollection<Voucher>(query.ToList());
        }

        [RelayCommand]
        public async Task AddNewVoucherAsync() => await Shell.Current.GoToAsync("VoucherEntryPage");

        [RelayCommand]
        public async Task ActionMenuAsync(Voucher voucher)
        {
            if (voucher == null) return;

            string action = await Application.Current.MainPage.DisplayActionSheet(
                $"{voucher.VoucherNumber}", "Cancel", "Delete", "Edit Voucher", "Print Duplicate");

            if (action == "Edit Voucher")
            {
                await Shell.Current.GoToAsync($"VoucherEntryPage?VoucherId={voucher.Id}");
            }
            else if (action == "Print Duplicate")
            {
                // Trigger your existing QuestPDF generation logic from the previous module here
                await Application.Current.MainPage.DisplayAlert("Print", "Generating Duplicate A5 PDF...", "OK");
            }
            else if (action == "Delete")
            {
                bool confirm = await Application.Current.MainPage.DisplayAlert("Delete", $"Delete {voucher.VoucherNumber}?", "Yes", "No");
                if (confirm)
                {
                    var db = await DatabaseHelper.GetDatabaseAsync();
                    await db.DeleteAsync(voucher);
                    await LoadDataAsync();
                }
            }
        }
    }
}