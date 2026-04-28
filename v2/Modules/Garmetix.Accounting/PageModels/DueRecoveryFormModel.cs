using Garmetix.CoreServices.Accounting;
using Syncfusion.Maui.DataForm;

namespace Garmetix.Accounting.FormModels
{
    public class DueRecoveryFormModel : FormModel<DueRecoveryEntry>
    {
        private readonly IDataModel<DueRecovery> DataModel = new DataModel<DueRecovery>();

        public override void InitFormViewModel()
        {
            Entity = new DueRecoveryEntry
            {
                Id = Guid.NewGuid(),
                DueInvoiceNumber = String.Empty,
                Amount = 0,
                Paid = false,
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
            var newData = new DueRecovery
            {
                Id = IsNew ? Guid.NewGuid() : Entity.Id,
                InvoiceNumber = Entity.DueInvoiceNumber,
                Amount = Entity.Amount,
                Paid = Entity.Paid,
                OnDate = Entity.OnDate,
                CompanyId = Entity.Company,
                StoreGroupId = Entity.StoreGroup,
                StoreId = Entity.Store,
                CreatedBy = DatabaseService.Instance.CurrentUser.Name,
                Synced = false,
                CreatedAt = DateTime.Now,
                Deleted = false,
                UpdatedAt = DateTime.Now,
            };
            var result = await DataModel.SaveAsync(newData, IsNew);
            if (result != null)
            {
                await AccountingServices.ClearCustomerDue(newData.InvoiceNumber, newData.OnDate);
            }
            Save(result != null);
        }
    }
}