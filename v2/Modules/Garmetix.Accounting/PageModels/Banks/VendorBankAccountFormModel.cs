using Syncfusion.Maui.DataForm;

namespace Garmetix.Accounting.FormModels
{

    public class VendorBankAccountFormModel : FormModel<VendorBankAccountEntry>
    {
        private readonly IDataModel<VendorBankAccount> DataModel = new DataModel<VendorBankAccount>();

        public override void InitFormViewModel()
        {
            Entity = new VendorBankAccountEntry
            {
                Id = Guid.NewGuid(),
                Company = DatabaseService.CompanyId,
            };
        }

        public override void OnGenerateDataFormItem(object sender, GenerateDataFormItemEventArgs e)
        {
            this.GeneratedFormItems(sender, e);
        }

        protected override async void SaveButton()
        {
            var newData = new VendorBankAccount
            {
                Id = IsNew ? Guid.NewGuid() : Entity.Id,
                Deleted = false,
                IFSCode = Entity.IFSCode,

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
                VendorId = Entity.Vendor
            };
            var result = await DataModel.SaveAsync(newData, IsNew);
            Save(result != null);
        }
    }
}