using Garmetix.CoreBase.HRM.Models;
using Garmetix.CoreServices.Payroll;
using Garmetix.Models.HRM;
using Syncfusion.Maui.DataForm;

namespace Garmetix.CoreBase.HRM.PageModels
{
    public class EmployeeFormModel : FormModel<EmployeeEntry>
    {
        protected IDataModel<Employee> DataModel = new DataModel<Employee>();

        public EmployeeFormModel() : base()
        {
            Title = "New Employee";
        }
        public async void InitFormViewModel(Guid employeeId)
        {
            if (employeeId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(Employee), "Employee cannot be null");
            }
            IsNew = false;
            var employee = await DataModel.GetByIdAsync(employeeId);
            Entity = new EmployeeEntry
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Title = employee.Title,
                Aadhar = employee.Aadhar,

                Mobile = employee.Mobile,
                DateOfBirth = employee.DateOfBirth,
                Gender = employee.Gender,
                Category = employee.Category,
                JoiningDate = employee.JoiningDate,
                City = employee.EmployeeDetails?.City ?? string.Empty,
                State = employee.EmployeeDetails?.State ?? string.Empty,
                StreetName = employee.EmployeeDetails?.StreetName ?? string.Empty,
                ZipCode = employee.EmployeeDetails?.ZipCode ?? string.Empty,
                Country = employee.EmployeeDetails?.Country ?? string.Empty,
                Email = employee.Email,
                Pan = employee.PAN,
                Company = employee.CompanyId,
                Store = employee.StoreId,
                StoreGroup = employee.StoreGroupId,
                AddressLine = employee.EmployeeDetails?.AddressLine ?? string.Empty,
            };
            Title = $"Edit Employee [ {employee.StaffName} ]";
        }
        public void InitFormViewModel(Employee employee)
        {
            if (employee == null)
            {
                throw new ArgumentNullException(nameof(employee), "Employee cannot be null");
            }
            IsNew = false;
            Entity = new EmployeeEntry
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Title = employee.Title,
                Aadhar = employee.Aadhar,

                Mobile = employee.Mobile,
                DateOfBirth = employee.DateOfBirth,
                Gender = employee.Gender,
                Category = employee.Category,
                JoiningDate = employee.JoiningDate,
                City = employee.EmployeeDetails?.City ?? string.Empty,
                State = employee.EmployeeDetails?.State ?? string.Empty,
                StreetName = employee.EmployeeDetails?.StreetName ?? string.Empty,
                ZipCode = employee.EmployeeDetails?.ZipCode ?? string.Empty,
                Country = employee.EmployeeDetails?.Country ?? string.Empty,
                Email = employee.Email,
                Pan = employee.PAN,
                Company = employee.CompanyId,
                Store = employee.StoreId,
                StoreGroup = employee.StoreGroupId,
                AddressLine = employee.EmployeeDetails?.AddressLine ?? string.Empty,
            };
            Title = $"Edit Employee [ {employee.StaffName} ]";
        }

        public override void InitFormViewModel()
        {
            Entity = new EmployeeEntry()
            {
                Id = Guid.NewGuid(),
                Company = DatabaseService.CompanyId,
                DateOfBirth = DateTime.Now.AddYears(-18), // Default to 18 years ago
                //EmpId = 0, // Default to 0, will be set later
                Gender = Gender.Male,
                Store = DatabaseService.StoreId,
                StoreGroup = DatabaseService.StoreGroupId,
                Title = "Mr.", // Default title
                Category = EmployeeCategory.Salesman, // Default

                JoiningDate = DateTime.Now, // Default to today
            };
        }

        public override void OnGenerateDataFormItem(object sender, GenerateDataFormItemEventArgs e)
        {
            this.GeneratedFormItems(sender, e);
        }

        protected override async void SaveButton()
        {
            var newData = new Employee
            {
                CreatedAt = DateTime.UtcNow,
                CreatedBy = DatabaseService.Instance.CurrentUser.Name,
                UpdatedAt = DateTime.UtcNow,
                SalaryPayments = null,
                Attendances = null,
                SalaryStructures = null,
                Synced = false,
                Title = Entity.Title,
                FirstName = Entity.FirstName,
                LastName = Entity.LastName,
                StoreGroupId = Entity.StoreGroup,
                StoreId = Entity.Store,
                DateOfBirth = Entity.DateOfBirth,
                Deleted = false,
                EmpId = (await DataModel.GetCountAsync()) + 1, //await DataModel.GetCountAsync()+1,
                Gender = Entity.Gender,
                JoiningDate = Entity.JoiningDate,
                LeavingDate = null, //Entity.LeavingDate,
                Working = IsNew ? true : false, // Entity.Working,
                Id = IsNew ? Guid.NewGuid() : Entity.Id,
                Aadhar = Entity.Aadhar,
                PAN = Entity.Pan,
                Email = Entity.Email,
                Category = Entity.Category,
                Mobile = Entity.Mobile,
                EmployeeDetails = new EmployeeDetail
                {
                    EmergencyContact = "",// Entity.EmergencyContact,
                    ZipCode = Entity.ZipCode,
                    State = Entity.State,
                    StreetName = Entity.StreetName,
                    AddressLine = Entity.AddressLine,
                    City = Entity.City,
                    Country = Entity.Country,
                    EmployeeId = IsNew ? Guid.NewGuid() : Entity.Id,
                    FatherName = " ",// Entity.FatherName,
                    MotherName = "",//"Entity.MotherName,
                    SpouseName = "",//"Entity.SpouseName,
                },
                CompanyId = Entity.Company,
            };
            newData.EmployeeDetails.Id = newData.Id;
            var result = await DataModel.SaveAsync(newData, IsNew);
            if (result != null)
            {
                _ = PayrollServices.AddSalesman(newData);
            }

            Save(result != null);
        }
    }
}