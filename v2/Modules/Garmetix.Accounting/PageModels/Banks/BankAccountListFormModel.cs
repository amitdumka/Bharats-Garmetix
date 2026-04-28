using Syncfusion.Maui.DataForm;

namespace Garmetix.CoreBase.Accounting.FormModels
{
    public class BankAccountListFormModel : FormModel<BankAccountListEntry>
    {
        private readonly IDataModel<BankAccountList> DataModel = new DataModel<BankAccountList>();

        public override void InitFormViewModel()
        {
            Entity = new BankAccountListEntry
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
            var newData = new BankAccountList
            {
                BankName = Entity.BankName,
                Id = IsNew ? Guid.NewGuid() : Entity.Id,
                Deleted = false,
                IFSCode = Entity.IFSCode,
                Synced = false,
                AccountHolderName = Entity.AccountHolderName,
                AccountNumber = Entity.AccountNumber,
                AccountType = Entity.AccountType,
                Branch = Entity.Branch,
                CompanyId = Entity.Company,

            };
            var result = await DataModel.SaveAsync(newData, IsNew);
            Save(result != null);
        }
    }
}