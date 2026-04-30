using Garmetix.Accounting.Models.Party;
using Garmetix.Base.PageModels;
using Garmetix.Core.DataModels;
using Garmetix.Core.Enums;
using Garmetix.Core.Interfaces;
using Garmetix.Core.Models.Accounting;
using Garmetix.Databases.Services;
using Syncfusion.Maui.DataForm;

namespace Garmetix.Accounting.PageModels.Parties
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

                OpeningDate = Entity.OpenningDate,
                OpeningBalance = Entity.OpenningBalance,

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