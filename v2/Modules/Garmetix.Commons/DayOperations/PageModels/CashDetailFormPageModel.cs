using Garmetix.Base.PageModels;
using Garmetix.Commons.DayOperations.Models;
using Garmetix.Core.Interfaces;
using Garmetix.Databases.Services;
using Garmetix.Models.DayOperations;
using Syncfusion.Maui.DataForm;

namespace Garmetix.Commons.DayOperations.PageModels
{
    public class CashDetailFormPageModel : FormModel<CashDetailEntry>
    {
        private IDataModel<CashDetail> DataModel = new DataModel<CashDetail>();
        public override void InitFormViewModel()
        {
            Entity = new CashDetailEntry
            {
                Id = Guid.NewGuid(),
                OnDate = DateTime.Now,
                Store = DatabaseService.StoreId,
                Amount = 0,
            };
        }

        public override void OnGenerateDataFormItem(object sender, GenerateDataFormItemEventArgs e)
        {
            this.GeneratedFormItems(sender, e);
        }

        protected override async void SaveButton()
        {
            var isValid = EntryForm?.Validate();
            if (isValid == false) return;
            var newData = new CashDetail
            {
                CreatedAt = DateTime.UtcNow, CreatedBy = DatabaseService.Instance.CurrentUser.Name,
                Deleted = false, Synced = false, UpdatedAt = DateTime.UtcNow,
                Id = IsNew ? Guid.NewGuid() : Entity!.Id,
                OnDate = IsNew ? DateTime.Now : Entity!.OnDate,
                StoreId = DatabaseService.StoreId,
                Amount = Entity!.Amount,
            };
            var result = await DataModel.SaveAsync(newData );
            Save(result != null);
        }
    }
}