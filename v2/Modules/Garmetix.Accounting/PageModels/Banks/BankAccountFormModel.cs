using Garmetix.CoreServices.Accounting;
using Syncfusion.Maui.DataForm;

namespace Garmetix.CoreBase.Accounting.FormModels
{
    public class BankAccountFormModel : FormModel<BankAccountEntry>
    {
        private readonly IDataModel<BankAccount> DataModel = new DataModel<BankAccount>();

        public override void InitFormViewModel()
        {
            Entity = new BankAccountEntry
            {
                Id = Guid.NewGuid(),
                Company = DatabaseService.CompanyId,
                OpeningDate = DateTime.Now,
                Active = true,
                OpeningBalance = 0,
                ClosingBalance = 0
            };
        }

        public override void OnGenerateDataFormItem(object sender, GenerateDataFormItemEventArgs e)
        {
            this.GeneratedFormItems(sender, e);
        }

        protected override async void SaveButton()
        {
            var newData = new BankAccount
            {
                Id = IsNew ? Guid.NewGuid() : Entity.Id,
                Deleted = false,
                IFSCode = Entity.IFSCode,
                LedgerId = Guid.Empty,
                OpeningBalance = Entity.OpeningBalance,
                OpeningDate = Entity.OpeningDate,
                Synced = false,
                AccountHolderName = Entity.AccountHolderName,
                AccountNumber = Entity.AccountNumber,
                BankId = Entity.Bank,
                AccountType = Entity.AccountType,
                Active = Entity.Active,
                Branch = Entity.Branch,
                ClosingBalance = Entity.ClosingBalance,
                ClosingDate = Entity.ClosingDate,
                CompanyId = Entity.Company,
            };

            if (IsNew)
            {

                var leder = new Ledger
                {
                    Synced = false,
                    UpdatedAt = DateTime.Now,
                    Id = newData.Id,
                    LedgerType = LedgerType.BankAccount,
                    OpenningBalance = 0,
                    OpenningDate = Entity.OpeningDate,
                    CompanyId = Entity.Company,
                    Name = Entity.AccountHolderName,
                    CreatedAt = DateTime.Now,
                    Deleted = false,
                    CreatedBy = DatabaseService.Instance.CurrentUser.Name,
                    IsParty = false,
                    LedgerGroupId = await AccountingServices.GetBankLedgerGroupIdAsync(),
                };
                newData.Ledger = leder;
            }

            var result = await DataModel.SaveAsync(newData, IsNew);
            Save(result != null);
        }
    }
}