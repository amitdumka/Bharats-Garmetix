using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Garmetix.CoreBase.Accounting.Models
{
    public class VoucherEntry : CEntity
    {
        [Display(Name = "Voucher Number"), ReadOnly(true)]
        public string VoucherNumber { get; set; }

        [Display(Name = "Voucher Date"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Voucher Date is required")]
        public DateTime OnDate { get; set; } = DateTime.Now;

        [Display(Name = "Voucher")]
        public VoucherType VoucherType { get; set; } = VoucherType.Payment;

        [Display(Name = "Party Name")]
        [Required(ErrorMessage = "Party Name is required")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,#-]+$", ErrorMessage = "Invalid party name format")]
        [StringLength(150), MinLength(5), MaxLength(150)]
        public string PartyName { get; set; }


        [Display(Name = "Particulars"), Required(ErrorMessage = "Particulars is required")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,#-]+$", ErrorMessage = "Invalid particulars format")]
        [StringLength(200), MinLength(5), MaxLength(200)]
        public string Particulars { get; set; } = string.Empty;


        [Display(Name = "Amount"), DataType(DataType.Currency)]
        [Required(ErrorMessage = "Amount is required"), Range(1, double.MaxValue)]
        public decimal Amount { get; set; }

        [Display(Name = "Remarks"), StringLength(200), MinLength(1), MaxLength(200)]
        [RegularExpression(@"^[a-zA-Z0-9\s.,#-]+$", ErrorMessage = "Invalid remarks format")]
        public string Remarks { get; set; }

        [Display(Name = "Slip Number")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,#-]+$", ErrorMessage = "Invalid slip number format")]
        public string? SlipNumber { get; set; }

        [Display(Name = "Payment Mode")]
        public PaymentMode PaymentMode { get; set; }


        [Display(Name = "Payment Details")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,#-]+$", ErrorMessage = "Invalid payment details format")]
        public string PaymentDetails { get; set; }


        [Display(Name = "Issued By")]
        public Guid Employee { get; set; }


        [Display(Name = "Account Number")]
        public Guid AccountNumber { get; set; }



        [Display(Name = "Ledger")]
        public Guid Ledger { get; set; }

        [NotMapped]
        public Guid Company { get; set; }
        [NotMapped]
        public Guid StoreGroup { get; set; }
        [NotMapped]
        public Guid Store { get; set; }
    }
}