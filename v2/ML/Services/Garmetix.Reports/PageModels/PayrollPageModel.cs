using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input; 
using Garmetix.Models.ViewModels;
using Bharat.ToolKits.Notifications;

namespace Garmetix.Reports.PageModels
{
     
    internal partial class PayrollPageModel: BasePageModel
    {
        [ObservableProperty]
        private Guid _selectedEmployee=Guid.Empty;

        [ObservableProperty]
        private int _selectedYear=DateTime.Now.Year;
        
        [ObservableProperty]
        private int _selectedMonth=DateTime.Now.Month;

        [ObservableProperty]
        private List<ComboBoxItemVM> _employees;

        

        [RelayCommand]
        private void SelectEmployee(Guid employeeId)
        {
            SelectedEmployee = employeeId;
        }

        //TODO: Fetch Data from ModuleService and process the data to generate PDF and Store. 
        //TODO: Add Option to see all previous reports which can be downloaded.
        //TODO: add option to store in google drive or dropbox

        public PayrollPageModel()
        {
            LoadData();
        }
        public void LoadData()
        {
            var employees = Db.Employees.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.FullName }).ToList();
            
            if(employees!=null && employees.Count > 0)
            {

                Employees = employees;
                SelectedEmployee = employees[0].Id;
                _ = Notify.DisplayToastAsync("Employee is loading" + employees.Count);
            }
            LoadPeriod();
            SelectedMonth=DateTime.Now.Month;
            SelectedYear=DateTime.Now.Year;
        
        }

        [RelayCommand]
        private void PrintEmployeeReport()
        {
        }

        [RelayCommand]
        private void PrintMontlyReport() {
        }
        [RelayCommand]
        private void PrintYearlyReport() {
        }
        [RelayCommand]
        private void PrintAttanceReport() {
        }
        [RelayCommand]
        private void PrintSalaryLedger() { }
        [RelayCommand]
        private void PrintPayslip()
        {

        }

    }
}
