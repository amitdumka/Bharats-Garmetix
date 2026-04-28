 

using Garmetix.Core.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Garmetix.Accounting.Models.Banks
{
    public class BankAccountDetailEntry : CEntity
    {
        [Display(Name = "Bank Account")]
        public Guid BankAccount { get; set; }

        [Display(Name = "Customer Id")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,#-]+$", ErrorMessage = "Invalid customer ID format")]
        public string? CustomerId { get; set; }
        [Display(Name = "User Name")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,#-]+$", ErrorMessage = "Invalid user name format")]
        public string? UserName { get; set; }
        [Display(Name = "Password")]
        public string? Password { get; set; }
        [Display(Name = "Transcation Password")]
        public string? TranscationPassword { get; set; }
        [Display(Name = "Extra Password")]
        public string? ExtraPassword { get; set; }

        [Display(Name = "ATMPin")]
        public int ATMPin { get; set; }
        [Display(Name = "Mobile Pin")]
        public int MPin { get; set; }
        [Display(Name = "TPIN")]
        public int TPIN { get; set; }
        [Display(Name = "Extra PIN")]
        public int EPIN { get; set; }

        [Display(Name = "ATM Card Number")]
        public string? ATMCard { get; set; }

        [Display(Name = "Expiry Date")]
        public DateTime? ExpireDate { get; set; } = null;

        [Display(Name = "CVV")]
        public string? CVV { get; set; }
        [Display(Name = "Status")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,#-]+$", ErrorMessage = "Invalid status format")]
        [StringLength(50), MaxLength(50)]
        public string? Status { get; set; }

        [NotMapped]
        public Guid Company { get; set; }
    }
}