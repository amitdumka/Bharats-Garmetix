using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Garmetix.CoreBase.Accounting.Models
{
    public class LedgerGroupEntry : CEntity
    {
        [Display(Name = "Ledger Group")]
        [Required(ErrorMessage = "Ledger Group Name is required")]
        [StringLength(150), MinLength(5), MaxLength(150)]
        public string Name { get; set; }
        [Display(Name = "Ledger Category")]
        [Required(ErrorMessage = "Ledger Category is required")]
        public LedgerCategory Category { get; set; }

        [Display(Name = "Remarks"), StringLength(200), MaxLength(200)]
        [RegularExpression(@"^[a-zA-Z0-9\s.,#-]+$", ErrorMessage = "Invalid remarks format")]
        public string Remarks { get; set; }
        [NotMapped]
        public Guid Company { get; set; }
    }
}