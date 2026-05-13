//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;
//using Garmetix.Billing.AIBased.Helpers;
//using Garmetix.Models.Accounting;
//using System.Collections.ObjectModel;

//namespace Garmetix.AI.Billing.ViewModels
//{
//    public partial class BankRegistryViewModel : ObservableObject
//    {
//        [ObservableProperty] private bool isBusy;
//        [ObservableProperty] private ObservableCollection<BankAccount> bankAccounts = new();

//        public async Task LoadDataAsync()
//        {
//            IsBusy = true;
//            try
//            {
//                var db = await DatabaseHelper.GetDatabaseAsync();

//                // Fetch all accounts and manually fetch their linked Bank names
//                var accounts = await db.Table<BankAccount>().ToListAsync();
//                var banks = await db.Table<Bank>().ToListAsync();

//                foreach (var acc in accounts)
//                {
//                    acc.Bank = banks.FirstOrDefault(b => b.Id == acc.BankId) ?? new Bank { Name = "Unknown Bank" };
//                }

//                BankAccounts = new ObservableCollection<BankAccount>(accounts);
//            }
//            catch (Exception ex) { await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Error", ex.Message, "OK"); }
//            finally { IsBusy = false; }
//        }

//        [RelayCommand]
//        public async Task AddAccountAsync() => await Shell.Current.GoToAsync("BankFormPage");

//        [RelayCommand]
//        public async Task EditAccountAsync(BankAccount account)
//        {
//            if (account != null)
//                await Shell.Current.GoToAsync($"BankFormPage?AccountId={account.Id}");
//        }

//        [RelayCommand]
//        public async Task DeleteAccountAsync(BankAccount account)
//        {
//            if (account == null) return;

//            bool confirm = await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Warning", $"Delete {account.AccountNumber}?", "Yes", "No");
//            if (confirm)
//            {
//                var db = await DatabaseHelper.GetDatabaseAsync();
//                await db.DeleteAsync(account);
//                await LoadDataAsync();
//            }
//        }
//    }
//}