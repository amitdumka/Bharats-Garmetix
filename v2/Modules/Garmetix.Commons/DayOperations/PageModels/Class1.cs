using Garmetix.CoreBase.DayOperations.Models;
using Garmetix.Models.DayOperations;
using Syncfusion.Maui.DataForm;
using Syncfusion.Maui.DataGrid;

namespace Garmetix.CoreBase.DayOperations.PageModels
{
    public class PettyCashSheetFormPageModel : FormModel<PettyCashSheetEntry>
    {
        private IDataModel<PettyCashSheet> DataModel = new DataModel<PettyCashSheet>();
        public override void InitFormViewModel()
        {
            Entity = new PettyCashSheetEntry
            {
                Id = Guid.NewGuid(),
                OnDate = DateTime.Now,
                Store = DatabaseService.StoreId,
            };
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

    public class CashDetailFormPageModel : FormModel<CashDetailEntry>
    {
        private IDataModel<CashDetail> DataModel = new DataModel<CashDetail>();
        public override void InitFormViewModel()
        {
            Entity = new CashDetailEntry
            {
                Id = Guid.NewGuid(),
                OnDate = DateTime.Now,
                Store = DatabaseService.StoreId,
                Amount = 0,
            };
        }

        public override void OnGenerateDataFormItem(object sender, GenerateDataFormItemEventArgs e)
        {
            this.GeneratedFormItems(sender, e);
        }

        protected override async void SaveButton()
        {
            var isValid = EntryForm?.Validate();
            if (isValid == false) return;
            var newData = new CashDetail
            {
                CreatedAt = DateTime.UtcNow, CreatedBy = DatabaseService.Instance.CurrentUser.Name,
                Deleted = false, Synced = false, UpdatedAt = DateTime.UtcNow,
                Id = IsNew ? Guid.NewGuid() : Entity!.Id,
                OnDate = IsNew ? DateTime.Now : Entity!.OnDate,
                StoreId = DatabaseService.StoreId,
                Amount = Entity!.Amount,
            };
            var result = await DataModel.SaveAsync(newData );
            Save(result != null);
        }
    }

    public class PettyCashSheetPageModel : PageModel<PettyCashSheet>
    {
        public PettyCashSheetPageModel()
        {
            DefaultSortedColName = nameof(PettyCashSheet.OnDate);
            DefaultSortedOrder = Descending;
        }

        protected override ColumnCollection SetGridColumns()
        {
            GridColumns =
           [
                 GetColumn(nameof(PettyCashSheet.OnDate)),
                 GetColumn(nameof(PettyCashSheet.OpeningBalance)),
                 GetColumn(nameof(PettyCashSheet.CashInHand)),
                 GetColumn(nameof(PettyCashSheet.Sales)),
                 GetColumn(nameof(PettyCashSheet.Receipts)),
                 GetColumn(nameof(PettyCashSheet.DueReceipts)),
                 GetColumn(nameof(PettyCashSheet.BankWithdrawal)),
                 GetColumn(nameof(PettyCashSheet.Expenses)),
                 GetColumn(nameof(PettyCashSheet.BankDeposit)),
                 GetColumn(nameof(PettyCashSheet.NonCashSale)),
                 GetColumn(nameof(PettyCashSheet.Payments)),
                 GetColumn(nameof(PettyCashSheet.CustomerDue)),

            ];
            return GridColumns;
        }
    }

    public class CashDetailPageModel : PageModel<CashDetail>
    {
        public CashDetailPageModel()
        {
            DefaultSortedColName = nameof(CashDetail.OnDate);
            DefaultSortedOrder = Descending;
        }

        protected override ColumnCollection SetGridColumns()
        {
            GridColumns =
          [
                GetColumn(nameof(CashDetail.OnDate)),
                 GetColumn(nameof(CashDetail.Amount)),
                 GetColumn(nameof(CashDetail.N2000)),
                 GetColumn(nameof(CashDetail.N500)),
                 GetColumn(nameof(CashDetail.N200)),
                 GetColumn(nameof(CashDetail.N100)),
                 GetColumn(nameof(CashDetail.N50)),
                 GetColumn(nameof(CashDetail.NC20)),
                 GetColumn(nameof(CashDetail.NC10)),
                 GetColumn(nameof(CashDetail.NC5)),
                 GetColumn(nameof(CashDetail.NC2)),
                 GetColumn(nameof(CashDetail.NC1)),

            ];
            return GridColumns;
        }
    }
}