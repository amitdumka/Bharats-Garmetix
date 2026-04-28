using Garmetix.Core.Enums;
using Garmetix.Core.Models.Base;
using Syncfusion.Maui.DataForm;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Garmetix.Accounting.Models
{
    public class PartyEntry : CEntity
    {
        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Party Name"), StringLength(150)]
        public string Name { get; set; } = string.Empty;
        [Display(Name = "Address")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,#-]+$", ErrorMessage = "Invalid address format")]
        public string? Address { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email")]

        public string? Email { get; set; }
        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Phone")]
        public string? Phone { get; set; }
        [Display(Name = "GSTIN")]
        [RegularExpression(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$", ErrorMessage = "Invalid GSTIN format")]
        public string? GSTIN { get; set; }
        [Display(Name = "PAN")]
        [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$", ErrorMessage = "Invalid PAN format")]
        public string? PAN { get; set; }
        [Display(Name = "Category")]
        public PartyType Category { get; set; }
        [NotMapped]
        [DataFormDisplayOptions(ShowLabel = false)] // Correct usage based on the attribute definition

        public Guid? LedgerId { get; set; } = Guid.Empty;
        [NotMapped]
        public Guid Company { get; set; } = Guid.Empty;
    }
}