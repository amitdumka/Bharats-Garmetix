using Garmetix.CoreBase.Stores.Models;
using Garmetix.Models.Stores;
using Syncfusion.Maui.DataForm;

namespace Garmetix.CoreBase.Stores.PageModels
{
    public class StoreGroupFormModel : FormModel<StoreGroupEntry>
    {
        protected IDataModel<StoreGroup> DataModel = new DataModel<StoreGroup>();

        public StoreGroupFormModel() : base()
        {
            Title = "Store Group [New]";
        }

        public override void InitFormViewModel()
        {
            Entity = new StoreGroupEntry
            {
                Active = true,
                Company = DatabaseService.CompanyId,
                EndDate = null,
                Id = Guid.NewGuid(),
                StartDate = DateTime.Now,
                StoreCategory = StoreCategory.Garments,
                GroupCode = string.Empty
            };
        }

        public override void OnGenerateDataFormItem(object sender, GenerateDataFormItemEventArgs e)
        {
            this.GeneratedFormItems(sender, e);
        }

        protected override async void SaveButton()
        {
            var newData = new StoreGroup
            {
                Deleted = false,
                Synced = false,

                CompanyId = Entity.Company,
                Active = IsNew ? true : Entity.Active,
                Name = Entity.Name,
                GroupCode = Entity.GroupCode,

                StartDate = Entity.StartDate,
                EndDate = IsNew ? null : Entity.EndDate,
                Id = IsNew ? Guid.NewGuid() : Entity.Id,

                StoreCategory = Entity.StoreCategory,
            };
            var result = await DataModel.SaveAsync(newData, IsNew);

            Save(result != null);
        }
    }

    public class StoreFormModel : FormModel<StoreEntry>
    {
        protected IDataModel<Store> DataModel = new DataModel<Store>();

        public StoreFormModel() : base()
        {
            Title = "Store [New]";
        }

        public override void InitFormViewModel()
        {
            //TODO: Read from storage and set companyid and storegroup id and etc
            Entity = new StoreEntry
            {
                Id = Guid.NewGuid(),
                Active = true,
                StartDate = DateTime.Now,
                EndDate = null,
                StoreCategory = StoreCategory.Cloths,

                StoreGroup = DatabaseService.StoreGroupId,
                Company = DatabaseService.CompanyId
            };
        }

        public override void OnGenerateDataFormItem(object sender, GenerateDataFormItemEventArgs e)
        {
            this.GeneratedFormItems(sender, e);
            if (e.DataFormGroupItem != null)
            {
                if (e.DataFormGroupItem.Name == "Company Details" || e.DataFormGroupItem.Name == "Address" || e.DataFormGroupItem.Name == "Contact")
                {
                    e.DataFormGroupItem.ColumnCount = 2;
                }

                if (e.DataFormGroupItem.Name == "Tax Info" || e.DataFormGroupItem.Name == "Date" || e.DataFormGroupItem.Name == "Company")
                {
                    e.DataFormGroupItem.ColumnCount = 2;
                }
            }
        }

        protected override async void SaveButton()
        {
            var newData = new Store
            {
                Deleted = false,
                Synced = false,
                StoreGroupId = Entity.StoreGroup,
                CompanyId = Entity.Company,
                Active = IsNew ? true : Entity.Active,
                Name = Entity.Name,
                Address = Entity.Address,
                City = Entity.City,
                StoreCode = Entity.StoreCode,
                StartDate = Entity.StartDate,
                EndDate = IsNew ? null : Entity.EndDate,
                Id = IsNew ? Guid.NewGuid() : Entity.Id,
                Country = Entity.Country,
                State = Entity.State,
                ZipCode = Entity.ZipCode,
                ContactNumber = Entity.ContactNumber,
                Email = Entity.Email,
                StoreCategory = Entity.StoreCategory,
            };
            var result = await DataModel.SaveAsync(newData, IsNew);

            Save(result != null);
        }
    }

    public class CompanyFormModel : FormModel<CompanyEntry>
    {
        protected IDataModel<Company> DataModel = new DataModel<Company>();

        public CompanyFormModel() : base()
        {
            Title = "Company [New]";
        }

        public override void InitFormViewModel()
        {
            Entity = new CompanyEntry
            {
                CompanyType = CompanyType.Proprietorship,
                StartDate = DateTime.Now,
                EndDate = null,
                Country = "India",
                State = "Jharkhand",
                Id = Guid.NewGuid(),
                StoreCategory = StoreCategory.Cloths,
                Active = true,
            };
        }

        public override void OnGenerateDataFormItem(object sender, GenerateDataFormItemEventArgs e)
        {
            //if (e.DataFormItem != null && e.DataFormItem.FieldName == "Name" && e.DataFormItem is DataFormTextEdiorItem textEditorItem)
            //{
            //    textEditorItem.Keyboard = Keyboard.Text;
            //}
            if (e.DataFormGroupItem != null)
            {
                if (e.DataFormGroupItem.Name == "Company Details" || e.DataFormGroupItem.Name == "Address" || e.DataFormGroupItem.Name == "Contact")
                {
                    e.DataFormGroupItem.ColumnCount = 2;
                }

                if (e.DataFormGroupItem.Name == "Tax Info" || e.DataFormGroupItem.Name == "Date" || e.DataFormGroupItem.Name == "Company")
                {
                    e.DataFormGroupItem.ColumnCount = 2;
                }
            }
            if (e.DataFormItem != null)
            {
                if (e.DataFormItem.FieldName == "Id")
                {
                    e.DataFormItem.IsVisible = false;
                }
                if (e.DataFormItem.FieldName == "EndDate")
                {
                    e.DataFormItem.IsVisible = false;
                }
            }
        }

        protected override async void SaveButton()
        {
            var newData = new Company
            {
                Active = Entity.Active,
                Name = Entity.Name,
                CIN = Entity.CIN,
                Address = Entity.Address,
                City = Entity.City,
                Code = Entity.Code,
                CompanyType = Entity.CompanyType,
                ContactMobile = Entity.ContactMobile,
                ContactNumber = Entity.ContactNumber,
                ContactPerson = Entity.ContactPerson,
                Country = Entity.Country,
                EndDate = Entity.EndDate,
                Email = Entity.Email,
                GSTIN = Entity.GSTIN,
                Id = IsNew ? Guid.NewGuid() : Entity.Id,
                Pan = Entity.Pan,
                StartDate = Entity.StartDate,
                State = Entity.State,
                ZipCode = Entity.ZipCode,
                Deleted = false,
                StoreCategory = Entity.StoreCategory,
                Synced = false,
            };
            var result = await DataModel.SaveAsync(newData, IsNew);
            Save(result != null);
        }
    }
}