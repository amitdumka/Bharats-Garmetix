using Garmetix.Core.Enums;
using Garmetix.Core.Models.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Garmetix.HRM.Models
{
    public class SalaryPaymentEntry : CEntity
    {
        [Display(Name = "Company")]
        [Required(ErrorMessage = "Company is required.")]
        public Guid Company { get; set; }

        [Display(Name = "Division")]
        [Required(ErrorMessage = "Division is required.")]
        public Guid StoreGroup { get; set; }

        [Display(Name = "Store Location")]
        [Required(ErrorMessage = "Store Location is required.")]
        public Guid Store { get; set; }

        [Display(Name = "Voucher No"), ReadOnly(true)]
        public string VoucherNo { get; set; } = string.Empty;

        [Display(Name = "Staff Name")]
        [Required(ErrorMessage = "Staff Name is required.")]
        public Guid Employee { get; set; }

        [Display(Name = "Salary/Year(021992)")]
        [Required(ErrorMessage = "Salary Month is required.")]
        [Range(1, 12, ErrorMessage = "Salary Month must be between 1 and 12.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Salary Month can only contain numbers.")]
        public int SalaryMonth { get; set; }

        [Display(Name = "Payment Reason")]
        [Required(ErrorMessage = "Payment Reason is required.")]
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "Select Payment Reason")]
        [EnumDataType(typeof(SalaryComponent), ErrorMessage = "Invalid Salary Component.")]
        public SalaryComponent SalaryComponent { get; set; }

        [Required(ErrorMessage = "Payment Date is required.")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Payment Date")]
        public DateTime OnDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Payment Amount is required.")]
        [Range(1.01, double.MaxValue, ErrorMessage = "Payment Amount must be greater than zero.")]
        [DataType(DataType.Currency), Display(Name = "Payment Amount")]
        public decimal Amount { get; set; }

        [Display(Name = "Payment Mode")]
        [Required(ErrorMessage = "Payment Mode is required.")]
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "Select Payment Mode")]
        [EnumDataType(typeof(PaymentMode), ErrorMessage = "Invalid Payment Mode.")]
        public PaymentMode PaymentMode { get; set; }

        [Display(Name = "Payment Remarks")]
        [DataType(DataType.Text), StringLength(100, ErrorMessage = "Remarks cannot exceed 100 characters.")]
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "Enter Remarks")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,;:!?-]*$", ErrorMessage = "Invalid characters in remarks.")]
        public string Remarks { get; set; } = string.Empty;

        [Display(Name = "Gross Salary")]
        [Range(0, double.MaxValue, ErrorMessage = "Gross Salary must be a positive value.")]
        [DataType(DataType.Currency)]
        public decimal GrossSalary { get; set; } = 0;

        [Display(Name = "Deduction")]
        [Range(0, double.MaxValue, ErrorMessage = "Deduction must be a positive value.")]
        [DataType(DataType.Currency)]
        public decimal Deduction { get; set; } = 0;

        [Display(Name = "Net Salary")]
        [Range(0, double.MaxValue, ErrorMessage = "Net Salary must be a positive value.")]
        [DataType(DataType.Currency)]
        public decimal NetSalary { get; set; } = 0;

        [Display(Name = "Salary Slip Details")]
        public Guid? SalaryPaySlip { get; set; } = null;
    }
}