using Bharat.ToolKits.Notifications;
using Garmetix.CoreBase.HRM.Models;
using Garmetix.CoreServices.Payroll;
using Garmetix.Models.HRM;
using Syncfusion.Maui.DataForm;
using System.Diagnostics;

namespace Garmetix.CoreBase.HRM.PageModels
{
    public class AttendanceFormModel : FormModel<AttendanceEntry>
    {
        protected IDataModel<Attendance> DataModel = new DataModel<Attendance>();

        public AttendanceFormModel() : base()
        {
            Title = "New Attendance";
        }

        public void InitFormViewModel(Attendance attendance)
        {
            if (attendance == null)
            {
                throw new ArgumentNullException(nameof(attendance), "Attendance cannot be null");
            }
            IsNew = false;

            Entity = new AttendanceEntry
            {
                Employee = attendance.EmployeeId,
                OnDate = attendance.OnDate,
                Status = attendance.Status,
                Remarks = attendance.Remarks,
                Id = attendance.Id,
                Company = attendance.CompanyId,
                StoreGroup = attendance.StoreGroupId,
                Store = attendance.StoreId,
                CheckInTime = attendance.CheckInTime ?? DateTime.Now.TimeOfDay, // Default to current time if null
                EntryTime = attendance.EntryTime ?? DateTime.Now.TimeOfDay.ToString(), // Default to current time if null
            };

            Title = $"Edit Attendance [ {attendance.Employee.StaffName} ]";
        }

        public override void InitFormViewModel()
        {
            Entity = new AttendanceEntry()
            {
                CheckInTime = DateTime.Now.TimeOfDay, // Default to current time
                Id = Guid.NewGuid(),
                OnDate = DateTime.Now,
                Status = AttendanceStatus.Present, // Default to present
                Company = DatabaseService.CompanyId,
                Store = DatabaseService.StoreId,
                StoreGroup = DatabaseService.StoreGroupId,
            };
        }

        public override void OnGenerateDataFormItem(object sender, GenerateDataFormItemEventArgs e)
        {
            this.GeneratedFormItems(sender, e);
        }

        protected override async void SaveButton()
        {
            try
            {
                var newData = new Attendance
                {
                    Id = IsNew ? Guid.NewGuid() : Entity.Id,
                    StoreGroupId = Entity.StoreGroup,
                    StoreId = Entity.Store,
                    Synced = false,
                    Deleted = false,
                    EmployeeId = Entity.Employee,
                    Status = Entity.Status,
                    Remarks = Entity.Remarks,
                    CompanyId = Entity.Company,
                    OnDate = Entity.OnDate,
                    EntryTime = Entity.EntryTime,
                    CheckInTime = Entity.CheckInTime,
                    CheckOutTime = null,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = DatabaseService.Instance.CurrentUser.Name,
                };
                if (!PayrollServices.DuplicateAttendaceCheck(newData))
                {
                    var result = await DataModel.SaveAsync(newData, IsNew);
                    Save(result != null);
                }
                else
                {
                    _ = Notify.DisplayNotificationAsync("Error: Attendance  have already checked in for today, No Duplicate Allowed Ebtry allowed", speak: true);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                _ = Notify.DisplayNotificationAsync("Error: " + ex.Message, speak: true);
            }
        }
    }
}