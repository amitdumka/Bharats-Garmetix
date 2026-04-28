using Garmetix.Accounting.Models.Party;
using Garmetix.Base.PageModels;
using Garmetix.Core.DataModels;
using Garmetix.Core.Enums;
using Garmetix.Core.Interfaces;
using Garmetix.Core.Models.Accounting;
using Garmetix.Databases.Services;
using Syncfusion.Maui.DataForm;

namespace Garmetix.Accounting.FormModels
{
    public class LedgerGroupFormModel : FormModel<LedgerGroupEntry>
    {
        private readonly IDataModel<LedgerGroup> DataModel = new DataModel<LedgerGroup>();

        public override void InitFormViewModel()
        {
            Entity = new LedgerGroupEntry
            {
                Id = Guid.NewGuid(),
                Category = LedgerCategory.Credit,
                Company = DatabaseService.CompanyId,
                Name = string.Empty,
                Remarks = string.Empty
            };
        }

        public override void OnGenerateDataFormItem(object sender, GenerateDataFormItemEventArgs e)
        {
            this.GeneratedFormItems(sender, e);
        }

        protected override async void SaveButton()
        {
            var newData = new LedgerGroup
            {
                Id = IsNew ? Guid.NewGuid() : Entity.Id,
                Name = Entity.Name,
                Category = Entity.Category,
                Synced = false,
                Deleted = false,
                Remarks = Entity.Remarks,
                CompanyId = Entity.Company
            };
            var result = await DataModel.SaveAsync(newData, IsNew);
            Save(result != null);
        }
    }
}