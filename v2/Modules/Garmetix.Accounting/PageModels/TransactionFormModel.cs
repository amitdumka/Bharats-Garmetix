using Garmetix.Accounting.Models;
using Garmetix.Base.PageModels;
using Garmetix.Core.DataModels;
using Garmetix.Core.Interfaces;
using Garmetix.Core.Models.Accounting;
using Garmetix.Databases.Services;
using Syncfusion.Maui.DataForm;

namespace Garmetix.Accounting.FormModels
{
    public class TransactionFormModel : FormModel<TransactionEntry>
    {
        private readonly IDataModel<Transaction> DataModel = new DataModel<Transaction>();

        public override void OnGenerateDataFormItem(object sender, GenerateDataFormItemEventArgs e)
        {
            this.GeneratedFormItems(sender, e);
        }
        public override void InitFormViewModel()
        {
            Entity = new TransactionEntry
            {
                Id = Guid.NewGuid(),
                Company = DatabaseService.CompanyId, Name= string.Empty

            };
        }

        protected override async void SaveButton()
        {
            var newData = new Transaction
            {
                CompanyId = Entity.Company,
                Id = IsNew ? Guid.NewGuid() : Entity.Id,
                Name = Entity.Name,
            };
            var result = await DataModel.SaveAsync(newData, IsNew);
            Save(result != null);
        }
    }
}