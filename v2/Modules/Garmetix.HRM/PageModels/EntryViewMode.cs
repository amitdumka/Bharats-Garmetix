using Garmetix.HRM.Models;
using Garmetix.Models.HRM;
using Syncfusion.Maui.DataForm;

namespace Garmetix.CoreBase.HRM.PageModels
{
    public class EmployeeDetailFormModel : FormModel<EmployeeDetail>
    {
        protected IDataModel<EmployeeDetail> DataModel = new DataModel<EmployeeDetail>();

        //TODO: Need to implement EmployeeDetail model and its properties
        public EmployeeDetailFormModel() : base()
        {
            Title = "Employee Detail [New]";
        }

        public override void InitFormViewModel()
        {
            throw new NotImplementedException();
        }

        public override void OnGenerateDataFormItem(object sender, GenerateDataFormItemEventArgs e)
        {
            this.GeneratedFormItems(sender, e);
        }

        protected override void SaveButton()
        {
            throw new NotImplementedException();
        }
    }

    public class TimeSheetFormModel : FormModel<TimeSheet>
    {
        protected IDataModel<TimeSheet> DataModel = new DataModel<TimeSheet>();

        public TimeSheetFormModel() : base()
        {
            Title = "Time Sheet [New]";
        }

        public override void InitFormViewModel()
        {
            throw new NotImplementedException();
        }

        public override void OnGenerateDataFormItem(object sender, GenerateDataFormItemEventArgs e)
        {
            this.GeneratedFormItems(sender, e);
        }

        protected override void SaveButton()
        {
            throw new NotImplementedException();
        }
    }

    public class SalaryStructureFormModel : FormModel<SalaryStructureEntry>
    {
        protected IDataModel<SalaryStructure> DataModel = new DataModel<SalaryStructure>();

        public SalaryStructureFormModel() : base()
        {
            Title = "Salary Structure [New]";
        }

        public override void InitFormViewModel()
        {
            Entity = new SalaryStructureEntry
            {
                Id = Guid.NewGuid(),
                Company = DatabaseService.CompanyId,
                FromDate = DateTime.Now,
                ToDate = null,
            };
        }

        public override void OnGenerateDataFormItem(object sender, GenerateDataFormItemEventArgs e)
        {
            this.GeneratedFormItems(sender, e);
        }

        protected override async void SaveButton()
        {
            var newData = new SalaryStructure
            {
                Id = IsNew ? Guid.NewGuid() : Entity.Id,
                CompanyId = Entity.Company,
                FromDate = Entity.FromDate,
                ToDate = Entity.ToDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = DatabaseService.Instance.CurrentUser.Name,
                BasicSalary = Entity.BasicSalary,
                ConveyanceAllowance = Entity.ConveyanceAllowance,
                Deductions = Entity.Deductions,
                HRA = Entity.HRA,
                EmployeeId = Entity.Employee,
                Gratuity = Entity.Gratuity,
                Incentives = Entity.Incentives,
                ProfessionalTax = Entity.ProfessionalTax,
                ProvidentFund = Entity.ProvidentFund,
                SpecialAllowance = Entity.SpecialAllowance,
                Synced = false,
                YearlyBonus = Entity.YearlyBonus,

                Deleted = false,
            };
            var result = await DataModel.SaveAsync(newData, IsNew);
            Save(result != null);
        }
    }
}