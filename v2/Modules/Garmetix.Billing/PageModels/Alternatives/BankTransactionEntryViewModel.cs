//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;
//using Garmetix.AI.Billing.Models;
//using Garmetix.Billing.AIBased.Helpers;
//using Garmetix.Models.Accounting;
//using System.Collections.ObjectModel;
//using BankTransaction = Garmetix.AI.Billing.Models.BankTransaction;
//using ChequeLog = Garmetix.AI.Billing.Models.ChequeLog;
//using TransactionMode = Garmetix.AI.Billing.Models.TransactionMode;
//using TransactionType = Garmetix.AI.Billing.Models.TransactionType;

//namespace Garmetix.AI.Billing.ViewModels
//{
//    [QueryProperty(nameof(TransactionId), "TransactionId")]
//    public partial class BankTransactionEntryViewModel : ObservableObject
//    {
//        [ObservableProperty] private bool isBusy;
//        [ObservableProperty] private string transactionId;
//        [ObservableProperty] private string pageTitle = "Record Bank Transaction";

//        [ObservableProperty] private BankTransaction currentTxn;
//        [ObservableProperty] private ChequeLog currentCheque;

//        [ObservableProperty] private ObservableCollection<BankAccount> availableAccounts = new();
//        [ObservableProperty] private BankAccount selectedAccount;

//        // UI Triggers
//        [ObservableProperty] private bool isChequeMode;
//        [ObservableProperty] private Color headerColor = Color.FromArgb("#10B981"); // Default Green for Deposit

//        public Array TransactionTypes => Enum.GetValues(typeof(TransactionType));
//        public Array TransactionModes => Enum.GetValues(typeof(TransactionMode));

//        public BankTransactionEntryViewModel()
//        {
//            CurrentTxn = new BankTransaction();
//            CurrentCheque = new ChequeLog { ChequeDate = DateTime.Now };
//            _ = LoadAccountsAsync();
//        }

//        private async Task LoadAccountsAsync()
//        {
//            var db = await DatabaseHelper.GetDatabaseAsync();
//            var accounts = await db.Table<BankAccount>().Where(b => b.Active).ToListAsync();
//            AvailableAccounts = new ObservableCollection<BankAccount>(accounts);
//        }
//        // --- UI INTERCEPTORS FOR DROPDOWNS ---
//        [ObservableProperty] private TransactionType selectedTxnType = TransactionType.Deposit;
//        [ObservableProperty] private TransactionMode selectedTxnMode = TransactionMode.Cash;

//        // When the user changes Deposit/Withdraw, this fires instantly
//        partial void OnSelectedTxnTypeChanged(TransactionType value)
//        {
//            if (CurrentTxn != null)
//            {
//                CurrentTxn.TransactionType = value;
//            }

//            HeaderColor = value == TransactionType.Deposit
//                ? Color.FromArgb("#10B981") // Emerald Green for Deposit
//                : Color.FromArgb("#F43F5E"); // Rose Red for Withdraw
//        }

//        // When the user changes Cash/Cheque/NEFT, this fires instantly
//        partial void OnSelectedTxnModeChanged(TransactionMode value)
//        {
//            if (CurrentTxn != null)
//            {
//                CurrentTxn.TransactionMode = value;
//            }

//            IsChequeMode = value == TransactionMode.Cheque;
//        }
//        // --- DYNAMIC UI BEHAVIORS ---
//        //partial void OnCurrentTxnChanged(BankTransaction value)
//        //{
//        //    if (value != null)
//        //    {
//        //        value.PropertyChanged += (s, e) =>
//        //        {
//        //            if (e.PropertyName == nameof(BankTransaction.TransactionMode))
//        //            {
//        //                IsChequeMode = CurrentTxn.TransactionMode == TransactionMode.Cheque;
//        //            }
//        //            if (e.PropertyName == nameof(BankTransaction.TransactionType))
//        //            {
//        //                HeaderColor = CurrentTxn.TransactionType == TransactionType.Deposit
//        //                    ? Color.FromArgb("#10B981") // Emerald Green
//        //                    : Color.FromArgb("#F43F5E"); // Rose Red
//        //            }
//        //        };
//        //    }
//        //}

//        [RelayCommand]
//        public async Task SaveTransactionAsync()
//        {
//            if (SelectedAccount == null || CurrentTxn.Amount <= 0)
//            {
//                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Validation", "Please select an account and enter a valid amount.", "OK");
//                return;
//            }

//            if (IsChequeMode && string.IsNullOrWhiteSpace(CurrentCheque.ChequeNumber))
//            {
//                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Validation", "Cheque Number is mandatory for Cheque transactions.", "OK");
//                return;
//            }

//            IsBusy = true;
//            try
//            {
//                var db = await DatabaseHelper.GetDatabaseAsync();

//                CurrentTxn.BankAccountId = SelectedAccount.Id;

//                // Handle Bank Balance Calculation
//                decimal balanceModifier = CurrentTxn.TransactionType == Models.TransactionType.Deposit ? CurrentTxn.Amount : -CurrentTxn.Amount;

//                await db.RunInTransactionAsync(tran =>
//                {
//                    // 1. Save Transaction
//                    if (string.IsNullOrEmpty(TransactionId)) tran.Insert(CurrentTxn);
//                    else tran.Update(CurrentTxn);

//                    // 2. Save Cheque Log (if applicable)
//                    if (IsChequeMode)
//                    {
//                        CurrentCheque.BankTransactionId = CurrentTxn.Id;
//                        CurrentCheque.Type = CurrentTxn.TransactionType == TransactionType.Deposit ? ChequeType.Received : ChequeType.Issued;
//                        CurrentCheque.Amount = CurrentTxn.Amount;
//                        CurrentCheque.PartyName = CurrentTxn.PersonName;

//                        // If it's auto-reconciled, mark cheque as cleared instantly
//                        if (CurrentTxn.IsReconciled)
//                        {
//                            CurrentCheque.Status = ChequeStatus.Cleared;
//                            CurrentCheque.ClearanceDate = DateTime.Now;
//                        }

//                        var existingCheque = tran.Table<ChequeLog>().FirstOrDefault(c => c.BankTransactionId == CurrentTxn.Id);
//                        if (existingCheque == null) tran.Insert(CurrentCheque);
//                        else tran.Update(CurrentCheque);
//                    }

//                    // 3. Update Bank Account Balance
//                    var account = tran.Table<BankAccount>().FirstOrDefault(b => b.Id == SelectedAccount.Id);
//                    if (account != null)
//                    {
//                        account.ClosingBalance += balanceModifier;
//                        tran.Update(account);
//                    }
//                });

//                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Success", "Bank transaction recorded securely.", "OK");
//                await Shell.Current.GoToAsync("..");
//            }
//            catch (Exception ex) { await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Error", ex.Message, "OK"); }
//            finally { IsBusy = false; }
//        }

//        [RelayCommand]
//        public async Task CancelAsync() => await Shell.Current.GoToAsync("..");
//    }
//}