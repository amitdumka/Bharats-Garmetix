using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Billing.AIBased.Helpers;
using Garmetix.Models.Accounting;
using Garmetix.Models.Enums;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Collections.ObjectModel;

namespace Garmetix.AI.Billing.ViewModels
{
    [QueryProperty(nameof(VoucherId), "VoucherId")]
    public partial class VoucherEntryViewModel : ObservableObject
    {
        [ObservableProperty] private bool isBusy;
        [ObservableProperty] private string voucherId;
        [ObservableProperty] private string pageTitle = "New Accounting Voucher";
        
        [ObservableProperty] private Voucher currentVoucher;
        [ObservableProperty] private bool isBankMode;

        // Dropdown Data Sources
        public Array VoucherTypes => Enum.GetValues(typeof(VoucherType));
        public Array PaymentModes => Enum.GetValues(typeof(PaymentMode));
        
        [ObservableProperty] private ObservableCollection<Party> availableParties = new();
        [ObservableProperty] private ObservableCollection<BankAccount> availableBanks = new();

        public VoucherEntryViewModel()
        {
            CurrentVoucher = new Voucher
            {
                VoucherType = VoucherType.Payment,
                PaymentMode = PaymentMode.Cash,
                VoucherNumber = "VCH-" + DateTime.Now.ToString("yyyyMMddHHmm"),
                Particulars = string.Empty, PartyName = "Cash",
                OnDate = DateTime.Now
            };
            _ = LoadDependenciesAsync();
        }

        partial void OnVoucherIdChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                PageTitle = "Edit Voucher";
                _ = LoadVoucherAsync(Guid.Parse(value));
            }
        }

        // Triggered by the Code-Behind when the ComboBox changes
        public void CheckBankMode()
        {
            if (CurrentVoucher != null)
            {
                IsBankMode = CurrentVoucher.PaymentMode != PaymentMode.Cash;

                if (!IsBankMode)
                {
                    CurrentVoucher.AccountNumber = null;
                    CurrentVoucher.PaymentDetails = string.Empty;
                }
            }
        }
        // --- ADD THIS TO HANDLE THE COMBOBOX SELECTION ---
        [ObservableProperty] private BankAccount selectedBank;

        partial void OnSelectedBankChanged(BankAccount value)
        {
            if (value != null)
            {
                CurrentVoucher.AccountNumber = value.Id; // Map the ID securely to the database model
            }
            else
            {
                CurrentVoucher.AccountNumber = null;
            }
        }
        private async Task LoadDependenciesAsync()
        {
            var db = await DatabaseHelper.GetDatabaseAsync();
            var parties = await db.Table<Party>().ToListAsync();
            var banks = await db.Table<BankAccount>().Where(b => b.Active).ToListAsync();

            AvailableParties = new ObservableCollection<Party>(parties);
            AvailableBanks = new ObservableCollection<BankAccount>(banks);
        }

        //private async Task LoadVoucherAsync(Guid id)
        //{
        //    var db = await DatabaseHelper.GetDatabaseAsync();
        //    CurrentVoucher = await db.Table<Voucher>().Where(v => v.Id == id).FirstOrDefaultAsync();
        //    CheckBankMode();
        //}
        private async Task LoadVoucherAsync(Guid id)
        {
            var db = await DatabaseHelper.GetDatabaseAsync();
            CurrentVoucher = await db.Table<Voucher>().Where(v => v.Id == id).FirstOrDefaultAsync();

            // NEW: Pre-select the bank account in the UI if editing
            if (CurrentVoucher.AccountNumber.HasValue)
            {
                SelectedBank = AvailableBanks.FirstOrDefault(b => b.Id == CurrentVoucher.AccountNumber.Value);
            }

            CheckBankMode();
        }
        // Triggered by UI when Payment Mode Changes
        //public void CheckBankMode()
        //{
        //    IsBankMode = CurrentVoucher.PaymentMode != PaymentMode.Cash;
        //    if (!IsBankMode)
        //    {
        //        CurrentVoucher.AccountNumber = null; // Clear bank details if switched to cash
        //        CurrentVoucher.PaymentDetails = string.Empty;
        //    }
        //    OnPropertyChanged(nameof(CurrentVoucher));
        //}

        [RelayCommand]
        public async Task SaveAndPrintAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentVoucher.PartyName) || CurrentVoucher.Amount <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Validation", "Please enter a valid Party Name and Amount.", "OK");
                return;
            }

            if (IsBankMode && CurrentVoucher.AccountNumber == null)
            {
                await Application.Current.MainPage.DisplayAlert("Validation", "Please select a Bank Account for non-cash transactions.", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                var db = await DatabaseHelper.GetDatabaseAsync();
                
                // Link Party if it exists in the database
                var matchedParty = AvailableParties.FirstOrDefault(p => p.Name.Equals(CurrentVoucher.PartyName, StringComparison.OrdinalIgnoreCase));
                if (matchedParty != null)
                {
                    CurrentVoucher.PartyId = matchedParty.Id;
                    CurrentVoucher.IsParty = true;
                }

                // Save or Update
                if (string.IsNullOrEmpty(VoucherId)) await db.InsertAsync(CurrentVoucher);
                else await db.UpdateAsync(CurrentVoucher);

                // --- PRINTING LOGIC ---
                await GenerateAndPrintPdfAsync(CurrentVoucher);
            }
            catch (Exception ex) { await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK"); }
            finally { IsBusy = false; }
        }

        private async Task GenerateAndPrintPdfAsync(Voucher voucher)
        {
            // We use QuestPDF's ability to merge documents to print 2 copies (Original & Duplicate) in one PDF file.
            string filePath = System.IO.Path.Combine(Microsoft.Maui.Storage.FileSystem.CacheDirectory, $"{voucher.VoucherNumber}.pdf");

            // If it's a new entry, print 2 copies. If it's an edit/reprint, print 1 duplicate.
            bool isReprint = !string.IsNullOrEmpty(VoucherId);

            var document = Document.Merge(new List<IDocument>
            {
                new Garmetix.AI.Billing.PdfServices.VoucherDocument(voucher, isDuplicate: isReprint), // Copy 1
            });

            if (!isReprint)
            {
                // Add the duplicate page for the accountant file
                document = Document.Merge(new List<IDocument>
                {
                    new Garmetix.AI.Billing.PdfServices.VoucherDocument(voucher, isDuplicate: false), // Original for Party
                    new Garmetix.AI.Billing.PdfServices.VoucherDocument(voucher, isDuplicate: true)   // Duplicate for Office
                });
            }

            document.GeneratePdf(filePath);

            await Microsoft.Maui.ApplicationModel.Launcher.OpenAsync(new Microsoft.Maui.ApplicationModel.OpenFileRequest
            {
                Title = "Print Voucher",
                File = new Microsoft.Maui.Storage.ReadOnlyFile(filePath)
            });

            // Navigate back to history
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        public async Task CancelAsync() => await Shell.Current.GoToAsync("..");
    }
}