using Garmetix.Accounting.Models.Banks;
using Garmetix.Core.Interfaces;
using Syncfusion.Maui.DataForm;

namespace Garmetix.Accounting.FormModels
{
    public class BankCashTransactionFormModel : FormModel<BankCashTranscation>
    {
        private readonly IDataModel<BankCashTranscation> DataModel = new DataModel<BankCashTranscation>();

        public override void InitFormViewModel()
        {
            Entity = new BankCashTranscation
            {
                Amount = 0,
                Id = Guid.NewGuid(),
                CompanyId = DatabaseService.CompanyId,
                StoreId = DatabaseService.StoreId,
                StoreGroupId = DatabaseService.StoreGroupId,
                TransactionType = TransactionType.Deposit,
                OnDate = DateTime.Now,
                Deleted = false,
                CreatedBy = DatabaseService.Instance.CurrentUser.UserName,

                UpdatedAt = DateTime.UtcNow,
                Synced = false,
            };
        }

        public override void OnGenerateDataFormItem(object sender, GenerateDataFormItemEventArgs e)
        {
            this.GeneratedFormItems(sender, e);
        }

        protected override void SaveButton()
        {
            new Task(async () =>
            {
                var newData = new BankCashTranscation
                {
                    Id = IsNew ? Guid.NewGuid() : Entity.Id,
                    Amount = Entity.Amount,
                    Deleted = false,
                    CreatedBy = DatabaseService.Instance.CurrentUser.UserName,
                    Naration = Entity.Naration,
                    CreatedAt = DateTime.UtcNow,
                    CompanyId = Entity.CompanyId,
                    ChequeNumber = Entity.ChequeNumber,
                    BankAccountNumber = Entity.BankAccountNumber,
                    BankAccountId = Entity.BankAccountId,
                    TransactionType = Entity.TransactionType,
                    OnDate = Entity.OnDate,
                    Synced = false, StoreGroupId = Entity.StoreGroupId,
                    StoreId=Entity.StoreId, 
                    UpdatedAt = DateTime.UtcNow,
                    Reference = Entity.Reference,
                    
                };
                var result = await DataModel.CreateAsync(newData);
                Save(result != null);
            }
             ).Start();
        }
    }
}