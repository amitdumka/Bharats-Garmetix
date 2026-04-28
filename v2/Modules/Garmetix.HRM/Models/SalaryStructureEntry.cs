using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Garmetix.HRM.Models
{
    public class SalaryStructureEntry : CEntity
    {
        public Guid Company { get; set; }
        public Guid Employee { get; set; }

        [Required]
        [Display(Name = "From Date")]
        public DateTime FromDate { get; set; } = DateTime.Now;
        [Display(Name = "To Date")]
        public DateTime? ToDate { get; set; } = null;


        [Display(Name = "Basic Salary")]
        public decimal BasicSalary { get; set; } = 0;

        [Display(Name = "House Rent Allowance")]
        public decimal HRA { get; set; } = 0; // House Rent Allowance

        [Display(Name = "Special Allowance")]
        public decimal SpecialAllowance { get; set; } = 0;

        [Display(Name = "Conveyance Allowance")]
        public decimal ConveyanceAllowance { get; set; } = 0;

        [Display(Name = "Incentives")]
        public decimal Incentives { get; set; } = 0;

        // Deductions
        [Display(Name = "Provident Fund")]
        public decimal ProvidentFund { get; set; }

        [Display(Name = "Gratuity")]
        public decimal Gratuity { get; set; }

        [Display(Name = "Professional Tax")]
        public decimal ProfessionalTax { get; set; }
        [Display(Name = "Deductions")]
        public decimal Deductions { get; set; }

        //Bonus
        [Display(Name = "Yearly Bonus")]
        public decimal YearlyBonus { get; set; } = 0;
        [Display(Name = "Net Salary"), ReadOnly(true)]
        public decimal NetSalary
        {
            get
            {
                return BasicSalary + HRA + SpecialAllowance + ConveyanceAllowance + Incentives - (ProvidentFund + Gratuity + Deductions + ProfessionalTax);
            }
        }

        [Display(Name = "Gross Salary"), ReadOnly(true)]
        public decimal GrossSalary
        {
            get
            {
                return BasicSalary + HRA + SpecialAllowance + ConveyanceAllowance + Incentives;
            }
        }

        [Display(Name = "Total Deductions"), ReadOnly(true)]
        public decimal TotalDeductions
        {
            get
            {
                return ProvidentFund + Gratuity + Deductions + ProfessionalTax;
            }
        }




    }
}
