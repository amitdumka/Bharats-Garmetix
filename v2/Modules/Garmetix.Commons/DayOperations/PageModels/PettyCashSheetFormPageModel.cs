using Garmetix.Base.PageModels;
using Garmetix.Commons.DayOperations.Models;
using Garmetix.Core.Interfaces;
using Garmetix.CoreServices.Accounting;
using Garmetix.Databases.Services;
using Garmetix.Models.DayOperations;
using Syncfusion.Maui.DataForm;
using Syncfusion.Maui.DataGrid;

namespace Garmetix.Commons.DayOperations.PageModels
{
    public class PettyCashSheetFormPageModel : FormModel<PettyCashSheetEntry>
    {
        private IDataModel<PettyCashSheet> DataModel = new DataModel<PettyCashSheet>();
        private PettyCashSheet? _preCalculatedSheet;
        public override void InitFormViewModel()
        {
            Entity = new PettyCashSheetEntry
            {
                Id = Guid.NewGuid(),
                OnDate = DateTime.Now,
                Store = DatabaseService.StoreId
            };
            // Pre-calculate the sheet based on the last sheet and cash details of the day
           _=UpdatePreCalculatedDataAsync();
        }

         
  
        private async Task UpdatePreCalculatedDataAsync()
        {
            // Fetch the pre-calculated data for today. This will be based on the last saved sheet and today's cash details.
            if (_preCalculatedSheet == null)
            {
                _preCalculatedSheet = await AccountingServices.GetTodayPettyCashSheetPreCalculatedData(save: false);
            }
            // Update the form fields with the pre-calculated data
            if (_preCalculatedSheet != null)
            {
                Entity!.OpeningBalance = _preCalculatedSheet.OpeningBalance;
                Entity.CashInHand = _preCalculatedSheet.CashInHand;
                Entity.Sales = _preCalculatedSheet.Sales;
                Entity.Receipts = _preCalculatedSheet.Receipts;
                Entity.DueReceipts = _preCalculatedSheet.DueReceipts;
                Entity.BankWithdrawal = _preCalculatedSheet.BankWithdrawal;
                Entity.Expenses = _preCalculatedSheet.Expenses;
                Entity.BankDeposit = _preCalculatedSheet.BankDeposit;
                Entity.NonCashSale = _preCalculatedSheet.NonCashSale;
                Entity.Payments = _preCalculatedSheet.Payments;
                Entity.CustomerDue = _preCalculatedSheet.CustomerDue;
            }
            

        }
        public override void OnGenerateDataFormItem(object sender, GenerateDataFormItemEventArgs e)
        {
            this.GeneratedFormItems(sender, e);
        }

        protected override async void SaveButton()
        {
            var isValid = EntryForm?.Validate();
            if (isValid == false) return;
            var newData = new PettyCashSheet
            {
                CreatedAt = DateTime.UtcNow, CreatedBy = DatabaseService.Instance.CurrentUser.Name,
                Deleted = false, Synced = false, UpdatedAt = DateTime.UtcNow,
                Id = IsNew ? Guid.NewGuid() : Entity!.Id,
                OnDate = IsNew ? DateTime.Now : Entity!.OnDate,
                StoreId = DatabaseService.StoreId,
                BankDeposit = Entity!.BankDeposit,
                BankWithdrawal = Entity.BankWithdrawal,
                CashInHand = Entity.CashInHand,
                CustomerDue = Entity.CustomerDue,
                DueReceipts = Entity.DueReceipts,
                Expenses = Entity.Expenses,
                NonCashSale = Entity.NonCashSale,
                OpeningBalance = Entity.OpeningBalance,
                Payments = Entity.Payments,
                Receipts = Entity.Receipts,
                Sales = Entity.Sales
            };
            var result = await DataModel.SaveAsync(newData,  IsNew);
            Save(result !=null);
        }
    }
}