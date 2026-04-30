using Garmetix.Core.Interfaces;
using Syncfusion.Maui.DataForm;

namespace Garmetix.Accounting.PageModels.Parties
{
    public class PartyFormModel : FormModel<PartyEntry>
    {
        private readonly IDataModel<Party> DataModel = new DataModel<Party>();


        //TODO: Move to Service Part
        public static Guid GetLedgerGroupForParty(PartyType partyType)
        {
            //TODO : Move to Service and make it async as well return Task.Run(async delegate=>{});
            var db = DatabaseService.Instance.LocalDB;
            Guid? id;
            switch (partyType)
            {
                case PartyType.Customer:
                    id = db.LedgerGroups.FirstOrDefault(static x => x.Name == "Customers")?.Id ?? Guid.Empty;
                    break;

                case PartyType.Supplier:
                    id = db.LedgerGroups.FirstOrDefault(x => x.Name == "Vendors")?.Id ?? Guid.Empty;
                    break;

                case PartyType.Employee:
                    id = db.LedgerGroups.FirstOrDefault(x => x.Name == "Employees")?.Id ?? Guid.Empty;

                    break;

                case PartyType.Vendor:
                    id = db.LedgerGroups.FirstOrDefault(x => x.Name == "Vendors")?.Id ?? Guid.Empty;
                    break;

                case PartyType.Debitor:
                    id = db.LedgerGroups.FirstOrDefault(x => x.Name == "Debitors")?.Id ?? Guid.Empty;
                    break;

                case PartyType.Creditor:
                    id = db.LedgerGroups.FirstOrDefault(x => x.Name == "Creditors")?.Id ?? Guid.Empty;
                    break;

                case PartyType.Others:
                    id = db.LedgerGroups.FirstOrDefault(x => x.Name == "No Group")?.Id ?? Guid.Empty;
                    break;

                default:
                    id = db.LedgerGroups.FirstOrDefault(x => x.Name == "No Group")?.Id ?? Guid.Empty;
                    break;
            }
            if (id != Guid.Empty)
            {
                return id.Value;
            }
            else
            {
                return db.LedgerGroups.FirstOrDefault(x => x.Name == "No Group")?.Id ?? Guid.Empty;
            }
        }

        public override void InitFormViewModel()
        {
            Entity = new PartyEntry
            {
                Id = Guid.NewGuid(),
                Address = string.Empty,
                Name = string.Empty,
                Phone = string.Empty,
                Email = string.Empty,
                GSTIN = string.Empty,
                PAN = string.Empty,

                Company = DatabaseService.CompanyId,
                Category = PartyType.Customer
            };
        }

        public override void OnGenerateDataFormItem(object sender, GenerateDataFormItemEventArgs e)
        {
            this.GeneratedFormItems(sender, e);
        }

        protected override async void SaveButton()
        {
            var newData = new Party
            {
                Id = IsNew ? Guid.NewGuid() : Entity.Id,
                Name = Entity.Name,
                Category = Entity.Category,
                LedgerId = Entity.LedgerId.Value == Guid.Empty ? Guid.NewGuid() : Entity.LedgerId.Value,

                GSTIN = Entity.GSTIN,
                Address = Entity.Address,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = DatabaseService.Instance.CurrentUser.Name,
                UpdatedAt = DateTime.UtcNow,
                CompanyId = DatabaseService.CompanyId,
                Phone = Entity.Phone,
                PAN = Entity.PAN,

                Deleted = false,
                EmailId = Entity.Email,
                Synced = false,
                Ledger = new Ledger
                {
                    Name = Entity.Name,
                    LedgerType = LedgerType.Income,
                    CompanyId = DatabaseService.CompanyId,
                    Deleted = false,
                    Synced = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = DatabaseService.Instance.CurrentUser.Name,
                    UpdatedAt = DateTime.UtcNow,
                    Id = Entity.LedgerId.Value,
                    IsParty = true,
                    OpeningBalance = 0,
                    OpeningDate = DateTime.Now
                }
            };

            if (IsNew)
            {
                newData.Ledger.Id=newData.Id;
                //newData.LedgerId=newData.Ledger.Id= newData.Id;
                newData.Ledger.LedgerGroupId = GetLedgerGroupForParty(Entity.Category);
            }

            var result = await DataModel.SaveAsync(newData, IsNew);
            Save(result != null);
        }
    }
}