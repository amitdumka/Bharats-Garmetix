using Garmetix.Core.Enums;
using Garmetix.Core.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Garmetix.Accounting.Models
{
    public class CustomerDueEntry : CEntity
    {
        [Display(Name = "Invoice Number")]
        [Required(ErrorMessage = "Invoice Number is required")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,#-]+$", ErrorMessage = "Invalid invoice number format")]

        public required string InvoiceNumber { get; set; }
        [Display(Name = "Invoice Date"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Invoice Date is required")]
        public DateTime OnDate { get; set; } = DateTime.Now;
        [Display(Name = "Due Amount"), DataType(DataType.Currency)]
        [Required(ErrorMessage = "Due Amount is required"), Range(1, double.MaxValue)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal Amount { get; set; } = 0;
        [Display(Name = "Fully Paid")]
        public bool Paid { get; set; } = false;

        [Display(Name = "Clearing Date"), DataType(DataType.Date)]

        [NotMapped]
        public DateTime? ClearingDate { get; set; } = null;
        [NotMapped]
        public Guid Company { get; set; }
        [NotMapped]
        public Guid StoreGroup { get; set; }
        [NotMapped]
        public Guid Store { get; set; }
    }

    public class TransactionEntry : CEntity
    {
        [Display(Name = "Transaction Name"), Required(ErrorMessage = "Transaction Name is required")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,#-]+$", ErrorMessage = "Invalid transaction name format")]
        [StringLength(150), MinLength(5), MaxLength(150)]
        public required string Name { get; set; }


        [NotMapped]
        public Guid Company { get; set; }
    }

    public class DueRecoveryEntry : CEntity
    {
        [Display(Name = "Invoice Number"), Required]
        public required string DueInvoiceNumber { get; set; }


        [Display(Name = "Paying Date"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime OnDate { get; set; }
        [Display(Name = "Paid Amount"), Required, Range(1, double.MaxValue)]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true), DataType(DataType.Currency)]
        public decimal Amount { get; set; }


        [Display(Name = "Fully Paid")]
        public bool Paid { get; set; } = false;
        [NotMapped]
        public Guid Company { get; set; }
        [NotMapped]
        public Guid StoreGroup { get; set; }
        [NotMapped]
        public Guid Store { get; set; }

    }

    //Banking
    public class BankEntry : CEntity
    {
        [Display(Name = "Bank Name"), Required(ErrorMessage = "Bank Name is required")]
        [StringLength(150), MinLength(3), MaxLength(150)]
        [RegularExpression(@"^[a-zA-Z0-9\s.,#-]+$", ErrorMessage = "Invalid bank name format")]
        public required string Name { get; set; }
    }

    public class VendorBankAccountEntry : CEntity
    {
        [Display(Name = "Vendor")]
        public Guid Vendor { get; set; }

        [Display(Name = "Account Number"), Required(ErrorMessage = "Account Number is required")]
        [StringLength(30), MinLength(5), MaxLength(30)]
        [RegularExpression(@"^[a-zA-Z0-9\s.,#-]+$", ErrorMessage = "Invalid account number format")]
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

        [Display(Name = "Active", AutoGenerateField = false)]
        public bool Active { get; set; } = true;

        [Display(Name = "Closing Date", AutoGenerateField = false), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ClosingDate { get; set; } = null;

        [Display(Name = "Opening Balance"), DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal OpeningBalance { get; set; } = 0;


        [Display(Name = "Closing Balance", AutoGenerateField = false), DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal ClosingBalance { get; set; } = 0;

        [NotMapped]
        public Guid Company { get; set; }
    }

    public class BankAccountListEntry : CEntity
    {
        [Display(Name = "Account Number"), Required(ErrorMessage = "Account Number is required")]
        [StringLength(30), MinLength(5), MaxLength(30)]
        [RegularExpression(@"^[a-zA-Z0-9\s.,#-]+$", ErrorMessage = "Invalid account number format")]
        public required string AccountNumber { get; set; }

        [Display(Name = "Account Holder Name"), StringLength(100), MinLength(3), MaxLength(100)]
        [Required(ErrorMessage = "Account Holder Name is required")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,#-]+$", ErrorMessage = "Invalid account holder name format")]
        public string AccountHolderName { get; set; }= string.Empty;

        [Display(Name = "Bank"), Required(ErrorMessage = "Bank is required")]
        public string BankName { get; set; }=string.Empty;
        [Display(Name = "Branch")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,#-]+$", ErrorMessage = "Invalid branch format")]
        public string Branch { get; set; }= string.Empty;
        [Display(Name = "IFS Code")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,#-]+$", ErrorMessage = "Invalid IFS code format")]
        public string IFSCode { get; set; }= string.Empty;

        [Display(Name = "Account Type"), Required(ErrorMessage = "Account Type is required")]
        public AccountType AccountType { get; set; }

        [NotMapped]
        public Guid Company { get; set; }
    }
}