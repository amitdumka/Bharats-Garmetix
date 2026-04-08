using Syncfusion.Maui.DataForm;

namespace Garmetix.CoreBase.Accounting.FormModels
{
    public class CustomerDueFormModel : FormModel<CustomerDueEntry>
    {
        private readonly IDataModel<CustomerDue> DataModel = new DataModel<CustomerDue>();

        public override void InitFormViewModel()
        {
            Entity = new CustomerDueEntry
            {
                Id = Guid.NewGuid(),
                Amount = 0,
                Paid = false,
                ClearingDate = null,
                OnDate = DateTime.Now,
                Company = DatabaseService.CompanyId,
                StoreGroup = DatabaseService.StoreGroupId,
                Store = DatabaseService.StoreId
            };
        }

        public override void OnGenerateDataFormItem(object sender, GenerateDataFormItemEventArgs e)
        {
            this.GeneratedFormItems(sender, e);
        }

        protected override async void SaveButton()
        {
            var newData = new CustomerDue
            {
                Id = IsNew ? Guid.NewGuid() : Entity.Id,
                InvoiceNumber = Entity.InvoiceNumber,
                Amount = Entity.Amount,
                Paid = Entity.Paid,
                OnDate = Entity.OnDate,
                ClearingDate = Entity.ClearingDate,
                CompanyId = Entity.Company,
                StoreGroupId = Entity.StoreGroup,
                StoreId = Entity.Store,
                Synced = false,
                Deleted = false,
            };
            var result = await DataModel.SaveAsync(newData, IsNew);
            Save(result != null);
        }
    }
}