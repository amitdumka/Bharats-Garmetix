using Garmetix.Core.Enums;
using Garmetix.Core.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Garmetix.Accounting.Models.Party
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