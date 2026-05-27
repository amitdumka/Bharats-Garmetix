using Garmetix.Accounting.Models.Banks;
using Garmetix.Core.Interfaces;
using Syncfusion.Maui.DataForm;

namespace Garmetix.Accounting.FormModels
{
    public class BankTransactionFormModel : FormModel<BankTransactionEntry>
    {
        private readonly IDataModel<BankTransaction> DataModel = new DataModel<BankTransaction>();

        public override void InitFormViewModel()
        {
            Entity = new BankTransactionEntry
            {
                Amount = 0,
                Id = Guid.NewGuid(),
                Company = DatabaseService.CompanyId,
                TransactionMode = TransactionMode.Cash,
                TransactionType = TransactionType.Deposit,
                OnDate = DateTime.Now
            ,
                PersonName = DatabaseService.Instance.CurrentUser.UserName,
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
                var newData = new BankTransaction
                {
                    Id = IsNew ? Guid.NewGuid() : Entity.Id,
                    Amount = Entity.Amount,
                    Deleted = false,
                    CreatedBy = DatabaseService.Instance.CurrentUser.UserName,

                    CreatedAt = DateTime.UtcNow,
                    CompanyId = Entity.Company,
                    Narration = Entity.Narration
                    ,
                    BankAccountId = Entity.BankAccount,
                    TransactionMode = Entity.TransactionMode,
                    TransactionType = Entity.TransactionType,
                    OnDate = Entity.OnDate,
                    Synced = false,
                    UpdatedAt = DateTime.UtcNow,
                    Reference = Entity.Reference,
                    PersonName = Entity.PersonName,
                };
                var result = await DataModel.CreateAsync(newData);
                Save(result != null);
            }
             ).Start();
        }
    }
}