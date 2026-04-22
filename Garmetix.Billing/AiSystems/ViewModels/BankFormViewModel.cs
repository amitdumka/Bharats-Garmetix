using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Billing.AIBased.Helpers;
using Garmetix.Models.Accounting;

namespace Garmetix.AI.Billing.ViewModels
{
    [QueryProperty(nameof(AccountId), "AccountId")]
    public partial class BankFormViewModel : ObservableObject
    {
        [ObservableProperty] private string accountId;
        [ObservableProperty] private string pageTitle = "Add Bank Account";
        [ObservableProperty] private bool isBusy;

        [ObservableProperty] private BankAccount currentAccount = new BankAccount { AccountType = AccountType.Current };

        // We use a string for Bank Name to allow users to type new banks instantly
        [ObservableProperty] private string bankNameInput;

        public Array AccountTypes => Enum.GetValues(typeof(AccountType));

        partial void OnAccountIdChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                PageTitle = "Edit Bank Account";
                _ = LoadAccountAsync(Guid.Parse(value));
            }
        }

        private async Task LoadAccountAsync(Guid id)
        {
            var db = await DatabaseHelper.GetDatabaseAsync();
            CurrentAccount = await db.Table<BankAccount>().Where(b => b.Id == id).FirstOrDefaultAsync();

            if (CurrentAccount != null)
            {
                var linkedBank = await db.Table<Bank>().Where(b => b.Id == CurrentAccount.BankId).FirstOrDefaultAsync();
                BankNameInput = linkedBank?.Name;
            }
        }

        [RelayCommand]
        public async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentAccount.AccountNumber) || string.IsNullOrWhiteSpace(BankNameInput))
            {
                await Application.Current.MainPage.DisplayAlert("Validation", "Bank Name and Account Number are required.", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                var db = await DatabaseHelper.GetDatabaseAsync();

                // 1. Resolve or Create the Bank Entity safely
                var existingBank = await db.Table<Bank>().Where(b => b.Name.ToLower() == BankNameInput.ToLower()).FirstOrDefaultAsync();
                if (existingBank == null)
                {
                    existingBank = new Bank { Name = BankNameInput };
                    await db.InsertAsync(existingBank);
                }

                CurrentAccount.BankId = existingBank.Id;

                // 2. Save the Account
                if (string.IsNullOrEmpty(AccountId))
                {
                    CurrentAccount.ClosingBalance = CurrentAccount.OpeningBalance; // Initialize balance
                    await db.InsertAsync(CurrentAccount);
                }
                else
                {
                    await db.UpdateAsync(CurrentAccount);
                }

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex) { await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK"); }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        public async Task CancelAsync() => await Shell.Current.GoToAsync("..");
    }
}