using Bharat.ToolKits.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Core.Enums;
using Garmetix.Core.VM;
using Garmetix.CoreBase.DayOperations.Models;
using Garmetix.Databases.Services;
using Garmetix.Models.DayOperations; 
using Syncfusion.Maui.DataForm;

namespace Garmetix.CoreBase.DayOperations.PageModels
{ //TODO: make the route dynamic or default fallback url
    [ObservableRecipient]
    public partial class DayOperationPageModel : ObservableValidator, IDataFormSourceProvider
    {
        [ObservableProperty]
        protected AppOperation _appOperations = StorageOps.GetPref("AppOperation", AppOperation.Store);

        [ObservableProperty]
        protected string _icon;

        [ObservableProperty]
        protected UserType _role = UserType.Guest;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        protected bool _isBusy;

        [ObservableProperty]
        protected bool _isNew = true;

        [ObservableProperty]
        protected SfDataForm? _entryForm = null;

        public bool IsNotBusy => !IsBusy;

        [ObservableProperty]
        private string? _title = "Day Operation";

        [ObservableProperty]
        private DayBeginEntry? _dayBeginEntry;

        [ObservableProperty]
        private DayEndEntry? _dayEndEntry;

        [ObservableProperty]
        private PettyCashSheet? _pettyCashSheet;

        [ObservableProperty]
        private CashDetail? _cashDetail;

        [ObservableProperty]
        private bool _isDayBeginEntryVisible = false;

        [ObservableProperty]
        private bool _isDayEndEntryVisible = false;

        [ObservableProperty]
        private bool _isPettyCashSheetVisible = false;

        [ObservableProperty]
        private bool _isCashDetailVisible = false;

        protected static DatabaseContext _db => DatabaseService.Instance.LocalDB;

        public object GetSource(string sourceName)
        {
            try
            {
                if (sourceName == "Store")
                {
                    return CommonDataModel.GetStoreList(_db);
                }

                if (sourceName == "Company")
                {
                    return CommonDataModel.GetCompanyList(_db);
                }
                if (sourceName == "StoreGroup" || sourceName == "Group")
                {
                    return CommonDataModel.GetGroupList(_db);
                }
                if (sourceName == "LedgerGroup")
                {
                    return CommonDataModel.GetLedgerGroupList(_db);
                }
                if (sourceName == "Ledger")
                {
                    return CommonDataModel.GetLedgerList(_db);
                }
                if (sourceName == "Transaction")
                {
                    return CommonDataModel.GetTransactionList(_db);
                }
                if (sourceName == "Employee")
                {
                    return CommonDataModel.GetEmployeeList(_db);
                }
                if (sourceName == "BankAccount" || sourceName == "Account" || sourceName == "AccountNumber")
                {
                    return CommonDataModel.GetBankAccountList(_db);
                }
                if (sourceName == "Bank")
                {
                    return CommonDataModel.GetBankList(_db);
                }
                if (sourceName == "Salesman" || sourceName == "Salesmen")
                {
                    return CommonDataModel.GetSalesmanList(_db);
                }
                if (sourceName == "InvoiceNumber" || sourceName == "InvoiceNumbers")
                {
                    return CommonDataModel.GetInvoiceNumberList(_db);
                }
                if (sourceName == "Vendor" || sourceName == "Vendors")
                {
                    return CommonDataModel.GetVendorList(_db);
                }
                if (sourceName == "Party" || sourceName == "Parties")
                {
                    return CommonDataModel.GetVendorList(_db);
                }

                if (sourceName == "DueInvoiceNumber" || sourceName == "DueInvoiceNumbers")
                {
                    return CommonDataModel.GetDueInvoiceNumberList(_db);
                }

                return new List<ComboBoxItemVM>();
            }
            catch (Exception ex)
            {
                Notify.ShowError("Source Error", ex.Message, snabackbar: true);
                SentrySdk.CaptureException(ex);
                return new List<ComboBoxItemVM>();
            }
        }

        public void Initialize(string parameter)
        {
            if (parameter == "Begin")
            {
                IsDayBeginEntryVisible = true;
                DayBeginEntry = new DayBeginEntry
                {
                    Id = Guid.NewGuid(),
                    OnDate = DateTime.Now,
                    Store = DatabaseService.StoreId,
                };
                var yesterday = _db.CashDetails.Where(x => x.OnDate == DateTime.Now.AddDays(-1) && x.StoreId == DatabaseService.StoreId).FirstOrDefault();
                if (yesterday != null)
                {
                    DayBeginEntry.OpeningBalance = yesterday.Amount;
                    DayBeginEntry.CashDetail = new CashDetailEntry { Amount = yesterday.Amount, OnDate = DateTime.Now.AddDays(-1), Store = DatabaseService.StoreId, Id = Guid.NewGuid() };
                }
                else
                {
                    DayBeginEntry.OpeningBalance = 0;
                    DayBeginEntry.CashDetail = new CashDetailEntry
                    {
                        Amount = 0,
                        OnDate = DateTime.Now.AddDays(-1),
                        Store = DatabaseService.StoreId
                    ,
                        Id = Guid.NewGuid(),
                    };
                }
            }
            else if (parameter == "End")
            {
                //Create Day End Entry
                DayEndEntry = new DayEndEntry
                {
                    ClosingBalance = 0,
                    Id = Guid.NewGuid(),
                    OnDate = DateTime.Now,
                    Store = DatabaseService.StoreId,
                    CashDetail = new CashDetailEntry { Id = Guid.NewGuid(), OnDate = DateTime.Now, Store = DatabaseService.StoreId, Amount = 0 },
                    PettyCashSheet = new PettyCashSheetEntry { Id = Guid.NewGuid(), OnDate = DateTime.Now, Store = DatabaseService.StoreId, }
                };

                var today = _db.DayBegins.Where(c => c.OnDate.Date == DateTime.Now.Date && c.StoreId == DatabaseService.StoreId).FirstOrDefault();
                if (today != null)
                {
                }
            }
            else if (parameter == "PettyCashSheet")
            {
                PettyCashSheet = new PettyCashSheet { Id = Guid.NewGuid(), OnDate = DateTime.Now, StoreId = DatabaseService.StoreId, Deleted = false, Synced = false };
            }
            else if (parameter == "CashDetail")
            {
                CashDetail = new CashDetail { Id = Guid.NewGuid(), OnDate = DateTime.Now, StoreId = DatabaseService.StoreId, Amount = 0, Deleted = false, Synced = false };
            }
        }

        /// <summary>
        /// This method is used to generate the form items and Control it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnGenerateDataFormItem(object sender, GenerateDataFormItemEventArgs e)
        {
            //    this.GeneratedFormItems(sender, e);
            //}

            //protected void GeneratedFormItems(object sender, GenerateDataFormItemEventArgs e)
            //{
            if (e.DataFormItem != null)
            {
                if (e.DataFormItem.FieldName == "Id" || e.DataFormItem.FieldName.EndsWith("Id"))
                {
                    e.DataFormItem.IsVisible = false;
                }
                if (e.DataFormItem.FieldName == "EndDate" || e.DataFormItem.FieldName.Contains("Date") || e.DataFormItem.FieldName == "OnDate")
                {
                    e.DataFormItem.IsVisible = false;
                }

                if (AppOperations == AppOperation.Store)
                {
                    if (e.DataFormItem.FieldName == "CompanyId" || e.DataFormItem.FieldName == "StoreGroupId" || e.DataFormItem.FieldName == "Store" || e.DataFormItem.FieldName == "Company" || e.DataFormItem.FieldName == "StoreGroup" || e.DataFormItem.FieldName == "Store")
                    {
                        e.DataFormItem.IsVisible = false;
                    }
                }
                if (AppOperations == AppOperation.StoreGroup)
                {
                    if ((e.DataFormItem.FieldName == "StoreGroupId" || e.DataFormItem.FieldName == "Store" || e.DataFormItem.FieldName == "Company" || e.DataFormItem.FieldName == "StoreGroup" || e.DataFormItem.FieldName == "Store") && e.DataFormItem is DataFormComboBoxItem cbi)
                    {
                        cbi.SelectedValuePath = "Id";
                        cbi.DisplayMemberPath = "Name";
                    }
                    if (e.DataFormItem.FieldName == "CompanyId" || e.DataFormItem.FieldName == "Store" || e.DataFormItem.FieldName == "Company" || e.DataFormItem.FieldName == "StoreGroup" || e.DataFormItem.FieldName == "Store")
                    {
                        e.DataFormItem.IsVisible = false;
                    }
                }
                if (AppOperations == AppOperation.Company)
                {
                    if ((e.DataFormItem.FieldName == "CompanyId" || e.DataFormItem.FieldName == "StoreGroupId" || e.DataFormItem.FieldName == "Store" || e.DataFormItem.FieldName == "Company" || e.DataFormItem.FieldName == "StoreGroup" || e.DataFormItem.FieldName == "Store") && e.DataFormItem is DataFormComboBoxItem cbi)
                    {
                        cbi.SelectedValuePath = "Id";
                        cbi.DisplayMemberPath = "Name";
                    }
                }

                if ((e.DataFormItem.FieldName == "LedgerId" || e.DataFormItem.FieldName == "LedgerGroupId" || e.DataFormItem.FieldName == "Ledger" || e.DataFormItem.FieldName == "LedgerGroup" || e.DataFormItem.FieldName == "Transaction" || e.DataFormItem.FieldName == "TransactionId") && e.DataFormItem is DataFormComboBoxItem cbi2)
                {
                    cbi2.SelectedValuePath = "Id";
                    cbi2.DisplayMemberPath = "Name";
                }
                if ((e.DataFormItem.FieldName == "InvoiceNumber" || e.DataFormItem.FieldName == "Employee" || e.DataFormItem.FieldName == "EmployeId") && e.DataFormItem is DataFormComboBoxItem cbi3)
                {
                    cbi3.SelectedValuePath = "Id";
                    cbi3.DisplayMemberPath = "Name";
                }

                if ((e.DataFormItem.FieldName == "Tax" || e.DataFormItem.FieldName == "Account" || e.DataFormItem.FieldName == "BankAccount" || e.DataFormItem.FieldName == "AccountNumber" || e.DataFormItem.FieldName == "Bank") && e.DataFormItem is DataFormComboBoxItem cbi4)
                {
                    cbi4.SelectedValuePath = "Id";
                    cbi4.DisplayMemberPath = "Name";
                }
                if ((e.DataFormItem.FieldName == "PurchaseInvoice" || e.DataFormItem.FieldName == "Product" || e.DataFormItem.FieldName == "ProductSubCategory" || e.DataFormItem.FieldName == "ProductCategory" || e.DataFormItem.FieldName == "Vendor" || e.DataFormItem.FieldName == "Party") && e.DataFormItem is DataFormComboBoxItem cbi5)
                {
                    cbi5.SelectedValuePath = "Id";
                    cbi5.DisplayMemberPath = "Name";
                }
                if ((e.DataFormItem.FieldName == "DueInvoiceNumber" || e.DataFormItem.FieldName == "InvoiceNumber") && e.DataFormItem is DataFormComboBoxItem cbi6)
                {
                    cbi6.SelectedValuePath = "Name";
                    cbi6.DisplayMemberPath = "Name";
                }
            }
        }

        #region Command

        /// <summary>
        /// This method is used to navigate to previous page
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        protected async Task Back()
        {
            await Shell.Current.GoToAsync($"..");
        }

        [RelayCommand]
        public virtual Task Appearing()
        {
            return Task.CompletedTask;
        }

        [RelayCommand]
        public virtual void Edit(Object itemToEdit)
        {
        }

        [RelayCommand]
        public virtual void Delete(Object itemToDelete)
        {
        }

        [RelayCommand]
        private void SaveDayClosingEntry()
        {
            var isValid = EntryForm.Validate();
            if (!isValid)
            {
                Notify.ShowError("Please fill the required fields.", speak: false);
            }
            EntryForm.Commit();

            new Task(async () =>
            {
                try
                {
                    var cashDetail = new CashDetail
                    {
                        N100 = DayBeginEntry!.CashDetail.N100,
                        N200 = DayBeginEntry.CashDetail.N200,
                        N500 = DayBeginEntry.CashDetail.N500,
                        N2000 = DayBeginEntry.CashDetail.N2000,
                        N50 = DayBeginEntry.CashDetail.N50,
                        NC1 = DayBeginEntry.CashDetail.NC1,
                        NC10 = DayBeginEntry.CashDetail.NC10,
                        NC20 = DayBeginEntry.CashDetail.NC20,
                        NC2 = DayBeginEntry.CashDetail.NC2,
                        NC5 = DayBeginEntry.CashDetail.NC5,
                        Amount = DayBeginEntry.CashDetail.Amount,
                        Synced = false,
                        Deleted = false,
                        Id = DayBeginEntry.CashDetail.Id,
                        OnDate = DayBeginEntry.OnDate,
                        StoreId = DayBeginEntry.Store
                    };
                    var dayClosing = new DayEnd
                    {

                        CashDetailId = DayEndEntry.CashDetail.Id,
                        Synced = false,
                        UpdatedAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = DatabaseService.Instance.CurrentUser.Name,
                        Deleted = false,
                        Id = DayEndEntry.Id,
                        OnDate = DayEndEntry.OnDate,
                        ClosingBalance = DayEndEntry.ClosingBalance,
                        StoreId = DayBeginEntry.Store
                    };
                    var pettyCashSheet = new PettyCashSheet
                    {
                        BankDeposit = DayEndEntry.PettyCashSheet.BankDeposit,
                        BankWithdrawal = DayEndEntry.PettyCashSheet.BankWithdrawal,
                        CashInHand = DayEndEntry.PettyCashSheet.CashInHand,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = DatabaseService.Instance.CurrentUser.Name,
                        CustomerDue = DayEndEntry.PettyCashSheet.CustomerDue,
                        Deleted = false,
                        Id = DayEndEntry.PettyCashSheet.Id,
                        OnDate = DayEndEntry.PettyCashSheet.OnDate,
                        Synced = false,
                        UpdatedAt = DateTime.UtcNow,
                        DueReceipts = DayEndEntry.PettyCashSheet.DueReceipts,
                        Expenses = DayEndEntry.PettyCashSheet.Expenses,
                        StoreId = DayBeginEntry.Store,
                        NonCashSale = DayEndEntry.PettyCashSheet.NonCashSale
                        ,
                        OpeningBalance = DayEndEntry.PettyCashSheet.OpeningBalance
                        ,
                        Payments = DayEndEntry.PettyCashSheet.Payments,
                        Receipts = DayEndEntry.PettyCashSheet.Receipts
                        ,
                        Sales = DayEndEntry.PettyCashSheet.Sales
                    };
                    _db.CashDetails.Add(cashDetail);
                    _db.DayEnds.Add(dayClosing);
                    _db.PettyCashSheets.Add(pettyCashSheet);
                    var result = await _db.SaveChangesAsync();
                    if (result > 0)
                    {
                        //TODO: Create a PDF page to Share and Print Petty Cash Sheet and Cash Detail and DSR

                        //TODO: handle for moving to new page, Set Prefernce for Day Mode Begin 
                        //Disble write permission for Day Mode if process for Store Manager and Cashier
                        _ = Notify.ShowSuccess("Day operation  started successfully.", speak: true);
                        DayEndEntry = new DayEndEntry();
                        await Shell.Current.GoToAsync("///stores");
                    }
                    else
                    {
                        _ = Notify.ShowError("Something went wrong. Please try again.", speak: false);
                    }
                }
                catch (Exception ex)
                {
                    _ = Notify.ShowError(ex, speak: false, snabackbar: true);
                    await Shell.Current.GoToAsync("///stores");
                }
            }
             ).Start();
        }
        [RelayCommand]
        public void SaveDayBeginEntry()
        {
            var isValid = EntryForm.Validate();
            if (!isValid)
            {
                Notify.ShowError("Please fill the required fields.", speak: false);
            }
            EntryForm.Commit();

            new Task(async () =>
            {
                try
                {
                    var cashDetail = new CashDetail
                    {
                        N100 = DayBeginEntry!.CashDetail.N100,
                        N200 = DayBeginEntry.CashDetail.N200,
                        N500 = DayBeginEntry.CashDetail.N500,
                        N2000 = DayBeginEntry.CashDetail.N2000,
                        N50 = DayBeginEntry.CashDetail.N50,
                        NC1 = DayBeginEntry.CashDetail.NC1,
                        NC10 = DayBeginEntry.CashDetail.NC10,
                        NC20 = DayBeginEntry.CashDetail.NC20,
                        NC2 = DayBeginEntry.CashDetail.NC2,
                        NC5 = DayBeginEntry.CashDetail.NC5,
                        Amount = DayBeginEntry.CashDetail.Amount,
                        Synced = false,
                        Deleted = false,
                        Id = DayBeginEntry.CashDetail.Id,
                        OnDate = DayBeginEntry.OnDate,
                        StoreId = DayBeginEntry.Store
                    };
                    var dayBegin = new DayBegin
                    {
                        CashDetailId = DayBeginEntry.CashDetail.Id,
                        Synced = false,
                        UpdatedAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = DatabaseService.Instance.CurrentUser.Name,
                        Deleted = false,
                        Id = DayBeginEntry.Id,
                        OnDate = DayBeginEntry.OnDate,
                        OpeningBalance = DayBeginEntry.OpeningBalance,
                        StoreId = DayBeginEntry.Store
                    };
                    _db.CashDetails.Add(cashDetail);
                    _db.DayBegins.Add(dayBegin);
                    var result = await _db.SaveChangesAsync();
                    if (result > 0)
                    {
                        //TODO: handle for moving to new page, Set Prefernce for Day Mode Begin 
                        //Disble write permission for Day Mode if process for Store Manager and Cashier
                        _ = Notify.ShowSuccess("Day operation  started successfully.", speak: true);
                        DayBeginEntry = new DayBeginEntry();
                        await Shell.Current.GoToAsync("///main");
                    }
                    else
                    {
                        _ = Notify.ShowError("Something went wrong. Please try again.", speak: false);
                    }
                }
                catch (Exception ex)
                {
                    _ = Notify.ShowError(ex, speak: false, snabackbar: true);
                    await Shell.Current.GoToAsync("//home");
                }
            }
             ).Start();
        }

        [RelayCommand]
        public void Cancel()
        {
            if (IsDayBeginEntryVisible)
            {
                DayBeginEntry = new DayBeginEntry();
            }
            if (IsDayEndEntryVisible)
            {
                DayEndEntry = new DayEndEntry();
            } else
            if (IsPettyCashSheetVisible)
            {
                //PettyCashSheetEntry = new PettyCashSheetEntry();
            }else if(IsCashDetailVisible)
            {
                //CashDetailEntry = new CashDetailEntry();
            }
        }

        [RelayCommand]
        public void Refresh()
        {
            if (IsDayBeginEntryVisible)
            {
                // Fetch Yesterday's data like Cash Details and Closing Balance.
            }
        }

        [RelayCommand]
        private void Share()
        {
            if (IsDayBeginEntryVisible)
            {
                //Share Day Begin Data to Email , WhatsApp, others
            }
        }

        #endregion Command
    }
}