using Syncfusion.Maui.DataForm;

namespace Garmetix.CoreBase.Accounting.FormModels
{
    public class BankAccountDetailFormModel : FormModel<BankAccountDetailEntry>
    {
        private readonly IDataModel<BankAccountDetail> DataModel = new DataModel<BankAccountDetail>();

        public override void InitFormViewModel()
        {
            Entity = new BankAccountDetailEntry
            {
                Id = Guid.NewGuid(),
                Company = DatabaseService.CompanyId,
                BankAccount = Guid.Empty, // Set to a valid BankAccount ID if needed
            };
        }

        public override void OnGenerateDataFormItem(object sender, GenerateDataFormItemEventArgs e)
        {
            this.GeneratedFormItems(sender, e);
        }

        protected override async void SaveButton()
        {
            var newData = new BankAccountDetail
            {
                ExpireDate = Entity.ExpireDate,
                Id = IsNew ? Guid.NewGuid() : Entity.Id,
                MPin = Entity.MPin,
                Password = Entity.Password,
                TPIN = Entity.TPIN,
                TranscationPassword = Entity.TranscationPassword,
                Status = Entity.Status,
                UserName = Entity.UserName,

                ExtraPassword = Entity.ExtraPassword,
                CVV = Entity.CVV,
                EPIN = Entity.EPIN,
                Deleted = false,
                CustomerId = Entity.CustomerId,
                Synced = false,
                BankAccountId = Entity.BankAccount,
                ATMCard = Entity.ATMCard,
                ATMPin = Entity.ATMPin,
                CompanyId = Entity.Company

            };
            var result = await DataModel.SaveAsync(newData, IsNew);
            Save(result != null);
        }
    }
}