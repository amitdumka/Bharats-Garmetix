using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Garmetix.AI.Billing.Models;
using Garmetix.Billing.AIBased.Helpers;

namespace Garmetix.AI.Billing.ViewModels
{
    [QueryProperty(nameof(PartyId), "PartyId")]
    public partial class PartyFormViewModel : ObservableObject
    {
        [ObservableProperty] private string partyId;
        [ObservableProperty] private string pageTitle = "New Party/Ledger";
        [ObservableProperty] private Party currentParty = new Party { Category = PartyType.Customer };

        public Array PartyTypes => Enum.GetValues(typeof(PartyType));

        partial void OnPartyIdChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                PageTitle = "Edit Party/Ledger";
                _ = LoadPartyAsync(Guid.Parse(value));
            }
        }

        private async Task LoadPartyAsync(Guid id)
        {
            var db = await DatabaseHelper.GetDatabaseAsync();
            CurrentParty = await db.Table<Party>().Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        [RelayCommand]
        public async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentParty.Name))
            {
                await Application.Current.MainPage.DisplayAlert("Validation", "Name is required.", "OK");
                return;
            }

            var db = await DatabaseHelper.GetDatabaseAsync();
            if (string.IsNullOrEmpty(PartyId)) await db.InsertAsync(CurrentParty);
            else await db.UpdateAsync(CurrentParty);

            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        public async Task CancelAsync() => await Shell.Current.GoToAsync("..");
    }
}