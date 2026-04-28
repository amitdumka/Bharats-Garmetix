using Garmetix.Core.Enums;
using Garmetix.Core.Models.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Garmetix.Accounting.Models.Vouchers
{
    public class CashVoucherEntry : CEntity
    {
        [Display(Name = "Voucher Number"), ReadOnly(true)]

        public string VoucherNumber { get; set; } = string.Empty;
        [Display(Name = "Voucher Date"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Voucher Date is required")]
        public DateTime OnDate { get; set; } = DateTime.Now;

        [Display(Name = "Voucher Type")]
        public VoucherType VoucherType { get; set; } = VoucherType.Payment;

        [Display(Name = "Party Name")]
        [Required(ErrorMessage = "Party Name is required")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,#-]+$", ErrorMessage = "Invalid party name format")]
        [StringLength(150), MinLength(5), MaxLength(150)]
        public string PartyName { get; set; }

        [Display(Name = "Particulars"), Required(ErrorMessage = "Particulars is required")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,#-]+$", ErrorMessage = "Invalid particulars format")]
        [StringLength(150), MinLength(1), MaxLength(150)]
        public string Particulars { get; set; }

        [Display(Name = "Amount"), DataType(DataType.Currency)]
        [Required(ErrorMessage = "Amount is required"), Range(1, double.MaxValue)]
        public decimal Amount { get; set; }

        [Display(Name = "Remarks"), StringLength(200), MinLength(0), MaxLength(200)]
        public string Remarks { get; set; }

        [Display(Name = "Slip Number")]
        public string? SlipNumber { get; set; }

        public Guid Employee { get; set; }
        public Guid Ledger { get; set; }
        public Guid Transaction { get; set; }
        [NotMapped]
        public Guid Company { get; set; }
        [NotMapped]
        public Guid StoreGroup { get; set; }
        [NotMapped]
        public Guid Store { get; set; }
    }
}