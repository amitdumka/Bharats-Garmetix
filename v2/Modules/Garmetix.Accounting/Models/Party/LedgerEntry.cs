using System.ComponentModel.DataAnnotations;

namespace Garmetix.CoreBase.Accounting.Models
{
    public class LedgerEntry : CEntity
    {
        [Display(Name = "Ledger Name"), Required(ErrorMessage = "Ledger Name is required")]
        [StringLength(150), MinLength(5), MaxLength(150)]
        [RegularExpression(@"^[a-zA-Z0-9\s.,#-]+$", ErrorMessage = "Invalid ledger name format")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Ledger Group")]
        public Guid LedgerGroup { get; set; }

        [Display(Name = "Ledger Type")]
        public LedgerType LedgerType { get; set; }

        [Display(Name = "Opening Date"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Opening Date is required")]
        public DateTime OpenningDate { get; set; } = DateTime.Now;

        [Display(Name = "Opening Balance"), DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal OpenningBalance { get; set; } = 0;

        //[Display(Name = "Is Party", AutoGenerateFilter = false)]
        //public bool IsParty { get; set; } = false;
        [Display(Name = "Company", AutoGenerateField = false, AutoGenerateFilter = false)]
        public Guid Company { get; set; }
    }
}