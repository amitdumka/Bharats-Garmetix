using Bharat.ToolKits.Notifications;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Base.PageModels;
using Garmetix.Core.DataModels;
using Garmetix.Core.Interfaces;
using Garmetix.Core.Models.HRM;
using Garmetix.CoreServices.Payroll;
using Garmetix.Databases.Services;
using Garmetix.HRM.Models;
using Garmetix.Settings;
using Syncfusion.Maui.DataForm;
using System.Diagnostics;

namespace Garmetix.HRM.PageModels
{
    public partial class SalaryPaySlipFormModel : FormModel<SalaryPaySlipEntry>
    {
        protected IDataModel<SalaryPaySlip> DataModel = new DataModel<SalaryPaySlip>();

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
                    await PayrollServices.ShareOverEmail(result, isPaySlip: true);
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
            await PayrollServices.Share(isPaySlip: true);
        }

        [RelayCommand]
        private Task SaveNPrintButton()
        {
            PrintNow = true;
            SaveButton();
            return Task.CompletedTask;
        }

        #endregion Printing

        public SalaryPaySlipFormModel() : base()
        {
            Title = "Salary Pay Slip [New]";
        }

        public override void InitFormViewModel()
        {
            this.EnablePrinting = SettingsService.IsPrintingEnabled();

            Entity = new SalaryPaySlipEntry()
            {
                Id = IsNew ? Guid.NewGuid() : Guid.NewGuid(),
                Company = DatabaseService.CompanyId,
                MonthYear = DateTime.Now.Month + "/" + DateTime.Now.Year,
                PayPeriodStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1),
                PayPeriodEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month - 1)),
                Remarks = string.Empty,
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
                var newData = new SalaryPaySlip
                {
                    Id = IsNew ? Guid.NewGuid() : Entity!.Id,
                    CompanyId = Entity!.Company,
                    MonthYear = Entity.MonthYear,
                    PayPeriodStart = Entity.PayPeriodStart,
                    PayPeriodEnd = Entity.PayPeriodEnd,
                    Remarks = Entity.Remarks,
                    Deleted = false,
                    BasicSalary = Entity.BasicSalary,
                    ConveyanceAllowance = Entity.ConveyanceAllowance,
                    Deductions = Entity.Deductions,
                    HRA = Entity.HRA,
                    EmployeeId = Entity.Employee,
                    Gratuity = Entity.Gratuity,
                    IncomeTax = Entity.IncomeTax,
                    OtherDeductions = Entity.OtherDeductions,
                    OtherEarnings = Entity.OtherEarnings,
                    Incentives = Entity.Incentives,
                    ProfessionalTax = Entity.ProfessionalTax,
                    ProvidentFund = Entity.ProvidentFund,
                    SpecialAllowance = Entity.SpecialAllowance,
                    Synced = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = DatabaseService.Instance.CurrentUser.Name,
                    UpdatedAt = DateTime.UtcNow,
                };
                var result = await DataModel.SaveAsync(newData, IsNew);
                if (Save(result != null) && PrintNow && this.EnablePrinting)
                {
                    await PayrollServices.PrintPaySlip(newData);
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