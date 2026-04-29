using Garmetix.Core.Models.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Garmetix.HRM.Models
{
    public class SalaryPaySlipEntry : CEntity
    {
        public Guid Company { get; set; }


        public Guid Employee { get; set; }

        [Required]
        [MaxLength(20, ErrorMessage = "MonthYear is required")]
        [Display(Name = "Month & Year")]
        public string MonthYear { get; set; } = DateTime.Now.ToString("MMMM yyyy");

        [Required]
        [Display(Name = "Period Start")]
        public DateTime PayPeriodStart { get; set; }

        [Required]
        [Display(Name = "Period End")]
        public DateTime? PayPeriodEnd { get; set; } = null;

        // Earnings
        [Required]
        [Display(Name = "Basic Salary")]
        public decimal BasicSalary { get; set; } = 0;

        [Required]
        [Display(Name = "House Rent Allowance")]
        public decimal HRA { get; set; } = 0;

        [Required]
        [Display(Name = "Special Allowance")]
        public decimal SpecialAllowance { get; set; }

        [Required]
        [Display(Name = "Conveyance Allowance")]
        public decimal ConveyanceAllowance { get; set; }

        [Required]
        [Display(Name = "Incentives")]
        public decimal Incentives { get; set; } = 0;

        public decimal OtherEarnings { get; set; } = 0;

        // Deductions

        [Display(Name = "Provident Fund")]
        public decimal ProvidentFund { get; set; } = 0;
        [Display(Name = "Gratuity")]
        public decimal Gratuity { get; set; }
        [Display(Name = "Deductions")]
        public decimal Deductions { get; set; }
        [Display(Name = "Professional Tax")]
        public decimal ProfessionalTax { get; set; }

        [Display(Name = "Income Tax")]
        public decimal IncomeTax { get; set; }

        [Display(Name = "Other Deductions")]
        public decimal OtherDeductions { get; set; }

        [MaxLength(200)]
        [Display(Name = "Remarks"), DataType(DataType.Text)]
        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Special characters are not allowed in Remarks")]
        public string? Remarks { get; set; }
        [Display(Name = "Net Salary"), ReadOnly(true)]
        public decimal NetSalary
        {
            get
            {
                return BasicSalary + HRA + SpecialAllowance + ConveyanceAllowance + Incentives + OtherEarnings - (ProvidentFund + Gratuity + Deductions + IncomeTax + ProfessionalTax + OtherDeductions);
            }
        }
    }
}
