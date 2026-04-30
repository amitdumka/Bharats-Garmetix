using Bharat.ToolKits.Notifications;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Accounting.Models.Vouchers;
using Garmetix.Core.Interfaces;
using Garmetix.Core.Settings;
using Garmetix.CoreServices.Accounting;
using Syncfusion.Maui.DataForm;
using System.Diagnostics;

namespace Garmetix.Accounting.FormModels
{
    public partial class VoucherFormModel : FormModel<VoucherEntry>

    {
        private readonly IDataModel<Voucher> DataModel = new DataModel<Voucher>();
        private bool PrintNow = false;


        public override void InitFormViewModel()
        {
            try
            {
                this.EnablePrinting = SettingsServices.IsPrintingEnabled();
                Entity = new VoucherEntry
                {
                    Id = Guid.NewGuid(),
                    OnDate = DateTime.Now,
                    Amount = 0,
                    VoucherType = VoucherType.Payment,
                    Company = DatabaseService.CompanyId,
                    StoreGroup = DatabaseService.StoreGroupId,
                    Store = DatabaseService.StoreId,
                    Employee = SessionService.CurrentSession?.EmployeeId ?? Guid.Empty,
                    PaymentMode = PaymentMode.Cash,
                    VoucherNumber = AccountingServices.CreateVoucherNumber(DateTime.Now, VoucherType.Payment)
                };
            }
            catch (Exception ex)
            {
                Entity = new VoucherEntry
                {
                    Id = Guid.NewGuid(),
                    OnDate = DateTime.Now,
                    Amount = 0,
                    VoucherType = VoucherType.Payment,
                    Company = DatabaseService.CompanyId,
                    StoreGroup = DatabaseService.StoreGroupId,
                    Store = DatabaseService.StoreId,
                    PaymentMode = PaymentMode.Cash,
                };
                Debug.WriteLine(ex.Message);
            }
        }

        public override void OnGenerateDataFormItem(object sender, GenerateDataFormItemEventArgs e)
        {
            this.GeneratedFormItems(sender, e);
        }
        //[RelayCommand]
        protected override async Task MailIt()
        {
            try
            {
                var result = await Shell.Current.DisplayPromptAsync("Send Voucher", "Enter Email Address", "Send", "Cancel");

                result = result.Trim();
                if (!string.IsNullOrEmpty(result))
                {
                    await AccountingServices.ShareOverEmail(result);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

            }
        }

        //[RelayCommand]
        protected override async Task ShareIt()
        {
            await AccountingServices.ShareVoucher();
        }

        [RelayCommand]
        private Task SaveNPrintButton()
        {
            PrintNow = true;
            SaveButton();
            return Task.CompletedTask;
        }

        protected override async void SaveButton()
        {
            try
            {
                this.IsBusy = true;
                var newData = new Voucher
                {
                    StoreGroupId = Entity!.StoreGroup,
                    StoreId = Entity.Store,
                    AccountNumber = Entity.AccountNumber == Guid.Empty ? null : Entity.AccountNumber,
                    LedgerId = Entity.Ledger == Guid.Empty ? null : Entity.Ledger,
                    CompanyId = Entity.Company,
                    EmployeeId = Entity.Employee,
                    IsParty = false,
                    PartyId = null,
                    Id = IsNew ? Guid.NewGuid() : Entity.Id,

                    Synced = true,
                    Deleted = false,
                    VoucherType = Entity.VoucherType,
                    PartyName = Entity.PartyName,
                    Amount = Entity.Amount,
                    OnDate = Entity.OnDate,
                    PaymentMode = Entity.PaymentMode,
                    PaymentDetails = Entity.PaymentDetails,
                    VoucherNumber = IsNew ? AccountingServices.CreateVoucherNumber(DateTime.Now, Entity.VoucherType) : Entity.VoucherNumber,

                    Remarks = Entity.Remarks,

                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,

                    CreatedBy = DatabaseService.Instance.CurrentUser.Name,

                    Particulars = Entity.Particulars,
                    SlipNumber = Entity.SlipNumber
                };

                var partyid = await AccountingServices.GetPartyId(Entity.Ledger);
                if (partyid != null && partyid != Guid.Empty)
                {
                    newData.IsParty = true;
                    newData.PartyId = partyid;
                }

                var result = await DataModel.SaveAsync(newData, IsNew);

                if (Save(result != null) && PrintNow && this.EnablePrinting)
                {
                    _ = AccountingServices.PrintVoucher(newData, IsNew);
                    PrintNow = false;
                    CanShare = true;


                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                _ = Notify.DisplayNotificationAsync("Error" + ex.Message, isLong: true);
                this.IsBusy = false;
            }
            finally
            {
                PrintNow = false;
                this.IsBusy = false;
            }
        }
    }
}