using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Billing.AIBased.Helpers;
using Garmetix.Models.Accounting;
using System.Collections.ObjectModel;

namespace Garmetix.AI.Billing.ViewModels
{
    public partial class PartyRegistryViewModel : ObservableObject
    {
        [ObservableProperty] private bool isBusy;
        [ObservableProperty] private string searchText;

        private System.Collections.Generic.List<Party> _allParties = new();
        [ObservableProperty] private ObservableCollection<Party> filteredParties = new();

        public async Task LoadDataAsync()
        {
            IsBusy = true;
            try
            {
                var db = await DatabaseHelper.GetDatabaseAsync();
                _allParties = await db.Table<Party>().OrderBy(p => p.Name).ToListAsync();
                ApplyFilters();
            }
            catch (Exception ex) { await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK"); }
            finally { IsBusy = false; }
        }

        partial void OnSearchTextChanged(string value) => ApplyFilters();

        private void ApplyFilters()
        {
            var query = _allParties.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var search = SearchText.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(search) ||
                                        (p.Phone != null && p.Phone.Contains(search)));
            }
            FilteredParties = new ObservableCollection<Party>(query.ToList());
        }

        [RelayCommand]
        public async Task AddNewPartyAsync() => await Shell.Current.GoToAsync("PartyFormPage");

        [RelayCommand]
        public async Task ActionMenuAsync(Party party)
        {
            if (party == null) return;

            string action = await Application.Current.MainPage.DisplayActionSheet(
                $"{party.Name}", "Cancel", "Delete", "Edit Details");

            if (action == "Edit Details")
            {
                await Shell.Current.GoToAsync($"PartyFormPage?PartyId={party.Id}");
            }
            else if (action == "Delete")
            {
                bool confirm = await Application.Current.MainPage.DisplayAlert("Delete", $"Permanently remove {party.Name}?", "Yes", "No");
                if (confirm)
                {
                    var db = await DatabaseHelper.GetDatabaseAsync();
                    await db.DeleteAsync(party);
                    await LoadDataAsync();
                }
            }
        }
    }
}