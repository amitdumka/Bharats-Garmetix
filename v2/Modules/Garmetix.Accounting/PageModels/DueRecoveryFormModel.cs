using Garmetix.Core.Interfaces;
using Garmetix.CoreServices.Accounting;
using Syncfusion.Maui.DataForm;
using System.ComponentModel;

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
        https://www.syncfusion.com/blogs/post/ai-powered-smart-net-maui-data-forms

            //// Check if the current item being generated is the Payment Details field
            //if (e.DataFormItem != null && e.DataFormItem.FieldName == nameof(DueRecoveryEntry.PaymentDetails))
            //{
            //    // Bind the IsVisible property to our computed boolean
            //    e.DataFormItem.SetBinding(DataFormItem.IsVisibleProperty, nameof(DueRecoveryEntry.IsPaymentDetailsVisible));
            //}

            // 1. Safely subscribe to the model's PropertyChanged event.
            // (We unsubscribe first to ensure we don't accidentally subscribe multiple times)
            //Entity.PropertyChanged -= OnModelPropertyChanged;
            //Entity.PropertyChanged += OnModelPropertyChanged;

            //// 2. Set the INITIAL visibility when the form is drawn
            //if (e.DataFormItem != null && e.DataFormItem.FieldName == nameof(DueRecoveryEntry.PaymentDetails))
            //{
            //    e.DataFormItem.IsVisible = Entity.IsPaymentDetailsVisible;
            //}

            // 1. Check if we are generating the PaymentDetails field
            this.GeneratedFormItems(sender, e);
            if (e.DataFormItem != null && e.DataFormItem.FieldName == nameof(DueRecoveryEntry.PaymentDetails))
            {
                // 2. Safely grab the current DataObject (your model)
                if (EntryForm!=null && EntryForm.DataObject is DueRecoveryEntry model)
                {
                    // 3. Create a binding that EXPLICITLY points to the model as the Source
                    var visibilityBinding = new Binding(
                        path: nameof(DueRecoveryEntry.IsPaymentDetailsVisible),
                        source: model
                    );

                    // 4. Apply the binding to the DataFormItem
                    e.DataFormItem.SetBinding(DataFormItem.IsVisibleProperty, visibilityBinding);
                }
            }
            
        }
        [Obsolete]
        private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // 3. Set the DYNAMIC visibility when the user changes the Payment Mode

            // The CommunityToolkit automatically fires this notification because we used 
            // [NotifyPropertyChangedFor(nameof(IsPaymentDetailsVisible))] on the PaymentMode property.
            if (e.PropertyName == nameof(DueRecoveryEntry.IsPaymentDetailsVisible))
            {
                if (sender is DueRecoveryEntry model)
                {
                    // Grab the specific UI field from Syncfusion
                    var paymentDetailsItem = EntryForm?.GetDataFormItem(nameof(DueRecoveryEntry.PaymentDetails));

                    if (paymentDetailsItem != null)
                    {
                        // Explicitly set the visibility, which forces Syncfusion to update the layout
                        paymentDetailsItem.IsVisible = model.IsPaymentDetailsVisible;
                    }
                }
            }
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