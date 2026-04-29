using Garmetix.Core.Enums;
using Garmetix.Core.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace Garmetix.HRM.Models
{
    public class AttendanceEntry : CEntity
    {
        [Required, Display(Name = "Employee")]
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "Select Employee")]
        public Guid Employee { get; set; }

        [Required, Display(Name = "Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime OnDate { get; set; } = DateTime.Now;

        [Required, Display(Name = "Attendance Status")]
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "Select Status")]
        [EnumDataType(typeof(AttendanceStatus), ErrorMessage = "Invalid attendance status.")]
        public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;

        [Display(Name = "Entry Time")]
        public string? EntryTime { get; set; } = DateTime.Now.TimeOfDay.ToString();
        [Display(Name = "Check In Time"), DataType(DataType.Time)]
        public TimeSpan? CheckInTime { get; set; } = DateTime.Now.TimeOfDay;


        [Required, Display(Name = "Remarks"), DataType(DataType.Text), StringLength(100)]
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "Enter Remarks")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,;:!?-]*$", ErrorMessage = "Invalid characters in remarks.")]
        public string Remarks { get; set; }
        [Display(Name = "Company")]
        [Required(ErrorMessage = "Company is required.")]
        public Guid Company { get; set; }

        [Display(Name = "Divison")]
        [Required(ErrorMessage = "Division is required.")]

        public Guid StoreGroup { get; set; }

        [Required(ErrorMessage = "Store location is required.")]
        [Display(Name = "Store Location")]
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "Select Store Location")]
        public Guid Store { get; set; }
    }
}