using Garmetix.Core.Interfaces;
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
                Store = DatabaseService.StoreId, PaymentMode= PaymentMode.Cash
            };
        }

        public override void OnGenerateDataFormItem(object sender, GenerateDataFormItemEventArgs e)
        {
            // Check if the current item being generated is the Payment Details field
            if (e.DataFormItem != null && e.DataFormItem.FieldName == nameof(DueRecoveryEntry.PaymentDetails))
            {
                // Bind the IsVisible property to our computed boolean
                e.DataFormItem.SetBinding(DataFormItem.IsVisibleProperty, nameof(DueRecoveryEntry.IsPaymentDetailsVisible));
            }
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
                PaymentMode = Entity.PaymentMode,
                PaymentDetails = Entity.PaymentDetails,
                // ClearingDate = Entity.Paid ? DateTime.Now : null,
                CreatedBy = DatabaseService.Instance.CurrentUser.Name,
                Synced = false,
                CreatedAt = DateTime.Now,
                Deleted = false,
                UpdatedAt = DateTime.Now,
            };
            var result = await DataModel.SaveAsync(newData, IsNew);

            //TODO : if payment option is  Card then we need to ask card details and save it in database and also need to update the due invoice as paid and clear the due amount from customer ledger

            if (result != null)
            {
               // await AccountingServices.ClearCustomerDue(newData.InvoiceNumber, newData.OnDate);
                await AccountingServices.UpdateDueInvoice(newData.InvoiceNumber, newData);
            }
            Save(result != null);
        }
    }
}