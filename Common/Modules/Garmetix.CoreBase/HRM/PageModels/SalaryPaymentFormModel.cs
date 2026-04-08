using Bharat.ToolKits.Notifications;
using CommunityToolkit.Mvvm.Input;
using Garmetix.CoreBase.HRM.Models;
using Garmetix.CoreServices.Payroll;
using Garmetix.Models.HRM;
using Syncfusion.Maui.DataForm;
using System.Diagnostics;

namespace Garmetix.CoreBase.HRM.PageModels
{
    public partial class SalaryPaymentFormModel : FormModel<SalaryPaymentEntry>
    {
        protected IDataModel<SalaryPayment> DataModel = new DataModel<SalaryPayment>();

        #region Printing

        private bool PrintNow = false;

        protected override async Task MailIt()
        {
            try
            {
                var result = await Shell.Current.DisplayPromptAsync("Send Voucher", "Enter Email Address", "Send", "Cancel");

                result = result.Trim();
                if (!string.IsNullOrEmpty(result))
                {
                    await PayrollServices.ShareOverEmail(result, isSalaryPayment: true);
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
            await PayrollServices.Share(isSalaryPayment: true);
        }

        [RelayCommand]
        private Task SaveNPrintButton()
        {
            PrintNow = true;
            SaveButton();
            return Task.CompletedTask;
        }

        #endregion Printing

        public SalaryPaymentFormModel() : base()
        {
            Title = "New Salary Payment";
        }

        public async Task InitFormViewModel(Guid salaryPaymentId)
        {
            //this.EnablePrinting = SettingsService.IsPrintingEnabled();
            if (salaryPaymentId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(SalaryPayment), "Salary Payment ID cannot be empty");
            }
            var payment = await DataModel.GetByIdAsync(salaryPaymentId);
            InitFormViewModel(payment!);
        }

        public void InitFormViewModel(SalaryPayment payment)
        {
            this.EnablePrinting = SettingsService.IsPrintingEnabled();

            if (payment == null)
            {
                throw new ArgumentNullException(nameof(payment), "Salary Payment cannot be null");
            }
            IsNew = false;
            Entity = new SalaryPaymentEntry
            {
                VoucherNo = payment.VoucherNumber,
                Employee = payment.EmployeeId,
                OnDate = payment.OnDate,
                Amount = payment.Amount,
                Remarks = payment?.Remarks!,
                Id = payment!.Id,
                Company = payment.CompanyId,
                StoreGroup = payment.StoreGroupId,
                Store = payment.StoreId,
                SalaryComponent = payment.SalaryComponent,
                SalaryMonth = payment.SalaryMonth,
                Deduction = payment.TotalDeductions,
                GrossSalary = payment.GrossSalary,
                NetSalary = payment.NetSalary,
                PaymentMode = payment.PaymentMode,
                SalaryPaySlip = payment.SalaryPaySlipId,

            };
            Title = $"Edit Salary Payment [ {payment.Employee?.StaffName} ]";
        }

        public override void InitFormViewModel()
        {
            this.EnablePrinting = SettingsService.IsPrintingEnabled();

            Entity = new SalaryPaymentEntry()
            {
                Id = Guid.NewGuid(),
                VoucherNo = PayrollServices.GetSalaryPaymentVouherNumber(DateTime.Now).Result,
                Company = DatabaseService.CompanyId,
                Store = DatabaseService.StoreId,
                StoreGroup = DatabaseService.StoreGroupId,
                OnDate = DateTime.Now,
                Amount = 0, // Default to 0, will be set later
                Remarks = string.Empty, // Default to empty
                Deduction = 0, // Default to 0, will be set later
                GrossSalary = 0, // Default to 0, will be set later
                NetSalary = 0, // Default to 0, will be set later
                PaymentMode = PaymentMode.Cash, // Default to cash
                SalaryComponent = SalaryComponent.NetSalary, // Default to Net Salary
                SalaryMonth = Int32.Parse(DateTime.Now.AddMonths(-1).ToString("MMyyyy")), // Default to current month
            };
        }

        public override void OnGenerateDataFormItem(object sender, GenerateDataFormItemEventArgs e)
        {
            this.GeneratedFormItems(sender, e);
        }

        protected override async void SaveButton()
        {
            try
            {
                var newData = new SalaryPayment
                {
                    StoreId = Entity!.Store,
                    Synced = false,
                    VoucherNumber = IsNew ? PayrollServices.GetSalaryPaymentVouherNumber(DateTime.Now).Result : Entity.VoucherNo,
                    SalaryComponent = Entity.SalaryComponent,
                    SalaryMonth = Entity.SalaryMonth,
                    StoreGroupId = Entity.StoreGroup,
                    CompanyId = Entity.Company,
                    Amount = Entity.Amount,
                    EmployeeId = Entity.Employee,
                    Remarks = Entity.Remarks,
                    Deleted = false,
                    Id = IsNew ? Guid.NewGuid() : Entity.Id,
                    OnDate = Entity.OnDate,
                    TotalDeductions = Entity.Deduction,
                    GrossSalary = Entity.GrossSalary,
                    NetSalary = Entity.NetSalary,
                    PaymentMode = Entity.PaymentMode,
                    SalaryPaySlipId = Entity.SalaryPaySlip,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = DatabaseService.Instance.CurrentUser.Name,
                    CreatedAt = DateTime.UtcNow,
                };
                var result = await DataModel.SaveAsync(newData, IsNew);
                if (Save(result != null) && PrintNow && this.EnablePrinting)
                {
                    await PayrollServices.PrintSalaryPayment(newData, IsNew);
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