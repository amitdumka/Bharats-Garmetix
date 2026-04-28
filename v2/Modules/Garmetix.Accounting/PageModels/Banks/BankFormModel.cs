using Syncfusion.Maui.DataForm;

namespace Garmetix.CoreBase.Accounting.FormModels
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

    public class ChequeLogFormModel : FormModel<ChequeLogEntry>
    {
        private readonly IDataModel<ChequeLog> DataModel = new DataModel<ChequeLog>();

        public override void InitFormViewModel()
        {
            Entity = new ChequeLogEntry
            {
                Id = Guid.NewGuid(),
                Company = DatabaseService.CompanyId,
                OnDate = DateTime.Now,
                PersonName = DatabaseService.Instance.CurrentUser.UserName,
                Amount = 0,
                InHouse = false,
                ChequeDate = DateTime.Now.AddDays(1),
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
                var newData = new ChequeLog
                {
                    Id = IsNew ? Guid.NewGuid() : Entity!.Id,
                    Deleted = false,
                    CreatedBy = DatabaseService.Instance.CurrentUser.UserName,
                    CreatedAt = DateTime.UtcNow,
                    CompanyId = Entity!.Company,
                    CheequeNumber = Entity.CheequeNumber,
                    ChequeBank = Entity.ChequeBank,
                    ChequeNumber = Entity.ChequeNumber,
                    Status = Entity.Status,
                    Narration = Entity.Narration,

                    BankAccountId = Entity.BankAccount,
                    OnDate = Entity.OnDate,
                    Synced = false,
                    UpdatedAt = DateTime.UtcNow,
                    PersonName = Entity.PersonName,
                    Amount = Entity.Amount,
                    InHouse = Entity.InHouse,
                    ChequeDate = Entity.ChequeDate
                };
                var result = await DataModel.CreateAsync(newData);
                Save(result != null);
            }
              ).Start();
        }
    }

    public class BankFormModel : FormModel<BankEntry>
    {
        private readonly IDataModel<Bank> DataModel = new DataModel<Bank>();

        public override void InitFormViewModel()
        {
            Entity = new BankEntry
            {
                Id = Guid.NewGuid(),
                Name = string.Empty,
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
                var newData = new Bank
                {
                    Id = IsNew ? Guid.NewGuid() : Entity.Id,
                    Name = Entity.Name,
                };
                var result = await DataModel.CreateAsync(newData);
                Save(result != null);
            }
            ).Start();
        }
    }
}