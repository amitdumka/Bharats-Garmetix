using System.ComponentModel.DataAnnotations;

namespace Garmetix.CoreBase.Accounting.Models
{
    public class BankAccountEntry : CEntity
    {
        [Display(Name = "Account Number"), Required(ErrorMessage = "Account Number is required")]
        [StringLength(30), MinLength(5), MaxLength(30)]
        public string AccountNumber { get; set; } = string.Empty;
        [Display(Name = "Account Holder Name"), StringLength(100), MinLength(3), MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9\s.,#-]+$", ErrorMessage = "Invalid account holder name format")]
        [Required(ErrorMessage = "Account Holder Name is required")]
        public string AccountHolderName { get; set; } = string.Empty;
        [Display(Name = "Bank")]
        public Guid Bank { get; set; }
        [Display(Name = "Account Type")]
        public AccountType AccountType { get; set; } = AccountType.Current;
        [Display(Name = "Branch")]
        public string? Branch { get; set; }
        [Display(Name = "IFS Code")]
        public string? IFSCode { get; set; }
        [Display(Name = "Opening Date"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime OpeningDate { get; set; } = DateTime.Now;

        [Display(AutoGenerateField = false)]
        public bool Active { get; set; } = true;
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Closing Date", AutoGenerateField = false)]
        public DateTime? ClosingDate { get; set; } = null;

        [Display(Name = "Opening Balance"), DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal OpeningBalance { get; set; } = 0;

        [Display(Name = "Closing Balance"), DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal ClosingBalance { get; set; } = 0;

        [Display(AutoGenerateField = false)]
        public Guid Company { get; set; }
    }
}