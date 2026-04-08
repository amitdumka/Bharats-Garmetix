using System.ComponentModel.DataAnnotations;

namespace Garmetix.HRM.Models
{

    public class CheckOutEntry : CEntity
    {

        [Display(Name = "Employee")]
        public Guid Employee { get; set; }
        [Display(Name = "On Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "On Date is required.")]
        public DateTime OnDate { get; set; } = DateTime.Today;

        [Display(Name = "Check Out Time")]
        [DataType(DataType.Time), DisplayFormat(DataFormatString = "{0:HH:mm:ss}", ApplyFormatInEditMode = true)]

        [RegularExpression(@"^([01]\d|2[0-3]):([0-5]\d):([0-5]\d)$", ErrorMessage = "Invalid time format. Use HH:mm:ss.")]

        [Required(AllowEmptyStrings = false, ErrorMessage = "Check Out Time is required.")]
        public TimeSpan? CheckOutTime { get; set; } = DateTime.Now.TimeOfDay;
    }
    public class CheckInEntry : CEntity
    {
        [Display(Name = "Employee")]
        [Required(ErrorMessage = "Employee is required.")]

        public Guid Employee { get; set; }
        [Display(Name = "On Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "On Date is required.")]

        public DateTime OnDate { get; set; }

        [Display(Name = "Check In Time")]
        [DataType(DataType.Time), DisplayFormat(DataFormatString = "{0:HH:mm:ss}", ApplyFormatInEditMode = true)]
        [RegularExpression(@"^([01]\d|2[0-3]):([0-5]\d):([0-5]\d)$", ErrorMessage = "Invalid time format. Use HH:mm:ss.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Check In Time is required.")]
        public TimeSpan? CheckInTime { get; set; }
    }
}
