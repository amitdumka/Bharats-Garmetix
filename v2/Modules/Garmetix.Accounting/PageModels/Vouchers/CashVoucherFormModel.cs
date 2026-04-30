using Bharat.ToolKits.Notifications;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Accounting.Models.Vouchers;
using Garmetix.Base.PageModels;
using Garmetix.Core.DataModels;
using Garmetix.Core.Enums;
using Garmetix.Core.Interfaces;
using Garmetix.Core.Models.Accounting;
using Garmetix.Core.Sessions;
using Garmetix.Core.Settings;
using Garmetix.CoreServices.Accounting;
using Garmetix.Databases.Services;
using Syncfusion.Maui.DataForm;
using System.Diagnostics;

namespace Garmetix.Accounting.FormModels
{
    public partial class CashVoucherFormModel : FormModel<CashVoucherEntry>
    {
        private readonly IDataModel<CashVoucher> DataModel = new DataModel<CashVoucher>();
        private bool PrintNow = false;

        public override void OnGenerateDataFormItem(object sender, GenerateDataFormItemEventArgs e)
        {
            this.GeneratedFormItems(sender, e);
        }

        public override void InitFormViewModel()
        {
            try
            {
                this.EnablePrinting = SettingsServices.IsPrintingEnabled();
                Entity = new CashVoucherEntry
                {
                    Amount = 0,
                    SlipNumber = string.Empty,
                    PartyName = string.Empty,
                    Particulars = string.Empty,
                    Remarks = string.Empty,
                    Id = Guid.NewGuid(),
                    OnDate = DateTime.Now,
                    Company = DatabaseService.CompanyId,
                    StoreGroup = DatabaseService.StoreGroupId,
                    Store = DatabaseService.StoreId,
                    Employee = SessionService.CurrentSession.EmployeeId.Value,
                    VoucherType = VoucherType.Payment,
                    VoucherNumber = AccountingServices.CreateCashVoucherNumber(DateTime.Now, VoucherType.Payment)
                };
            }
            catch (Exception ex)
            {
                Entity = new CashVoucherEntry
                {
                    Amount = 0,
                    SlipNumber = string.Empty,
                    PartyName = string.Empty,
                    Particulars = string.Empty,
                    Remarks = string.Empty,
                    Id = Guid.NewGuid(),
                    OnDate = DateTime.Now,
                    Company = DatabaseService.CompanyId,
                    StoreGroup = DatabaseService.StoreGroupId,
                    Store = DatabaseService.StoreId,
                    Employee = SessionService.CurrentSession.EmployeeId.Value,
                    VoucherType = VoucherType.Payment,
                    //VoucherNumber = AccountingServices.CreateCashVoucherNumber(DateTime.Now, VoucherType.Payment)
                };
                Debug.WriteLine(ex.Message);
                Notify.LogError(ex.Message);
            }
        }

        [RelayCommand]
        private void SaveNPrintButton()
        {
            PrintNow = true;
            SaveButton();
        }

        protected override async void SaveButton()
        {

            try
            {
                IsBusy = true;
                //TODO: make all Save as async and run in background thread
                //TODO: handle Exception error
                var newData = new CashVoucher
                {
                    Deleted = false,
                    Synced = false,
                    SlipNumber = Entity.SlipNumber,
                    StoreGroupId = Entity.StoreGroup,
                    StoreId = Entity.Store,
                    TransactionId = Entity.Transaction,

                    Id = IsNew ? Guid.NewGuid() : Entity.Id,
                    VoucherType = Entity.VoucherType,
                    VoucherNumber = IsNew ? AccountingServices.CreateCashVoucherNumber(DateTime.Now, Entity.VoucherType) : Entity.VoucherNumber,
                    PartyName = Entity.PartyName,
                    Particulars = Entity.Particulars,
                    Amount = Entity.Amount,
                    OnDate = Entity.OnDate,
                    CompanyId = Entity.Company,
                    LedgerId = Entity.Ledger == Guid.Empty ? null : Entity.Ledger,
                    Remarks = Entity.Remarks,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = DatabaseService.Instance.CurrentUser.Name,
                    UpdatedAt = DateTime.UtcNow,
                    EmployeeId = Entity.Employee
                };
                var result = await DataModel.SaveAsync(newData, IsNew);
                if (Save(result != null) && PrintNow && this.EnablePrinting)
                {
                    _ = AccountingServices.PrintVoucher(newData, IsNew);
                    PrintNow = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Notify.ShowError(ex.Message, speak:false);
                IsBusy = false;
                PrintNow = false;
            }
            finally
            {

                this.PrintNow = false;
                this.IsBusy = false;
            }


        }
    }
}