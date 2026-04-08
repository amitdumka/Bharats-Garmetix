using Syncfusion.Maui.DataForm;

namespace Garmetix.CoreBase.Accounting.FormModels
{
    public class LedgerFormModel : FormModel<LedgerEntry>
    {
        private readonly IDataModel<Ledger> DataModel = new DataModel<Ledger>();

        public override void InitFormViewModel()
        {
            Entity = new LedgerEntry()
            {
                Id = Guid.NewGuid(),
                OpenningBalance = 0,
                OpenningDate = DateTime.Now,
                Company = DatabaseService.CompanyId,
                //IsParty = false,
                LedgerType = LedgerType.Expenses,
                Name = string.Empty
            };
        }

        public override void OnGenerateDataFormItem(object sender, GenerateDataFormItemEventArgs e)
        {
            this.GeneratedFormItems(sender, e);
        }

        protected override async void SaveButton()
        {
            var newData = new Ledger
            {
                Id = IsNew ? Guid.NewGuid() : Entity.Id,
                Name = Entity.Name,
                LedgerGroupId = Entity.LedgerGroup,
                LedgerType = Entity.LedgerType,

                OpenningDate = Entity.OpenningDate,
                OpenningBalance = Entity.OpenningBalance,

                CompanyId = Entity.Company,
                Deleted = false,
                IsParty = false,
                CreatedAt = DateTime.Now,
                CreatedBy = DatabaseService.Instance.CurrentUser.Name,
                UpdatedAt = DateTime.Now,
                Synced = false,
            };
            var result = await DataModel.SaveAsync(newData, IsNew);
            Save(result != null);
        }
    }
}