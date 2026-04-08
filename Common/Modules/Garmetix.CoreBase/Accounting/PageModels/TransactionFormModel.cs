using Syncfusion.Maui.DataForm;

namespace Garmetix.CoreBase.Accounting.FormModels
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
                Company = DatabaseService.CompanyId

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